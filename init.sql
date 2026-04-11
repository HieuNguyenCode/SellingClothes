DROP DATABASE IF EXISTS SellingClothes;
CREATE DATABASE SellingClothes;
USE SellingClothes;

CREATE TABLE IF NOT EXISTS users
(
    IDUser   CHAR(36)                   NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã định danh duy nhất của người dùng',
    UserName VARCHAR(255)               NOT NULL UNIQUE COMMENT 'Tên đăng nhập hoặc họ tên người dùng',
    Password VARCHAR(255)               NOT NULL COMMENT 'Mật khẩu đăng nhập (nên được băm/hash)',
    Role     ENUM ('admin', 'customer') NOT NULL             DEFAULT 'customer' COMMENT 'Vai trò của người dùng trên hệ thống',
    INDEX idx_username (UserName)
);

CREATE TABLE IF NOT EXISTS Company
(
    IDCompany CHAR(36)     NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã định danh công ty/thương hiệu',
    Name      VARCHAR(255) NOT NULL UNIQUE COMMENT 'Tên công ty hoặc thương hiệu',
    INDEX idx_company_name (Name)
);

CREATE TABLE IF NOT EXISTS Type
(
    IDType CHAR(36)     NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã định danh loại sản phẩm (danh mục)',
    Name   VARCHAR(255) NOT NULL UNIQUE COMMENT 'Tên loại sản phẩm (VD: Áo khoác, Quần Jean)',
    INDEX idx_type_name (Name)
);

CREATE TABLE IF NOT EXISTS Product
(
    IDProduct  CHAR(36)     NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã định danh sản phẩm',
    Name       VARCHAR(255) NOT NULL COMMENT 'Tên sản phẩm' UNIQUE,
    Price      INT          NOT NULL COMMENT 'Giá niêm yết của sản phẩm',
    IDCompany  CHAR(36)     NOT NULL COMMENT 'Khóa ngoại tham chiếu đến công ty/thương hiệu',
    IDType     CHAR(36)     NOT NULL COMMENT 'Khóa ngoại tham chiếu đến loại sản phẩm',
    `Describe` TEXT COMMENT 'Mô tả chi tiết về sản phẩm',
    Image      VARCHAR(255) NOT NULL COMMENT 'Đường dẫn lưu trữ file hình ảnh đại diện sản phẩm',
    
    IsPublished BOOLEAN      NOT NULL             DEFAULT FALSE COMMENT 'Cờ đánh dấu sản phẩm đã được xuất bản và hiển thị trên cửa hàng',
    IsDeleted   BOOLEAN      NOT NULL             DEFAULT FALSE COMMENT 'Cờ đánh dấu sản phẩm đã bị xóa (soft delete)',
    
    UpdateBy   CHAR(36) COMMENT 'Người cập nhật cuối cùng',
    CreateBy   CHAR(36)     NOT NULL COMMENT 'Người tạo bản ghi',
    UpdateAt   TIMESTAMP                         DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Thời gian cập nhật cuối',
    CreateAt   TIMESTAMP                         DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời gian tạo bản ghi',
    CONSTRAINT fk_product_com FOREIGN KEY (IDCompany) REFERENCES Company (IDCompany),
    CONSTRAINT fk_product_type FOREIGN KEY (IDType) REFERENCES Type (IDType),
    CONSTRAINT fk_product_user_update FOREIGN KEY (UpdateBy) REFERENCES users (IDUser),
    CONSTRAINT fk_product_user_create FOREIGN KEY (CreateBy) REFERENCES users (IDUser),
    INDEX idx_product_name (Name)
);

CREATE TABLE IF NOT EXISTS Size
(
    IDSize    CHAR(36)     NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã định danh kích cỡ',
    IDProduct CHAR(36)     NOT NULL COMMENT 'Khóa ngoại tham chiếu đến sản phẩm',
    Name      VARCHAR(255) NOT NULL COMMENT 'Tên kích cỡ (VD: S, M, L, XL)',

    CONSTRAINT fk_size_product FOREIGN KEY (IDProduct) REFERENCES Product (IDProduct)
);

CREATE TABLE IF NOT EXISTS Color
(
    IDColor   CHAR(36)     NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã định danh màu sắc',
    IDProduct CHAR(36)     NOT NULL COMMENT 'Khóa ngoại tham chiếu đến sản phẩm',
    Name      VARCHAR(255) NOT NULL COMMENT 'Tên màu sắc (VD: Đen, Trắng, Đỏ)',

    CONSTRAINT fk_color_product FOREIGN KEY (IDProduct) REFERENCES Product (IDProduct)
);

CREATE TABLE IF NOT EXISTS Image
(
    IDImage   CHAR(36)     NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã định danh hình ảnh',
    URL       VARCHAR(255) NOT NULL COMMENT 'Đường dẫn lưu trữ file hình ảnh',
    IDProduct CHAR(36)     NOT NULL COMMENT 'Sản phẩm mà hình ảnh này thuộc về',

    CreateBy  CHAR(36)     NOT NULL COMMENT 'Người tải ảnh lên',
    CreateAt  TIMESTAMP                         DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời gian tải ảnh lên',
    CONSTRAINT fk_img_product FOREIGN KEY (IDProduct) REFERENCES Product (IDProduct),
    CONSTRAINT fk_img_user FOREIGN KEY (CreateBy) REFERENCES users (IDUser)
);

CREATE TABLE IF NOT EXISTS Combo
(
    IDCombo  CHAR(36)     NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã định danh combo sản phẩm',
    Name     VARCHAR(255) NOT NULL COMMENT 'Tên gọi của combo',
    Price    INT          NOT NULL COMMENT 'Giá bán tổng hợp của combo',
    Image    VARCHAR(255) NOT NULL COMMENT 'Đường dẫn lưu trữ file hình ảnh đại diện combo',
    
    IsPublished BOOLEAN      NOT NULL             DEFAULT FALSE COMMENT 'Cờ đánh dấu sản phẩm đã được xuất bản và hiển thị trên cửa hàng',
    IsDeleted   BOOLEAN      NOT NULL             DEFAULT FALSE COMMENT 'Cờ đánh dấu sản phẩm đã bị xóa (soft delete)',

    UpdateBy CHAR(36) COMMENT 'Người cập nhật cuối cùng',
    CreateBy CHAR(36)     NOT NULL COMMENT 'Người tạo combo',
    UpdateAt TIMESTAMP                         DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Thời gian cập nhật cuối',
    CreateAt TIMESTAMP                         DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời gian tạo combo',
    CONSTRAINT fk_combo_user_update FOREIGN KEY (UpdateBy) REFERENCES users (IDUser),
    CONSTRAINT fk_combo_user_create FOREIGN KEY (CreateBy) REFERENCES users (IDUser)
);

CREATE TABLE IF NOT EXISTS ComboProduct
(
    IDComboProduct CHAR(36) NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã chi tiết liên kết Combo và Product',
    IDCombo        CHAR(36) NOT NULL COMMENT 'Khóa ngoại tham chiếu đến Combo',
    IDProduct      CHAR(36) NOT NULL COMMENT 'Khóa ngoại tham chiếu đến Product nằm trong Combo',
    Quantity       INT      NOT NULL COMMENT 'Số lượng sản phẩm này trong combo (VD: Combo gồm 2 áo và 1 quần thì Quantity của áo là 2, của quần là 1)',
    UpdateBy       CHAR(36) COMMENT 'Người cập nhật liên kết',
    CreateBy       CHAR(36) NOT NULL COMMENT 'Người tạo liên kết',
    UpdateAt       TIMESTAMP                     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Thời gian cập nhật cuối',
    CreateAt       TIMESTAMP                     DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời gian tạo bản ghi',
    CONSTRAINT fk_cp_combo FOREIGN KEY (IDCombo) REFERENCES Combo (IDCombo),
    CONSTRAINT fk_cp_product FOREIGN KEY (IDProduct) REFERENCES Product (IDProduct),
    CONSTRAINT fk_cp_user_update FOREIGN KEY (UpdateBy) REFERENCES users (IDUser),
    CONSTRAINT fk_cp_user_create FOREIGN KEY (CreateBy) REFERENCES users (IDUser)
);

CREATE TABLE IF NOT EXISTS Sale
(
    IDSale CHAR(36)     NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã chương trình khuyến mãi/giảm giá',
    Name   VARCHAR(255) NOT NULL COMMENT 'Tên chương trình khuyến mãi'
);

CREATE TABLE IF NOT EXISTS SaleProduct
(
    IDSaleProduct CHAR(36) NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã chi tiết áp dụng khuyến mãi cho sản phẩm',
    IDSale        CHAR(36) NOT NULL COMMENT 'Khóa ngoại tham chiếu chương trình khuyến mãi',
    IDProduct     CHAR(36) NOT NULL COMMENT 'Khóa ngoại tham chiếu sản phẩm được giảm giá',
    Price         INT      NOT NULL COMMENT 'Giá bán sau khi áp dụng giảm giá (giá cố định trong suốt thời gian khuyến mãi)',
    StartDate     DATETIME NOT NULL COMMENT 'Thời gian bắt đầu áp dụng giảm giá',
    EndDate       DATETIME COMMENT 'Thời gian kết thúc giảm giá',

    UpdateBy      CHAR(36) COMMENT 'Người cập nhật thông tin',
    CreateBy      CHAR(36) NOT NULL COMMENT 'Người thiết lập khuyến mãi',
    UpdateAt      TIMESTAMP                     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Thời gian cập nhật cuối',
    CreateAt      TIMESTAMP                     DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời gian tạo thiết lập',
    CONSTRAINT fk_sp_sale FOREIGN KEY (IDSale) REFERENCES Sale (IDSale),
    CONSTRAINT fk_sp_product FOREIGN KEY (IDProduct) REFERENCES Product (IDProduct),
    CONSTRAINT fk_sp_user_update FOREIGN KEY (UpdateBy) REFERENCES users (IDUser),
    CONSTRAINT fk_sp_user_create FOREIGN KEY (CreateBy) REFERENCES users (IDUser),
    UNIQUE (IDSale, IDProduct)
);

CREATE TABLE IF NOT EXISTS SaleCombo
(
    IDSaleCombo CHAR(36) NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã chi tiết áp dụng khuyến mãi cho combo',
    IDSale      CHAR(36) NOT NULL COMMENT 'Khóa ngoại tham chiếu chương trình khuyến mãi',
    IDCombo     CHAR(36) NOT NULL COMMENT 'Khóa ngoại tham chiếu combo được giảm giá',
    Price       INT      NOT NULL COMMENT 'Giá bán sau khi áp dụng giảm giá (giá cố định trong suốt thời gian khuyến mãi)',
    StartDate   DATETIME NOT NULL COMMENT 'Thời gian bắt đầu áp dụng giảm giá',
    EndDate     DATETIME COMMENT 'Thời gian kết thúc giảm giá',

    UpdateBy    CHAR(36) COMMENT 'Người cập nhật thông tin',
    CreateBy    CHAR(36) NOT NULL COMMENT 'Người thiết lập khuyến mãi',
    UpdateAt    TIMESTAMP                     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Thời gian cập nhật cuối',
    CreateAt    TIMESTAMP                     DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời gian tạo thiết lập',
    CONSTRAINT fk_sc_sale FOREIGN KEY (IDSale) REFERENCES Sale (IDSale),
    CONSTRAINT fk_sc_combo FOREIGN KEY (IDCombo) REFERENCES Combo (IDCombo),
    CONSTRAINT fk_sc_user_update FOREIGN KEY (UpdateBy) REFERENCES users (IDUser),
    CONSTRAINT fk_sc_user_create FOREIGN KEY (CreateBy) REFERENCES users (IDUser),
    UNIQUE (IDSale, IDCombo)
);

CREATE TABLE IF NOT EXISTS ShoppingCart
(
    IDShoppingCart CHAR(36)     NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã định danh giỏ hàng',
    IDUser         CHAR(36)     NULL COMMENT 'Khóa ngoại định danh người dùng (NULL nếu chưa đăng nhập)',
    SessionID      VARCHAR(255) NULL COMMENT 'Mã phiên làm việc lưu trên trình duyệt cho khách vãng lai',
    TotalPrice     INT          NOT NULL             DEFAULT 0 COMMENT 'Tổng giá trị hiện tại của giỏ hàng',

    UpdateBy       CHAR(36)     NULL COMMENT 'Người cập nhật cuối cùng',
    CreateBy       CHAR(36)     NULL COMMENT 'Người tạo giỏ hàng',
    UpdateAt       TIMESTAMP                         DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Thời gian cập nhật giỏ hàng cuối cùng',
    CreateAt       TIMESTAMP                         DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời gian khởi tạo giỏ hàng',

    CONSTRAINT fk_cart_user FOREIGN KEY (IDUser) REFERENCES users (IDUser),
    CONSTRAINT fk_cart_user_update FOREIGN KEY (UpdateBy) REFERENCES users (IDUser),
    CONSTRAINT fk_cart_user_create FOREIGN KEY (CreateBy) REFERENCES users (IDUser)
);

CREATE TABLE IF NOT EXISTS ShoppingCartItem
(
    IDShoppingCartItem CHAR(36) NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã chi tiết từng món trong giỏ hàng',
    IDShoppingCart     CHAR(36) NOT NULL COMMENT 'Khóa ngoại thuộc về giỏ hàng nào',
    IDProduct          CHAR(36) NULL COMMENT 'Mã sản phẩm được chọn (NULL nếu chọn Combo)',
    IDCombo            CHAR(36) NULL COMMENT 'Mã combo được chọn (NULL nếu chọn Product)',
    IDColor            CHAR(36) NULL COMMENT 'Mã màu sắc sản phẩm được chọn',
    IDSize             CHAR(36) NULL COMMENT 'Mã kích cỡ sản phẩm được chọn (nếu có)',
    Quantity           INT      NOT NULL             DEFAULT 1 COMMENT 'Số lượng sản phẩm/combo muốn mua',
    UnitPrice          INT      NOT NULL COMMENT 'Đơn giá lưu cứng tại thời điểm khách thêm vào giỏ',

    UpdateBy           CHAR(36) NULL COMMENT 'Người cập nhật số lượng/món hàng',
    CreateBy           CHAR(36) NULL COMMENT 'Người thêm món hàng vào giỏ',
    UpdateAt           TIMESTAMP                     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Thời gian thay đổi chi tiết giỏ hàng',
    CreateAt           TIMESTAMP                     DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời gian thêm món hàng vào giỏ',

    CONSTRAINT fk_ci_cart FOREIGN KEY (IDShoppingCart) REFERENCES ShoppingCart (IDShoppingCart),
    CONSTRAINT fk_ci_product FOREIGN KEY (IDProduct) REFERENCES Product (IDProduct),
    CONSTRAINT fk_ci_combo FOREIGN KEY (IDCombo) REFERENCES Combo (IDCombo),
    CONSTRAINT fk_ci_color FOREIGN KEY (IDColor) REFERENCES Color (IDColor),
    CONSTRAINT fk_ci_user_update FOREIGN KEY (UpdateBy) REFERENCES users (IDUser),
    CONSTRAINT fk_ci_user_create FOREIGN KEY (CreateBy) REFERENCES users (IDUser),
    CONSTRAINT fk_ci_size FOREIGN KEY (IDSize) REFERENCES Size (IDSize),

    CONSTRAINT CHK_CartItem_Type CHECK (
        (IDProduct IS NOT NULL AND IDCombo IS NULL) OR
        (IDProduct IS NULL AND IDCombo IS NOT NULL)
        )
);

CREATE TABLE IF NOT EXISTS CartComboProduct
(
    IDCartComboProduct CHAR(36) NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã chi tiết liên kết Combo và Product trong giỏ hàng',
    IDShoppingCartItem CHAR(36) NOT NULL COMMENT 'Khóa ngoại tham chiếu đến món hàng trong giỏ (dòng sản phẩm hoặc combo)',
    IDProduct         CHAR(36) NOT NULL COMMENT 'Khóa ngoại tham chiếu đến sản phẩm nằm trong combo của món hàng này',
    Quantity          INT      NOT NULL COMMENT 'Số lượng sản phẩm này trong combo của món hàng',
    IDColor            CHAR(36) NULL COMMENT 'Màu sắc cụ thể của sản phẩm khách đã chọn',
    IDSize             CHAR(36) NULL COMMENT 'Kích cỡ cụ thể của sản',
    UpdateBy          CHAR(36) COMMENT 'Người cập nhật liên kết',
    CreateBy          CHAR(36) NOT NULL COMMENT 'Người tạo liên kết',
    UpdateAt          TIMESTAMP                     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Thời gian cập nhật cuối',
    CreateAt          TIMESTAMP                     DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời gian tạo bản ghi',
    CONSTRAINT fk_ccp_cart_item FOREIGN KEY (IDShoppingCartItem) REFERENCES ShoppingCartItem (IDShoppingCartItem),
    CONSTRAINT fk_ccp_product FOREIGN KEY (IDProduct) REFERENCES Product (IDProduct),
    CONSTRAINT fk_ccp_user_update FOREIGN KEY (UpdateBy) REFERENCES users (IDUser),
    CONSTRAINT fk_ccp_user_create FOREIGN KEY (CreateBy) REFERENCES users (IDUser),
    CONSTRAINT fk_ccp_color FOREIGN KEY (IDColor) REFERENCES Color (IDColor),
    CONSTRAINT fk_ccp_size FOREIGN KEY (IDSize) REFERENCES Size (IDSize)
);

CREATE TABLE IF NOT EXISTS Orders
(
    IDOrder         CHAR(36)     NOT NULL PRIMARY KEY                                   DEFAULT (UUID()) COMMENT 'Mã định danh hóa đơn/đơn hàng',
    IDUser          CHAR(36)     NULL COMMENT 'Tài khoản người đặt mua (NULL nếu mua không cần tài khoản)',
    SessionID       VARCHAR(255) NULL COMMENT 'Mã phiên làm việc nếu khách vãng lai đặt hàng',

    CustomerName    VARCHAR(255) NOT NULL COMMENT 'Họ tên người nhận hàng',
    PhoneNumber     VARCHAR(20)  NOT NULL COMMENT 'Số điện thoại liên hệ nhận hàng',
    ShippingAddress TEXT         NOT NULL COMMENT 'Địa chỉ giao hàng chi tiết',

    TotalPrice      INT          NOT NULL COMMENT 'Tổng số tiền khách phải thanh toán cho đơn này',

    OrderStatus     ENUM ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled') DEFAULT 'Pending' COMMENT 'Tiến trình xử lý đơn hàng',
    PaymentMethod   ENUM ('COD', 'BankTransfer', 'CreditCard')                          DEFAULT 'COD' COMMENT 'Phương thức khách hàng chọn thanh toán',
    PaymentStatus   ENUM ('Unpaid', 'Paid', 'Refunded')                                 DEFAULT 'Unpaid' COMMENT 'Trạng thái chuyển tiền thực tế',

    UpdateBy        CHAR(36)     NULL COMMENT 'Nhân viên/Hệ thống cập nhật trạng thái đơn',
    CreateBy        CHAR(36)     NULL COMMENT 'Người khởi tạo đơn hàng',
    UpdateAt        TIMESTAMP                                                           DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Thời gian cập nhật trạng thái cuối cùng',
    CreateAt        TIMESTAMP                                                           DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời điểm chốt đơn hàng',

    CONSTRAINT fk_order_user FOREIGN KEY (IDUser) REFERENCES users (IDUser),
    CONSTRAINT fk_order_user_update FOREIGN KEY (UpdateBy) REFERENCES users (IDUser),
    CONSTRAINT fk_order_user_create FOREIGN KEY (CreateBy) REFERENCES users (IDUser)
);

CREATE TABLE IF NOT EXISTS OrderDetail
(
    IDOrderDetail CHAR(36) NOT NULL PRIMARY KEY DEFAULT (UUID()) COMMENT 'Mã chi tiết từng dòng trong hóa đơn',
    IDOrder       CHAR(36) NOT NULL COMMENT 'Khóa ngoại thuộc về hóa đơn nào',
    IDProduct     CHAR(36) NULL COMMENT 'Sản phẩm đã mua (NULL nếu là Combo)',
    IDCombo       CHAR(36) NULL COMMENT 'Combo đã mua (NULL nếu là Product)',
    IDColor       CHAR(36) NULL COMMENT 'Màu sắc cụ thể của sản phẩm khách đã chọn',
    IDSize        CHAR(36) NULL COMMENT 'Kích cỡ cụ thể của sản phẩm khách đã chọn (nếu có)',

    Quantity      INT      NOT NULL             DEFAULT 1 COMMENT 'Số lượng mua',
    UnitPrice     INT      NOT NULL COMMENT 'Đơn giá cố định lúc xuất hóa đơn (không thay đổi nếu giá gốc đổi)',
    SubTotal      INT AS (Quantity * UnitPrice) STORED COMMENT 'Cột tính toán tự động: Thành tiền = Số lượng x Đơn giá',

    UpdateBy      CHAR(36) NULL COMMENT 'Người cập nhật chi tiết',
    CreateBy      CHAR(36) NULL COMMENT 'Người tạo chi tiết',
    UpdateAt      TIMESTAMP                     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Thời gian cập nhật bản ghi',
    CreateAt      TIMESTAMP                     DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời gian tạo bản ghi',

    CONSTRAINT fk_od_order FOREIGN KEY (IDOrder) REFERENCES Orders (IDOrder),
    CONSTRAINT fk_od_product FOREIGN KEY (IDProduct) REFERENCES Product (IDProduct),
    CONSTRAINT fk_od_combo FOREIGN KEY (IDCombo) REFERENCES Combo (IDCombo),
    CONSTRAINT fk_od_color FOREIGN KEY (IDColor) REFERENCES Color (IDColor),
    CONSTRAINT fk_od_user_update FOREIGN KEY (UpdateBy) REFERENCES users (IDUser),
    CONSTRAINT fk_od_user_create FOREIGN KEY (CreateBy) REFERENCES users (IDUser),
    CONSTRAINT fk_od_size FOREIGN KEY (IDSize) REFERENCES Size (IDSize),

    CONSTRAINT CHK_OrderDetail_Type CHECK (
        (IDProduct IS NOT NULL AND IDCombo IS NULL) OR
        (IDProduct IS NULL AND IDCombo IS NOT NULL)
        )
);

INSERT INTO users (UserName, Password, Role) VALUES 
('admin', '$2a$11$/5EILZrELlTN5FTThpcm/uM1LaVjkfCRwjeBJOxBBBtIJFOVJNRWi', 'admin');