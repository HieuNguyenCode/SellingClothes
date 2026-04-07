namespace Core.Entities;

public partial class Order
{
    /// <summary>
    /// Mã định danh hóa đơn/đơn hàng
    /// </summary>
    public Guid Idorder { get; set; }

    /// <summary>
    /// Tài khoản người đặt mua (NULL nếu mua không cần tài khoản)
    /// </summary>
    public Guid? Iduser { get; set; }

    /// <summary>
    /// Mã phiên làm việc nếu khách vãng lai đặt hàng
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// Họ tên người nhận hàng
    /// </summary>
    public string CustomerName { get; set; } = null!;

    /// <summary>
    /// Số điện thoại liên hệ nhận hàng
    /// </summary>
    public string PhoneNumber { get; set; } = null!;

    /// <summary>
    /// Địa chỉ giao hàng chi tiết
    /// </summary>
    public string ShippingAddress { get; set; } = null!;

    /// <summary>
    /// Tổng số tiền khách phải thanh toán cho đơn này
    /// </summary>
    public int TotalPrice { get; set; }

    /// <summary>
    /// Tiến trình xử lý đơn hàng
    /// </summary>
    public string? OrderStatus { get; set; }

    /// <summary>
    /// Phương thức khách hàng chọn thanh toán
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// Trạng thái chuyển tiền thực tế
    /// </summary>
    public string? PaymentStatus { get; set; }

    /// <summary>
    /// Nhân viên/Hệ thống cập nhật trạng thái đơn
    /// </summary>
    public Guid? UpdateBy { get; set; }

    /// <summary>
    /// Người khởi tạo đơn hàng
    /// </summary>
    public Guid? CreateBy { get; set; }

    /// <summary>
    /// Thời gian cập nhật trạng thái cuối cùng
    /// </summary>
    public DateTime? UpdateAt { get; set; }

    /// <summary>
    /// Thời điểm chốt đơn hàng
    /// </summary>
    public DateTime? CreateAt { get; set; }

    public virtual User? CreateByNavigation { get; set; }

    public virtual User? IduserNavigation { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User? UpdateByNavigation { get; set; }
}
