namespace Core.Entities;

public class User
{
    /// <summary>
    ///     Mã định danh duy nhất của người dùng
    /// </summary>
    public Guid Iduser { get; set; }

    /// <summary>
    ///     Tên đăng nhập hoặc họ tên người dùng
    /// </summary>
    public string UserName { get; set; } = null!;

    /// <summary>
    ///     Mật khẩu đăng nhập (nên được băm/hash)
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    ///     Vai trò của người dùng trên hệ thống
    /// </summary>
    public string Role { get; set; } = null!;

    public virtual ICollection<Combo> ComboCreateByNavigations { get; set; } = new List<Combo>();

    public virtual ICollection<ComboProduct> ComboProductCreateByNavigations { get; set; } = new List<ComboProduct>();

    public virtual ICollection<ComboProduct> ComboProductUpdateByNavigations { get; set; } = new List<ComboProduct>();

    public virtual ICollection<Combo> ComboUpdateByNavigations { get; set; } = new List<Combo>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Order> OrderCreateByNavigations { get; set; } = new List<Order>();

    public virtual ICollection<OrderDetail> OrderDetailCreateByNavigations { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderDetail> OrderDetailUpdateByNavigations { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Order> OrderIduserNavigations { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderUpdateByNavigations { get; set; } = new List<Order>();

    public virtual ICollection<Product> ProductCreateByNavigations { get; set; } = new List<Product>();

    public virtual ICollection<Product> ProductUpdateByNavigations { get; set; } = new List<Product>();

    public virtual ICollection<SaleCombo> SaleComboCreateByNavigations { get; set; } = new List<SaleCombo>();

    public virtual ICollection<SaleCombo> SaleComboUpdateByNavigations { get; set; } = new List<SaleCombo>();

    public virtual ICollection<SaleProduct> SaleProductCreateByNavigations { get; set; } = new List<SaleProduct>();

    public virtual ICollection<SaleProduct> SaleProductUpdateByNavigations { get; set; } = new List<SaleProduct>();

    public virtual ICollection<ShoppingCart> ShoppingCartCreateByNavigations { get; set; } = new List<ShoppingCart>();

    public virtual ICollection<ShoppingCart> ShoppingCartIduserNavigations { get; set; } = new List<ShoppingCart>();

    public virtual ICollection<ShoppingCartItem> ShoppingCartItemCreateByNavigations { get; set; } =
        new List<ShoppingCartItem>();

    public virtual ICollection<ShoppingCartItem> ShoppingCartItemUpdateByNavigations { get; set; } =
        new List<ShoppingCartItem>();

    public virtual ICollection<ShoppingCart> ShoppingCartUpdateByNavigations { get; set; } = new List<ShoppingCart>();

    public virtual ICollection<CartComboProduct> CartComboProductCreateByNavigations { get; set; } =
        new List<CartComboProduct>();

    public virtual ICollection<CartComboProduct> CartComboProductUpdateByNavigations { get; set; } = new List<CartComboProduct>();
}
