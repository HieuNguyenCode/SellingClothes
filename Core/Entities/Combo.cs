using System;
using System.Collections.Generic;

namespace APINhatKyDienTu.Core.Entities;

public partial class Combo
{
    /// <summary>
    /// Mã định danh combo sản phẩm
    /// </summary>
    public Guid Idcombo { get; set; }

    /// <summary>
    /// Tên gọi của combo
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Giá bán tổng hợp của combo
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Đường dẫn lưu trữ file hình ảnh đại diện combo
    /// </summary>
    public string Image { get; set; } = null!;

    /// <summary>
    /// Người cập nhật cuối cùng
    /// </summary>
    public Guid? UpdateBy { get; set; }

    /// <summary>
    /// Người tạo combo
    /// </summary>
    public Guid CreateBy { get; set; }

    /// <summary>
    /// Thời gian cập nhật cuối
    /// </summary>
    public DateTime? UpdateAt { get; set; }

    /// <summary>
    /// Thời gian tạo combo
    /// </summary>
    public DateTime? CreateAt { get; set; }

    public virtual ICollection<Comboproduct> Comboproduct { get; set; } = new List<Comboproduct>();

    public virtual Users CreateByNavigation { get; set; } = null!;

    public virtual ICollection<Orderdetail> Orderdetail { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Salecombo> Salecombo { get; set; } = new List<Salecombo>();

    public virtual ICollection<Shoppingcartitem> Shoppingcartitem { get; set; } = new List<Shoppingcartitem>();

    public virtual Users? UpdateByNavigation { get; set; }
}
