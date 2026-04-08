namespace Core.Entities;

public class Product
{
    /// <summary>
    ///     Mã định danh sản phẩm
    /// </summary>
    public Guid Idproduct { get; set; }

    /// <summary>
    ///     Tên sản phẩm
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Giá niêm yết của sản phẩm
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    ///     Khóa ngoại tham chiếu đến công ty/thương hiệu
    /// </summary>
    public Guid Idcompany { get; set; }

    /// <summary>
    ///     Khóa ngoại tham chiếu đến loại sản phẩm
    /// </summary>
    public Guid Idtype { get; set; }

    /// <summary>
    ///     Mô tả chi tiết về sản phẩm
    /// </summary>
    public string? Describe { get; set; }

    /// <summary>
    ///     Đường dẫn lưu trữ file hình ảnh đại diện sản phẩm
    /// </summary>
    public string Image { get; set; } = null!;

    /// <summary>
    ///     Cờ đánh dấu sản phẩm đã được xuất bản và hiển thị trên cửa hàng
    /// </summary>
    public bool? IsPublished { get; set; }

    /// <summary>
    ///     Cờ đánh dấu sản phẩm đã bị xóa (soft delete)
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Người cập nhật cuối cùng
    /// </summary>
    public Guid? UpdateBy { get; set; }

    /// <summary>
    ///     Người tạo bản ghi
    /// </summary>
    public Guid CreateBy { get; set; }

    /// <summary>
    ///     Thời gian cập nhật cuối
    /// </summary>
    public DateTime? UpdateAt { get; set; }

    /// <summary>
    ///     Thời gian tạo bản ghi
    /// </summary>
    public DateTime? CreateAt { get; set; }

    public virtual ICollection<Color> Colors { get; set; } = new List<Color>();

    public virtual ICollection<ComboProduct> ComboProducts { get; set; } = new List<ComboProduct>();

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual Company IdcompanyNavigation { get; set; } = null!;

    public virtual Type IdtypeNavigation { get; set; } = null!;

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<SaleProduct> SaleProducts { get; set; } = new List<SaleProduct>();

    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();

    public virtual ICollection<Size> Sizes { get; set; } = new List<Size>();

    public virtual User? UpdateByNavigation { get; set; }
}