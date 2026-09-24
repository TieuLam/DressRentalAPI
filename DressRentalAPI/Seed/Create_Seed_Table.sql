-- =============================================================================
-- HỆ THỐNG QUẢN LÝ CHO THUÊ VÁY (RENTAL DRESS MANAGEMENT SYSTEM)
-- Script DDL dành cho Microsoft SQL Server (Đơn vị tiền tệ: VNĐ)
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 1. NHÓM PHÂN LOẠI & SẢN PHẨM (CATALOG & INVENTORY)
-- -----------------------------------------------------------------------------

-- Bảng Type (Thể loại lớn: Váy dạ hội, Váy tiệc, Váy cưới, Phụ kiện...)
CREATE TABLE [dbo].[Type] (
    [Id] BIGINT IDENTITY(1, 1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_Type] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Bảng Category (Danh mục: Cúp ngực, Đầm đuôi cá, Áo choàng lông...)
CREATE TABLE [dbo].[Category] (
    [Id] BIGINT IDENTITY(1, 1) NOT NULL,
    [TypeId] BIGINT NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Category_Type] FOREIGN KEY ([TypeId]) 
        REFERENCES [dbo].[Type] ([Id])
);
GO

-- Bảng Product (Mẫu thiết kế trưng bày trên website)
CREATE TABLE [dbo].[Product] (
    [Id] BIGINT IDENTITY(1, 1) NOT NULL,
    [CategoryId] BIGINT NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Color] NVARCHAR(50) NULL,
    [OriginalPrice] BIGINT NOT NULL DEFAULT 0,  -- Giá gốc sản phẩm (VNĐ)
    [Price3Days] BIGINT NOT NULL,               -- Giá thuê gói 3 ngày (VNĐ)
    [Price7Days] BIGINT NOT NULL,               -- Giá thuê gói 7 ngày (VNĐ)
    [DepositRate] INT NOT NULL DEFAULT 100,     -- Tỷ lệ cọc (% ví dụ: 100 = 100%)
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Product_Category] FOREIGN KEY ([CategoryId]) 
        REFERENCES [dbo].[Category] ([Id])
);
GO

-- Bảng ProductImage (Bộ sưu tập ảnh mẫu của từng mẫu váy)
CREATE TABLE [dbo].[ProductImage] (
    [Id] BIGINT IDENTITY(1, 1) NOT NULL,
    [ProductId] BIGINT NOT NULL,
    [ImageUrl] NVARCHAR(500) NOT NULL,
    [IsThumbnail] BIT NOT NULL DEFAULT 0,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_ProductImage] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProductImage_Product] FOREIGN KEY ([ProductId]) 
        REFERENCES [dbo].[Product] ([Id]) ON DELETE CASCADE
);
GO

-- Bảng ProductItem (Từng chiếc váy vật lý thực tế trong kho có gắn mác barcode)
CREATE TABLE [dbo].[ProductItem] (
    [Id] BIGINT IDENTITY(1, 1) NOT NULL,
    [ProductId] BIGINT NOT NULL,
    [ItemCode] VARCHAR(50) NOT NULL,                   -- Mã barcode/mã may trên váy (VD: VAY-1001-M-01)
    [Size] VARCHAR(20) NOT NULL,                       -- S, M, L, Freesize...
    [CurrentCondition] NVARCHAR(100) NOT NULL DEFAULT N'Mới 99%',
    [Status] VARCHAR(50) NOT NULL DEFAULT 'AVAILABLE', 
    -- 'AVAILABLE', 'RENTED', 'IN_LAUNDRY', 'IN_REPAIR', 'DISCARDED'
    CONSTRAINT [PK_ProductItem] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_ProductItem_ItemCode] UNIQUE NONCLUSTERED ([ItemCode] ASC),
    CONSTRAINT [FK_ProductItem_Product] FOREIGN KEY ([ProductId]) 
        REFERENCES [dbo].[Product] ([Id])
);
GO


-- -----------------------------------------------------------------------------
-- 2. NHÓM NGƯỜI DÙNG & KHÁCH HÀNG (AUTH & CUSTOMER PROFILE)
-- -----------------------------------------------------------------------------

-- Bảng User (Tài khoản hệ thống: Admin, Nhân viên, hoặc Khách đăng ký)
CREATE TABLE [dbo].[User] (
    [Id] BIGINT IDENTITY(1, 1) NOT NULL,
    [UserName] VARCHAR(100) NOT NULL,
    [Password] VARCHAR(255) NOT NULL,                  -- Mật khẩu băm (BCrypt/Argon2)
    [Role] VARCHAR(50) NOT NULL DEFAULT 'STAFF',       -- 'ADMIN', 'STAFF', 'CUSTOMER'
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_User_UserName] UNIQUE NONCLUSTERED ([UserName] ASC)
);
GO

-- Bảng CustomerProfile (Thông tin người thuê thực tế - hỗ trợ khách vãng lai)
CREATE TABLE [dbo].[CustomerProfile] (
    [Id] BIGINT IDENTITY(1, 1) NOT NULL,
    [UserId] BIGINT NULL,                              -- NULL nếu là khách vãng lai không đăng ký tài khoản
    [Name] NVARCHAR(100) NOT NULL,
    [Email] VARCHAR(255) NULL,
    [PhoneNumber] VARCHAR(20) NOT NULL,
    [Address] NVARCHAR(500) NULL,
    CONSTRAINT [PK_CustomerProfile] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_CustomerProfile_UserId] UNIQUE NONCLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_CustomerProfile_User] FOREIGN KEY ([UserId]) 
        REFERENCES [dbo].[User] ([Id]) ON DELETE SET NULL
);
GO

-- Bảng ConsultationBooking (Lịch hẹn tư vấn & thử đồ tại showroom)
CREATE TABLE [dbo].[ConsultationBooking] (
    [Id] BIGINT IDENTITY(1, 1) NOT NULL,
    [CustomerId] BIGINT NOT NULL,
    [AppointmentTime] DATETIME2(7) NOT NULL,
    [Notes] NVARCHAR(MAX) NULL,
    [Status] VARCHAR(50) NOT NULL DEFAULT 'PENDING',   -- 'PENDING', 'CONFIRMED', 'COMPLETED', 'CANCELLED'
    CONSTRAINT [PK_ConsultationBooking] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ConsultationBooking_Customer] FOREIGN KEY ([CustomerId]) 
        REFERENCES [dbo].[CustomerProfile] ([Id]) ON DELETE CASCADE
);
GO


-- -----------------------------------------------------------------------------
-- 3. NHÓM ĐƠN THUÊ, CHI TIẾT & THANH TOÁN (ORDERS & PAYMENTS)
-- -----------------------------------------------------------------------------

-- Bảng Order (Đơn thuê tổng thể)
CREATE TABLE [dbo].[Order] (
    [Id] BIGINT IDENTITY(1, 1) NOT NULL,
    [CustomerId] BIGINT NOT NULL,
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NOT NULL,
    [ActualReturnDate] DATE NULL,                      -- Ngày khách mang trả đồ thực tế
    [TotalAmount] BIGINT NOT NULL DEFAULT 0,           -- Tổng tiền thuê (VNĐ)
    [TotalDeposit] BIGINT NOT NULL DEFAULT 0,          -- Tổng tiền cọc cần giữ (VNĐ)
    [Status] VARCHAR(50) NOT NULL DEFAULT 'PENDING',
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSDATETIME(),
    -- 'PENDING', 'CONFIRMED', 'DELIVERED', 'COMPLETED', 'CANCELLED'
    CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Order_CustomerProfile] FOREIGN KEY ([CustomerId]) 
        REFERENCES [dbo].[CustomerProfile] ([Id]),
    CONSTRAINT [CK_Order_Dates] CHECK ([EndDate] >= [StartDate])
);
GO

-- Bảng OrderDetail (Chi tiết từng chiếc váy/món đồ trong đơn)
CREATE TABLE [dbo].[OrderDetail] (
    [Id] BIGINT IDENTITY(1, 1) NOT NULL,
    [OrderId] BIGINT NOT NULL,
    [ItemId] BIGINT NOT NULL,                          -- Trỏ tới chiếc váy vật lý cụ thể
    [RentalType] VARCHAR(50) NOT NULL,                 -- '3_DAYS', '7_DAYS'
    [Price] BIGINT NOT NULL,                           -- Giá thuê áp dụng tại thời điểm tạo đơn (VNĐ)
    [DepositAmount] BIGINT NOT NULL,                   -- Tiền cọc của riêng chiếc váy này (VNĐ)
    CONSTRAINT [PK_OrderDetail] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OrderDetail_Order] FOREIGN KEY ([OrderId]) 
        REFERENCES [dbo].[Order] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderDetail_ProductItem] FOREIGN KEY ([ItemId]) 
        REFERENCES [dbo].[ProductItem] ([Id])
);
GO

-- Bảng Payment (Quản lý các giao dịch thu/chi: Tiền mặt, Chuyển khoản)
CREATE TABLE [dbo].[Payment] (
    [Id] BIGINT IDENTITY(1, 1) NOT NULL,
    [OrderId] BIGINT NOT NULL,
    [PaymentMethod] VARCHAR(50) NOT NULL,              -- 'CASH', 'BANK_TRANSFER'
    [PaymentType] VARCHAR(50) NOT NULL,                -- 'DEPOSIT', 'RENTAL_FEE', 'REFUND', 'PENALTY'
    [Amount] BIGINT NOT NULL,                          -- Số tiền giao dịch (VNĐ)
    [TransactionCode] VARCHAR(100) NULL,               -- Mã giao dịch ngân hàng (nếu có)
    [PaidAt] DATETIME2(7) NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Payment_Order] FOREIGN KEY ([OrderId]) 
        REFERENCES [dbo].[Order] ([Id]) ON DELETE CASCADE
);
GO


-- -----------------------------------------------------------------------------
-- 4. BIÊN BẢN KIỂM TRA TRẢ ĐỒ & PHẠT (RETURN INSPECTION)
-- -----------------------------------------------------------------------------

-- Bảng ReturnInspection (Kiểm tra từng chiếc váy khi khách trả: phạt trễ, rách, bẩn)
CREATE TABLE [dbo].[ReturnInspection] (
    [Id] BIGINT IDENTITY(1, 1) NOT NULL,
    [OrderDetailId] BIGINT NOT NULL,                   -- 1 dòng OrderDetail có tối đa 1 biên bản kiểm tra
    [InspectUserId] BIGINT NULL,                       -- Nhân viên thực hiện nghiệm thu
    [LateFee] BIGINT NOT NULL DEFAULT 0,               -- Tiền phạt trễ hạn (VNĐ)
    [DamageFee] BIGINT NOT NULL DEFAULT 0,             -- Tiền phạt làm rách/ố bẩn (VNĐ)
    [DamageDescription] NVARCHAR(MAX) NULL,            -- Mô tả chi tiết lỗi hỏng
    [RefundedDeposit] BIGINT NOT NULL DEFAULT 0,       -- Tiền cọc hoàn trả thực tế sau trừ phạt (VNĐ)
    [InspectAt] DATETIME2(7) NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT [PK_ReturnInspection] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_ReturnInspection_OrderDetailId] UNIQUE NONCLUSTERED ([OrderDetailId] ASC),
    CONSTRAINT [FK_ReturnInspection_OrderDetail] FOREIGN KEY ([OrderDetailId]) 
        REFERENCES [dbo].[OrderDetail] ([Id]),
    CONSTRAINT [FK_ReturnInspection_User] FOREIGN KEY ([InspectUserId]) 
        REFERENCES [dbo].[User] ([Id]) ON DELETE SET NULL
);
GO


-- -----------------------------------------------------------------------------
-- 5. CHỈ MỤC TỐI ƯU HIỆU NĂNG TRUY VẤN (INDEXES)
-- -----------------------------------------------------------------------------

-- Tìm kiếm khách hàng theo SĐT (dành cho khách vãng lai quay lại)
CREATE NONCLUSTERED INDEX [IX_CustomerProfile_PhoneNumber] 
    ON [dbo].[CustomerProfile] ([PhoneNumber] ASC);
GO

-- Quét mã vạch phục vụ xuất/nhập kho tại quầy
CREATE NONCLUSTERED INDEX [IX_ProductItem_ItemCode] 
    ON [dbo].[ProductItem] ([ItemCode] ASC);
GO

-- Kiểm tra khả dụng / xung đột lịch thuê (Double Booking)
CREATE NONCLUSTERED INDEX [IX_Order_RentalDates_Status] 
    ON [dbo].[Order] ([StartDate] ASC, [EndDate] ASC, [Status] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OrderDetail_ItemId_OrderId] 
    ON [dbo].[OrderDetail] ([ItemId] ASC, [OrderId] ASC);
GO