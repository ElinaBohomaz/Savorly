using System.Windows;
using System.Windows.Controls;
using Savorly.Models;
using Savorly.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Savorly.Services;

namespace Savorly.Views
{
    public partial class RecipeDetailPage : Page
    {
        private AppDbContext _context;
        private Recipe _currentRecipe;

        public RecipeDetailPage(int recipeId)
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadRecipe(recipeId);
        }

        private void LoadRecipe(int recipeId)
        {
            try
            {
                _currentRecipe = _context.Recipes
                    .Include(r => r.Ingredients)
                    .Include(r => r.Steps)
                    .Include(r => r.Tags)
                    .FirstOrDefault(r => r.RecipeId == recipeId);

                if (_currentRecipe == null)
                {
                    System.Diagnostics.Debug.WriteLine($"🔍 Рецепт {recipeId} не знайдено в базі, шукаємо через DatabaseService...");

                    var drinks = DatabaseService.GetRecipesByTypeWithDetails(RecipeType.Drink);
                    var foods = DatabaseService.GetRecipesByTypeWithDetails(RecipeType.Food);

                    _currentRecipe = drinks.FirstOrDefault(r => r.RecipeId == recipeId)
                                   ?? foods.FirstOrDefault(r => r.RecipeId == recipeId);

                    if (_currentRecipe != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ Рецепт знайдено через DatabaseService: {_currentRecipe.Title}");
                    }
                }

                if (_currentRecipe != null)
                {
                    this.DataContext = _currentRecipe;

                    var ingredients = _currentRecipe.Ingredients?
                        .OrderBy(i => i.IngredientId)
                        .ToList() ?? new List<Ingredient>();
                    IngredientsItemsControl.ItemsSource = ingredients;

                    var steps = _currentRecipe.Steps?
                        .OrderBy(s => s.StepNumber)
                        .ToList() ?? new List<RecipeStep>();
                    StepsItemsControl.ItemsSource = steps;

                    System.Diagnostics.Debug.WriteLine($"📋 Завантажено: {ingredients.Count} інгредієнтів, {steps.Count} кроків");

                    UpdateTagsDisplay();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Рецепт з ID {recipeId} не знайдено ні в базі, ні через сервіс");
                    ShowErrorMessage("Рецепт не знайдено");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 Помилка завантаження рецепту: {ex.Message}");
                ShowErrorMessage($"Помилка завантаження: {ex.Message}");
            }
        }

        private void UpdateTagsDisplay()
        {
            if (_currentRecipe?.Tags != null && _currentRecipe.Tags.Any())
            {
                var tagsText = string.Join(" • ", _currentRecipe.Tags.Select(t => t.Name));

                var tagsContainer = FindName("TagsContainer") as Border;
                if (tagsContainer == null)
                {
                    var mainGrid = FindName("MainGrid") as Grid;
                    if (mainGrid != null)
                    {
                        var tagsRow = new RowDefinition { Height = GridLength.Auto };
                        mainGrid.RowDefinitions.Insert(4, tagsRow); 
                        tagsContainer = new Border
                        {
                            Name = "TagsContainer",
                            Background = System.Windows.Media.Brushes.White,
                            Margin = new Thickness(40, 0, 40, 20),
                            Padding = new Thickness(0, 0, 0, 0)
                        };

                        var tagsTextBlock = new TextBlock
                        {
                            Text = tagsText,
                            FontSize = 14,
                            Foreground = new System.Windows.Media.SolidColorBrush(
                                System.Windows.Media.Color.FromRgb(204, 85, 0)),
                            TextWrapping = TextWrapping.Wrap,
                            FontWeight = FontWeights.Medium
                        };

                        tagsContainer.Child = tagsTextBlock;
                        Grid.SetRow(tagsContainer, 4);
                        mainGrid.Children.Add(tagsContainer);
                    }
                }
                else
                {
                    var tagsTextBlock = tagsContainer.Child as TextBlock;
                    if (tagsTextBlock != null)
                    {
                        tagsTextBlock.Text = tagsText;
                    }
                }
            }
        }

        private void ShowErrorMessage(string message)
        {
            var errorTextBlock = new TextBlock
            {
                Text = message,
                FontSize = 16,
                Foreground = System.Windows.Media.Brushes.Red,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };

            var backButton = new Button
            {
                Content = "Повернутися назад",
                Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(204, 85, 0)),
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 20, 0, 0),
                Padding = new Thickness(20, 10, 20, 10),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            backButton.Click += (s, e) => BackButton_Click(s, e);

            var stackPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            stackPanel.Children.Add(errorTextBlock);
            stackPanel.Children.Add(backButton);
            Content = stackPanel;
        }

        private void AddToNotebookBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentRecipe == null)
            {
                MessageBox.Show("Рецепт не завантажено");
                return;
            }

            var ingredients = _currentRecipe.Ingredients?.ToList() ?? new List<Ingredient>();

            if (!ingredients.Any())
            {
                MessageBox.Show("Інгредієнти не знайдені для цього рецепту");
                return;
            }

            var notebookWindow = new NotebookWindow(ingredients);
            notebookWindow.ShowDialog();
        }

        private void AddComment_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CommentTextBox.Text))
            {
                var newComment = new Border
                {
                    Background = System.Windows.Media.Brushes.White,
                    Margin = new Thickness(0, 0, 0, 15),
                    Padding = new Thickness(15),
                    CornerRadius = new CornerRadius(12)
                };

                var stackPanel = new StackPanel();

                var headerPanel = new StackPanel { Orientation = Orientation.Horizontal };

                var avatar = new Border
                {
                    Width = 32,
                    Height = 32,
                    Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(76, 175, 80)),
                    CornerRadius = new CornerRadius(16)
                };

                var avatarText = new TextBlock
                {
                    Text = "В",
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                avatar.Child = avatarText;

                var userInfoPanel = new StackPanel { Margin = new Thickness(10, 0, 0, 0) };
                userInfoPanel.Children.Add(new TextBlock
                {
                    Text = "Ви",
                    FontSize = 13,
                    FontWeight = FontWeights.SemiBold
                });
                userInfoPanel.Children.Add(new TextBlock
                {
                    Text = "щойно",
                    FontSize = 11,
                    Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(153, 153, 153))
                });

                headerPanel.Children.Add(avatar);
                headerPanel.Children.Add(userInfoPanel);

                var commentText = new TextBlock
                {
                    Text = CommentTextBox.Text,
                    FontSize = 13,
                    Margin = new Thickness(0, 10, 0, 0),
                    TextWrapping = TextWrapping.Wrap,
                    LineHeight = 18
                };

                stackPanel.Children.Add(headerPanel);
                stackPanel.Children.Add(commentText);
                newComment.Child = stackPanel;

                CommentsPanel.Children.Insert(0, newComment);
                CommentTextBox.Text = string.Empty;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.GoBack();
            }
        }
    }
}