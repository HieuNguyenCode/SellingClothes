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
    public int Price { get; set; }

    /// <summary>
    ///     Đường dẫn lưu trữ file hình ảnh đại diện combo
    /// </summary>
    public string Image { get; set; } = null!;

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

    public virtual ICollection<ComboProduct> ComboProducts { get; set; } = new List<ComboProduct>();

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<SaleCombo> SaleCombos { get; set; } = new List<SaleCombo>();

    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();

    public virtual User? UpdateByNavigation { get; set; }
}