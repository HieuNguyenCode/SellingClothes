namespace Core.Entities;

public class Company
{
    /// <summary>
    ///     Mã định danh công ty/thương hiệu
    /// </summary>
    public Guid Idcompany { get; set; }

    /// <summary>
    ///     Tên công ty hoặc thương hiệu
    /// </summary>
    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}