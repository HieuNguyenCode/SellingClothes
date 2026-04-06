namespace Core.Entities;

public class Comboproduct
{
    /// <summary>
    ///     Mã chi tiết liên kết Combo và Product
    /// </summary>
    public Guid IdcomboProduct { get; set; }

    /// <summary>
    ///     Khóa ngoại tham chiếu đến Combo
    /// </summary>
    public Guid Idcombo { get; set; }

    /// <summary>
    ///     Khóa ngoại tham chiếu đến Product nằm trong Combo
    /// </summary>
    public Guid Idproduct { get; set; }

    /// <summary>
    ///     Người cập nhật liên kết
    /// </summary>
    public Guid? UpdateBy { get; set; }

    /// <summary>
    ///     Người tạo liên kết
    /// </summary>
    public Guid CreateBy { get; set; }

    /// <summary>
    ///     Thời gian cập nhật cuối
    /// </summary>
    public DateTime? UpdateAt { get; set; }

    /// <summary>
    ///     Thời gian tạo bản ghi
    /// </summary>
    public DateTime? CreateAt { get; set; }

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual Combo IdcomboNavigation { get; set; } = null!;

    public virtual Product IdproductNavigation { get; set; } = null!;

    public virtual User? UpdateByNavigation { get; set; }
}
