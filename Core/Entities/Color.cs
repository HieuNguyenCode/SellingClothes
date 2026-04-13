namespace Core.Entities;

public class Color
{
    /// <summary>
    ///     Mã định danh màu sắc
    /// </summary>
    public Guid Idcolor { get; set; }

    /// <summary>
    ///     Khóa ngoại tham chiếu đến sản phẩm
    /// </summary>
    public Guid Idproduct { get; set; }

    /// <summary>
    ///     Tên màu sắc (VD: Đen, Trắng, Đỏ)
    /// </summary>
    public string Name { get; set; } = null!;

    public virtual ICollection<CartComboProduct> CartComboProducts { get; set; } = new List<CartComboProduct>();

    public virtual Product IdproductNavigation { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
}