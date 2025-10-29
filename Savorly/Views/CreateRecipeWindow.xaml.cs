using Microsoft.EntityFrameworkCore;
using Savorly.Data;
using Savorly.Models;
using Savorly.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Savorly.Views
{
    public partial class CreateRecipeWindow : Window
    {
        private AppDbContext _context;
        private ObservableCollection<string> _ingredients;
        private ObservableCollection<RecipeStep> _steps;
        private int _currentStepNumber = 1;

        public CreateRecipeWindow()
        {
            InitializeComponent();
            _context = new AppDbContext();
            _ingredients = new ObservableCollection<string>();
            _steps = new ObservableCollection<RecipeStep>();

            IngredientsListBox.ItemsSource = _ingredients;
            StepsListBox.ItemsSource = _steps;

            SetDefaultPreviewImage();
        }

        private void SetDefaultPreviewImage()
        {
            PreviewImage.Source = null;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClearForm_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Ви впевнені, що хочете очистити всю форму? Всі дані будуть втрачені.",
                "Очищення форми", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ClearForm();
            }
        }

        private void ClearForm()
        {
            TitleTextBox.Text = string.Empty;
            ShortDescriptionTextBox.Text = string.Empty;
            DescriptionTextBox.Text = string.Empty;
            PreparationTimeTextBox.Text = string.Empty;
            ServingsTextBox.Text = string.Empty;
            ImagePathTextBox.Text = string.Empty;
            FoodTypeRadio.IsChecked = true;

            _ingredients.Clear();
            _steps.Clear();
            _currentStepNumber = 1;

            UpdatePreview();
            HideValidationError();
        }

        private void SaveRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (!UserService.IsLoggedIn)
            {
                MessageBox.Show("Будь ласка, увійдіть у систему", "Помилка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!ValidateForm())
            {
                return;
            }

            try
            {
                var recipe = new Recipe
                {
                    Title = TitleTextBox.Text.Trim(),
                    ShortDescription = ShortDescriptionTextBox.Text.Trim(),
                    Description = DescriptionTextBox.Text.Trim(),
                    ImagePath = string.IsNullOrWhiteSpace(ImagePathTextBox.Text) ?
                               GetDefaultImage() : ImagePathTextBox.Text.Trim(),
                    PreparationTime = int.Parse(PreparationTimeTextBox.Text),
                    Servings = int.Parse(ServingsTextBox.Text),
                    Type = FoodTypeRadio.IsChecked == true ? RecipeType.Food : RecipeType.Drink,
                    IsFavorite = false,
                    CreatedBy = UserService.CurrentUser.Username
                };

                foreach (var ingredientText in _ingredients)
                {
                    recipe.Ingredients.Add(new Ingredient { Name = ingredientText });
                }

                foreach (var step in _steps)
                {
                    recipe.Steps.Add(new RecipeStep
                    {
                        StepNumber = step.StepNumber,
                        Instruction = step.Instruction
                    });
                }

                AddDefaultTags(recipe);

                _context.Recipes.Add(recipe);
                _context.SaveChanges();

                UpdateUserCreatedRecipes(recipe.RecipeId);

                MessageBox.Show("Рецепт успішно створено!", "Успіх",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні рецепту: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetDefaultImage()
        {
            return FoodTypeRadio.IsChecked == true ?
                "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=400&h=300" :
                "https://images.unsplash.com/photo-1544145945-f90425340c7e?auto=format&fit=crop&w=400&h=300";
        }

        private void AddDefaultTags(Recipe recipe)
        {
            if (FoodTypeRadio.IsChecked == true)
            {
                recipe.Tags.Add(new Tag { Name = "#свіжий" });
                recipe.Tags.Add(new Tag { Name = "#домашній" });
            }
            else
            {
                recipe.Tags.Add(new Tag { Name = "#освіжаючий" });
                recipe.Tags.Add(new Tag { Name = "#домашній" });
            }
        }

        private void UpdateUserCreatedRecipes(int recipeId)
        {
            try
            {
                var createdIds = GetCreatedRecipes();

                if (!createdIds.Contains(recipeId))
                {
                    createdIds.Add(recipeId);
                    UpdateCreatedRecipes(createdIds);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка оновлення створених рецептів: {ex.Message}");
            }
        }

        private List<int> GetCreatedRecipes()
        {
            if (!UserService.IsLoggedIn || string.IsNullOrEmpty(UserService.CurrentUser.CreatedRecipesIds))
                return new List<int>();

            try
            {
                return JsonSerializer.Deserialize<List<int>>(UserService.CurrentUser.CreatedRecipesIds) ?? new List<int>();
            }
            catch
            {
                return new List<int>();
            }
        }

        private void UpdateCreatedRecipes(List<int> recipeIds)
        {
            if (!UserService.IsLoggedIn) return;

            try
            {
                using var context = new AppDbContext();
                var user = context.Users.Find(UserService.CurrentUser.UserId);
                if (user != null)
                {
                    user.CreatedRecipesIds = JsonSerializer.Serialize(recipeIds);
                    context.SaveChanges();
                    UserService.CurrentUser.CreatedRecipesIds = user.CreatedRecipesIds;
                    UserService.SaveUserData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка оновлення створених рецептів: {ex.Message}");
            }
        }

        private bool ValidateForm()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                errors.Add("• Введіть назву рецепту");
            }
            else if (TitleTextBox.Text.Trim().Length < 3)
            {
                errors.Add("• Назва рецепту має містити щонайменше 3 символи");
            }

            if (string.IsNullOrWhiteSpace(ShortDescriptionTextBox.Text))
            {
                errors.Add("• Введіть короткий опис");
            }

            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
            {
                errors.Add("• Введіть повний опис рецепту");
            }

            if (string.IsNullOrWhiteSpace(PreparationTimeTextBox.Text) ||
                !int.TryParse(PreparationTimeTextBox.Text, out int prepTime) ||
                prepTime <= 0)
            {
                errors.Add("• Введіть коректний час приготування (більше 0 хвилин)");
            }

            if (string.IsNullOrWhiteSpace(ServingsTextBox.Text) ||
                !int.TryParse(ServingsTextBox.Text, out int servings) ||
                servings <= 0)
            {
                errors.Add("• Введіть коректну кількість порцій (більше 0)");
            }

            if (_ingredients.Count == 0)
            {
                errors.Add("• Додайте щонайменше один інгредієнт");
            }

            if (_steps.Count == 0)
            {
                errors.Add("• Додайте щонайменше один крок приготування");
            }

            if (errors.Count > 0)
            {
                ShowValidationError(errors);
                return false;
            }

            HideValidationError();
            return true;
        }

        private void TitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void ShortDescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void DescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void ImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            AddIngredient();
        }

        private void NewIngredientTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddIngredient();
                e.Handled = true;
            }
        }

        private void DeleteIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string ingredient)
            {
                _ingredients.Remove(ingredient);
                UpdatePreview();
            }
        }

        private void AddStep_Click(object sender, RoutedEventArgs e)
        {
            AddStep();
        }

        private void NewStepTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                AddStep();
                e.Handled = true;
            }
        }

        private void DeleteStep_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is RecipeStep step)
            {
                _steps.Remove(step);
                for (int i = 0; i < _steps.Count; i++)
                {
                    _steps[i].StepNumber = i + 1;
                }
                _currentStepNumber = _steps.Count + 1;
                UpdatePreview();
            }
        }

        private void AddIngredient()
        {
            var ingredient = NewIngredientTextBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(ingredient))
            {
                _ingredients.Add(ingredient);
                NewIngredientTextBox.Clear();
                UpdatePreview();
                HideValidationError();
            }
        }

        private void AddStep()
        {
            var instruction = NewStepTextBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(instruction))
            {
                var step = new RecipeStep
                {
                    StepNumber = _currentStepNumber,
                    Instruction = instruction
                };

                _steps.Add(step);
                _currentStepNumber++;
                NewStepTextBox.Clear();
                UpdatePreview();
                HideValidationError();
            }
        }

        private void UpdatePreview()
        {
            PreviewTitleText.Text = string.IsNullOrWhiteSpace(TitleTextBox.Text)
                ? "Назва рецепту"
                : TitleTextBox.Text;

            PreviewDescriptionText.Text = string.IsNullOrWhiteSpace(ShortDescriptionTextBox.Text)
                ? "Опис рецепту..."
                : ShortDescriptionTextBox.Text;

            if (int.TryParse(PreparationTimeTextBox.Text, out int time) && time > 0)
            {
                PreviewTimeText.Text = time.ToString();
            }
            else
            {
                PreviewTimeText.Text = "0";
            }

            if (int.TryParse(ServingsTextBox.Text, out int servings) && servings > 0)
            {
                PreviewServingsText.Text = servings.ToString();
            }
            else
            {
                PreviewServingsText.Text = "0";
            }

            PreviewTypeText.Text = FoodTypeRadio.IsChecked == true ? "🍳 Страва" : "🥤 Напій";

            UpdatePreviewImage();
        }

        private void UpdatePreviewImage()
        {
            if (!string.IsNullOrWhiteSpace(ImagePathTextBox.Text) &&
                Uri.TryCreate(ImagePathTextBox.Text, UriKind.Absolute, out Uri uri))
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = uri;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    PreviewImage.Source = bitmap;
                }
                catch
                {
                    SetDefaultPreviewImage();
                }
            }
            else
            {
                SetDefaultPreviewImage();
            }
        }

        private void ShowValidationError(List<string> errors)
        {
            ValidationMessageText.Text = string.Join("\n", errors);
            ValidationBorder.Visibility = Visibility.Visible;
        }

        private void HideValidationError()
        {
            ValidationBorder.Visibility = Visibility.Collapsed;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}