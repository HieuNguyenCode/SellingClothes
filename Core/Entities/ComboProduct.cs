using System;
using System.Collections.Generic;

namespace APINhatKyDienTu.Core.Entities;

public partial class Comboproduct
{
    /// <summary>
    /// Mã chi tiết liên kết Combo và Product
    /// </summary>
    public Guid IdcomboProduct { get; set; }

    /// <summary>
    /// Khóa ngoại tham chiếu đến Combo
    /// </summary>
    public Guid Idcombo { get; set; }

    /// <summary>
    /// Khóa ngoại tham chiếu đến Product nằm trong Combo
    /// </summary>
    public Guid Idproduct { get; set; }

    /// <summary>
    /// Số lượng sản phẩm này trong combo (VD: Combo gồm 2 áo và 1 quần thì Quantity của áo là 2, của quần là 1)
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Người cập nhật liên kết
    /// </summary>
    public Guid? UpdateBy { get; set; }

    /// <summary>
    /// Người tạo liên kết
    /// </summary>
    public Guid CreateBy { get; set; }

    /// <summary>
    /// Thời gian cập nhật cuối
    /// </summary>
    public DateTime? UpdateAt { get; set; }

    /// <summary>
    /// Thời gian tạo bản ghi
    /// </summary>
    public DateTime? CreateAt { get; set; }

    public virtual Users CreateByNavigation { get; set; } = null!;

    public virtual Combo IdcomboNavigation { get; set; } = null!;

    public virtual Product IdproductNavigation { get; set; } = null!;

    public virtual Users? UpdateByNavigation { get; set; }
}
