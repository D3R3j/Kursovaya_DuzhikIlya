using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Kursovaya_DuzhikIlya.pages; // Пространство имен для страниц
using Kursovaya_DuzhikIlya; // Пространство имен для контекста базы данных

namespace Kursovaya_DuzhikIlya.pages
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
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

        // Обработка нажатия кнопки "Зарегистрироваться"
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password;
            string confirmPassword = ConfirmPasswordTextBox.Password;

            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Все поля должны быть заполнены!");
                return;
            }

            // Проверка совпадения паролей
            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            try
            {
                // Проверка существования пользователя с таким логином
                var existingUser = Manager.Context.Users.FirstOrDefault(u => u.Login == login);
                if (existingUser != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!");
                    return;
                }

                // Создание нового пользователя
                var newUser = new User
                {
                    Login = login,
                    PasswordHash = HashPassword(password),
                    RoleID = 2 // Роль по умолчанию (например, кладовщик)
                };

                // Добавление пользователя в базу данных
                Manager.Context.Users.Add(newUser);
                Manager.Context.SaveChanges();

                MessageBox.Show("Регистрация успешна!");
                Manager.MainFrame.Navigate(new LoginPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}");
            }
        }
    }
}