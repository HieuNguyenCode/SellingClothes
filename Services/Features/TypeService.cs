using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Services.Dto.Requests;
using Services.Dto.Responses;
using Services.Interfaces;
using Type = Core.Entities.Type;

namespace Services.Features;

public class TypeService(AppDbContext appDbContext) : ITypeService
{
    public async Task<ServiceResponse<List<TypeDto>>> GetAllTypesAsync(string? search, int? page, int? pageSize)
    {
        var query = appDbContext.Types.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(c => c.Name.Contains(search));

        query = query.OrderBy(c => c.Name);

        var validPage = (page ?? 1) > 0 ? page ?? 1 : 1;
        var validPageSize = (pageSize ?? 10) > 0 ? pageSize ?? 10 : 10;

        var totalCount = await query.CountAsync();

        var types = await query
            .Skip((validPage - 1) * validPageSize)
            .Take(validPageSize)
            .Select(c => new TypeDto
            {
                Idtype = c.Idtype,
                Name = c.Name
            })
            .ToListAsync();

        return new ServiceResponse<List<TypeDto>>
        {
            Status = 200,
            Message = "Lấy danh sách thương hiệu/công ty thành công.",
            Data = types,
            PageSize = validPageSize,
            PageNumber = validPage,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / validPageSize)
        };
    }

    public async Task<ServiceResponse<TypeDto>> GetTypeByIdAsync(Guid id)
    {
        var type = await appDbContext.Types.AsNoTracking().FirstOrDefaultAsync(t => t.Idtype == id);
        if (type == null)
            return new ServiceResponse<TypeDto>
            {
                Status = 404,
                Message = "Không tìm thấy loại thiết bị."
            };

        var typeDto = new TypeDto
        {
            Idtype = type.Idtype,
            Name = type.Name
        };
        return new ServiceResponse<TypeDto>
        {
            Status = 200,
            Message = "Lấy thông tin loại thiết bị thành công.",
            Data = typeDto
        };
    }

    public async Task<ServiceResponse> CreateTypeAsync(TypeUpdateDto typeDto)
    {
        var type = await appDbContext.Types.AsNoTracking().FirstOrDefaultAsync(t => t.Name == typeDto.Name);
        if (type != null)
            return new ServiceResponse
            {
                Status = 400,
                Message = "Loại thiết bị đã tồn tại."
            };

        var newType = new Type
        {
            Name = typeDto.Name
        };
        await appDbContext.Types.AddAsync(newType);
        await appDbContext.SaveChangesAsync();
        return new ServiceResponse
        {
            Status = 200,
            Message = "Tạo loại thiết bị thành công."
        };
    }

    public async Task<ServiceResponse> UpdateTypeAsync(Guid id, TypeUpdateDto typeDto)
    {
        var updatedType = await appDbContext.Types.FirstOrDefaultAsync(t => t.Idtype == id);
        if (updatedType == null)
            return new ServiceResponse
            {
                Status = 404,
                Message = "Không tìm thấy loại thiết bị."
            };

        var isNameExist = await appDbContext.Types
            .AnyAsync(t => t.Idtype != id && t.Name == typeDto.Name);
        if (isNameExist)
            return new ServiceResponse
            {
                Status = 400,
                Message = "Tên loại thiết bị đã tồn tại."
            };

        updatedType.Name = typeDto.Name;
        appDbContext.Update(updatedType);
        await appDbContext.SaveChangesAsync();
        return new ServiceResponse
        {
            Status = 200,
            Message = "Cập nhật loại thiết bị thành công."
        };
    }

    public async Task<ServiceResponse> DeleteTypeAsync(Guid id)
    {
        var type = await appDbContext.Types
            .Include(t => t.Products)
            .FirstOrDefaultAsync(t => t.Idtype == id);
        if (type == null)
            return new ServiceResponse
            {
                Status = 404,
                Message = "Không tìm thấy loại thiết bị."
            };

        if (type.Products.Count != 0)
            return new ServiceResponse
            {
                Status = 400,
                Message = "Không thể xóa loại thiết bị vì có sản phẩm liên quan."
            };

        appDbContext.Types.Remove(type);
        await appDbContext.SaveChangesAsync();
        return new ServiceResponse
        {
            Status = 200,
            Message = "Xóa loại thiết bị thành công."
        };
    }
}