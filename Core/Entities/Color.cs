namespace Core.Entities;

public class Color
{
    /// <summary>
    ///     Mã định danh màu sắc
    /// </summary>
    public Guid Idcolor { get; set; }

    /// <summary>
    ///     Tên màu sắc (VD: Đen, Trắng, Đỏ)
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Mức giá chênh lệch hoặc phụ phí áp dụng cho màu này
    /// </summary>
    public int Price { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Shoppingcartitem> Shoppingcartitems { get; set; } = new List<Shoppingcartitem>();
}
