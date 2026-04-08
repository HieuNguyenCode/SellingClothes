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
    public async Task<ServiceResponse<List<CombosDto>>> GetCombosAsync(string? role, string? search, int? page, int? pageSize)
    {
        var now = DateTime.Now;

        var query = appDbContext.Combos.AsNoTracking().AsQueryable()
            .Where(c => c.IsDeleted == false && (role == "admin" || c.IsPublished));

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Name.Contains(search.Trim()));
        }

        query = query.OrderBy(c => c.Name);
        List<CombosDto> listCombos;

        if (page.HasValue && pageSize.HasValue)
        {
            var validPage = page.Value > 0 ? page.Value : 1;
            var validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

            var totalCount = await query.CountAsync();

            listCombos = await query
                .Skip((validPage - 1) * validPageSize)
                .Take(validPageSize)
                .Select(c => new CombosDto
                {
                    Id = c.Idcombo,
                    Name = c.Name,
                    Price = c.Price,
                    PriceSale = c.SaleCombos
                        .Where(s => s.StartDate <= now && (s.EndDate == null || s.EndDate >= now))
                        .Min(s => (int?)s.Price),
                    Image = c.Image
                })
                .ToListAsync();

            return new ServiceResponse<List<CombosDto>>
            {
                Status = 200,
                Message = "Lấy danh sách combo thành công.",
                Data = listCombos,
                PageSize = validPageSize,
                PageNumber = validPage,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / validPageSize)
            };
        }

        listCombos = await query
            .Select(c => new CombosDto
            {
                Id = c.Idcombo,
                Name = c.Name,
                Price = c.Price,
                PriceSale = c.SaleCombos
                    .Where(s => s.StartDate <= now && (s.EndDate == null || s.EndDate >= now))
                    .Min(s => (int?)s.Price),
                Image = c.Image
            })
            .ToListAsync();

        return new ServiceResponse<List<CombosDto>>
        {
            Status = 200,
            Message = "Lấy danh sách combo thành công.",
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
                Image = c.Image,
                Products = c.ComboProducts.Select(cp => new ComboProductDto
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

        var isComboExist = await appDbContext.Combos.AnyAsync(c => c.Name == dto.Name);
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

            await appDbContext.Combos.AddAsync(newCombo);

            if (dto.ListProducts.Count == 0)
            {
                SaveFile.DeleteFile(uploadedImageUrl);
                return new ServiceResponse
                {
                    Status = 404,
                    Message = "Combo phải bao gồm ít nhất một sản phẩm. Vui lòng thêm sản phẩm vào combo trước khi tạo."
                };
            }

            var comboProducts = new List<ComboProduct>();

            foreach (var item in dto.ListProducts)
            {
                var product = await appDbContext.Products
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

                if (product.IsDeleted)
                {
                    SaveFile.DeleteFile(uploadedImageUrl);
                    return new ServiceResponse
                    {
                        Status = 400,
                        Message = $"Sản phẩm '{item.Name}' đã bị xóa khỏi hệ thống và không thể thêm vào combo."
                    };
                }

                comboProducts.Add(new ComboProduct
                {
                    Idcombo = newComboId,
                    Idproduct = product.Idproduct,
                    Quantity = item.Quantity,
                    CreateBy = userId
                });
            }

            await appDbContext.ComboProducts.AddRangeAsync(comboProducts);


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

        var combo = await appDbContext.Combos
            .Include(c => c.ComboProducts)
            .FirstOrDefaultAsync(c => c.Idcombo == id);

        if (combo == null)
        {
            return new ServiceResponse { Status = 404, Message = "Không tìm thấy Combo." };
        }

        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        try
        {
            var isNameExist = await appDbContext.Combos.AnyAsync(c => c.Name == dto.Name && c.Idcombo != id);
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

            if (combo.ComboProducts.Count != 0)
            {
                appDbContext.ComboProducts.RemoveRange(combo.ComboProducts);
            }

            if (dto.ListProducts.Count == 0)
            {
                SaveFile.DeleteFile(uploadedImageUrl);
                return new ServiceResponse
                {
                    Status = 404,
                    Message = "Combo phải bao gồm ít nhất một sản phẩm. Vui lòng thêm sản phẩm vào combo trước khi tạo."
                };
            }

            var newComboProducts = new List<ComboProduct>();
            foreach (var item in dto.ListProducts)
            {
                var productEntity = await appDbContext.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Name == item.Name);

                if (productEntity == null)
                {
                    SaveFile.DeleteFile(uploadedImageUrl);
                    return new ServiceResponse { Status = 404, Message = $"Không tìm thấy sản phẩm: {item.Name}" };
                }

                if (productEntity.IsDeleted)
                {
                    SaveFile.DeleteFile(uploadedImageUrl);
                    return new ServiceResponse
                    {
                        Status = 400,
                        Message = $"Sản phẩm '{item.Name}' đã bị xóa khỏi hệ thống và không thể thêm vào combo."
                    };
                }

                newComboProducts.Add(new ComboProduct
                {
                    Idcombo = id,
                    Idproduct = productEntity.Idproduct,
                    Quantity = item.Quantity,
                    CreateBy = userId
                });
            }

            await appDbContext.ComboProducts.AddRangeAsync(newComboProducts);

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
        var combo = await appDbContext.Combos
            .Include(c => c.ComboProducts)
            .FirstOrDefaultAsync(c => c.Idcombo == id);

        if (combo == null)
        {
            return new ServiceResponse { Status = 404, Message = "Không tìm thấy Combo." };
        }

        if (combo.ComboProducts.Count > 0)
        {
            appDbContext.ComboProducts.RemoveRange(combo.ComboProducts);
        }

        appDbContext.Combos.Remove(combo);
        await appDbContext.SaveChangesAsync();

        return new ServiceResponse { Status = 200, Message = "Xóa Combo thành công." };
    }
}
