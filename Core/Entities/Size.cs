using System;
using System.Collections.Generic;

namespace APINhatKyDienTu.Core.Entities;

public partial class Size
{
    /// <summary>
    /// Mã định danh kích cỡ
    /// </summary>
    public Guid Idsize { get; set; }

    /// <summary>
    /// Khóa ngoại tham chiếu đến sản phẩm
    /// </summary>
    public Guid Idproduct { get; set; }

    /// <summary>
    /// Tên kích cỡ (VD: S, M, L, XL)
    /// </summary>
    public string Name { get; set; } = null!;

    public virtual Product IdproductNavigation { get; set; } = null!;

    public virtual ICollection<Orderdetail> Orderdetail { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Shoppingcartitem> Shoppingcartitem { get; set; } = new List<Shoppingcartitem>();
}
