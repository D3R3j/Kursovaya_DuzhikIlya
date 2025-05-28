using System;
using System.Reflection;
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
                var context = Manager.Context;

                if (reportType == "Движение товаров")
                {
                    var data = context.StockMovements
                        .Include("Product") // Загрузка связанного товара
                        .Include("Warehouse") // Загрузка связанного склада
                        .Where(sm => sm.Date >= startDate && sm.Date <= endDate)
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
                    BaseFont baseFont = BaseFont.CreateFont("c:\\windows\\fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    iTextSharp.text.Font font = new iTextSharp.text.Font(baseFont, 12);
                    Paragraph title = new Paragraph("Отчет по складским операциям\n\n", font);
                    title.Alignment = Element.ALIGN_CENTER;
                    document.Add(title);
                    PdfPTable table = new PdfPTable(ReportGrid.Columns.Count);
                    foreach (var column in ReportGrid.Columns)
                    {
                        PdfPCell headerCell = new PdfPCell(new Phrase(column.Header?.ToString(), font));
                        table.AddCell(headerCell);
                    }
                    Type itemType = null;
                    foreach (var item in ReportData)
                    {
                        if (itemType == null && item != null)
                        {
                            itemType = item.GetType();
                        }

                        foreach (var column in ReportGrid.Columns)
                        {
                            if (column is DataGridTextColumn textColumn &&
                                textColumn.Binding is Binding binding)
                            {
                                string path = binding.Path.Path;

                                object value = null;
                                if (path.Contains("."))
                                {
                                    // Обработка навигационных свойств (например: Product.Name)
                                    string[] parts = path.Split('.');
                                    object current = item;
                                    foreach (string part in parts)
                                    {
                                        var prop = current?.GetType().GetProperty(part);
                                        if (prop == null) break;
                                        current = prop.GetValue(current);
                                    }
                                    value = current;
                                }
                                else
                                {
                                    var property = itemType.GetProperty(path);
                                    value = property?.GetValue(item);
                                }

                                string displayValue = value?.ToString() ?? "";
                                table.AddCell(new Phrase(displayValue, font));
                            }
                            else
                            {
                                table.AddCell("");
                            }
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
            int index = grid.Items.IndexOf(item);
            if (index < 0) return null;

            var row = grid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
            return row;
        }

        private static DataGridCell GetCell(DataGrid grid, DataGridRow row, DataGridColumn column)
        {
            if (row == null || column == null) return null;

            int columnIndex = grid.Columns.IndexOf(column);
            if (columnIndex == -1) return null;

            var presenter = GetVisualChild<DataGridCellsPresenter>(row);
            if (presenter == null) return null;

            var item = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
            return item as DataGridCell;
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