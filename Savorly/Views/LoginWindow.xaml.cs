using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Savorly.Services;

namespace Savorly.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                EmailTextBox.Focus();
            };

            KeyDown += LoginWindow_KeyDown;
        }

        private void LoginWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
            else if (e.Key == Key.Escape)
            {
                CancelButton_Click(sender, e);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(email))
            {
                ShowError("Будь ласка, введіть email");
                EmailTextBox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Будь ласка, введіть пароль");
                PasswordBox.Focus();
                return;
            }

            var result = UserService.Login(email, password);

            if (result.success)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                ShowError(result.message);
                PasswordBox.Focus();
                PasswordBox.SelectAll();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
        }

        private void RegisterHyperlink_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();

            var registerWindow = new RegisterWindow();
            registerWindow.Owner = Application.Current.MainWindow;
            registerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            registerWindow.ShowDialog();
        }
    }
}