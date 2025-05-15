using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Kursovaya_DuzhikIlya; // Пространство имен для контекста базы данных
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Kursovaya_DuzhikIlya.pages
{
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
            LoadReportTypes();
        }

        private void LoadReportTypes()
        {
            ReportTypes = new List<string> { "Движение товаров", "Инвентаризация" };
            ReportTypeSelector.ItemsSource = ReportTypes;
        }

        private List<string> ReportTypes { get; set; }
        private List<object> ReportData { get; set; }

        // Обработка нажатия кнопки "Сформировать отчет"
        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null || ReportTypeSelector.SelectedItem == null)
            {
                MessageBox.Show("Выберите все параметры отчета!");
                return;
            }

            var startDate = StartDatePicker.SelectedDate.Value;
            var endDate = EndDatePicker.SelectedDate.Value;
            var reportType = ReportTypeSelector.SelectedItem.ToString();

            try
            {
                // Используем существующий контекст из Manager
                var context = Manager.Context;

                if (reportType == "Движение товаров")
                {
                    var data = context.StockMovements
                        .Where(sm => sm.Date >= startDate && sm.Date <= endDate)
                        .ToList();
                    ReportData = data.Cast<object>().ToList();
                }
                else if (reportType == "Инвентаризация")
                {
                    // Явно загружаем связанный объект Inventory
                    var data = context.InventoryResults
                        .Include("Inventory") // Подключаем связанный объект Inventory
                        .Where(ir => ir.Inventory.StartDate >= startDate && ir.Inventory.EndDate <= endDate)
                        .ToList();
                    ReportData = data.Cast<object>().ToList();
                }

                ReportGrid.ItemsSource = ReportData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка генерации отчета: {ex.Message}");
            }
        }

        // Обработка нажатия кнопки "Экспорт в PDF"
        private void ExportToPDF_Click(object sender, RoutedEventArgs e)
        {
            if (ReportData == null || !ReportData.Any())
            {
                MessageBox.Show("Нет данных для экспорта!");
                return;
            }

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF-файлы (*.pdf)|*.pdf",
                    DefaultExt = ".pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    iTextSharp.text.Document document = new iTextSharp.text.Document();
                    PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    document.Open();

                    // Заголовок отчета
                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Отчет по складским операциям\n\n");
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    document.Add(title);

                    // Таблица с данными
                    PdfPTable table = new PdfPTable(ReportGrid.Columns.Count);
                    foreach (var column in ReportGrid.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(column.Header.ToString()));
                        table.AddCell(cell);
                    }

                    foreach (var item in ReportData)
                    {
                        foreach (var column in ReportGrid.Columns)
                        {
                            var cellContent = DataGridHelper.GetCellContent(ReportGrid, item, column);
                            table.AddCell(cellContent);
                        }
                    }

                    document.Add(table);
                    document.Close();
                    MessageBox.Show("Отчет успешно сохранен!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта в PDF: {ex.Message}");
            }
        }
    }

    // Вспомогательный класс для получения содержимого ячеек DataGrid
    public static class DataGridHelper
    {
        public static string GetCellContent(DataGrid grid, object item, DataGridColumn column)
        {
            var row = GetRow(grid, item);
            if (row == null) return "";

            var presenter = GetVisualChild<DataGridCellsPresenter>(row);
            if (presenter == null) return "";

            var cell = GetCell(grid, row, column);
            return cell?.Content?.ToString() ?? "";
        }

        private static DataGridRow GetRow(DataGrid grid, object item)
        {
            if (item == null) return null;

            // Получаем индекс элемента в коллекции Items
            int index = grid.Items.IndexOf(item);
            if (index < 0) return null;

            // Получаем строку через ItemContainerGenerator
            var row = grid.Items[index] as DataGridRow;
            if (row == null)
            {
                // Альтернативный поиск через визуальное дерево
                row = GetVisualChild<DataGridRow>(grid);
            }

            return row;
        }

        private static DataGridCell GetCell(DataGrid grid, DataGridRow row, DataGridColumn column)
        {
            if (row == null || column == null) return null;

            int columnIndex = grid.Columns.IndexOf(column);
            if (columnIndex < 0) return null;

            var presenter = GetVisualChild<DataGridCellsPresenter>(row);
            if (presenter == null) return null;

            var item = presenter.Items[columnIndex];
            return item is DependencyObject depObj
                ? GetVisualChild<DataGridCell>(depObj)
                : null;
        }

        private static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T result)
                    return result;

                var descendant = GetVisualChild<T>(child);
                if (descendant != null)
                    return descendant;
            }

            return null;
        }
    }
}