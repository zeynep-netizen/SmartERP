using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.Entities
{
    internal class Product
    {

        public int ProductID { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int SupplierID { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }

        // ERP Hesaplanmış Özellik: Kritik Stok Durumu
        public bool IsCriticalStock => StockQuantity <= ReorderLevel;
    }
}
