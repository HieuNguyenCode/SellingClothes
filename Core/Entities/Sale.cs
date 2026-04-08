using System;
using System.Collections.Generic;

namespace APINhatKyDienTu.Core.Entities;

public partial class Sale
{
    /// <summary>
    /// Mã chương trình khuyến mãi/giảm giá
    /// </summary>
    public Guid Idsale { get; set; }

    /// <summary>
    /// Tên chương trình khuyến mãi
    /// </summary>
    public string Name { get; set; } = null!;

    public virtual ICollection<Salecombo> Salecombo { get; set; } = new List<Salecombo>();

    public virtual ICollection<Saleproduct> Saleproduct { get; set; } = new List<Saleproduct>();
}
