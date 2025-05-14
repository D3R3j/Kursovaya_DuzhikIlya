using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Kursovaya_DuzhikIlya
{
    internal class Manager
    {
        public static Frame MainFrame { get; set; }
        public static WarehouseEntities Context { get; } = WarehouseEntities.GetContext();
    }
}
