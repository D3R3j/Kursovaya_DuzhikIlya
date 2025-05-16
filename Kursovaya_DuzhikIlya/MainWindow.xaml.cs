using System;
using System.Collections.Generic;
using System.Linq;
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
using Kursovaya_DuzhikIlya.pages;

namespace Kursovaya_DuzhikIlya
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Manager.MainFrame = MainFrame;

            // Скрываем кнопку "Главная", если пользователь не залогинен
            if (Manager.CurrentUser == null)
            {
                HomeMenuItem.Visibility = Visibility.Collapsed;
            }
            else
            {
                HomeMenuItem.Visibility = Visibility.Visible;
            }

            // Загружаем стартовую страницу
            Manager.MainFrame.Navigate(new pages.LoginPage());
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            // Переход на главную страницу
            Manager.MainFrame.Navigate(new pages.MainPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Очистка текущего пользователя
            Manager.CurrentUser = null;

            // Закрываем приложение
            Application.Current.Shutdown();
        }
    }
}
