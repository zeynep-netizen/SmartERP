# 🏭 SmartERP Core - Kurumsal Kaynak Planlama ve Envanter Yönetim Sistemi

**SmartERP Core**, işletmelerin envanter takibi, tedarikçi yönetimi, satış siparişi onay döngüleri ve stok hareket denetim izlerini (Audit Trail) ilişkisel veri tabanı mimarisiyle yöneten modüler bir masaüstü ERP uygulamasıdır.

## 🚀 Öne Çıkan Özellikler ve Mimari Yapı

* **Katmanlı Mimari (N-Tier Architecture):** Kod tabanı bakım kolaylığı ve ölçeklenebilirlik için `Entities`, `DataAccess` (DAL), `BusinessLogic` (BLL) ve `UI` katmanlarına ayrılmıştır.
* **Veri Tabanı Seviyesinde İş Kuralı Otomasyonu (MySQL Triggers):** Satış siparişleri `Approved` (Onaylandı) statüsüne geçtiğinde, ilgili ürünlerin stok miktarları otomatik olarak düşülür, müşteri cari bakiyesi güncellenir ve `StockMovements` tablosuna denetim kaydı işlenir.
* **Kritik Stok ve Tedarik Takibi:** Belirlenen yeniden sipariş seviyesinin (`ReorderLevel`) altına düşen ürünler sistem tarafından otomatik tespit edilerek görsel uyarı mekanizmasıyla raporlanır.
* **Tam İzlenebilirlik (Audit Trail):** Tüm depo mal kabul (`IN`) ve sipariş sevkiyat (`OUT`) hareketleri zaman damgası ve referans notuyla kayıt altında tutulur.

## 🛠️ Kullanılan Teknolojiler

* **Backend & Arayüz:** C# (.NET Framework / Windows Forms)
* **Veri Tabanı:** MySQL 8.0 (Relational Schema, Triggers, Foreign Key Constraints)
* **Veri Erişimi:** ADO.NET (`MySql.Data`)

## ⚙️ Kurulum ve Çalıştırma

1. `Database/01_schema.sql` dosyasını MySQL Workbench üzerinde çalıştırarak veri tabanı tablolarını oluşturun.
2. `Database/02_seed_and_triggers.sql` dosyasını çalıştırarak örnek verileri ve otomatik stok tetikleyicilerini yükleyin.
3. `DataAccess/DatabaseHelper.cs` içerisindeki bağlantı dizesine kendi MySQL sunucu bilgilerinizi girin.
4. `SmartERP.sln` çözüm dosyasını Visual Studio ile açıp projeyi derleyin ve çalıştırın.
