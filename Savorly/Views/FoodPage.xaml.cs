using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Savorly.Data;
using Savorly.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Windows.Media;
using Savorly.Services;

namespace Savorly.Views
{
    public partial class FoodPage : Page
    {
        private AppDbContext _context;
        private List<Recipe> _allRecipes;

        public FoodPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadRecipes();
            SetupCategoryClickHandlers();
        }

        private void LoadRecipes()
        {
            _allRecipes = _context.Recipes
                .Include(r => r.Tags)
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Where(r => r.Type == RecipeType.Food)
                .ToList();

            FavoriteService.UpdateRecipesFavoriteStatus(_context, _allRecipes);

            UpdateRecipeDisplay();
        }

        private void UpdateRecipeDisplay()
        {
            RecipesItemsControl.ItemsSource = null;
            RecipesItemsControl.ItemsSource = _allRecipes;
        }

        private void SetupCategoryClickHandlers()
        {
            foreach (var child in CategoriesPanel.Children)
            {
                if (child is Border border)
                {
                    border.MouseLeftButtonDown += (s, e) =>
                    {
                        foreach (var otherChild in CategoriesPanel.Children)
                        {
                            if (otherChild is Border otherBorder)
                            {
                                otherBorder.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                                var otherTextBlock = otherBorder.Child as TextBlock;
                                if (otherTextBlock != null)
                                {
                                    otherTextBlock.Foreground = Brushes.Black;
                                }
                            }
                        }
                        border.Background = new SolidColorBrush(Color.FromRgb(255, 107, 0));
                        var currentTextBlock = border.Child as TextBlock;
                        if (currentTextBlock != null)
                        {
                            currentTextBlock.Foreground = Brushes.White;
                        }

                        FilterRecipesByCategory(currentTextBlock.Text);
                    };
                }
            }
        }

        private void FilterRecipesByCategory(string category)
        {
            if (category == "#усі")
            {
                RecipesItemsControl.ItemsSource = _allRecipes;
            }
            else
            {
                var filteredRecipes = _allRecipes.Where(r =>
                    r.Tags.Any(t => t.Name.Contains(category.Replace("#", ""))) ||
                    r.Title.ToLower().Contains(category.Replace("#", "").ToLower())
                ).ToList();
                RecipesItemsControl.ItemsSource = filteredRecipes;
            }
        }

        private void ScrollLeft_Click(object sender, RoutedEventArgs e)
        {
            CategoriesScrollViewer.ScrollToHorizontalOffset(CategoriesScrollViewer.HorizontalOffset - 200);
        }

        private void ScrollRight_Click(object sender, RoutedEventArgs e)
        {
            CategoriesScrollViewer.ScrollToHorizontalOffset(CategoriesScrollViewer.HorizontalOffset + 200);
        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Border border && border.DataContext is Recipe recipe)
            {
                try
                {
                    FavoriteService.ToggleFavorite(recipe, _context);
                    UpdateRecipeDisplay();

                    System.Diagnostics.Debug.WriteLine($"⭐ {recipe.Title}: {(recipe.IsFavorite ? "додано до" : "видалено з")} улюблених");
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("не авторизований"))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Користувач не авторизований для додавання в улюблені");
                    MessageBox.Show("Будь ласка, увійдіть в систему, щоб додавати рецепти в обране", "Увага",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Помилка оновлення улюблених: {ex.Message}");
                    MessageBox.Show($"Помилка: {ex.Message}", "Помилка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            e.Handled = true;
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

        private void ShowMessage(string message)
        {
            MessageBox.Show(message, "Savorly", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}