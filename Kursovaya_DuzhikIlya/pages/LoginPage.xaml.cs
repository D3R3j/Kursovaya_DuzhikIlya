using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kursovaya_DuzhikIlya.pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            var user = Manager.Context.Users
                .FirstOrDefault(u => u.Login == login);

            if (user == null)
            {
                MessageBox.Show("Пользователь не найден!");
                return;
            }

            // Сравнение хэша пароля (предполагается, что пароль хранится в виде хэша)
            if (VerifyPassword(password, user.PasswordHash))
            {
                MessageBox.Show("Авторизация успешна!");
                Manager.MainFrame.Navigate(new MainPage());
            }
            else
            {
                MessageBox.Show("Неверный пароль!");
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new RegisterPage());
        }

        // Метод для проверки хэша пароля (реализуйте свой алгоритм)
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Пример: использование SHA256 для сравнения
            using (var sha256 = SHA256.Create())
            {
                string hash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword)));
                return hash == storedHash;
            }
        }
    }
}
