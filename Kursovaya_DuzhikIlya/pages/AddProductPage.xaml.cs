using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kursovaya_DuzhikIlya.pages
{
    public partial class AddProductPage : Page
    {
        public List<Category> Categories { get; set; }
        public List<Warehouse> Warehouses { get; set; }
        public List<Supplier> Suppliers { get; set; }

        public Category SelectedCategory { get; set; }
        public Warehouse SelectedWarehouse { get; set; }
        public Supplier SelectedSupplier { get; set; }

        public AddProductPage()
        {
            InitializeComponent();
            DataContext = this;

            LoadCategories();
            LoadWarehouses();
            LoadSuppliers();
        }

        private void LoadCategories()
        {
            try
            {
                Categories = Manager.Context.Categories.ToList();
                CategorySelector.ItemsSource = Categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}");
            }
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

        private void LoadSuppliers()
        {
            try
            {
                Suppliers = Manager.Context.Suppliers.ToList();
                SupplierSelector.ItemsSource = Suppliers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки поставщиков: {ex.Message}");
            }
        }

        private void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            string productName = ProductNameTextBox.Text.Trim();
            string description = ProductDescriptionTextBox.Text.Trim();
            var selectedCategory = CategorySelector.SelectedItem as Category;
            var selectedWarehouse = WarehouseSelector.SelectedItem as Warehouse;

            if (string.IsNullOrWhiteSpace(productName))
            {
                MessageBox.Show("Введите название товара!");
                return;
            }

            if (selectedCategory == null || selectedWarehouse == null)
            {
                MessageBox.Show("Выберите категорию и склад!");
                return;
            }

            if (!int.TryParse(MinStockLevelTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество.");
                return;
            }

            try
            {
                var context = Manager.Context;

                // 1. Добавляем товар
                var newProduct = new Product
                {
                    Name = productName,
                    Description = description,
                    UnitOfMeasure = UnitOfMeasureTextBox.Text.Trim() ?? "шт.",
                    MinStockLevel = int.TryParse(MinStockLevelTextBox.Text, out int minStockLevel) ? minStockLevel : 0,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    CategoryID = selectedCategory.CategoryID,
                    SupplierID = SupplierSelector.SelectedItem != null ? ((Supplier)SupplierSelector.SelectedItem).SupplierID : (int?)null
                };

                context.Products.Add(newProduct);
                context.SaveChanges(); // Получаем newProduct.ProductID

                // 2. (Опционально) Создаем документ
                var document = new Document
                {
                    Type = "Акт",
                    Number = $"DOC-{newProduct.ProductID}-{DateTime.Now.Ticks % 10000}",
                    IssueDate = DateTime.Now,
                    ExpiryDate = null,
                    FileLink = null,
                    SupplierID = SupplierSelector.SelectedItem != null ? ((Supplier)SupplierSelector.SelectedItem).SupplierID : (int?)null
                };

                context.Documents.Add(document);
                context.SaveChanges();

                // 3. Добавляем движение товара
                var stockMovement = new StockMovement
                {
                    MovementType = "Поступление",
                    Date = DateTime.Now,
                    Quantity = quantity,
                    ProductID = newProduct.ProductID,
                    WarehouseID = selectedWarehouse.WarehouseID,
                    DocumentID = document.DocumentID
                };

                // 4. Найдем последний акт инвентаризации для этого склада
                var lastInventory = context.InventoryResults
                    .Where(ir => ir.WarehouseID == selectedWarehouse.WarehouseID)
                    .Select(ir => ir.Inventory)
                    .OrderByDescending(i => i.StartDate)
                    .FirstOrDefault();

                if (lastInventory != null)
                {
                    // 5. Добавляем результат инвентаризации
                    var inventoryResult = new InventoryResult
                    {
                        InventoryID = lastInventory.InventoryID,
                        ProductID = newProduct.ProductID,
                        ActualQuantity = quantity,
                        WarehouseID = selectedWarehouse.WarehouseID
                    };

                    context.InventoryResults.Add(inventoryResult);
                }

                context.SaveChanges();

                MessageBox.Show("Товар и данные по инвентаризации успешно добавлены!");

                context.StockMovements.Add(stockMovement);
                context.SaveChanges();

                MessageBox.Show("Товар и начальное движение успешно добавлены!");

                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении товара: {ex.Message}");
            }
        }
    }
}