using System;
using System.Collections.Generic;

namespace APINhatKyDienTu.Core.Entities;

public partial class ShoppingCart
{
    /// <summary>
    /// Mã định danh giỏ hàng
    /// </summary>
    public Guid IdshoppingCart { get; set; }

    /// <summary>
    /// Khóa ngoại định danh người dùng (NULL nếu chưa đăng nhập)
    /// </summary>
    public Guid? Iduser { get; set; }

    /// <summary>
    /// Mã phiên làm việc lưu trên trình duyệt cho khách vãng lai
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// Tổng giá trị hiện tại của giỏ hàng
    /// </summary>
    public int TotalPrice { get; set; }

    /// <summary>
    /// Người cập nhật cuối cùng
    /// </summary>
    public Guid? UpdateBy { get; set; }

    /// <summary>
    /// Người tạo giỏ hàng
    /// </summary>
    public Guid? CreateBy { get; set; }

    /// <summary>
    /// Thời gian cập nhật giỏ hàng cuối cùng
    /// </summary>
    public DateTime? UpdateAt { get; set; }

    /// <summary>
    /// Thời gian khởi tạo giỏ hàng
    /// </summary>
    public DateTime? CreateAt { get; set; }

    public virtual User? CreateByNavigation { get; set; }

    public virtual User? IduserNavigation { get; set; }

    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();

    public virtual User? UpdateByNavigation { get; set; }
}
