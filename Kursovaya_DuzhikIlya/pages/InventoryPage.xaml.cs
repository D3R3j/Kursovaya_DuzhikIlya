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

namespace Kursovaya_DuzhikIlya.pages
{
    /// <summary>
    /// Логика взаимодействия для InventoryPage.xaml
    /// </summary>
    public partial class InventoryPage : Page
    {
        public InventoryPage()
        {
            InitializeComponent();
        }

        private void LoadWarehouses()
        {
            WarehouseSelector.ItemsSource = Manager.Context.Warehouses.ToList();
        }

        private void LoadInventoryData()
        {
            var warehouseId = (WarehouseSelector.SelectedItem as Warehouse)?.WarehouseID ?? 0;
            var inventoryItems = Manager.Context.InventoryResults
                .Where(ir => ir.WarehouseID == warehouseId)
                .ToList();

            InventoryGrid.ItemsSource = inventoryItems;
        }

        private void SaveInventory_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in InventoryGrid.Items)
            {
                var result = item as InventoryResult;
                if (result != null)
                {
                    var entry = Manager.Context.InventoryResults.Find(result.ResultID);
                    if (entry != null)
                    {
                        entry.ActualQuantity = result.ActualQuantity;
                    }
                }
            }

            Manager.Context.SaveChanges();
            MessageBox.Show("Изменения сохранены!");
        }
    }
}
