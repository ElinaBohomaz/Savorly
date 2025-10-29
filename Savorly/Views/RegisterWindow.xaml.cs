using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Savorly.Services;

namespace Savorly.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();

            Loaded += (s, e) => UsernameTextBox.Focus();

            KeyDown += RegisterWindow_KeyDown;
        }

        private void RegisterWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RegisterButton_Click(sender, e);
            }
            else if (e.Key == Key.Escape)
            {
                CancelButton_Click(sender, e);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(username))
            {
                ShowError("Будь ласка, введіть ім'я користувача");
                UsernameTextBox.Focus();
                return;
            }

            if (username.Length < 3)
            {
                ShowError("Ім'я користувача має містити щонайменше 3 символи");
                UsernameTextBox.Focus();
                UsernameTextBox.SelectAll();
                return;
            }

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

            if (password.Length < 6)
            {
                ShowError("Пароль має містити щонайменше 6 символів");
                PasswordBox.Focus();
                PasswordBox.SelectAll();
                return;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                ShowError("Будь ласка, підтвердіть пароль");
                ConfirmPasswordBox.Focus();
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Паролі не співпадають");
                ConfirmPasswordBox.Focus();
                ConfirmPasswordBox.SelectAll();
                return;
            }

            if (TermsCheckBox.IsChecked != true)
            {
                ShowError("Будь ласка, погодьтесь з умовами використання");
                return;
            }

            var result = UserService.Register(username, email, password, confirmPassword);

            if (result.success)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                ShowError(result.message);

                if (result.message.Contains("ім'я"))
                    UsernameTextBox.Focus();
                else if (result.message.Contains("email"))
                    EmailTextBox.Focus();
                else
                    PasswordBox.Focus();
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

        private void LoginHyperlink_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();

            var loginWindow = new LoginWindow();
            loginWindow.Owner = Application.Current.MainWindow;
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            loginWindow.ShowDialog();
        }

        private void TermsText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Умови використання:\n\n• Ви відповідаєте за зміст ваших рецептів\n• Заборонено публікувати образливий контент\n• Ми зберігаємо ваші дані безпечно\n• Ви можете видалити акаунт у будь-який час",
                           "Умови використання",
                           MessageBoxButton.OK,
                           MessageBoxImage.Information);
        }
    }
}