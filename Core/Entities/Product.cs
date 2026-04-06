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
    public decimal Price { get; set; }

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

    public virtual ICollection<Comboproduct> Comboproducts { get; set; } = new List<Comboproduct>();

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual Company IdcompanyNavigation { get; set; } = null!;

    public virtual Type IdtypeNavigation { get; set; } = null!;

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Saleproduct> Saleproducts { get; set; } = new List<Saleproduct>();

    public virtual ICollection<Shoppingcartitem> Shoppingcartitems { get; set; } = new List<Shoppingcartitem>();

    public virtual User? UpdateByNavigation { get; set; }
}
