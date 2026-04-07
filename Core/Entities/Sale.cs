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

    public virtual ICollection<SaleCombo> SaleCombos { get; set; } = new List<SaleCombo>();

    public virtual ICollection<SaleProduct> SaleProducts { get; set; } = new List<SaleProduct>();
}