using System.Windows;
using System.Windows.Controls;
using Savorly.Views;

namespace Savorly
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FoodBtn.IsChecked = true;
            NavigateToFoodPage();
        }

        private void FoodBtn_Checked(object sender, RoutedEventArgs e)
        {
            NavigateToFoodPage();
        }

        private void DrinksBtn_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DrinksPage());
        }

        private void SearchBtn_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SearchPage());
        }

        private void FavoritesBtn_Checked(object sender, RoutedEventArgs e)
        {
            var favoritesPage = new FavoritesPage();
            MainFrame.Navigate(favoritesPage);

            if (MainFrame.Content is FavoritesPage page)
            {
                page.RefreshFavorites();
            }
        }

        private void ProfileBtn_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProfilePage());
        }

        private void NavigateToFoodPage()
        {
            MainFrame.Navigate(new FoodPage());
        }
    }
}