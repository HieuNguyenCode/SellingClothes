using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Dto.Requests;
using Services.Dto.Responses;
using Services.Helpers;
using Services.Interfaces;

namespace Services.Features;

public class ComboService(
    AppDbContext appDbContext,
    ILogger<ComboService> logger) : IComboService
{
    public async Task<ServiceResponse<List<CombosDto>>> GetCombosAsync(string? search, int? page, int? pageSize)
    {
        var now = DateTime.Now;

        var query = appDbContext.Combo.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Name.Contains(search));
        }

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
                PriceSale = c.Salecombo
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

        var combo = await appDbContext.Combo
            .AsNoTracking()
            .Where(c => c.Idcombo == id)
            .Select(c => new ComboDto
            {
                Id = c.Idcombo,
                Name = c.Name,
                Price = c.Price,
                PriceSale = c.Salecombo
                    .Where(s => s.StartDate <= now && (s.EndDate == null || s.EndDate >= now))
                    .Select(s => (int?)s.Price)
                    .Min(),
                Image = c.Image,
                Products = c.Comboproduct.Select(cp => new ComboProductDto
                {
                    Id = cp.IdproductNavigation.Idproduct,
                    Name = cp.IdproductNavigation.Name,
                    Quantity = cp.Quantity
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (combo == null)
        {
            return new ServiceResponse<ComboDto>
            {
                Status = 404,
                Message = "Không tìm thấy combo."
            };
        }

        return new ServiceResponse<ComboDto>
        {
            Status = 200,
            Message = "Lấy thông tin combo thành công.",
            Data = combo
        };
    }

    public async Task<ServiceResponse> AddComboAsync(ComboUpdateDto dto, string? sub)
    {
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
        {
            return new ServiceResponse
            {
                Status = 400,
                Message = "Thông tin người dùng không hợp lệ"
            };
        }

        var isComboExist = await appDbContext.Combo.AnyAsync(c => c.Name == dto.Name);
        if (isComboExist)
        {
            return new ServiceResponse { Status = 400, Message = "Combo đã tồn tại." };
        }

        string? uploadedImageUrl = null;
        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        try
        {
            uploadedImageUrl = await SaveFile.SaveFileAsync(dto.Image, "combos");
            if (string.IsNullOrEmpty(uploadedImageUrl))
            {
                return new ServiceResponse { Status = 400, Message = "Lỗi khi tải ảnh combo lên hệ thống." };
            }

            var newComboId = Guid.NewGuid();

            var newCombo = new Combo
            {
                Idcombo = newComboId,
                Name = dto.Name,
                Price = dto.Price,
                Image = uploadedImageUrl,
                CreateBy = userId
            };

            await appDbContext.Combo.AddAsync(newCombo);

            if (dto.ListProducts.Count != 0)
            {
                var comboProducts = new List<Comboproduct>();

                foreach (var item in dto.ListProducts)
                {
                    var product = await appDbContext.Product
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Name == item.Name);

                    if (product == null)
                    {
                        SaveFile.DeleteFile(uploadedImageUrl);
                        return new ServiceResponse
                        {
                            Status = 404,
                            Message = $"Không tìm thấy sản phẩm: '{item.Name}'"
                        };
                    }

                    comboProducts.Add(new Comboproduct
                    {
                        Idcombo = newComboId,
                        Idproduct = product.Idproduct,
                        Quantity = item.Quantity,
                        CreateBy = userId
                    });
                }

                await appDbContext.Comboproduct.AddRangeAsync(comboProducts);
            }

            await appDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResponse { Status = 201, Message = "Thêm Combo thành công." };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            if (!string.IsNullOrEmpty(uploadedImageUrl))
            {
                SaveFile.DeleteFile(uploadedImageUrl);
            }

            logger.LogError(ex, "Lỗi khi thêm Combo.");
            return new ServiceResponse { Status = 500, Message = "Lỗi hệ thống. Đã hoàn tác thay đổi." };
        }
    }

    public async Task<ServiceResponse> UpdateComboAsync(Guid id, ComboUpdateDto dto, string? sub)
    {
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
        {
            return new ServiceResponse
            {
                Status = 400,
                Message = "Thông tin người dùng không hợp lệ"
            };
        }

        var uploadedImageUrl = string.Empty;
        var oldImageUrlToDeleteOnSuccess = string.Empty;

        var combo = await appDbContext.Combo
            .Include(c => c.Comboproduct)
            .FirstOrDefaultAsync(c => c.Idcombo == id);

        if (combo == null)
        {
            return new ServiceResponse { Status = 404, Message = "Không tìm thấy Combo." };
        }

        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        try
        {
            var isNameExist = await appDbContext.Combo.AnyAsync(c => c.Name == dto.Name && c.Idcombo != id);
            if (isNameExist)
            {
                return new ServiceResponse { Status = 400, Message = "Tên Combo đã tồn tại." };
            }

            combo.Name = dto.Name;
            combo.Price = dto.Price;
            combo.UpdateBy = userId;

            if (dto.Image.Length > 0)
            {
                uploadedImageUrl = await SaveFile.SaveFileAsync(dto.Image, "combos");
                if (string.IsNullOrEmpty(uploadedImageUrl))
                {
                    return new ServiceResponse { Status = 400, Message = "Lỗi khi tải ảnh mới lên." };
                }

                oldImageUrlToDeleteOnSuccess = combo.Image;
                combo.Image = uploadedImageUrl;
            }

            if (combo.Comboproduct.Count != 0)
            {
                appDbContext.Comboproduct.RemoveRange(combo.Comboproduct);
            }

            if (dto.ListProducts.Count != 0)
            {
                var newComboProducts = new List<Comboproduct>();
                foreach (var item in dto.ListProducts)
                {
                    var productEntity = await appDbContext.Product
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Name == item.Name);

                    if (productEntity == null)
                    {
                        SaveFile.DeleteFile(uploadedImageUrl);
                        return new ServiceResponse { Status = 404, Message = $"Không tìm thấy sản phẩm: {item.Name}" };
                    }

                    newComboProducts.Add(new Comboproduct
                    {
                        Idcombo = id,
                        Idproduct = productEntity.Idproduct,
                        Quantity = item.Quantity,
                        CreateBy = userId
                    });
                }

                await appDbContext.Comboproduct.AddRangeAsync(newComboProducts);
            }

            await appDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            if (!string.IsNullOrEmpty(oldImageUrlToDeleteOnSuccess))
            {
                SaveFile.DeleteFile(oldImageUrlToDeleteOnSuccess);
            }

            return new ServiceResponse { Status = 200, Message = "Cập nhật Combo thành công." };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            SaveFile.DeleteFile(uploadedImageUrl);

            logger.LogError(ex, "Lỗi khi cập nhật Combo ID: {Id}", id);
            return new ServiceResponse { Status = 500, Message = "Lỗi hệ thống. Đã hoàn tác thay đổi." };
        }
    }

    public async Task<ServiceResponse> DeleteComboAsync(Guid id)
    {
        var combo = await appDbContext.Combo
            .Include(c => c.Comboproduct)
            .FirstOrDefaultAsync(c => c.Idcombo == id);

        if (combo == null)
        {
            return new ServiceResponse { Status = 404, Message = "Không tìm thấy Combo." };
        }

        if (combo.Comboproduct.Count > 0)
        {
            appDbContext.Comboproduct.RemoveRange(combo.Comboproduct);
        }

        appDbContext.Combo.Remove(combo);
        await appDbContext.SaveChangesAsync();

        return new ServiceResponse { Status = 200, Message = "Xóa Combo thành công." };
    }
}
