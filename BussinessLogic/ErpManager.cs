using System;
using System.Data;
using MySql.Data.MySqlClient;
using SmartERP.DataAccess;

namespace SmartERP.BusinessLogic
{
    public class ErpManager
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();

        public DataTable GetInventoryList()
        {
            string query = @"
                SELECT p.ProductID, p.SKU, p.ProductName, p.Category, 
                       s.CompanyName AS Supplier, p.UnitPrice, p.CostPrice, 
                       p.StockQuantity, p.ReorderLevel,
                       CASE WHEN p.StockQuantity <= p.ReorderLevel THEN 'KRİTİK STOK' ELSE 'Normal' END AS StockStatus
                FROM Products p
                LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                ORDER BY p.ProductID;";
            return _db.ExecuteQuery(query);
        }

        public DataTable GetSalesOrders()
        {
            string query = @"
                SELECT o.OrderID, c.CustomerName, o.OrderDate, o.Status, o.TotalAmount
                FROM SalesOrders o
                INNER JOIN Customers c ON o.CustomerID = c.CustomerID
                ORDER BY o.OrderID DESC;";
            return _db.ExecuteQuery(query);
        }

        public DataTable GetStockMovements()
        {
            string query = @"
                SELECT m.MovementID, p.SKU, p.ProductName, m.MovementType, 
                       m.Quantity, m.ReferenceNote, m.MovementDate
                FROM StockMovements m
                INNER JOIN Products p ON m.ProductID = p.ProductID
                ORDER BY m.MovementDate DESC;";
            return _db.ExecuteQuery(query);
        }

        public void ApproveOrder(int orderId)
        {
            string query = "UPDATE SalesOrders SET Status = 'Approved' WHERE OrderID = @OrderID AND Status = 'Draft';";
            _db.ExecuteNonQuery(query, new MySqlParameter("@OrderID", orderId));
        }

        public void AddStock(int productId, int quantity, string note)
        {
            string updateQuery = "UPDATE Products SET StockQuantity = StockQuantity + @Qty WHERE ProductID = @PID;";
            _db.ExecuteNonQuery(updateQuery,
                new MySqlParameter("@Qty", quantity),
                new MySqlParameter("@PID", productId));

            string logQuery = "INSERT INTO StockMovements (ProductID, MovementType, Quantity, ReferenceNote) VALUES (@PID, 'IN', @Qty, @Note);";
            _db.ExecuteNonQuery(logQuery,
                new MySqlParameter("@PID", productId),
                new MySqlParameter("@Qty", quantity),
                new MySqlParameter("@Note", note));
        }
    }
}
