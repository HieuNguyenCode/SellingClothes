namespace Core.Entities;

public class OrderDetailProduct
{
    /// <summary>
    ///     Mã chi thiết sản phẩm combo order
    /// </summary>
    public Guid IdorderDetailProduct { get; set; }

    /// <summary>
    ///     Khóa ngoại thuộc combo order nào
    /// </summary>
    public Guid IdorderDetail { get; set; }

    /// <summary>
    ///     Sản phẩm đã mua
    /// </summary>
    public Guid? Idproduct { get; set; }

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
}