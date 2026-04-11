namespace Core.Entities;

public class Size
{
    /// <summary>
    ///     Mã định danh kích cỡ
    /// </summary>
    public Guid Idsize { get; set; }

    /// <summary>
    ///     Khóa ngoại tham chiếu đến sản phẩm
    /// </summary>
    public Guid Idproduct { get; set; }

    /// <summary>
    ///     Tên kích cỡ (VD: S, M, L, XL)
    /// </summary>
    public string Name { get; set; } = null!;

    public virtual ICollection<CartComboProduct> Cartcomboproduct { get; set; } = new List<CartComboProduct>();

    public virtual Product IdproductNavigation { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
}
