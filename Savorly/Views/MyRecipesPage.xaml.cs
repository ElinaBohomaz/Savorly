using System.Windows;
using System.Windows.Controls;
using Savorly.Data;
using Savorly.Models;
using Savorly.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Savorly.Views
{
    public partial class MyRecipesPage : Page
    {
        private AppDbContext _context;

        public MyRecipesPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            Loaded += MyRecipesPage_Loaded;
        }

        private void MyRecipesPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMyRecipes();
        }

        private void LoadMyRecipes()
        {
            if (!UserService.IsLoggedIn)
            {
                ShowNoRecipesMessage("Будь ласка, увійдіть у систему");
                return;
            }

            try
            {
                var myRecipes = _context.Recipes
                    .Include(r => r.Tags)
                    .Include(r => r.Ingredients)
                    .Include(r => r.Steps)
                    .Where(r => r.CreatedBy == UserService.CurrentUser.Username)
                    .OrderByDescending(r => r.RecipeId)
                    .ToList();

                MyRecipesItemsControl.ItemsSource = myRecipes;

                if (myRecipes.Count == 0)
                {
                    ShowNoRecipesMessage("У вас ще немає рецептів");
                }
                else
                {
                    NoRecipesMessage.Visibility = Visibility.Collapsed;
                    MyRecipesItemsControl.Visibility = Visibility.Visible;
                }
            }
            catch (System.Exception ex)
            {
                ShowNoRecipesMessage("Помилка завантаження рецептів");
                System.Diagnostics.Debug.WriteLine($"❌ Помилка завантаження моїх рецептів: {ex.Message}");
            }
        }

        private void ShowNoRecipesMessage(string message)
        {
            if (NoRecipesMessage.Child is StackPanel stackPanel)
            {
                foreach (var child in stackPanel.Children)
                {
                    if (child is TextBlock textBlock && textBlock.FontSize == 20)
                    {
                        textBlock.Text = message;
                        break;
                    }
                }
            }

            NoRecipesMessage.Visibility = Visibility.Visible;
            MyRecipesItemsControl.Visibility = Visibility.Collapsed;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Recipe recipe)
            {
                var editWindow = new EditRecipeWindow(recipe);
                if (editWindow.ShowDialog() == true)
                {
                    LoadMyRecipes();

                    MessageBox.Show("Рецепт успішно оновлено", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Recipe recipe)
            {
                var result = MessageBox.Show($"Ви впевнені, що хочете видалити рецепт \"{recipe.Title}\"?",
                    "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Recipes.Remove(recipe);
                        _context.SaveChanges();
                        LoadMyRecipes();

                        MessageBox.Show("Рецепт успішно видалено", "Успіх",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Помилка видалення рецепту: {ex.Message}", "Помилка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ViewRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Recipe recipe)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Navigate(new RecipeDetailPage(recipe.RecipeId));
                }
            }
        }

        private void CreateRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!UserService.IsLoggedIn)
            {
                MessageBox.Show("Будь ласка, увійдіть у систему", "Помилка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var createWindow = new CreateRecipeWindow();
            if (createWindow.ShowDialog() == true)
            {
                LoadMyRecipes();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.ProfileBtn.IsChecked = true;
                mainWindow.MainFrame.Navigate(new ProfilePage());

                System.Diagnostics.Debug.WriteLine("🔙 Повернення до профілю зі сторінки 'Мої рецепти'");
            }
        }

        public void RefreshRecipes()
        {
            LoadMyRecipes();
        }
    }
}