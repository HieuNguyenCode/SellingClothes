namespace Core.Entities;

public partial class Image
{
    /// <summary>
    /// Mã định danh hình ảnh
    /// </summary>
    public Guid Idimage { get; set; }

    /// <summary>
    /// Đường dẫn lưu trữ file hình ảnh
    /// </summary>
    public string Url { get; set; } = null!;

    /// <summary>
    /// Sản phẩm mà hình ảnh này thuộc về
    /// </summary>
    public Guid Idproduct { get; set; }

    /// <summary>
    /// Người tải ảnh lên
    /// </summary>
    public Guid CreateBy { get; set; }

    /// <summary>
    /// Thời gian tải ảnh lên
    /// </summary>
    public DateTime? CreateAt { get; set; }

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual Product IdproductNavigation { get; set; } = null!;
}
