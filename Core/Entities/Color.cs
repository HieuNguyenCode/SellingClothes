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

    public virtual Product IdproductNavigation { get; set; } = null!;

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Shoppingcartitem> Shoppingcartitems { get; set; } = new List<Shoppingcartitem>();
}
