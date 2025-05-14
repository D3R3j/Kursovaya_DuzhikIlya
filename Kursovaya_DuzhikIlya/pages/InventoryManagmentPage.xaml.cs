using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace Kursovaya_DuzhikIlya.pages
{
    /// <summary>
    /// Логика взаимодействия для InventoryManagmentPage.xaml
    /// </summary>
    public partial class InventoryManagmentPage : Page
    {
        private WarehouseEntities _context;

        public InventoryManagmentPage()
        {
            InitializeComponent();
            _context = WarehouseEntities.GetContext();
            LoadInventoryActs();
        }

        private void LoadInventoryActs()
        {
            try
            {
                // Перезагрузка данных из БД
                _context.Inventories.Load();
                InventoryActsGrid.ItemsSource = _context.Inventories.Local.ToBindingList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Создание нового акта инвентаризации
        private void CreateAct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newInventory = new Inventory
                {
                    StartDate = DateTime.Now,
                    EmployeeID = GetCurrentUserID(), // Получение ID текущего пользователя
                    Status = "Активная"
                };

                _context.Inventories.Add(newInventory);
                _context.SaveChanges();

                LoadInventoryActs();
                MessageBox.Show("Акт инвентаризации создан!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания акта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Удаление выбранного акта
        private void DeleteAct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedAct = InventoryActsGrid.SelectedItem as Inventory;
                if (selectedAct == null)
                {
                    MessageBox.Show("Выберите акт для удаления!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Подтверждение удаления
                if (MessageBox.Show("Вы уверены, что хотите удалить этот акт инвентаризации?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _context.Inventories.Remove(selectedAct);
                    _context.SaveChanges();
                    LoadInventoryActs();
                    MessageBox.Show("Акт удален!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления акта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Получение ID текущего пользователя (пример)
        private int GetCurrentUserID()
        {
            // Реализуйте логику получения ID пользователя из сессии или авторизации
            return 1; // Временное значение для тестирования
        }
    }
}
