using System;
using System.Collections.Generic;

namespace APINhatKyDienTu.Core.Entities;

public partial class User
{
    /// <summary>
    /// Mã định danh duy nhất của người dùng
    /// </summary>
    public Guid Iduser { get; set; }

    /// <summary>
    /// Tên đăng nhập hoặc họ tên người dùng
    /// </summary>
    public string UserName { get; set; } = null!;

    /// <summary>
    /// Mật khẩu đăng nhập (nên được băm/hash)
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// Vai trò của người dùng trên hệ thống
    /// </summary>
    public string Role { get; set; } = null!;

    public virtual ICollection<Combo> ComboCreateByNavigations { get; set; } = new List<Combo>();

    public virtual ICollection<Combo> ComboUpdateByNavigations { get; set; } = new List<Combo>();

    public virtual ICollection<Comboproduct> ComboproductCreateByNavigations { get; set; } = new List<Comboproduct>();

    public virtual ICollection<Comboproduct> ComboproductUpdateByNavigations { get; set; } = new List<Comboproduct>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Order> OrderCreateByNavigations { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderIduserNavigations { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderUpdateByNavigations { get; set; } = new List<Order>();

    public virtual ICollection<Orderdetail> OrderdetailCreateByNavigations { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Orderdetail> OrderdetailUpdateByNavigations { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Product> ProductCreateByNavigations { get; set; } = new List<Product>();

    public virtual ICollection<Product> ProductUpdateByNavigations { get; set; } = new List<Product>();

    public virtual ICollection<Salecombo> SalecomboCreateByNavigations { get; set; } = new List<Salecombo>();

    public virtual ICollection<Salecombo> SalecomboUpdateByNavigations { get; set; } = new List<Salecombo>();

    public virtual ICollection<Saleproduct> SaleproductCreateByNavigations { get; set; } = new List<Saleproduct>();

    public virtual ICollection<Saleproduct> SaleproductUpdateByNavigations { get; set; } = new List<Saleproduct>();

    public virtual ICollection<Shoppingcart> ShoppingcartCreateByNavigations { get; set; } = new List<Shoppingcart>();

    public virtual ICollection<Shoppingcart> ShoppingcartIduserNavigations { get; set; } = new List<Shoppingcart>();

    public virtual ICollection<Shoppingcart> ShoppingcartUpdateByNavigations { get; set; } = new List<Shoppingcart>();

    public virtual ICollection<Shoppingcartitem> ShoppingcartitemCreateByNavigations { get; set; } = new List<Shoppingcartitem>();

    public virtual ICollection<Shoppingcartitem> ShoppingcartitemUpdateByNavigations { get; set; } = new List<Shoppingcartitem>();
}
