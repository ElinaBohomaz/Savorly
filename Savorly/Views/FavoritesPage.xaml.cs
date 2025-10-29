using Microsoft.EntityFrameworkCore;
using Savorly.Data;
using Savorly.Models;
using Savorly.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Savorly.Views
{
    public partial class FavoritesPage : Page
    {
        private AppDbContext _context;
        private List<Recipe> _allFavorites;
        private RecipeType? _currentFavoritesFilter = null;

        public FavoritesPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            _allFavorites = new List<Recipe>();

            Loaded += FavoritesPage_Loaded;
        }

        private void FavoritesPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFavorites();
        }

        private void LoadFavorites()
        {
            try
            {
                if (UserService.IsLoggedIn)
                {
                    var favoriteIds = UserService.GetFavoriteRecipes();

                    if (favoriteIds != null && favoriteIds.Any())
                    {
                        _allFavorites = _context.Recipes
                            .Include(r => r.Tags)
                            .Include(r => r.Ingredients)
                            .Include(r => r.Steps)
                            .Where(r => favoriteIds.Contains(r.RecipeId))
                            .ToList();

                        System.Diagnostics.Debug.WriteLine($"✅ Завантажено {_allFavorites.Count} улюблених рецептів");
                    }
                    else
                    {
                        _allFavorites = new List<Recipe>();
                        System.Diagnostics.Debug.WriteLine("ℹ️ У користувача немає улюблених рецептів");
                    }
                }
                else
                {
                    _allFavorites = new List<Recipe>();
                    System.Diagnostics.Debug.WriteLine("❌ Користувач не авторизований");
                    ShowNotLoggedInMessage();
                    return;
                }

                UpdateFavoritesDisplay();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 Помилка завантаження улюблених: {ex.Message}");
                _allFavorites = new List<Recipe>();
                UpdateFavoritesDisplay();
            }
        }

        private void ShowNotLoggedInMessage()
        {
            if (NoFavoritesMessage != null)
            {
                NoFavoritesMessage.Visibility = Visibility.Visible;
                FavoritesItemsControl.Visibility = Visibility.Collapsed;

                var stackPanel = NoFavoritesMessage.Child as StackPanel;
                if (stackPanel != null)
                {
                    foreach (var child in stackPanel.Children)
                    {
                        if (child is TextBlock textBlock)
                        {
                            if (textBlock.FontSize == 20)
                            {
                                textBlock.Text = "Будь ласка, увійдіть у систему";
                            }
                            else if (textBlock.FontSize == 14)
                            {
                                textBlock.Text = "Увійдіть, щоб переглянути свої обрані рецепти";
                            }
                        }
                        else if (child is Button button)
                        {
                            button.Content = "Увійти в систему";
                            button.Click -= GoToRecipesButton_Click;
                            button.Click += GoToLoginButton_Click;
                        }
                    }
                }
            }
        }

        private void GoToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Owner = Application.Current.MainWindow;
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            loginWindow.ShowDialog();

            if (UserService.IsLoggedIn)
            {
                LoadFavorites();
            }
        }

        private void RemoveFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.DataContext is Recipe recipe)
                {
                    if (UserService.IsLoggedIn)
                    {
                        var favoriteIds = UserService.GetFavoriteRecipes();
                        if (favoriteIds.Contains(recipe.RecipeId))
                        {
                            favoriteIds.Remove(recipe.RecipeId);
                            UserService.UpdateFavoriteRecipes(favoriteIds);
                            System.Diagnostics.Debug.WriteLine($"🗑️ Видалено рецепт {recipe.RecipeId} з улюблених");
                        }
                    }

                    _allFavorites.Remove(recipe);
                    UpdateFavoritesDisplay();

                    MessageBox.Show($"Рецепт \"{recipe.Title}\" видалено з обраного", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 Помилка видалення: {ex.Message}");
                MessageBox.Show($"Помилка видалення з обраного: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateFavoritesDisplay()
        {
            try
            {
                if (FavoritesItemsControl == null || NoFavoritesMessage == null || FavoritesCountText == null)
                    return;

                var filteredFavorites = _allFavorites.AsQueryable();
                if (_currentFavoritesFilter.HasValue)
                {
                    filteredFavorites = filteredFavorites.Where(r => r.Type == _currentFavoritesFilter.Value);
                }

                var finalResults = filteredFavorites.ToList();
                FavoritesItemsControl.ItemsSource = finalResults;
                UpdateFavoritesCount(finalResults.Count);

                NoFavoritesMessage.Visibility = finalResults.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                FavoritesItemsControl.Visibility = finalResults.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

                System.Diagnostics.Debug.WriteLine($"🔄 Відображено {finalResults.Count} рецептів у обраному");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 Помилка оновлення відображення: {ex.Message}");
                FavoritesItemsControl.ItemsSource = new List<Recipe>();
                NoFavoritesMessage.Visibility = Visibility.Visible;
                FavoritesItemsControl.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateFavoritesCount(int count)
        {
            if (FavoritesCountText == null) return;

            string typeText = _currentFavoritesFilter switch
            {
                RecipeType.Food => "страв",
                RecipeType.Drink => "напоїв",
                _ => "рецептів"
            };

            FavoritesCountText.Text = $"Знайдено {count} {typeText} в обраному";
        }

        private void FavoritesFilter_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                switch (radioButton.Content.ToString())
                {
                    case "Усі обране":
                        _currentFavoritesFilter = null;
                        break;
                    case "🍳 Страви":
                        _currentFavoritesFilter = RecipeType.Food;
                        break;
                    case "🥤 Напої":
                        _currentFavoritesFilter = RecipeType.Drink;
                        break;
                }

                UpdateFavoritesDisplay();
            }
        }

        private void RecipeCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Recipe recipe)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Navigate(new RecipeDetailPage(recipe.RecipeId));
                }
            }
        }

        private void GoToRecipesButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.FoodBtn.IsChecked = true;
            }
        }

        public void RefreshFavorites()
        {
            LoadFavorites();
        }
    }
}