using Mysqlx.Crud;
using SmartERP.Entities;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

USE SmartERP_DB;

--1.Tedarikçi Verileri
INSERT INTO Suppliers (CompanyName, ContactName, Email, Phone, LeadTimeDays) VALUES
('Anadolu Endüstriyel Tedarik A.Ş.', 'Mehmet Yılmaz', 'mehmet@anadolutedarik.com', '0212 555 1010', 2),
('Trakya Lojistik ve Donanım Ltd.', 'Ayşe Demir', 'ayse@trakyad hardware.com', '0284 555 2020', 4),
('Marmara Otomasyon Sistemleri', 'Caner Kaya', 'caner@marmaraotomasyon.com', '0216 555 3030', 5);

--2.Ürün Verileri(ABC Analizi için farklı maliyet / satış dengelerinde)
INSERT INTO Products(SKU, ProductName, Category, SupplierID, UnitPrice, CostPrice, StockQuantity, ReorderLevel) VALUES
('ELK-101', 'Endüstriyel PLC Kontrol Ünitesi', 'Otomasyon', 3, 18500.00, 12000.00, 15, 5),
('ELK-102', 'Frekans İnvertörü 5.5kW', 'Otomasyon', 3, 9200.00, 6100.00, 22, 8),
('MEK-201', 'Paslanmaz Rulman Seti (10lu)', 'Mekanik', 1, 1450.00, 850.00, 85, 20),
('MEK-202', 'Hidrolik Valf Bloğu', 'Mekanik', 2, 4800.00, 3100.00, 12, 10),
('SARF-301', 'Endüstriyel Sensör Kablosu (50m)', 'Sarf Malzeme', 1, 650.00, 320.00, 140, 30),
('SARF-302', 'Termal Etiket Rulosu (1000li)', 'Sarf Malzeme', 2, 240.00, 110.00, 200, 50);

--3.Müşteri(Cari Hesap) Verileri
INSERT INTO Customers (CustomerName, TaxNumber, Email, City, CreditLimit, CurrentBalance) VALUES
('Balkan Gıda Sanayi A.Ş.', '1234567890', 'satinalma@balkangida.com', 'Edirne', 150000.00, 24500.00),
('Kuzey Metal ve Makina Ltd.', '9876543210', 'info@kuzeymetal.com', 'Tekirdağ', 100000.00, 12000.00),
('Ege Paketleme Çözümleri', '4561237890', 'siparis@egepaketleme.com', 'İstanbul', 200000.00, 0.00);

--4.Örnek Geçmiş Siparişler(Raporlama ve Python Analizi için)
INSERT INTO SalesOrders(CustomerID, OrderDate, Status, TotalAmount) VALUES
(1, DATE_SUB(NOW(), INTERVAL 15 DAY), 'Shipped', 46200.00),
(2, DATE_SUB(NOW(), INTERVAL 8 DAY), 'Shipped', 21300.00),
(3, DATE_SUB(NOW(), INTERVAL 2 DAY), 'Draft', 18500.00);

INSERT INTO SalesOrderItems(OrderID, ProductID, Quantity, UnitPrice) VALUES
(1, 1, 2, 18500.00),
(1, 2, 1, 9200.00),
(2, 4, 4, 4800.00),
(2, 5, 3, 650.00),
(3, 1, 1, 18500.00);

--5.ERP İş Kuralı Otomasyonu: Sipariş 'Approved' olduğunda stoğu düş ve logla
DELIMITER //
CREATE TRIGGER trg_AfterOrderApproved
AFTER UPDATE ON SalesOrders
FOR EACH ROW
BEGIN
    IF OLD.Status <> 'Approved' AND NEW.Status = 'Approved' THEN
        -- Ürün stoklarını düş
        UPDATE Products p
        INNER JOIN SalesOrderItems soi ON p.ProductID = soi.ProductID
        SET p.StockQuantity = p.StockQuantity - soi.Quantity
        WHERE soi.OrderID = NEW.OrderID;

--Stok hareket loguna(Audit Trail) kayıt at
        INSERT INTO StockMovements (ProductID, MovementType, Quantity, ReferenceNote)
        SELECT soi.ProductID, 'OUT', soi.Quantity, CONCAT('Sipariş Onayı #', NEW.OrderID)
        FROM SalesOrderItems soi
        WHERE soi.OrderID = NEW.OrderID;

--Müşteri cari bakiyesini güncelle
        UPDATE Customers
        SET CurrentBalance = CurrentBalance + NEW.TotalAmount
        WHERE CustomerID = NEW.CustomerID;
END IF;
END //
DELIMITER;