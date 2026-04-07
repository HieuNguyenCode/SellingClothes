using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Dto.Requests;
using Services.Dto.Responses;
using Services.Interfaces;

namespace Services.Features;

public class ComboService(
    AppDbContext appDbContext,
    ILogger<ComboService> logger) : IComboService
{
    public async Task<ServiceResponse<List<CombosDto>>> GetCombosAsync(string? search, int? page, int? pageSize)
    {
        var now = DateTime.Now;

        var query = appDbContext.Combos.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Name.Contains(search));

        query = query.OrderBy(c => c.Name);

        var validPage = (page ?? 1) > 0 ? page ?? 1 : 1;
        var validPageSize = (pageSize ?? 10) > 0 ? pageSize ?? 10 : 10;

        var listCombos = await query
            .Skip((validPage - 1) * validPageSize)
            .Take(validPageSize)
            .Select(c => new CombosDto
            {
                Id = c.Idcombo,
                Name = c.Name,
                Price = c.Price,
                PriceSale = c.SaleCombos
                    .Where(s => s.StartDate <= now && (s.EndDate == null || s.EndDate >= now))
                    .Select(s => (int?)s.Price)
                    .Min()
                // Image = c.Image
            })
            .ToListAsync();

        return new ServiceResponse<List<CombosDto>>
        {
            Status = 200,
            Message = "Lấy danh sách sản phẩm thành công.",
            Data = listCombos
        };
    }

    public async Task<ServiceResponse<ComboDto>> GetComboByIdAsync(Guid id)
    {
        var now = DateTime.Now;

        var combo = await appDbContext.Combos
            .AsNoTracking()
            .Where(c => c.Idcombo == id)
            .Select(c => new ComboDto
            {
                Id = c.Idcombo,
                Name = c.Name,
                Price = c.Price,
                PriceSale = c.SaleCombos
                    .Where(s => s.StartDate <= now && (s.EndDate == null || s.EndDate >= now))
                    .Select(s => (int?)s.Price)
                    .Min(),
                Products = c.ComboProducts.Select(cp => new ComboProductDto
                {
                    Id = cp.IdproductNavigation.Idproduct,
                    Name = cp.IdproductNavigation.Name,
                    Quantity = cp.Quantity
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (combo == null)
            return new ServiceResponse<ComboDto>
            {
                Status = 404,
                Message = "Không tìm thấy combo."
            };

        return new ServiceResponse<ComboDto>
        {
            Status = 200,
            Message = "Lấy thông tin combo thành công.",
            Data = combo
        };
    }

    public async Task<ServiceResponse> AddComboAsync(ComboUpdateDto combo)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateComboAsync(Guid id, ComboUpdateDto combo)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteComboAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}