namespace Core.Entities;

public class CartComboProduct
{
    /// <summary>
    ///     Mã chi tiết liên kết Combo và Product trong giỏ hàng
    /// </summary>
    public Guid IdcartComboProduct { get; set; }

    /// <summary>
    ///     Khóa ngoại tham chiếu đến món hàng trong giỏ (dòng sản phẩm hoặc combo)
    /// </summary>
    public Guid IdshoppingCartItem { get; set; }

    /// <summary>
    ///     Khóa ngoại tham chiếu đến sản phẩm nằm trong combo của món hàng này
    /// </summary>
    public Guid Idproduct { get; set; }

    /// <summary>
    ///     Số lượng sản phẩm này trong combo của món hàng
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    ///     Màu sắc cụ thể của sản phẩm khách đã chọn
    /// </summary>
    public Guid? Idcolor { get; set; }

    /// <summary>
    ///     Kích cỡ cụ thể của sản
    /// </summary>
    public Guid? Idsize { get; set; }

    /// <summary>
    ///     Người cập nhật liên kết
    /// </summary>
    public Guid? UpdateBy { get; set; }

    /// <summary>
    ///     Người tạo liên kết
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

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual Color? IdcolorNavigation { get; set; }

    public virtual Product IdproductNavigation { get; set; } = null!;

    public virtual ShoppingCartItem IdshoppingCartItemNavigation { get; set; } = null!;

    public virtual Size? IdsizeNavigation { get; set; }

    public virtual User? UpdateByNavigation { get; set; }
}
