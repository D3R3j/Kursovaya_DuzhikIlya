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
    /// Логика взаимодействия для EditProductPage.xaml
    /// </summary>
    public partial class EditProductPage : Page
    {
        private int _inventoryResultId;

        public List<Category> Categories { get; set; }
        public Category SelectedCategory { get; set; }

        public EditProductPage(int inventoryResultId)
        {
            InitializeComponent();
            DataContext = this;
            _inventoryResultId = inventoryResultId;
            LoadCategories();
            LoadCurrentProduct();
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

        private void LoadCurrentProduct()
        {
            try
            {
                var inventoryResult = Manager.Context.InventoryResults
                    .FirstOrDefault(ir => ir.ResultID == _inventoryResultId);

                if (inventoryResult == null) return;

                var product = inventoryResult.Product;
                ProductNameTextBox.Text = product.Name;
                UnitOfMeasureTextBox.Text = product.UnitOfMeasure ?? "шт.";
                MinStockLevelTextBox.Text = product.MinStockLevel.ToString();

                if (product.Category != null)
                {
                    SelectedCategory = product.Category;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
            {
                MessageBox.Show("Введите название товара!");
                return;
            }

            if (SelectedCategory == null)
            {
                MessageBox.Show("Выберите категорию!");
                return;
            }

            try
            {
                var inventoryResult = Manager.Context.InventoryResults
                    .FirstOrDefault(ir => ir.ResultID == _inventoryResultId);

                if (inventoryResult == null) return;

                var product = inventoryResult.Product;
                if (product == null) return;

                // Обновляем поля товара
                product.Name = ProductNameTextBox.Text.Trim();
                product.UnitOfMeasure = UnitOfMeasureTextBox.Text.Trim();
                product.MinStockLevel = int.Parse(MinStockLevelTextBox.Text);
                product.CategoryID = SelectedCategory.CategoryID;

                Manager.Context.SaveChanges();
                MessageBox.Show("Данные о товаре обновлены!");

                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения изменений: {ex.Message}");
            }
        }
    }
}
