using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Kursovaya_DuzhikIlya.pages; // Пространство имен для страниц
using Kursovaya_DuzhikIlya;

namespace Kursovaya_DuzhikIlya.pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        // Хэширование пароля с использованием SHA256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        // Обработка нажатия кнопки "Войти"
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            try
            {
                var user = Manager.Context.Users.FirstOrDefault(u => u.Login == login);
                if (user != null && user.PasswordHash == HashPassword(password))
                {
                    Manager.CurrentUser = user;

                    // Обновляем меню (перезапускаем главное окно или обновляем состояние)
                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    if (mainWindow != null)
                    {
                        mainWindow.HomeMenuItem.Visibility = Visibility.Visible;
                        mainWindow.BackMenuItem.Visibility = Visibility.Visible;

                    }

                    Manager.MainFrame.Navigate(new MainPage());
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка входа: {ex.Message}");
            }
        }

        // Обработка нажатия кнопки "Регистрация"
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу регистрации
            Manager.MainFrame.Navigate(new RegisterPage());
        }
    }
}