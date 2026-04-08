using System;
using System.Collections.Generic;

namespace APINhatKyDienTu.Core.Entities;

public partial class Color
{
    /// <summary>
    /// Mã định danh màu sắc
    /// </summary>
    public Guid Idcolor { get; set; }

    /// <summary>
    /// Khóa ngoại tham chiếu đến sản phẩm
    /// </summary>
    public Guid Idproduct { get; set; }

    /// <summary>
    /// Tên màu sắc (VD: Đen, Trắng, Đỏ)
    /// </summary>
    public string Name { get; set; } = null!;

    public virtual Product IdproductNavigation { get; set; } = null!;

    public virtual ICollection<Orderdetail> Orderdetail { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Shoppingcartitem> Shoppingcartitem { get; set; } = new List<Shoppingcartitem>();
}
