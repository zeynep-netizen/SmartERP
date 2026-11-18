using SmartERP.BusinessLogic;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SmartERP
{
    public partial class Form1 : Form
    {
        private readonly ErpManager _erp = new ErpManager();
        private DataGridView dgvInventory;
        private DataGridView dgvOrders;
        private DataGridView dgvMovements;
        private Button btnApproveOrder;
        private Button btnAddStock;
        private Button btnRefresh;

        public Form1()
        {
            InitializeComponent();
            SetupErpDashboard();
            LoadAllData();
        }

        private void SetupErpDashboard()
        {
            this.Text = "SmartERP Core v1.0 - Kurumsal Kaynak Planlama Sistemi";
            this.Size = new Size(1150, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // Üst Başlık Paneli
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(30, 41, 59)
            };
            Label lblTitle = new Label
            {
                Text = "  SmartERP | Envanter, Sipariş ve Stok Hareket Yönetimi",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Left,
                AutoSize = false,
                Width = 650,
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(lblTitle);

            // Butonlar
            btnRefresh = CreateButton("Verileri Yenile", Color.FromArgb(59, 130, 246), 720);
            btnRefresh.Click += (s, e) => LoadAllData();

            btnAddStock = CreateButton("+10 Stok Girişi Yap", Color.FromArgb(16, 185, 129), 860);
            btnAddStock.Click += BtnAddStock_Click;

            btnApproveOrder = CreateButton("Seçili Siparişi Onayla", Color.FromArgb(245, 158, 11), 1000);
            btnApproveOrder.Click += BtnApproveOrder_Click;

            headerPanel.Controls.Add(btnRefresh);
            headerPanel.Controls.Add(btnAddStock);
            headerPanel.Controls.Add(btnApproveOrder);
            this.Controls.Add(headerPanel);

            // Sekmeli Yapı (TabControl)
            TabControl tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            TabPage tabInventory = new TabPage("Envanter & Kritik Stok Takibi");
            dgvInventory = CreateStyledGrid();
            dgvInventory.CellFormatting += DgvInventory_CellFormatting;
            tabInventory.Controls.Add(dgvInventory);

            TabPage tabOrders = new TabPage("Satış Siparişleri (Cari & Onay)");
            dgvOrders = CreateStyledGrid();
            tabOrders.Controls.Add(dgvOrders);

            TabPage tabMovements = new TabPage("Stok Hareket Logları (Audit Trail)");
            dgvMovements = CreateStyledGrid();
            tabMovements.Controls.Add(dgvMovements);

            tabControl.TabPages.Add(tabInventory);
            tabControl.TabPages.Add(tabOrders);
            tabControl.TabPages.Add(tabMovements);

            this.Controls.Add(tabControl);
            tabControl.BringToFront();
        }

        private Button CreateButton(string text, Color bg, int left)
        {
            return new Button
            {
                Text = text,
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Size = new Size(130, 36),
                Location = new Point(left, 12),
                Cursor = Cursors.Hand
            };
        }

        private DataGridView CreateStyledGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false
            };
        }

        private void LoadAllData()
        {
            try
            {
                dgvInventory.DataSource = _erp.GetInventoryList();
                dgvOrders.DataSource = _erp.GetSalesOrders();
                dgvMovements.DataSource = _erp.GetStockMovements();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı bağlantı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvInventory_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvInventory.Columns[e.ColumnIndex].Name == "StockStatus" && e.Value != null)
            {
                if (e.Value.ToString() == "KRİTİK STOK")
                {
                    e.CellStyle.BackColor = Color.FromArgb(254, 226, 226);
                    e.CellStyle.ForeColor = Color.FromArgb(185, 28, 28);
                    e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                }
            }
        }

        private void BtnApproveOrder_Click(object sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen 'Satış Siparişleri' sekmesinden onaylanacak bir sipariş seçin.");
                return;
            }

            string status = dgvOrders.SelectedRows[0].Cells["Status"].Value.ToString();
            if (status != "Draft")
            {
                MessageBox.Show("Yalnızca 'Draft' (Taslak) durumundaki siparişler onaylanabilir.");
                return;
            }

            int orderId = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["OrderID"].Value);
            _erp.ApproveOrder(orderId);
            MessageBox.Show($"Sipariş #{orderId} onaylandı! Stok otomatik düşüldü ve hareket loguna işlendi.", "ERP Otomasyonu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadAllData();
        }

        private void BtnAddStock_Click(object sender, EventArgs e)
        {
            if (dgvInventory.SelectedRows.Count == 0) return;

            int productId = Convert.ToInt32(dgvInventory.SelectedRows[0].Cells["ProductID"].Value);
            string productName = dgvInventory.SelectedRows[0].Cells["ProductName"].Value.ToString();

            _erp.AddStock(productId, 10, "Depo Mal Kabul Girişi");
            MessageBox.Show($"{productName} ürününe +10 adet stok eklendi.", "Stok Girişi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadAllData();
        }
    }
}