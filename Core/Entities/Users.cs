using System;
using System.Collections.Generic;

namespace APINhatKyDienTu.Core.Entities;

public partial class Users
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

    public virtual ICollection<Combo> ComboCreateByNavigation { get; set; } = new List<Combo>();

    public virtual ICollection<Combo> ComboUpdateByNavigation { get; set; } = new List<Combo>();

    public virtual ICollection<Comboproduct> ComboproductCreateByNavigation { get; set; } = new List<Comboproduct>();

    public virtual ICollection<Comboproduct> ComboproductUpdateByNavigation { get; set; } = new List<Comboproduct>();

    public virtual ICollection<Image> Image { get; set; } = new List<Image>();

    public virtual ICollection<Orderdetail> OrderdetailCreateByNavigation { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Orderdetail> OrderdetailUpdateByNavigation { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Orders> OrdersCreateByNavigation { get; set; } = new List<Orders>();

    public virtual ICollection<Orders> OrdersIduserNavigation { get; set; } = new List<Orders>();

    public virtual ICollection<Orders> OrdersUpdateByNavigation { get; set; } = new List<Orders>();

    public virtual ICollection<Product> ProductCreateByNavigation { get; set; } = new List<Product>();

    public virtual ICollection<Product> ProductUpdateByNavigation { get; set; } = new List<Product>();

    public virtual ICollection<Salecombo> SalecomboCreateByNavigation { get; set; } = new List<Salecombo>();

    public virtual ICollection<Salecombo> SalecomboUpdateByNavigation { get; set; } = new List<Salecombo>();

    public virtual ICollection<Saleproduct> SaleproductCreateByNavigation { get; set; } = new List<Saleproduct>();

    public virtual ICollection<Saleproduct> SaleproductUpdateByNavigation { get; set; } = new List<Saleproduct>();

    public virtual ICollection<Shoppingcart> ShoppingcartCreateByNavigation { get; set; } = new List<Shoppingcart>();

    public virtual ICollection<Shoppingcart> ShoppingcartIduserNavigation { get; set; } = new List<Shoppingcart>();

    public virtual ICollection<Shoppingcart> ShoppingcartUpdateByNavigation { get; set; } = new List<Shoppingcart>();

    public virtual ICollection<Shoppingcartitem> ShoppingcartitemCreateByNavigation { get; set; } = new List<Shoppingcartitem>();

    public virtual ICollection<Shoppingcartitem> ShoppingcartitemUpdateByNavigation { get; set; } = new List<Shoppingcartitem>();
}
