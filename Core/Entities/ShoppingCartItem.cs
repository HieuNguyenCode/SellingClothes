namespace Core.Entities;

public class ShoppingCartItem
{
    /// <summary>
    ///     Mã chi tiết từng món trong giỏ hàng
    /// </summary>
    public Guid IdshoppingCartItem { get; set; }

    /// <summary>
    ///     Khóa ngoại thuộc về giỏ hàng nào
    /// </summary>
    public Guid IdshoppingCart { get; set; }

    /// <summary>
    ///     Mã sản phẩm được chọn (NULL nếu chọn Combo)
    /// </summary>
    public Guid? Idproduct { get; set; }

    /// <summary>
    ///     Mã combo được chọn (NULL nếu chọn Product)
    /// </summary>
    public Guid? Idcombo { get; set; }

    /// <summary>
    ///     Mã màu sắc sản phẩm được chọn
    /// </summary>
    public Guid? Idcolor { get; set; }

    /// <summary>
    ///     Mã kích cỡ sản phẩm được chọn (nếu có)
    /// </summary>
    public Guid? Idsize { get; set; }

    /// <summary>
    ///     Số lượng sản phẩm/combo muốn mua
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    ///     Đơn giá lưu cứng tại thời điểm khách thêm vào giỏ
    /// </summary>
    public int UnitPrice { get; set; }

    /// <summary>
    ///     Người cập nhật số lượng/món hàng
    /// </summary>
    public Guid? UpdateBy { get; set; }

    /// <summary>
    ///     Người thêm món hàng vào giỏ
    /// </summary>
    public Guid? CreateBy { get; set; }

    /// <summary>
    ///     Thời gian thay đổi chi tiết giỏ hàng
    /// </summary>
    public DateTime? UpdateAt { get; set; }

    /// <summary>
    ///     Thời gian thêm món hàng vào giỏ
    /// </summary>
    public DateTime? CreateAt { get; set; }

    public virtual ICollection<CartComboProduct> CartComboProducts { get; set; } = new List<CartComboProduct>();

    public virtual User? CreateByNavigation { get; set; }

    public virtual Color? IdcolorNavigation { get; set; }

    public virtual Combo? IdcomboNavigation { get; set; }

    public virtual Product? IdproductNavigation { get; set; }

    public virtual ShoppingCart IdshoppingCartNavigation { get; set; } = null!;

    public virtual Size? IdsizeNavigation { get; set; }

    public virtual User? UpdateByNavigation { get; set; }
}