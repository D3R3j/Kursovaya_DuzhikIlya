using System;
using System.Windows;
using System.Windows.Controls;

namespace Kursovaya_DuzhikIlya.pages
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // Переход на страницу управления товарами
        private void ManageProducts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.MainFrame.Navigate(new InventoryManagmentPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка навигации: {ex.Message}");
            }
        }

        // Переход на страницу инвентаризации
        private void Inventory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.MainFrame.Navigate(new InventoryPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка навигации: {ex.Message}");
            }
        }

        // Переход на страницу отчетов
        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.MainFrame.Navigate(new ReportsPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка навигации: {ex.Message}");
            }
        }
    }
}