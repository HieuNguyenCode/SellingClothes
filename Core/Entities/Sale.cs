namespace Core.Entities;

public class Sale
{
    /// <summary>
    ///     Mã chương trình khuyến mãi/giảm giá
    /// </summary>
    public Guid Idsale { get; set; }

    /// <summary>
    ///     Tên chương trình khuyến mãi
    /// </summary>
    public string Name { get; set; } = null!;

    public virtual ICollection<Salecombo> Salecombos { get; set; } = new List<Salecombo>();

    public virtual ICollection<Saleproduct> Saleproducts { get; set; } = new List<Saleproduct>();
}
