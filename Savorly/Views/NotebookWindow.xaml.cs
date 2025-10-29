using Savorly.Models;
using Savorly.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Savorly.Views
{
    public partial class NotebookWindow : Window
    {
        public class NotebookItem
        {
            public string Name { get; set; }
            public bool IsChecked { get; set; }
        }

        private ObservableCollection<NotebookItem> _notebookItems;

        public NotebookWindow()
        {
            InitializeComponent();
            _notebookItems = new ObservableCollection<NotebookItem>();
            NotebookListBox.ItemsSource = _notebookItems;
            LoadShoppingList(); 
            UpdateStatistics();
        }

        public NotebookWindow(List<Ingredient> ingredients) : this()
        {
            foreach (var ingredient in ingredients)
            {
                _notebookItems.Add(new NotebookItem
                {
                    Name = $"{ingredient.Name} {ingredient.Amount} {ingredient.Unit}".Trim(),
                    IsChecked = false
                });
            }
            SaveShoppingList(); 
            UpdateStatistics();
        }

        private void LoadShoppingList()
        {
            if (!UserService.IsLoggedIn) return;

            try
            {
                var savedList = UserService.GetShoppingList();
                if (!string.IsNullOrEmpty(savedList))
                {
                    var items = JsonSerializer.Deserialize<List<NotebookItem>>(savedList);
                    if (items != null)
                    {
                        _notebookItems.Clear();
                        foreach (var item in items)
                        {
                            _notebookItems.Add(item);
                        }
                    }
                }
            }
            catch
            {
                
            }
        }

        private void SaveShoppingList()
        {
            if (!UserService.IsLoggedIn) return;

            try
            {
                var itemsList = _notebookItems.ToList();
                var json = JsonSerializer.Serialize(itemsList);
                UserService.UpdateShoppingList(json);
            }
            catch
            {
                
            }
        }

        private void AddNewItem()
        {
            if (!string.IsNullOrWhiteSpace(NewItemTextBox.Text))
            {
                _notebookItems.Add(new NotebookItem
                {
                    Name = NewItemTextBox.Text.Trim(),
                    IsChecked = false
                });
                NewItemTextBox.Text = string.Empty;
                SaveShoppingList(); 
                UpdateStatistics();
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is NotebookItem item)
            {
                _notebookItems.Remove(item);
                SaveShoppingList(); 
                UpdateStatistics();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox?.DataContext is NotebookItem item)
            {
                item.IsChecked = true;
                SaveShoppingList(); 
                UpdateStatistics();
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox?.DataContext is NotebookItem item)
            {
                item.IsChecked = false;
                SaveShoppingList(); 
                UpdateStatistics();
            }
        }

        private void ClearCompleted_Click(object sender, RoutedEventArgs e)
        {
            var completedItems = _notebookItems.Where(item => item.IsChecked).ToList();
            foreach (var item in completedItems)
            {
                _notebookItems.Remove(item);
            }
            SaveShoppingList(); 
            UpdateStatistics();
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            _notebookItems.Clear();
            SaveShoppingList(); 
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            var total = _notebookItems.Count;
            var completed = _notebookItems.Count(item => item.IsChecked);
            var remaining = total - completed;

            TotalCountText.Text = total.ToString();
            CompletedCountText.Text = completed.ToString();
            RemainingCountText.Text = remaining.ToString();
            ItemsCountText.Text = $"({total})";
        }

        private void AddCustomItem_Click(object sender, RoutedEventArgs e)
        {
            AddNewItem();
        }

        private void NewItemTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddNewItem();
            }
        }

        private void PrintList_Click(object sender, RoutedEventArgs e)
        {
            if (_notebookItems.Count == 0)
            {
                return;
            }

            var remainingItems = _notebookItems.Where(item => !item.IsChecked).ToList();
            if (remainingItems.Count == 0)
            {
                return;
            }

            string list = "📝 Список покупок:\n\n";
            foreach (var item in remainingItems)
            {
                list += $"• {item.Name}\n";
            }
           
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SaveShoppingList(); 
            this.Close();
        }
    }
}