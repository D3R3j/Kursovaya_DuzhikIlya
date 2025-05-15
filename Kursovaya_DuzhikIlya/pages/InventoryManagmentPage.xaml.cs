using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Kursovaya_DuzhikIlya; // Пространство имен для контекста базы данных

namespace Kursovaya_DuzhikIlya.pages
{
    public partial class InventoryManagmentPage : Page
    {
        public InventoryManagmentPage()
        {
            InitializeComponent();
            LoadInventoryData();
        }

        // Список инвентаризаций
        private List<Inventory> InventoryActs { get; set; }
        private Inventory SelectedInventory => InventoryActsGrid.SelectedItem as Inventory;

        // Загрузка данных
        private void LoadInventoryData()
        {
            try
            {
                var context = Manager.Context; // Используем существующий контекст
                InventoryActs = context.Inventories
                    .Include("User") // Используем строку вместо лямбды
                    .ToList();
                InventoryActsGrid.ItemsSource = InventoryActs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        // Обработка нажатия кнопки "Создать акт"
        private void CreateAct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = Manager.Context; // Используйте существующий контекст
                var newInventory = new Inventory
                {
                    StartDate = DateTime.Now,
                    EmployeeID = GetCurrentUserID(),
                };
                context.Inventories.Add(newInventory);
                context.SaveChanges();
                Manager.MainFrame.Navigate(new InventoryPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания акта: {ex.Message}");
            }
        }

        // Обработка нажатия кнопки "Удалить акт"
        private void DeleteAct_Click(object sender, RoutedEventArgs e)
        {
            var selected = SelectedInventory;
            if (selected == null)
            {
                MessageBox.Show("Выберите акт для удаления!");
                return;
            }

            if (MessageBox.Show($"Удалить акт от {selected.StartDate}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    // Используем существующий контекст
                    var context = Manager.Context;

                    var inventory = context.Inventories.Find(selected.InventoryID);
                    if (inventory != null)
                    {
                        context.Inventories.Remove(inventory);
                        context.SaveChanges();
                    }

                    LoadInventoryData(); // Обновляем список
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        // Получение ID текущего пользователя (пример)
        private int GetCurrentUserID()
        {
            // Реализуйте получение текущего пользователя из сессии
            return 1; // Пример: первый пользователь
        }

        // Обновление данных при изменении видимости страницы
        private void InventoryManagmentPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                LoadInventoryData();
            }
        }
    }
}