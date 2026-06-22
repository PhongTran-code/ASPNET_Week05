# MiniEquipmentInventory (ASP.NET Core MVC - Lab05)

## 📋 Mô Tả Dự Án

**MiniEquipmentInventory** là một ứng dụng web xây dựng trên nền tảng **ASP.NET Core MVC (net8.0)** nhằm quản lý danh mục và tình trạng tồn kho của các thiết bị (Equipment). Dự án áp dụng các kỹ thuật nâng cao của Entity Framework Core như kiểm soát truy cập đồng thời (Concurrency Control), xóa mềm toàn cục (Soft Delete), cơ chế kiểm tra sức khỏe hệ thống (Health Checks), ghi nhật ký thao tác (Audit Logs), Minimal API và một trang Admin Dashboard trực quan.

---

## 🛠️ Công Nghệ Sử Dụng

- **Backend**: ASP.NET Core MVC (.NET 8.0)
- **Database ORM**: Entity Framework Core với SQL Server
- **Logging**: Serilog (ghi ra Console và lưu file xoay vòng theo ngày trong thư mục `logs/`)
- **Frontend**: Razor Views, HTML5, CSS3 (Vanilla CSS), JavaScript, jQuery, jQuery Validation

---

## 📁 Cấu Trúc Thư Mục Hiện Tại (Lab05)

```text
MiniEquipmentInventory.sln          # Solution file
│
└── MiniEquipmentInventory.Mvc/     # Thư mục dự án chính
    ├── Program.cs                  # Entry point, cấu hình dịch vụ, Middleware, Minimal API & Health Checks
    ├── MiniEquipmentInventory.Mvc.csproj
    ├── appsettings.json            # File cấu hình database connection và Serilog logging levels
    ├── appsettings.Development.json
    │
    ├── Controllers/               # Các Controller xử lý yêu cầu MVC
    │   ├── AuditLogsController.cs  # Lấy danh sách nhật ký thao tác (Audit Logs)
    │   ├── CategoryController.cs   # Quản lý danh mục thiết bị (Category)
    │   ├── EquipmentController.cs  # Quản lý thiết bị (Equipment)
    │   ├── HomeController.cs       # Trang chủ thống kê (Dashboard)
    │   └── OrdersController.cs     # Quản lý giao dịch đặt hàng
    │
    ├── Data/                      # Cơ sở dữ liệu & Migrations
    │   ├── AppDbContext.cs         # EF Core DbContext, cấu hình Fluent API, Seed Data & Query Filters
    │   └── Migrations/             # Các file migration lịch sử database schema
    │
    ├── Models/                    # Thực thể dữ liệu (Entities)
    │   ├── AuditLog.cs             # Lịch sử ghi log thao tác
    │   ├── Category.cs             # Danh mục thiết bị (1 - Nhiều)
    │   ├── Equipment.cs            # Thông tin thiết bị (khóa chính, SKU độc nhất, RowVersion kiểm soát xung đột)
    │   ├── Order.cs                # Đơn đặt hàng
    │   └── OrderItem.cs            # Chi tiết sản phẩm trong đơn đặt hàng
    │
    ├── Options/                   # Options Pattern
    │   └── AppSettings.cs          # Map cấu hình giới hạn tồn kho thấp (LowStockThreshold) từ appsettings.json
    │
    ├── Services/                  # Lớp xử lý nghiệp vụ (Business Logic Layer)
    │   ├── IEquipmentService.cs    # Giao diện service quản lý thiết bị
    │   ├── EquipmentService.cs     # Thực thi service thiết bị (CRUD, Soft Delete, Restore)
    │   ├── IOrderService.cs        # Giao diện service đơn hàng
    │   └── OrderService.cs         # Thực thi service đặt hàng áp dụng EF Core Transactions
    │
    ├── ViewModels/                # Data Transfer Objects phục vụ Razor Views
    │   ├── CategoryListItemViewModel.cs
    │   ├── DashboardViewModel.cs   # Dữ liệu hiển thị thống kê cho trang chủ
    │   ├── EquipmentCreateViewModel.cs
    │   ├── EquipmentDetailViewModel.cs
    │   ├── EquipmentEditViewModel.cs
    │   ├── EquipmentListItemViewModel.cs
    │   ├── EquipmentTrashItemViewModel.cs
    │   └── OrderCreateViewModel.cs
    │
    ├── Views/                     # Razor Views render HTML
    │   ├── AuditLogs/              # Giao diện hiển thị bảng nhật ký thao tác
    │   ├── Category/               # Giao diện danh mục
    │   ├── Equipment/              # Giao diện quản lý thiết bị (Danh sách, Chi tiết, Thêm mới, Chỉnh sửa, Thùng rác)
    │   ├── Home/                   # Giao diện trang chủ Dashboard
    │   ├── Orders/                 # Giao diện đặt hàng
    │   └── Shared/                 # Các view dùng chung (_Layout.cshtml, Error.cshtml...)
    │
    └── wwwroot/                   # Tệp tin tĩnh (CSS, JS, các thư viện ngoài)
```

---

## 🎯 Chức Năng Chính Nâng Cao (Lab05)

### 1. Admin Dashboard (Trang chủ)
- Hiển thị 4 thẻ màu thống kê động trực quan:
  - **Equipment**: Tổng số lượng thiết bị hiện có (bao gồm cả các thiết bị đã xóa mềm).
  - **Active**: Số thiết bị đang hoạt động bình thường (`IsDeleted = false`).
  - **Deleted**: Số thiết bị đang nằm trong thùng rác (`IsDeleted = true`).
  - **Logs Today**: Số lượng nhật ký thao tác phát sinh trong ngày hôm nay.
- Khung **Quick Links** giúp điều hướng nhanh chóng tới tất cả các tính năng quan trọng của hệ thống.

### 2. Quản lý thiết bị (CRUD & Kiểm soát đồng thời)
- Xem danh sách thiết bị sắp xếp tăng dần theo ID khóa chính để giữ trật tự hiển thị.
- Xem chi tiết thiết bị và cảnh báo hàng tồn kho ở ngưỡng thấp (`LowStockThreshold`).
- Thêm mới thiết bị và xác thực SKU độc nhất.
- Chỉnh sửa thông tin thiết bị áp dụng **Concurrency Control** thông qua trường `RowVersion` (dạng `byte[]` Timestamp). Khi xảy ra xung đột cập nhật đồng thời bởi hai người dùng khác nhau, hệ thống sẽ phát hiện `DbUpdateConcurrencyException`, báo lỗi rõ ràng và yêu cầu tải lại dữ liệu để bảo vệ tính toàn vẹn.

### 3. Xóa mềm (Soft Delete) & Thùng rác (Trash)
- Nút **Delete** không xóa vĩnh viễn dòng dữ liệu khỏi database, mà chỉ đánh dấu `IsDeleted = true` và lưu lại thời điểm xóa `DeletedAt`.
- Sử dụng EF Core Global Query Filter (`HasQueryFilter(e => !e.IsDeleted)`) để tự động ẩn các thiết bị đã xóa mềm ở tất cả các trang danh sách và truy vấn thông thường.
- Trang **Trash (Thùng rác)** hiển thị danh sách các thiết bị đã bị xóa mềm và cung cấp tính năng **Restore (Khôi phục)** đưa thiết bị trở lại danh sách hoạt động bằng cách gọi `IgnoreQueryFilters()`.

### 4. Nhật ký thao tác (Audit Logs)
- Tự động ghi nhật ký vào database (`AuditLogs` table) ở mức độ chi tiết khi người dùng thực hiện:
  - Thêm mới thiết bị (`Information`): *Product created. ProductId={id}*
  - Chỉnh sửa thiết bị (`Information`): *Product updated. ProductId={id}*
  - Xóa mềm thiết bị (`Warning`): *Product soft deleted. ProductId={id}*
  - Khôi phục thiết bị (`Information`): *Product restored. ProductId={id}*
  - Lỗi truy cập API không hợp lệ (`Error`): *Invalid request: EquipmentId={id} TraceId={TraceId}*
- Trang giao diện hiển thị bảng nhật ký sắp xếp giảm dần theo thời gian. Màu chữ của các dòng log tự động chuyển màu nổi bật dựa trên cấp độ: xám đen cho `Information`, vàng cam cho `Warning`, và đỏ rực cho `Error`.

### 5. Kiểm tra sức khỏe hệ thống (Health Checks)
- Cung cấp hai endpoint kiểm tra trạng thái hoạt động:
  - **`/health/live`**: Endpoint tự kiểm tra tình trạng sống của ứng dụng Web.
  - **`/health/ready`**: Endpoint kiểm tra tính sẵn sàng bằng cách kết nối trực tiếp tới DbContext (SQL Server). Kết quả được render dưới dạng bảng HTML tùy biến chuyên nghiệp, tích hợp nút điều hướng của Layout chính.

### 6. Minimal API
- Route API `/api/equipment/{id:int}` trả về thông tin thiết bị định dạng JSON một cách nhanh chóng dưới dạng AsNoTracking.
- Khi truy cập với một ID không tồn tại, API sẽ tự động ghi log lỗi `Error` vào bảng Audit Logs và trả về phản hồi theo chuẩn cấu trúc lỗi **ProblemDetails** (HTTP 404 Not Found) kèm theo mã định danh `traceId`.

---

## 📝 Các Tệp Tin Cốt Lõi

| File | Vai Trò |
|------|---------|
| `Program.cs` | Cấu hình Startup, Dependency Injection, Middleware, Logger (Serilog), Custom Health Checks & Minimal API endpoint |
| `Data/AppDbContext.cs` | Khai báo DbContext, thiết lập index duy nhất cho SKU, cấu hình Global Query Filter cho xóa mềm và Seed dữ liệu ban đầu |
| `Controllers/HomeController.cs` | Xử lý nạp dữ liệu thống kê động cho Admin Dashboard |
| `Controllers/EquipmentController.cs` | Điều phối hoạt động CRUD, xử lý ngoại lệ xung đột concurrency khi cập nhật thiết bị |
| `Services/EquipmentService.cs` | Thực hiện các nghiệp vụ CRUD, logic xóa mềm, khôi phục thiết bị và ghi nhật ký AuditLog |
| `Views/Home/Index.cshtml` | Giao diện hiển thị Dashboard dạng lưới và vùng liên kết nhanh |
| `Views/AuditLogs/Index.cshtml` | Giao diện hiển thị bảng nhật ký thao tác phân màu theo cấp độ cảnh báo |
