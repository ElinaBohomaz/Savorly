using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Savorly.Data;
using Savorly.Models;
using Savorly.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace Savorly.Views
{
    public partial class EditRecipeWindow : Window
    {
        private AppDbContext _context;
        private ObservableCollection<string> _ingredients;
        private ObservableCollection<RecipeStep> _steps;
        private int _currentStepNumber = 1;
        private Recipe _originalRecipe;

        public EditRecipeWindow(Recipe recipe)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _originalRecipe = recipe;

            _ingredients = new ObservableCollection<string>();
            _steps = new ObservableCollection<RecipeStep>();

            IngredientsListBox.ItemsSource = _ingredients;
            StepsListBox.ItemsSource = _steps;

            LoadRecipeData(recipe);
        }

        private void LoadRecipeData(Recipe recipe)
        {
            try
            {
                var fullRecipe = _context.Recipes
                    .Include(r => r.Ingredients)
                    .Include(r => r.Steps)
                    .Include(r => r.Tags)
                    .FirstOrDefault(r => r.RecipeId == recipe.RecipeId);

                if (fullRecipe != null)
                {
                    TitleTextBox.Text = fullRecipe.Title;
                    ShortDescriptionTextBox.Text = fullRecipe.ShortDescription;
                    DescriptionTextBox.Text = fullRecipe.Description;
                    PreparationTimeTextBox.Text = fullRecipe.PreparationTime.ToString();
                    ServingsTextBox.Text = fullRecipe.Servings.ToString();
                    ImagePathTextBox.Text = fullRecipe.ImagePath;

                    if (fullRecipe.Type == RecipeType.Food)
                    {
                        FoodTypeRadio.IsChecked = true;
                    }
                    else
                    {
                        DrinkTypeRadio.IsChecked = true;
                    }

                    _ingredients.Clear();
                    foreach (var ingredient in fullRecipe.Ingredients)
                    {
                        _ingredients.Add(ingredient.Name);
                    }

                    _steps.Clear();
                    foreach (var step in fullRecipe.Steps.OrderBy(s => s.StepNumber))
                    {
                        _steps.Add(new RecipeStep
                        {
                            StepNumber = step.StepNumber,
                            Instruction = step.Instruction
                        });
                    }
                    _currentStepNumber = _steps.Count + 1;

                    UpdatePreview();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження рецепту: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            try
            {
                var recipe = _context.Recipes
                    .Include(r => r.Ingredients)
                    .Include(r => r.Steps)
                    .Include(r => r.Tags)
                    .FirstOrDefault(r => r.RecipeId == _originalRecipe.RecipeId);

                if (recipe != null)
                {
                    recipe.Title = TitleTextBox.Text.Trim();
                    recipe.ShortDescription = ShortDescriptionTextBox.Text.Trim();
                    recipe.Description = DescriptionTextBox.Text.Trim();
                    recipe.ImagePath = string.IsNullOrWhiteSpace(ImagePathTextBox.Text) ?
                                     GetDefaultImage() : ImagePathTextBox.Text.Trim();
                    recipe.PreparationTime = int.Parse(PreparationTimeTextBox.Text);
                    recipe.Servings = int.Parse(ServingsTextBox.Text);
                    recipe.Type = FoodTypeRadio.IsChecked == true ? RecipeType.Food : RecipeType.Drink;

                    recipe.Ingredients.Clear();
                    foreach (var ingredientText in _ingredients)
                    {
                        recipe.Ingredients.Add(new Ingredient { Name = ingredientText });
                    }

                    recipe.Steps.Clear();
                    foreach (var step in _steps)
                    {
                        recipe.Steps.Add(new RecipeStep
                        {
                            StepNumber = step.StepNumber,
                            Instruction = step.Instruction
                        });
                    }

                    recipe.Tags.Clear();
                    AddDefaultTags(recipe);

                    _context.SaveChanges();

                    MessageBox.Show("Рецепт успішно оновлено!", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при оновленні рецепту: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetForm_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Ви впевнені, що хочете скинути всі зміни? Поточні зміни будуть втрачені.",
                "Скидання змін", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                LoadRecipeData(_originalRecipe);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("У вас є незбережені зміни. Вийти без збереження?",
                "Незбережені зміни", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("У вас є незбережені зміни. Скасувати редагування?",
                "Незбережені зміни", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.DialogResult = false;
                this.Close();
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

        private void SetDefaultPreviewImage()
        {
            PreviewImage.Source = null;
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
    }
}