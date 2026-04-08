using System;
using System.Collections.Generic;

namespace APINhatKyDienTu.Core.Entities;

public partial class Company
{
    /// <summary>
    /// Mã định danh công ty/thương hiệu
    /// </summary>
    public Guid Idcompany { get; set; }

    /// <summary>
    /// Tên công ty hoặc thương hiệu
    /// </summary>
    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Product { get; set; } = new List<Product>();
}
