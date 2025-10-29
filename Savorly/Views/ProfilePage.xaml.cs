using System.Windows;
using System.Windows.Controls;
using Savorly.Data;
using Savorly.Models;
using Savorly.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Savorly.Views
{
    public partial class ProfilePage : Page
    {
        private AppDbContext _context;

        public ProfilePage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            Loaded += ProfilePage_Loaded;
        }

        private void ProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            CheckUserLogin();
        }

        private void CheckUserLogin()
        {
            if (UserService.IsLoggedIn)
            {
                ShowProfileScreen();
            }
            else
            {
                ShowLoginScreen();
            }
        }

        private void ShowLoginScreen()
        {
            LoginSection.Visibility = Visibility.Visible;
            ProfileSection.Visibility = Visibility.Collapsed;
        }

        private void ShowProfileScreen()
        {
            LoginSection.Visibility = Visibility.Collapsed;
            ProfileSection.Visibility = Visibility.Visible;
            LoadUserData();
        }

        private void LoadUserData()
        {
            var user = UserService.GetCurrentUser();
            if (user != null)
            {
                UserNameText.Text = user.Username;
                UserEmailText.Text = user.Email;
                var recipeCount = _context.Recipes.Count(r => r.CreatedBy == user.Username);

                var favoritesCount = _context.Recipes.Count(r => r.IsFavorite);

                UserStatsText.Text = $"{recipeCount} створено • {favoritesCount} в обраному";
            }
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() == true)
            {
                ShowProfileScreen();
            }
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            if (registerWindow.ShowDialog() == true)
            {
                ShowProfileScreen();
            }
        }

        private void CreateRecipeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!UserService.IsLoggedIn)
            {
                ShowMessage("Будь ласка, увійдіть у систему");
                return;
            }

            var createRecipeWindow = new CreateRecipeWindow();
            createRecipeWindow.ShowDialog();

            LoadUserData();
        }

        private void MyRecipesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!UserService.IsLoggedIn)
            {
                ShowMessage("Будь ласка, увійдіть у систему");
                return;
            }

            var myRecipesPage = new MyRecipesPage();
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(myRecipesPage);
        }

        private void MyFavoritesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!UserService.IsLoggedIn)
            {
                ShowMessage("Будь ласка, увійдіть у систему");
                return;
            }
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.FavoritesBtn.IsChecked = true;
            }
        }

        private void NotebookBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!UserService.IsLoggedIn)
            {
                ShowMessage("Будь ласка, увійдіть у систему");
                return;
            }

            var notebookWindow = new NotebookWindow();
            notebookWindow.ShowDialog();
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            UserService.Logout();
            ShowLoginScreen();
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(message, "Savorly", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}