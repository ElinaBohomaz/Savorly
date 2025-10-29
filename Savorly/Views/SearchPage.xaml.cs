using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Savorly.Data;
using Savorly.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

namespace Savorly.Views
{
    public partial class SearchPage : Page
    {
        private AppDbContext _context;
        private List<Recipe> _allRecipes;
        private string _currentSearchText = "";
        private string _currentFilterTag = "";
        private RecipeType? _currentTypeFilter = null;
        private bool _isInitialized = false;

        public SearchPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            _allRecipes = new List<Recipe>();

            Loaded += SearchPage_Loaded;
        }

        private void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
            {
                LoadAllRecipes();
                _isInitialized = true;
            }
        }

        private void LoadAllRecipes()
        {
            try
            {
                _allRecipes = _context.Recipes
                    .Include(r => r.Tags)
                    .Include(r => r.Ingredients)
                    .Include(r => r.Steps)
                    .ToList();

                if (IsLoaded)
                {
                    UpdateSearchResults();
                }
            }
            catch (Exception ex)
            {
                _allRecipes = new List<Recipe>();
                if (IsLoaded)
                {
                    UpdateSearchResults();
                }
            }
        }

        private void UpdateSearchResults()
        {
            try
            {
                if (SearchResultsItemsControl == null || ResultsTitle == null || NoResultsMessage == null)
                    return;

                if (_allRecipes == null)
                {
                    _allRecipes = new List<Recipe>();
                }

                var results = _allRecipes.AsQueryable();

                if (!string.IsNullOrWhiteSpace(_currentSearchText))
                {
                    var searchText = _currentSearchText.ToLower();
                    results = results.Where(r =>
                        r.Title.ToLower().Contains(searchText) ||
                        r.Description.ToLower().Contains(searchText) ||
                        r.Ingredients.Any(i => i.Name.ToLower().Contains(searchText)) ||
                        r.Tags.Any(t => t.Name.ToLower().Contains(searchText)));
                }

                if (!string.IsNullOrWhiteSpace(_currentFilterTag))
                {
                    results = results.Where(r =>
                        r.Tags.Any(t => t.Name.ToLower() == _currentFilterTag.ToLower()));
                }

                if (_currentTypeFilter.HasValue)
                {
                    results = results.Where(r => r.Type == _currentTypeFilter.Value);
                }

                var finalResults = results.ToList();

                SearchResultsItemsControl.ItemsSource = finalResults;
                UpdateResultsTitle(finalResults.Count);
                NoResultsMessage.Visibility = finalResults.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                SearchResultsItemsControl.ItemsSource = new List<Recipe>();
                NoResultsMessage.Visibility = Visibility.Visible;
            }
        }

        private void UpdateResultsTitle(int count)
        {
            if (ResultsTitle == null) return;

            string title = "Всі рецепти";

            if (!string.IsNullOrWhiteSpace(_currentSearchText))
            {
                title = $"Результати пошуку '{_currentSearchText}'";
            }
            else if (!string.IsNullOrWhiteSpace(_currentFilterTag))
            {
                title = $"Рецепти з тегом {_currentFilterTag}";
            }

            if (_currentTypeFilter.HasValue)
            {
                title += _currentTypeFilter.Value == RecipeType.Food ? " (Страви)" : " (Напої)";
            }

            title += $" - {count} знайдено";

            ResultsTitle.Text = title;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _currentSearchText = SearchTextBox.Text.Trim();
            UpdateSearchResults();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateSearchResults();
        }

        private void FilterTag_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Child is TextBlock textBlock)
            {
                string tagText = textBlock.Text;

                if (tagText == "#усі")
                {
                    _currentFilterTag = "";
                }
                else
                {
                    _currentFilterTag = tagText;
                }

                UpdateTagSelection(tagText);
                UpdateSearchResults();
            }
        }

        private void UpdateTagSelection(string selectedTag)
        {
            try
            {
                if (TagsPanel == null) return;

                foreach (var child in TagsPanel.Children)
                {
                    if (child is Border tagBorder && tagBorder.Child is TextBlock tagText)
                    {
                        if (tagText.Text == selectedTag || (selectedTag == "#усі" && tagText.Text == "#усі"))
                        {
                            tagBorder.Background = new SolidColorBrush(Color.FromRgb(255, 107, 0));
                            tagText.Foreground = Brushes.White;
                        }
                        else
                        {
                            tagBorder.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                            tagText.Foreground = Brushes.Black;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void ScrollTagsLeft_Click(object sender, RoutedEventArgs e)
        {
            TagsScrollViewer.ScrollToHorizontalOffset(TagsScrollViewer.HorizontalOffset - 200);
        }

        private void ScrollTagsRight_Click(object sender, RoutedEventArgs e)
        {
            TagsScrollViewer.ScrollToHorizontalOffset(TagsScrollViewer.HorizontalOffset + 200);
        }

        private void TypeFilter_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                switch (radioButton.Content.ToString())
                {
                    case "Усі":
                        _currentTypeFilter = null;
                        break;
                    case "🍳 Страви":
                        _currentTypeFilter = RecipeType.Food;
                        break;
                    case "🥤 Напої":
                        _currentTypeFilter = RecipeType.Drink;
                        break;
                }

                UpdateSearchResults();
            }
        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Border border && border.DataContext is Recipe recipe)
                {
                    recipe.IsFavorite = !recipe.IsFavorite;
                    _context.SaveChanges();
                    UpdateSearchResults();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                ShowMessage("Помилка збереження змін");
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

        private void ShowMessage(string message)
        {
            MessageBox.Show(message, "Savorly", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}