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
    /// Логика взаимодействия для ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
        }

        private void LoadReportTypes()
        {
            ReportTypeSelector.ItemsSource = new List<string> { "Ежедневный", "Еженедельный", "Ежемесячный" };
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            var startDate = StartDatePicker.SelectedDate ?? DateTime.Now.AddDays(-7);
            var endDate = EndDatePicker.SelectedDate ?? DateTime.Now;

            var reportData = Manager.Context.StockMovements
                .Where(m => m.Date >= startDate && m.Date <= endDate)
                .ToList();

            ReportGrid.ItemsSource = reportData;
        }

        private void ExportToPDF_Click(object sender, RoutedEventArgs e)
        {
            // Реализация экспорта в PDF через iTextSharp
            MessageBox.Show("Экспорт в PDF не реализован в этом примере.");
        }
    }
}
