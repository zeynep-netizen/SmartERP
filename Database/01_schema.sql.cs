using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

CREATE DATABASE IF NOT EXISTS SmartERP_DB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE SmartERP_DB;

--1.Tedarikçiler Tablosu
CREATE TABLE Suppliers (
    SupplierID INT AUTO_INCREMENT PRIMARY KEY,
    CompanyName VARCHAR(150) NOT NULL,
    ContactName VARCHAR(100),
    Email VARCHAR(100),
    Phone VARCHAR(20),
    LeadTimeDays INT DEFAULT 3, -- Ortalama tedarik süresi (gün)
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

--2.Ürünler ve Envanter Tablosu
CREATE TABLE Products (
    ProductID INT AUTO_INCREMENT PRIMARY KEY,
    SKU VARCHAR(50) UNIQUE NOT NULL, -- Stok Kodu (Örn: ELK - 001)
    ProductName VARCHAR(150) NOT NULL,
    Category VARCHAR(80),
    SupplierID INT,
    UnitPrice DECIMAL(10, 2) NOT NULL,
    CostPrice DECIMAL(10, 2) NOT NULL, -- Kar marjı analizi için maliyet
    StockQuantity INT DEFAULT 0,
    ReorderLevel INT DEFAULT 10, -- Kritik stok eşiği
    FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID) ON DELETE SET NULL
);

--3.Müşteriler(Cari Hesap) Tablosu
CREATE TABLE Customers (
    CustomerID INT AUTO_INCREMENT PRIMARY KEY,
    CustomerName VARCHAR(150) NOT NULL,
    TaxNumber VARCHAR(20) UNIQUE,
    Email VARCHAR(100),
    City VARCHAR(50),
    CreditLimit DECIMAL(12, 2) DEFAULT 50000.00, -- Cari risk limiti
    CurrentBalance DECIMAL(12, 2) DEFAULT 0.00
);

--4.Satış Siparişleri(Başlık)
CREATE TABLE SalesOrders (
    OrderID INT AUTO_INCREMENT PRIMARY KEY,
    CustomerID INT NOT NULL,
    OrderDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    Status ENUM('Draft', 'Approved', 'Shipped', 'Cancelled') DEFAULT 'Draft',
    TotalAmount DECIMAL(12, 2) DEFAULT 0.00,
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
);

--5.Satış Sipariş Detayları(Satırlar)
CREATE TABLE SalesOrderItems (
    OrderItemID INT AUTO_INCREMENT PRIMARY KEY,
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES SalesOrders(OrderID) ON DELETE CASCADE,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

--6.Stok Hareket Logları(ERP İzlenebilirlik Standardı)
CREATE TABLE StockMovements (
    MovementID INT AUTO_INCREMENT PRIMARY KEY,
    ProductID INT NOT NULL,
    MovementType ENUM('IN', 'OUT', 'ADJUSTMENT') NOT NULL,
    Quantity INT NOT NULL,
    ReferenceNote VARCHAR(255), -- Örn: "Sipariş #104 Sevkiyatı"
    MovementDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);