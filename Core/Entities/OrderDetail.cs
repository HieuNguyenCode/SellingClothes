namespace Core.Entities;

public class OrderDetail
{
    /// <summary>
    ///     Mã chi tiết từng dòng trong hóa đơn
    /// </summary>
    public Guid IdorderDetail { get; set; }

    /// <summary>
    ///     Khóa ngoại thuộc về hóa đơn nào
    /// </summary>
    public Guid Idorder { get; set; }

    /// <summary>
    ///     Sản phẩm đã mua (NULL nếu là Combo)
    /// </summary>
    public Guid? Idproduct { get; set; }

    /// <summary>
    ///     Combo đã mua (NULL nếu là Product)
    /// </summary>
    public Guid? Idcombo { get; set; }

    /// <summary>
    ///     Màu sắc cụ thể của sản phẩm khách đã chọn
    /// </summary>
    public Guid? Idcolor { get; set; }

    /// <summary>
    ///     Kích cỡ cụ thể của sản phẩm khách đã chọn (nếu có)
    /// </summary>
    public Guid? Idsize { get; set; }

    /// <summary>
    ///     Số lượng mua
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    ///     Đơn giá cố định lúc xuất hóa đơn (không thay đổi nếu giá gốc đổi)
    /// </summary>
    public int UnitPrice { get; set; }

    /// <summary>
    ///     Cột tính toán tự động: Thành tiền = Số lượng x Đơn giá
    /// </summary>
    public int? SubTotal { get; set; }

    /// <summary>
    ///     Người cập nhật chi tiết
    /// </summary>
    public Guid? UpdateBy { get; set; }

    /// <summary>
    ///     Người tạo chi tiết
    /// </summary>
    public Guid? CreateBy { get; set; }

    /// <summary>
    ///     Thời gian cập nhật bản ghi
    /// </summary>
    public DateTime? UpdateAt { get; set; }

    /// <summary>
    ///     Thời gian tạo bản ghi
    /// </summary>
    public DateTime? CreateAt { get; set; }

    public virtual User? CreateByNavigation { get; set; }

    public virtual Color? IdcolorNavigation { get; set; }

    public virtual Combo? IdcomboNavigation { get; set; }

    public virtual Order IdorderNavigation { get; set; } = null!;

    public virtual Product? IdproductNavigation { get; set; }

    public virtual Size? IdsizeNavigation { get; set; }

    public virtual User? UpdateByNavigation { get; set; }
}