namespace Services.Dto.Responses;

public class OrderAdminResponseDto
{
    public Guid Idorder { get; set; }
    public Guid? Iduser { get; set; }
    public string CustomerName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string ShippingAddress { get; set; } = null!;
    public int TotalPrice { get; set; }
    public string OrderStatus { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public string PaymentStatus { get; set; } = null!;
    public DateTime CreateAt { get; set; }

    public List<OrderDetailAdminDto> Items { get; set; } = new();
}

public class OrderDetailAdminDto
{
    public string ItemName { get; set; } = null!;
    public string ItemType { get; set; } = null!; // "Product" hoặc "Combo"
    public string? SizeName { get; set; }
    public string? ColorName { get; set; }
    public int Quantity { get; set; }
    public int UnitPrice { get; set; }
    public int SubTotal { get; set; }
    public List<OrderDetailComboItemDto>? ComboItems { get; set; }
}

public class OrderDetailComboItemDto
{
    public string ProductName { get; set; } = null!;
    public string? SizeName { get; set; }
    public string? ColorName { get; set; }
    public int Quantity { get; set; }
}

public class PagedResult<T>
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public List<T> Data { get; set; } = new();
}