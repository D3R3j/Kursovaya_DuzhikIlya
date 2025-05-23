﻿using System;
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
            ImageMenu.Visibility = Visibility.Visible;
            // Скрываем кнопку "Главная", если пользователь не залогинен
            if (Manager.CurrentUser == null)
            {
                HomeMenuItem.Visibility = Visibility.Hidden;
                BackMenuItem.Visibility = Visibility.Hidden;

            }
            else
            {
                HomeMenuItem.Visibility = Visibility.Visible;
                BackMenuItem.Visibility = Visibility.Visible;
            }

            // Загружаем стартовую страницу
            Manager.MainFrame.Navigate(new pages.LoginPage());
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            // Переход на главную страницу
            Manager.MainFrame.Navigate(new pages.MainPage());
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {

            if (Manager.MainFrame.CanGoBack)
            {
                Manager.MainFrame.GoBack();

                // Проверяем, не перешли ли мы на LoginPage
                if (Manager.MainFrame.Content is pages.LoginPage)
                {
                    Manager.CurrentUser = null;

                    // Скрываем пункты меню
                    HomeMenuItem.Visibility = Visibility.Hidden;
                    BackMenuItem.Visibility = Visibility.Hidden;
                }
            }

        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            if (!(Manager.MainFrame.Content is pages.LoginPage))
            {
                Manager.CurrentUser = null;
                HomeMenuItem.Visibility = Visibility.Hidden;
                BackMenuItem.Visibility = Visibility.Hidden;
                Manager.MainFrame.Navigate(new pages.LoginPage());
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
}
