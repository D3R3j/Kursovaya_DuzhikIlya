using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Kursovaya_DuzhikIlya; // Пространство имен для контекста базы данных

namespace Kursovaya_DuzhikIlya.pages
{
    public partial class InventoryPage : Page
    {
        public InventoryPage()
        {
            InitializeComponent();
            LoadWarehouses();
        }

        private void LoadWarehouses()
        {
            try
            {
                Warehouses = Manager.Context.Warehouses.ToList();
                WarehouseSelector.ItemsSource = Warehouses;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки складов: {ex.Message}");
            }
        }

        private List<Warehouse> Warehouses { get; set; }
        private Warehouse SelectedWarehouse => WarehouseSelector.SelectedItem as Warehouse;

        private void WarehouseSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadInventoryData();
        }

        private void LoadInventoryData()
        {
            if (SelectedWarehouse == null) return;

            try
            {
                InventoryItems = Manager.Context.InventoryResults
                    .Where(ir => ir.WarehouseID == SelectedWarehouse.WarehouseID)
                    .Select(ir => new InventoryItemViewModel
                    {
                        ProductName = ir.Product.Name,
                        Category = ir.Product.Category.Name,
                        SystemQuantity = ir.Product.StockMovements
                            .Sum(sm => sm.MovementType == "Поступление" ? sm.Quantity : -sm.Quantity),
                        ActualQuantity = ir.ActualQuantity,
                        InventoryResultId = ir.ResultID
                    }).ToList();

                InventoryGrid.ItemsSource = InventoryItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private List<InventoryItemViewModel> InventoryItems { get; set; }

        private void SaveInventory_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryItems == null || !InventoryItems.Any())
            {
                MessageBox.Show("Нет данных для сохранения!");
                return;
            }

            try
            {
                foreach (var item in InventoryItems)
                {
                    var result = Manager.Context.InventoryResults.Find(item.InventoryResultId);
                    if (result != null)
                    {
                        result.ActualQuantity = item.ActualQuantity;
                    }
                }

                Manager.Context.SaveChanges();
                MessageBox.Show("Изменения сохранены!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения данных: {ex.Message}");
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddProductPage());
        }

        private void InventoryPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                LoadWarehouses();
                LoadInventoryData();
            }
        }
    }

    public class InventoryItemViewModel
    {
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int SystemQuantity { get; set; }
        public int ActualQuantity { get; set; }
        public int InventoryResultId { get; set; }
    }
}