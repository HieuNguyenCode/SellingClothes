namespace Services.Dto.Responses;

public class DashboardDto
{
    public int TotalRevenue { get; set; }
    public int NewOrdersCount { get; set; }
    public int TotalCustomers { get; set; }
    public double RevenueGrowth { get; set; } // Tăng trưởng doanh thu (%)
    public List<TopSellingItemDto> TopSellingItems { get; set; } = new();
    public List<OrderAdminResponseDto> RecentOrders { get; set; } = new();
}

public class TopSellingItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!; // "Product" hoặc "Combo"
    public int TotalSold { get; set; }
    public int Revenue { get; set; }
    public string? Image { get; set; }
}