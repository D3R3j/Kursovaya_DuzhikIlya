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
                using (var context = WarehouseEntities.GetContext())
                {
                    InventoryActs = context.Inventories
                        .Include("User") // Загрузка связанных данных (ответственный)
                        .ToList();

                    InventoryActsGrid.ItemsSource = InventoryActs;
                }
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
                using (var context = WarehouseEntities.GetContext())
                {
                    // Создание нового акта инвентаризации
                    var newInventory = new Inventory
                    {
                        StartDate = DateTime.Now,
                        EmployeeID = GetCurrentUserID(), // Получение ID текущего пользователя
                    };

                    context.Inventories.Add(newInventory);
                    context.SaveChanges();

                    // Переход к странице деталей инвентаризации
                    Manager.MainFrame.Navigate(new InventoryPage());
                }
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

            // Подтверждение удаления
            if (MessageBox.Show($"Удалить акт от {selected.StartDate}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = WarehouseEntities.GetContext())
                    {
                        var inventory = context.Inventories.Find(selected.InventoryID);
                        if (inventory != null)
                        {
                            context.Inventories.Remove(inventory);
                            context.SaveChanges();
                        }
                    }

                    LoadInventoryData(); // Обновление списка
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