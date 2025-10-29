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
    public partial class DrinksPage : Page
    {
        private AppDbContext _context;
        private List<Recipe> _allDrinks;

        public DrinksPage()
        {
            InitializeComponent();
            _context = new AppDbContext();

            CheckDrinksInDatabase();
            LoadDrinks();
            SetupCategoryClickHandlers();
            CheckDrinksData();
        }

        private void CheckDrinksInDatabase()
        {
            try
            {
                using var context = new AppDbContext();

                var drinkCount = context.Recipes.Count(r => r.Type == RecipeType.Drink);
                var foodCount = context.Recipes.Count(r => r.Type == RecipeType.Food);

                System.Diagnostics.Debug.WriteLine($"🔍 ПЕРЕВІРКА БАЗИ ДАНИХ:");
                System.Diagnostics.Debug.WriteLine($"🥤 Напоїв у базі: {drinkCount}");
                System.Diagnostics.Debug.WriteLine($"🍳 Страв у базі: {foodCount}");

                var firstDrinks = context.Recipes
                    .Where(r => r.Type == RecipeType.Drink)
                    .Take(5)
                    .ToList();

                foreach (var drink in firstDrinks)
                {
                    System.Diagnostics.Debug.WriteLine($"   - {drink.Title} (ID: {drink.RecipeId})");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 ПОМИЛКА ПЕРЕВІРКИ: {ex.Message}");
            }
        }

        private void CheckDrinksData()
        {
            var totalDrinksInDb = _context.Recipes.Count(r => r.Type == RecipeType.Drink);
            System.Diagnostics.Debug.WriteLine($"🔍 Перевірка: знайдено {totalDrinksInDb} напоїв у базі даних");

            if (totalDrinksInDb == 0)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ УВАГА: Напоїв не знайдено у базі даних!");
             
            }
        }

        private void LoadDrinks()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🥤 Завантаження напоїв...");

                _allDrinks = _context.Recipes
                    .Include(r => r.Tags)
                    .Include(r => r.Ingredients)
                    .Include(r => r.Steps)
                    .Where(r => r.Type == RecipeType.Drink)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"🥤 Завантажено з бази: {_allDrinks.Count} напоїв");

                if (_allDrinks.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("🔄 Напоїв не знайдено в базі, використовуємо DatabaseService...");
                    _allDrinks = DatabaseService.GetRecipesByTypeWithDetails(RecipeType.Drink);
                    System.Diagnostics.Debug.WriteLine($"🥤 Завантажено з DatabaseService: {_allDrinks.Count} напоїв");
                }

                FavoriteService.UpdateRecipesFavoriteStatus(_context, _allDrinks);

                foreach (var drink in _allDrinks.Take(3))
                {
                    System.Diagnostics.Debug.WriteLine($"   - {drink.Title}: {drink.Ingredients?.Count ?? 0} інгр., {drink.Steps?.Count ?? 0} кроків, Favorite: {drink.IsFavorite}");
                }

                UpdateDrinksDisplay();

                if (_allDrinks.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Напоїв не знайдено!");
                    ShowMessage("Напоїв не знайдено. Спробуйте оновити базу даних.");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("✅ Напої успішно завантажено!");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Помилка завантаження напоїв: {ex.Message}");
                ShowMessage($"Помилка завантаження напоїв: {ex.Message}");
            }
        }

        private void UpdateDrinksDisplay()
        {
            DrinksItemsControl.ItemsSource = null;
            DrinksItemsControl.ItemsSource = _allDrinks;
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

                        border.Background = new SolidColorBrush(Color.FromRgb(0, 122, 204));
                        var currentTextBlock = border.Child as TextBlock;
                        if (currentTextBlock != null)
                        {
                            currentTextBlock.Foreground = Brushes.White;
                        }

                        FilterDrinksByCategory(currentTextBlock.Text);
                    };
                }
            }
        }

        private void FilterDrinksByCategory(string category)
        {
            if (_allDrinks == null || _allDrinks.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Немає напоїв для фільтрації");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"🔍 Фільтрація за категорією: {category}");

            if (category == "#усі")
            {
                DrinksItemsControl.ItemsSource = _allDrinks;
                System.Diagnostics.Debug.WriteLine($"✅ Показано всі {_allDrinks.Count} напоїв");
            }
            else
            {
                var filteredDrinks = _allDrinks.Where(r =>
                    r.Tags?.Any(t => t.Name.Contains(category.Replace("#", ""))) == true ||
                    r.Title.ToLower().Contains(category.Replace("#", "").ToLower())
                ).ToList();

                DrinksItemsControl.ItemsSource = filteredDrinks;
                System.Diagnostics.Debug.WriteLine($"✅ Показано {filteredDrinks.Count} напоїв з категорії {category}");
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
            if (sender is Border border && border.DataContext is Recipe drink)
            {
                try
                {
                    FavoriteService.ToggleFavorite(drink, _context);
                    UpdateDrinksDisplay();

                    System.Diagnostics.Debug.WriteLine($"⭐ {drink.Title}: {(drink.IsFavorite ? "додано до" : "видалено з")} улюблених");
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

        private void DrinkCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Recipe drink)
            {
                System.Diagnostics.Debug.WriteLine($"🥤 Клікнуто на напій: {drink.Title} (ID: {drink.RecipeId})");

                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🔗 Перехід до RecipeDetailPage з ID: {drink.RecipeId}");
                    mainWindow.MainFrame.Navigate(new RecipeDetailPage(drink.RecipeId));
                }
            }
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(message, "Savorli", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void RefreshDrinks()
        {
            System.Diagnostics.Debug.WriteLine("🔄 Оновлення списку напоїв...");
            LoadDrinks();
        }

        private void ReinitializeDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔄 Примусове оновлення бази даних...");

                DatabaseService.InitializeDatabase();

                MessageBox.Show("Базу даних оновлено. Перезавантажте сторінку.",
                              "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);

                RefreshDrinks();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Помилка оновлення бази: {ex.Message}");
                MessageBox.Show($"Помилка оновлення бази: {ex.Message}",
                              "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}