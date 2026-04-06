namespace Core.Entities;

public class Combo
{
    /// <summary>
    ///     Mã định danh combo sản phẩm
    /// </summary>
    public Guid Idcombo { get; set; }

    /// <summary>
    ///     Tên gọi của combo
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Giá bán tổng hợp của combo
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    ///     Người cập nhật cuối cùng
    /// </summary>
    public Guid? UpdateBy { get; set; }

    /// <summary>
    ///     Người tạo combo
    /// </summary>
    public Guid CreateBy { get; set; }

    /// <summary>
    ///     Thời gian cập nhật cuối
    /// </summary>
    public DateTime? UpdateAt { get; set; }

    /// <summary>
    ///     Thời gian tạo combo
    /// </summary>
    public DateTime? CreateAt { get; set; }

    public virtual ICollection<Comboproduct> Comboproducts { get; set; } = new List<Comboproduct>();

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Salecombo> Salecombos { get; set; } = new List<Salecombo>();

    public virtual ICollection<Shoppingcartitem> Shoppingcartitems { get; set; } = new List<Shoppingcartitem>();

    public virtual User? UpdateByNavigation { get; set; }
}
