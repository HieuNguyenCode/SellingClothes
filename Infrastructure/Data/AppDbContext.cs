using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Type = Core.Entities.Type;

namespace Infrastructure.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CartComboProduct> Cartcomboproduct { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<Combo> Combos { get; set; }

    public virtual DbSet<ComboProduct> ComboProducts { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleCombo> SaleCombos { get; set; }

    public virtual DbSet<SaleProduct> SaleProducts { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<Type> Types { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_uca1400_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<CartComboProduct>(entity =>
        {
            entity.HasKey(e => e.IdcartComboProduct).HasName("PRIMARY");

            entity.ToTable("cartcomboproduct");

            entity.HasIndex(e => e.IdshoppingCartItem, "fk_ccp_cart_item");

            entity.HasIndex(e => e.Idcolor, "fk_ccp_color");

            entity.HasIndex(e => e.Idproduct, "fk_ccp_product");

            entity.HasIndex(e => e.Idsize, "fk_ccp_size");

            entity.HasIndex(e => e.CreateBy, "fk_ccp_user_create");

            entity.HasIndex(e => e.UpdateBy, "fk_ccp_user_update");

            entity.Property(e => e.IdcartComboProduct)
                .HasComment("Mã chi tiết liên kết Combo và Product trong giỏ hàng")
                .HasColumnName("IDCartComboProduct");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian tạo bản ghi")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreateBy).HasComment("Người tạo liên kết");
            entity.Property(e => e.Idcolor)
                .HasComment("Màu sắc cụ thể của sản phẩm khách đã chọn")
                .HasColumnName("IDColor");
            entity.Property(e => e.Idproduct)
                .HasComment("Khóa ngoại tham chiếu đến sản phẩm nằm trong combo của món hàng này")
                .HasColumnName("IDProduct");
            entity.Property(e => e.IdshoppingCartItem)
                .HasComment("Khóa ngoại tham chiếu đến món hàng trong giỏ (dòng sản phẩm hoặc combo)")
                .HasColumnName("IDShoppingCartItem");
            entity.Property(e => e.Idsize)
                .HasComment("Kích cỡ cụ thể của sản")
                .HasColumnName("IDSize");
            entity.Property(e => e.Quantity)
                .HasComment("Số lượng sản phẩm này trong combo của món hàng")
                .HasColumnType("int(11)");
            entity.Property(e => e.UpdateAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian cập nhật cuối")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdateBy).HasComment("Người cập nhật liên kết");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.CartComboProductCreateByNavigations)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ccp_user_create");

            entity.HasOne(d => d.IdcolorNavigation).WithMany(p => p.Cartcomboproduct)
                .HasForeignKey(d => d.Idcolor)
                .HasConstraintName("fk_ccp_color");

            entity.HasOne(d => d.IdproductNavigation).WithMany(p => p.CartComboProducts)
                .HasForeignKey(d => d.Idproduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ccp_product");

            entity.HasOne(d => d.IdshoppingCartItemNavigation).WithMany(p => p.Cartcomboproduct)
                .HasForeignKey(d => d.IdshoppingCartItem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ccp_cart_item");

            entity.HasOne(d => d.IdsizeNavigation).WithMany(p => p.Cartcomboproduct)
                .HasForeignKey(d => d.Idsize)
                .HasConstraintName("fk_ccp_size");

            entity.HasOne(d => d.UpdateByNavigation).WithMany(p => p.CartComboProductUpdateByNavigations)
                .HasForeignKey(d => d.UpdateBy)
                .HasConstraintName("fk_ccp_user_update");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Idcolor).HasName("PRIMARY");

            entity.ToTable("Color");

            entity.HasIndex(e => e.Idproduct, "fk_color_product");

            entity.Property(e => e.Idcolor)
                .HasComment("Mã định danh màu sắc")
                .HasColumnName("IDColor");
            entity.Property(e => e.Idproduct)
                .HasComment("Khóa ngoại tham chiếu đến sản phẩm")
                .HasColumnName("IDProduct");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasComment("Tên màu sắc (VD: Đen, Trắng, Đỏ)");

            entity.HasOne(d => d.IdproductNavigation).WithMany(p => p.Color)
                .HasForeignKey(d => d.Idproduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_color_product");
        });

        modelBuilder.Entity<Combo>(entity =>
        {
            entity.HasKey(e => e.Idcombo).HasName("PRIMARY");

            entity.ToTable("Combo");

            entity.HasIndex(e => e.CreateBy, "fk_combo_user_create");

            entity.HasIndex(e => e.UpdateBy, "fk_combo_user_update");

            entity.Property(e => e.Idcombo)
                .HasComment("Mã định danh combo sản phẩm")
                .HasColumnName("IDCombo");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian tạo combo")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreateBy).HasComment("Người tạo combo");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasComment("Đường dẫn lưu trữ file hình ảnh đại diện combo");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("'0'");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasComment("Tên gọi của combo");
            entity.Property(e => e.Price)
                .HasComment("Giá bán tổng hợp của combo")
                .HasColumnType("int(11)");
            entity.Property(e => e.UpdateAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian cập nhật cuối")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdateBy).HasComment("Người cập nhật cuối cùng");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.ComboCreateByNavigations)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_combo_user_create");

            entity.HasOne(d => d.UpdateByNavigation).WithMany(p => p.ComboUpdateByNavigations)
                .HasForeignKey(d => d.UpdateBy)
                .HasConstraintName("fk_combo_user_update");
        });

        modelBuilder.Entity<ComboProduct>(entity =>
        {
            entity.HasKey(e => e.IdcomboProduct).HasName("PRIMARY");

            entity.ToTable("comboproduct");

            entity.HasIndex(e => e.Idcombo, "fk_cp_combo");

            entity.HasIndex(e => e.Idproduct, "fk_cp_product");

            entity.HasIndex(e => e.CreateBy, "fk_cp_user_create");

            entity.HasIndex(e => e.UpdateBy, "fk_cp_user_update");

            entity.Property(e => e.IdcomboProduct)
                .HasComment("Mã chi tiết liên kết Combo và Product")
                .HasColumnName("IDComboProduct");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian tạo bản ghi")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreateBy).HasComment("Người tạo liên kết");
            entity.Property(e => e.Idcombo)
                .HasComment("Khóa ngoại tham chiếu đến Combo")
                .HasColumnName("IDCombo");
            entity.Property(e => e.Idproduct)
                .HasComment("Khóa ngoại tham chiếu đến Product nằm trong Combo")
                .HasColumnName("IDProduct");
            entity.Property(e => e.Quantity)
                .HasComment(
                    "Số lượng sản phẩm này trong combo (VD: Combo gồm 2 áo và 1 quần thì Quantity của áo là 2, của quần là 1)")
                .HasColumnType("int(11)");
            entity.Property(e => e.UpdateAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian cập nhật cuối")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdateBy).HasComment("Người cập nhật liên kết");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.ComboProductCreateByNavigations)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cp_user_create");

            entity.HasOne(d => d.IdcomboNavigation).WithMany(p => p.ComboProducts)
                .HasForeignKey(d => d.Idcombo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cp_combo");

            entity.HasOne(d => d.IdproductNavigation).WithMany(p => p.ComboProducts)
                .HasForeignKey(d => d.Idproduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cp_product");

            entity.HasOne(d => d.UpdateByNavigation).WithMany(p => p.ComboProductUpdateByNavigations)
                .HasForeignKey(d => d.UpdateBy)
                .HasConstraintName("fk_cp_user_update");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Idcompany).HasName("PRIMARY");

            entity.ToTable("Company");

            entity.HasIndex(e => e.Name, "idx_company_name").IsUnique();

            entity.Property(e => e.Idcompany)
                .HasComment("Mã định danh công ty/thương hiệu")
                .HasColumnName("IDCompany");
            entity.Property(e => e.Name).HasComment("Tên công ty hoặc thương hiệu");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Idimage).HasName("PRIMARY");

            entity.ToTable("Image");

            entity.HasIndex(e => e.Idproduct, "fk_img_product");

            entity.HasIndex(e => e.CreateBy, "fk_img_user");

            entity.Property(e => e.Idimage)
                .HasComment("Mã định danh hình ảnh")
                .HasColumnName("IDImage");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian tải ảnh lên")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreateBy).HasComment("Người tải ảnh lên");
            entity.Property(e => e.Idproduct)
                .HasComment("Sản phẩm mà hình ảnh này thuộc về")
                .HasColumnName("IDProduct");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasComment("Đường dẫn lưu trữ file hình ảnh")
                .HasColumnName("URL");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Images)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_img_user");

            entity.HasOne(d => d.IdproductNavigation).WithMany(p => p.Images)
                .HasForeignKey(d => d.Idproduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_img_product");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.IdorderDetail).HasName("PRIMARY");

            entity.ToTable("OrderDetail");

            entity.HasIndex(e => e.Idcolor, "fk_od_color");

            entity.HasIndex(e => e.Idcombo, "fk_od_combo");

            entity.HasIndex(e => e.Idorder, "fk_od_order");

            entity.HasIndex(e => e.Idproduct, "fk_od_product");

            entity.HasIndex(e => e.Idsize, "fk_od_size");

            entity.HasIndex(e => e.CreateBy, "fk_od_user_create");

            entity.HasIndex(e => e.UpdateBy, "fk_od_user_update");

            entity.Property(e => e.IdorderDetail)
                .HasComment("Mã chi tiết từng dòng trong hóa đơn")
                .HasColumnName("IDOrderDetail");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian tạo bản ghi")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreateBy).HasComment("Người tạo chi tiết");
            entity.Property(e => e.Idcolor)
                .HasComment("Màu sắc cụ thể của sản phẩm khách đã chọn")
                .HasColumnName("IDColor");
            entity.Property(e => e.Idcombo)
                .HasComment("Combo đã mua (NULL nếu là Product)")
                .HasColumnName("IDCombo");
            entity.Property(e => e.Idorder)
                .HasComment("Khóa ngoại thuộc về hóa đơn nào")
                .HasColumnName("IDOrder");
            entity.Property(e => e.Idproduct)
                .HasComment("Sản phẩm đã mua (NULL nếu là Combo)")
                .HasColumnName("IDProduct");
            entity.Property(e => e.Idsize)
                .HasComment("Kích cỡ cụ thể của sản phẩm khách đã chọn (nếu có)")
                .HasColumnName("IDSize");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasComment("Số lượng mua")
                .HasColumnType("int(11)");
            entity.Property(e => e.SubTotal)
                .HasComputedColumnSql("`Quantity` * `UnitPrice`", true)
                .HasComment("Cột tính toán tự động: Thành tiền = Số lượng x Đơn giá")
                .HasColumnType("int(11)");
            entity.Property(e => e.UnitPrice)
                .HasComment("Đơn giá cố định lúc xuất hóa đơn (không thay đổi nếu giá gốc đổi)")
                .HasColumnType("int(11)");
            entity.Property(e => e.UpdateAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian cập nhật bản ghi")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdateBy).HasComment("Người cập nhật chi tiết");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.OrderDetailCreateByNavigations)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("fk_od_user_create");

            entity.HasOne(d => d.IdcolorNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.Idcolor)
                .HasConstraintName("fk_od_color");

            entity.HasOne(d => d.IdcomboNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.Idcombo)
                .HasConstraintName("fk_od_combo");

            entity.HasOne(d => d.IdorderNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.Idorder)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_od_order");

            entity.HasOne(d => d.IdproductNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.Idproduct)
                .HasConstraintName("fk_od_product");

            entity.HasOne(d => d.IdsizeNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.Idsize)
                .HasConstraintName("fk_od_size");

            entity.HasOne(d => d.UpdateByNavigation).WithMany(p => p.OrderDetailUpdateByNavigations)
                .HasForeignKey(d => d.UpdateBy)
                .HasConstraintName("fk_od_user_update");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Idorder).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.Iduser, "fk_order_user");

            entity.HasIndex(e => e.CreateBy, "fk_order_user_create");

            entity.HasIndex(e => e.UpdateBy, "fk_order_user_update");

            entity.Property(e => e.Idorder)
                .HasComment("Mã định danh hóa đơn/đơn hàng")
                .HasColumnName("IDOrder");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời điểm chốt đơn hàng")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreateBy).HasComment("Người khởi tạo đơn hàng");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(255)
                .HasComment("Họ tên người nhận hàng");
            entity.Property(e => e.Iduser)
                .HasComment("Tài khoản người đặt mua (NULL nếu mua không cần tài khoản)")
                .HasColumnName("IDUser");
            entity.Property(e => e.OrderStatus)
                .HasDefaultValueSql("'Pending'")
                .HasComment("Tiến trình xử lý đơn hàng")
                .HasColumnType("enum('Pending','Processing','Shipped','Delivered','Cancelled')");
            entity.Property(e => e.PaymentMethod)
                .HasDefaultValueSql("'COD'")
                .HasComment("Phương thức khách hàng chọn thanh toán")
                .HasColumnType("enum('COD','BankTransfer','CreditCard')");
            entity.Property(e => e.PaymentStatus)
                .HasDefaultValueSql("'Unpaid'")
                .HasComment("Trạng thái chuyển tiền thực tế")
                .HasColumnType("enum('Unpaid','Paid','Refunded')");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasComment("Số điện thoại liên hệ nhận hàng");
            entity.Property(e => e.SessionId)
                .HasMaxLength(255)
                .HasComment("Mã phiên làm việc nếu khách vãng lai đặt hàng")
                .HasColumnName("SessionID");
            entity.Property(e => e.ShippingAddress)
                .HasComment("Địa chỉ giao hàng chi tiết")
                .HasColumnType("text");
            entity.Property(e => e.TotalPrice)
                .HasComment("Tổng số tiền khách phải thanh toán cho đơn này")
                .HasColumnType("int(11)");
            entity.Property(e => e.UpdateAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian cập nhật trạng thái cuối cùng")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdateBy).HasComment("Nhân viên/Hệ thống cập nhật trạng thái đơn");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.OrderCreateByNavigations)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("fk_order_user_create");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.OrderIduserNavigations)
                .HasForeignKey(d => d.Iduser)
                .HasConstraintName("fk_order_user");

            entity.HasOne(d => d.UpdateByNavigation).WithMany(p => p.OrderUpdateByNavigations)
                .HasForeignKey(d => d.UpdateBy)
                .HasConstraintName("fk_order_user_update");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Idproduct).HasName("PRIMARY");

            entity.ToTable("product");

            entity.HasIndex(e => e.Idcompany, "fk_product_com");

            entity.HasIndex(e => e.Idtype, "fk_product_type");

            entity.HasIndex(e => e.CreateBy, "fk_product_user_create");

            entity.HasIndex(e => e.UpdateBy, "fk_product_user_update");

            entity.HasIndex(e => e.Name, "idx_product_name").IsUnique();

            entity.Property(e => e.Idproduct)
                .HasComment("Mã định danh sản phẩm")
                .HasColumnName("IDProduct");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian tạo bản ghi")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreateBy).HasComment("Người tạo bản ghi");
            entity.Property(e => e.Describe)
                .HasComment("Mô tả chi tiết về sản phẩm")
                .HasColumnType("text");
            entity.Property(e => e.Idcompany)
                .HasComment("Khóa ngoại tham chiếu đến công ty/thương hiệu")
                .HasColumnName("IDCompany");
            entity.Property(e => e.Idtype)
                .HasComment("Khóa ngoại tham chiếu đến loại sản phẩm")
                .HasColumnName("IDType");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasComment("Đường dẫn lưu trữ file hình ảnh đại diện sản phẩm");
            entity.Property(e => e.IsDeleted).HasComment("Cờ đánh dấu sản phẩm đã bị xóa (soft delete)");
            entity.Property(e => e.IsPublished).HasComment("Cờ đánh dấu sản phẩm đã được xuất bản và hiển thị trên cửa hàng");
            entity.Property(e => e.Name).HasComment("Tên sản phẩm");
            entity.Property(e => e.Price)
                .HasComment("Giá niêm yết của sản phẩm")
                .HasColumnType("int(11)");
            entity.Property(e => e.UpdateAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian cập nhật cuối")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdateBy).HasComment("Người cập nhật cuối cùng");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.ProductCreateByNavigations)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_user_create");

            entity.HasOne(d => d.IdcompanyNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Idcompany)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_com");

            entity.HasOne(d => d.IdtypeNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Idtype)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_type");

            entity.HasOne(d => d.UpdateByNavigation).WithMany(p => p.ProductUpdateByNavigations)
                .HasForeignKey(d => d.UpdateBy)
                .HasConstraintName("fk_product_user_update");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Idsale).HasName("PRIMARY");

            entity.ToTable("Sale");

            entity.Property(e => e.Idsale)
                .HasComment("Mã chương trình khuyến mãi/giảm giá")
                .HasColumnName("IDSale");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasComment("Tên chương trình khuyến mãi");
        });

        modelBuilder.Entity<SaleCombo>(entity =>
        {
            entity.HasKey(e => e.IdsaleCombo).HasName("PRIMARY");

            entity.ToTable("SaleCombo");

            entity.HasIndex(e => new { e.Idsale, e.Idcombo }, "IDSale").IsUnique();

            entity.HasIndex(e => e.Idcombo, "fk_sc_combo");

            entity.HasIndex(e => e.CreateBy, "fk_sc_user_create");

            entity.HasIndex(e => e.UpdateBy, "fk_sc_user_update");

            entity.Property(e => e.IdsaleCombo)
                .HasComment("Mã chi tiết áp dụng khuyến mãi cho combo")
                .HasColumnName("IDSaleCombo");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian tạo thiết lập")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreateBy).HasComment("Người thiết lập khuyến mãi");
            entity.Property(e => e.EndDate)
                .HasComment("Thời gian kết thúc giảm giá")
                .HasColumnType("datetime");
            entity.Property(e => e.Idcombo)
                .HasComment("Khóa ngoại tham chiếu combo được giảm giá")
                .HasColumnName("IDCombo");
            entity.Property(e => e.Idsale)
                .HasComment("Khóa ngoại tham chiếu chương trình khuyến mãi")
                .HasColumnName("IDSale");
            entity.Property(e => e.Price)
                .HasComment("Giá bán sau khi áp dụng giảm giá (giá cố định trong suốt thời gian khuyến mãi)")
                .HasColumnType("int(11)");
            entity.Property(e => e.StartDate)
                .HasComment("Thời gian bắt đầu áp dụng giảm giá")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdateAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian cập nhật cuối")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdateBy).HasComment("Người cập nhật thông tin");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.SaleComboCreateByNavigations)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sc_user_create");

            entity.HasOne(d => d.IdcomboNavigation).WithMany(p => p.SaleCombos)
                .HasForeignKey(d => d.Idcombo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sc_combo");

            entity.HasOne(d => d.IdsaleNavigation).WithMany(p => p.SaleCombos)
                .HasForeignKey(d => d.Idsale)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sc_sale");

            entity.HasOne(d => d.UpdateByNavigation).WithMany(p => p.SaleComboUpdateByNavigations)
                .HasForeignKey(d => d.UpdateBy)
                .HasConstraintName("fk_sc_user_update");
        });

        modelBuilder.Entity<SaleProduct>(entity =>
        {
            entity.HasKey(e => e.IdsaleProduct).HasName("PRIMARY");

            entity.ToTable("saleproduct");

            entity.HasIndex(e => new { e.Idsale, e.Idproduct }, "IDSale").IsUnique();

            entity.HasIndex(e => e.Idproduct, "fk_sp_product");

            entity.HasIndex(e => e.CreateBy, "fk_sp_user_create");

            entity.HasIndex(e => e.UpdateBy, "fk_sp_user_update");

            entity.Property(e => e.IdsaleProduct)
                .HasComment("Mã chi tiết áp dụng khuyến mãi cho sản phẩm")
                .HasColumnName("IDSaleProduct");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian tạo thiết lập")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreateBy).HasComment("Người thiết lập khuyến mãi");
            entity.Property(e => e.EndDate)
                .HasComment("Thời gian kết thúc giảm giá")
                .HasColumnType("datetime");
            entity.Property(e => e.Idproduct)
                .HasComment("Khóa ngoại tham chiếu sản phẩm được giảm giá")
                .HasColumnName("IDProduct");
            entity.Property(e => e.Idsale)
                .HasComment("Khóa ngoại tham chiếu chương trình khuyến mãi")
                .HasColumnName("IDSale");
            entity.Property(e => e.Price)
                .HasComment("Giá bán sau khi áp dụng giảm giá (giá cố định trong suốt thời gian khuyến mãi)")
                .HasColumnType("int(11)");
            entity.Property(e => e.StartDate)
                .HasComment("Thời gian bắt đầu áp dụng giảm giá")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdateAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian cập nhật cuối")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdateBy).HasComment("Người cập nhật thông tin");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.SaleProductCreateByNavigations)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sp_user_create");

            entity.HasOne(d => d.IdproductNavigation).WithMany(p => p.SaleProducts)
                .HasForeignKey(d => d.Idproduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sp_product");

            entity.HasOne(d => d.IdsaleNavigation).WithMany(p => p.SaleProducts)
                .HasForeignKey(d => d.Idsale)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sp_sale");

            entity.HasOne(d => d.UpdateByNavigation).WithMany(p => p.SaleProductUpdateByNavigations)
                .HasForeignKey(d => d.UpdateBy)
                .HasConstraintName("fk_sp_user_update");
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.IdshoppingCart).HasName("PRIMARY");

            entity.ToTable("shoppingcart");

            entity.HasIndex(e => e.Iduser, "fk_cart_user");

            entity.HasIndex(e => e.CreateBy, "fk_cart_user_create");

            entity.HasIndex(e => e.UpdateBy, "fk_cart_user_update");

            entity.Property(e => e.IdshoppingCart)
                .HasComment("Mã định danh giỏ hàng")
                .HasColumnName("IDShoppingCart");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian khởi tạo giỏ hàng")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreateBy).HasComment("Người tạo giỏ hàng");
            entity.Property(e => e.Iduser)
                .HasComment("Khóa ngoại định danh người dùng (NULL nếu chưa đăng nhập)")
                .HasColumnName("IDUser");
            entity.Property(e => e.SessionId)
                .HasMaxLength(255)
                .HasComment("Mã phiên làm việc lưu trên trình duyệt cho khách vãng lai")
                .HasColumnName("SessionID");
            entity.Property(e => e.TotalPrice)
                .HasComment("Tổng giá trị hiện tại của giỏ hàng")
                .HasColumnType("int(11)");
            entity.Property(e => e.UpdateAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian cập nhật giỏ hàng cuối cùng")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdateBy).HasComment("Người cập nhật cuối cùng");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.ShoppingCartCreateByNavigations)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("fk_cart_user_create");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.ShoppingCartIduserNavigations)
                .HasForeignKey(d => d.Iduser)
                .HasConstraintName("fk_cart_user");

            entity.HasOne(d => d.UpdateByNavigation).WithMany(p => p.ShoppingCartUpdateByNavigations)
                .HasForeignKey(d => d.UpdateBy)
                .HasConstraintName("fk_cart_user_update");
        });

        modelBuilder.Entity<ShoppingCartItem>(entity =>
        {
            entity.HasKey(e => e.IdshoppingCartItem).HasName("PRIMARY");

            entity.ToTable("ShoppingCartItem");

            entity.HasIndex(e => e.IdshoppingCart, "fk_ci_cart");

            entity.HasIndex(e => e.Idcolor, "fk_ci_color");

            entity.HasIndex(e => e.Idcombo, "fk_ci_combo");

            entity.HasIndex(e => e.Idproduct, "fk_ci_product");

            entity.HasIndex(e => e.Idsize, "fk_ci_size");

            entity.HasIndex(e => e.CreateBy, "fk_ci_user_create");

            entity.HasIndex(e => e.UpdateBy, "fk_ci_user_update");

            entity.Property(e => e.IdshoppingCartItem)
                .HasComment("Mã chi tiết từng món trong giỏ hàng")
                .HasColumnName("IDShoppingCartItem");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian thêm món hàng vào giỏ")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreateBy).HasComment("Người thêm món hàng vào giỏ");
            entity.Property(e => e.Idcolor)
                .HasComment("Mã màu sắc sản phẩm được chọn")
                .HasColumnName("IDColor");
            entity.Property(e => e.Idcombo)
                .HasComment("Mã combo được chọn (NULL nếu chọn Product)")
                .HasColumnName("IDCombo");
            entity.Property(e => e.Idproduct)
                .HasComment("Mã sản phẩm được chọn (NULL nếu chọn Combo)")
                .HasColumnName("IDProduct");
            entity.Property(e => e.IdshoppingCart)
                .HasComment("Khóa ngoại thuộc về giỏ hàng nào")
                .HasColumnName("IDShoppingCart");
            entity.Property(e => e.Idsize)
                .HasComment("Mã kích cỡ sản phẩm được chọn (nếu có)")
                .HasColumnName("IDSize");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasComment("Số lượng sản phẩm/combo muốn mua")
                .HasColumnType("int(11)");
            entity.Property(e => e.UnitPrice)
                .HasComment("Đơn giá lưu cứng tại thời điểm khách thêm vào giỏ")
                .HasColumnType("int(11)");
            entity.Property(e => e.UpdateAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Thời gian thay đổi chi tiết giỏ hàng")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdateBy).HasComment("Người cập nhật số lượng/món hàng");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.ShoppingCartItemCreateByNavigations)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("fk_ci_user_create");

            entity.HasOne(d => d.IdcolorNavigation).WithMany(p => p.ShoppingCartItems)
                .HasForeignKey(d => d.Idcolor)
                .HasConstraintName("fk_ci_color");

            entity.HasOne(d => d.IdcomboNavigation).WithMany(p => p.ShoppingCartItems)
                .HasForeignKey(d => d.Idcombo)
                .HasConstraintName("fk_ci_combo");

            entity.HasOne(d => d.IdproductNavigation).WithMany(p => p.ShoppingCartItems)
                .HasForeignKey(d => d.Idproduct)
                .HasConstraintName("fk_ci_product");

            entity.HasOne(d => d.IdshoppingCartNavigation).WithMany(p => p.ShoppingCartItems)
                .HasForeignKey(d => d.IdshoppingCart)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ci_cart");

            entity.HasOne(d => d.IdsizeNavigation).WithMany(p => p.ShoppingCartItems)
                .HasForeignKey(d => d.Idsize)
                .HasConstraintName("fk_ci_size");

            entity.HasOne(d => d.UpdateByNavigation).WithMany(p => p.ShoppingCartItemUpdateByNavigations)
                .HasForeignKey(d => d.UpdateBy)
                .HasConstraintName("fk_ci_user_update");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.Idsize).HasName("PRIMARY");

            entity.ToTable("size");

            entity.HasIndex(e => e.Idproduct, "fk_size_product");

            entity.Property(e => e.Idsize)
                .HasComment("Mã định danh kích cỡ")
                .HasColumnName("IDSize");
            entity.Property(e => e.Idproduct)
                .HasComment("Khóa ngoại tham chiếu đến sản phẩm")
                .HasColumnName("IDProduct");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasComment("Tên kích cỡ (VD: S, M, L, XL)");

            entity.HasOne(d => d.IdproductNavigation).WithMany(p => p.Sizes)
                .HasForeignKey(d => d.Idproduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_size_product");
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.HasKey(e => e.Idtype).HasName("PRIMARY");

            ((EntityTypeBuilder)entity).ToTable("Type");

            entity.HasIndex(e => e.Name, "idx_type_name").IsUnique();

            entity.Property(e => e.Idtype)
                .HasComment("Mã định danh loại sản phẩm (danh mục)")
                .HasColumnName("IDType");
            entity.Property(e => e.Name).HasComment("Tên loại sản phẩm (VD: Áo khoác, Quần Jean)");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Iduser).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.UserName, "idx_username").IsUnique();

            entity.Property(e => e.Iduser)
                .HasComment("Mã định danh duy nhất của người dùng")
                .HasColumnName("IDUser");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasComment("Mật khẩu đăng nhập (nên được băm/hash)");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'customer'")
                .HasComment("Vai trò của người dùng trên hệ thống")
                .HasColumnType("enum('admin','customer')");
            entity.Property(e => e.UserName).HasComment("Tên đăng nhập hoặc họ tên người dùng");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
