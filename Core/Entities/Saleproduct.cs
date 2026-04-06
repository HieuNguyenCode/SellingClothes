namespace Core.Entities;

public class Saleproduct
{
    /// <summary>
    ///     Mã chi tiết áp dụng khuyến mãi cho sản phẩm
    /// </summary>
    public Guid IdsaleProduct { get; set; }

    /// <summary>
    ///     Khóa ngoại tham chiếu chương trình khuyến mãi
    /// </summary>
    public Guid Idsale { get; set; }

    /// <summary>
    ///     Khóa ngoại tham chiếu sản phẩm được giảm giá
    /// </summary>
    public Guid Idproduct { get; set; }

    /// <summary>
    ///     Thời gian bắt đầu áp dụng giảm giá
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    ///     Thời gian kết thúc giảm giá
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    ///     Người cập nhật thông tin
    /// </summary>
    public Guid? UpdateBy { get; set; }

    /// <summary>
    ///     Người thiết lập khuyến mãi
    /// </summary>
    public Guid CreateBy { get; set; }

    /// <summary>
    ///     Thời gian cập nhật cuối
    /// </summary>
    public DateTime? UpdateAt { get; set; }

    /// <summary>
    ///     Thời gian tạo thiết lập
    /// </summary>
    public DateTime? CreateAt { get; set; }

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual Product IdproductNavigation { get; set; } = null!;

    public virtual Sale IdsaleNavigation { get; set; } = null!;

    public virtual User? UpdateByNavigation { get; set; }
}
