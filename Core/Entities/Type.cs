namespace Core.Entities
{
    public partial class Type
    {
        /// <summary>
        /// Mã định danh loại sản phẩm (danh mục)
        /// </summary>
        public Guid Idtype { get; set; }

        /// <summary>
        /// Tên loại sản phẩm (VD: Áo khoác, Quần Jean)
        /// </summary>
        public string Name { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
