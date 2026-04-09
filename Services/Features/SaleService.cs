using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Dto.Requests;
using Services.Dto.Responses;
using Services.Interfaces;

namespace Services.Features;

public class SaleService(
    AppDbContext appDbContext,
    ILogger<SaleService> logger
) : ISaleService
{
    public async Task<ServiceResponse<List<SalesDto>>> GetAllSalesAsync(string? search, int? page, int? pageSize)
    {
        var query = appDbContext.Sales.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Name.Contains(search));
        }

        query = query.OrderBy(c => c.Name);
        List<SalesDto> listSale;
        if (page.HasValue && pageSize.HasValue)
        {
            var validPage = page.Value > 0 ? page.Value : 1;
            var validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

            var totalCount = await query.CountAsync();

            listSale = await query
                .Skip((validPage - 1) * validPageSize)
                .Take(validPageSize)
                .Select(c => new SalesDto
                {
                    Id = c.Idsale,
                    Name = c.Name
                })
                .ToListAsync();

            return new ServiceResponse<List<SalesDto>>
            {
                Status = 200,
                Message = "Lấy danh sách thương hiệu/công ty thành công.",
                Data = listSale,
                PageSize = validPageSize,
                PageNumber = validPage,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / validPageSize)
            };
        }

        listSale = await query
            .Select(c => new SalesDto
            {
                Id = c.Idsale,
                Name = c.Name
            })
            .ToListAsync();

        return new ServiceResponse<List<SalesDto>>
        {
            Status = 200,
            Message = "Lấy danh sách thương hiệu/công ty thành công.",
            Data = listSale
        };
    }

    public async Task<ServiceResponse<SaleDto>> GetSaleByIdAsync(Guid saleId)
    {
        var sale = await appDbContext.Sales
            .Where(c => c.Idsale == saleId)
            .Select(s => new SaleDto
            {
                Id = s.Idsale,
                Name = s.Name,
                SaleProducts = s.SaleProducts.Select(sp => new SaleProductsDto
                {
                    Id = sp.IdsaleProduct,
                    Name = sp.IdproductNavigation.Name,
                    Image = sp.IdproductNavigation.Image,
                    Price = sp.Price,
                    StartDate = sp.StartDate,
                    EndDate = sp.EndDate
                }).ToList(),
                SaleCombos = s.SaleCombos.Select(sc => new SaleCombosDto
                {
                    Id = sc.IdsaleCombo,
                    Name = sc.IdcomboNavigation.Name,
                    Image = sc.IdcomboNavigation.Image,
                    Price = sc.Price,
                    StartDate = sc.StartDate,
                    EndDate = sc.EndDate
                }).ToList()
            }).FirstOrDefaultAsync();

        return new ServiceResponse<SaleDto>
        {
            Status = 200,
            Message = "Danh sách sản phẩm được sale trong sự kiện láy thành công",
            Data = sale
        };
    }

    public async Task<ServiceResponse> CreateSaleAsync(SaleUpdateDto saleDto, string? sub)
    {
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
        {
            return new ServiceResponse
            {
                Status = 400,
                Message = "Thông tin người dùng không hợp lệ"
            };
        }

        var sale = await appDbContext.Sales.FirstOrDefaultAsync(c => c.Name == saleDto.Name);
        if (sale != null)
        {
            return new ServiceResponse
            {
                Status = 400,
                Message = "Tên sự kiện đã tồn tại. Vui lòng chọn tên khác."
            };
        }

        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        try
        {
            var idSale = Guid.NewGuid();
            var newsale = new Sale
            {
                Idsale = idSale,
                Name = saleDto.Name
            };

            await appDbContext.Sales.AddAsync(newsale);

            if (saleDto.SaleProducts.Count > 0)
            {
                var listSaleProducts = new List<SaleProduct>();
                foreach (var saleProductDto in saleDto.SaleProducts)
                {
                    var product = await appDbContext.Products.FirstOrDefaultAsync(p =>
                        p.Name == saleProductDto.Name &&
                        p.IsDeleted == false);
                    if (product == null)
                    {
                        return new ServiceResponse
                        {
                            Status = 400,
                            Message = $"Sản phẩm '{saleProductDto.Name}' không tồn tại."
                        };
                    }

                    listSaleProducts.Add(new SaleProduct
                    {
                        Idsale = idSale,
                        Idproduct = product.Idproduct,
                        Price = saleProductDto.Price,
                        StartDate = saleProductDto.StartDate,
                        EndDate = saleProductDto.EndDate,
                        CreateBy = userId
                    });
                }

                if (listSaleProducts.Count > 0)
                {
                    await appDbContext.SaleProducts.AddRangeAsync(listSaleProducts);
                }
            }

            if (saleDto.SaleCombos.Count > 0)
            {
                var listSaleCombos = new List<SaleCombo>();
                foreach (var saleComboDto in saleDto.SaleCombos)
                {
                    var combo = await appDbContext.Combos.FirstOrDefaultAsync(c
                        => c.Name == saleComboDto.Name && c.IsDeleted == false);
                    if (combo == null)
                    {
                        return new ServiceResponse
                        {
                            Status = 400,
                            Message = $"Combo '{saleComboDto.Name}' không tồn tại."
                        };
                    }

                    listSaleCombos.Add(new SaleCombo
                    {
                        Idsale = idSale,
                        Idcombo = combo.Idcombo,
                        Price = saleComboDto.Price,
                        StartDate = saleComboDto.StartDate,
                        EndDate = saleComboDto.EndDate,
                        CreateBy = userId
                    });
                }

                if (listSaleCombos.Count > 0)
                {
                    await appDbContext.SaleCombos.AddRangeAsync(listSaleCombos);
                }
            }

            await appDbContext.SaveChangesAsync();
            transaction.Commit();
            return new ServiceResponse
            {
                Status = 200,
                Message = "Tạo sự kiện khuyến mãi thành công."
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi tạo sự kiện khuyến mãi.");
            await transaction.RollbackAsync();
            return new ServiceResponse
            {
                Status = 500,
                Message = "Đã xảy ra lỗi khi tạo sự kiện khuyến mãi. Vui lòng thử lại sau."
            };
        }
    }

    public async Task<ServiceResponse> UpdateSaleAsync(Guid saleId, SaleUpdateDto saleDto, string? sub)
    {
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
        {
            return new ServiceResponse
            {
                Status = 400,
                Message = "Thông tin người dùng không hợp lệ"
            };
        }

        var sale = await appDbContext.Sales.FirstOrDefaultAsync(s => s.Idsale == saleId);
        if (sale == null)
        {
            return new ServiceResponse
            {
                Status = 404,
                Message = "Không tìm thấy sự kiện khuyến mãi."
            };
        }

        // Kiểm tra trùng lặp tên nếu người dùng có thay đổi tên sự kiện
        if (sale.Name != saleDto.Name)
        {
            var existingName = await appDbContext.Sales.AnyAsync(s => s.Name == saleDto.Name);
            if (existingName)
            {
                return new ServiceResponse
                {
                    Status = 400,
                    Message = "Tên sự kiện đã tồn tại. Vui lòng chọn tên khác."
                };
            }

            sale.Name = saleDto.Name;
        }

        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        try
        {
            // 1. Xóa danh sách SaleProducts và SaleCombos cũ
            var oldSaleProducts = await appDbContext.SaleProducts.Where(sp => sp.Idsale == saleId).ToListAsync();
            if (oldSaleProducts.Count > 0)
            {
                appDbContext.SaleProducts.RemoveRange(oldSaleProducts);
            }

            var oldSaleCombos = await appDbContext.SaleCombos.Where(sc => sc.Idsale == saleId).ToListAsync();
            if (oldSaleCombos.Count > 0)
            {
                appDbContext.SaleCombos.RemoveRange(oldSaleCombos);
            }

            // 2. Thêm mới danh sách SaleProducts theo DTO
            if (saleDto.SaleProducts.Count > 0)
            {
                var listSaleProducts = new List<SaleProduct>();
                foreach (var saleProductDto in saleDto.SaleProducts)
                {
                    var product = await appDbContext.Products.FirstOrDefaultAsync(p =>
                        p.Name == saleProductDto.Name && p.IsDeleted == false);
                    if (product == null)
                    {
                        return new ServiceResponse
                        {
                            Status = 400,
                            Message = $"Sản phẩm '{saleProductDto.Name}' không tồn tại."
                        };
                    }

                    listSaleProducts.Add(new SaleProduct
                    {
                        Idsale = saleId,
                        Idproduct = product.Idproduct,
                        Price = saleProductDto.Price,
                        StartDate = saleProductDto.StartDate,
                        EndDate = saleProductDto.EndDate,
                        CreateBy = userId
                    });
                }

                await appDbContext.SaleProducts.AddRangeAsync(listSaleProducts);
            }

            // 3. Thêm mới danh sách SaleCombos theo DTO
            if (saleDto.SaleCombos.Count > 0)
            {
                var listSaleCombos = new List<SaleCombo>();
                foreach (var saleComboDto in saleDto.SaleCombos)
                {
                    var combo = await appDbContext.Combos.FirstOrDefaultAsync(c
                        => c.Name == saleComboDto.Name && c.IsDeleted == false);
                    if (combo == null)
                    {
                        return new ServiceResponse
                        {
                            Status = 400,
                            Message = $"Combo '{saleComboDto.Name}' không tồn tại."
                        };
                    }

                    listSaleCombos.Add(new SaleCombo
                    {
                        Idsale = saleId,
                        Idcombo = combo.Idcombo,
                        Price = saleComboDto.Price,
                        StartDate = saleComboDto.StartDate,
                        EndDate = saleComboDto.EndDate,
                        CreateBy = userId
                    });
                }

                await appDbContext.SaleCombos.AddRangeAsync(listSaleCombos);
            }

            appDbContext.Sales.Update(sale);
            await appDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResponse
            {
                Status = 200,
                Message = "Cập nhật sự kiện khuyến mãi thành công."
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Lỗi khi cập nhật sự kiện khuyến mãi ID {saleId}.");
            await transaction.RollbackAsync();
            return new ServiceResponse
            {
                Status = 500,
                Message = "Đã xảy ra lỗi khi cập nhật sự kiện khuyến mãi. Vui lòng thử lại sau."
            };
        }
    }

    public async Task<ServiceResponse> DeleteSaleByIdAsync(Guid saleId, string? sub)
    {
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
        {
            return new ServiceResponse
            {
                Status = 400,
                Message = "Thông tin người dùng không hợp lệ"
            };
        }

        var sale = await appDbContext.Sales.FirstOrDefaultAsync(s => s.Idsale == saleId);
        if (sale == null)
        {
            return new ServiceResponse
            {
                Status = 404,
                Message = "Không tìm thấy sự kiện khuyến mãi để xóa."
            };
        }

        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        try
        {
            // Phải xóa các records con ở bảng phụ (SaleProducts, SaleCombos) trước để tránh lỗi Foreign Key Constraint
            var saleProducts = await appDbContext.SaleProducts.Where(sp => sp.Idsale == saleId).ToListAsync();
            if (saleProducts.Count > 0)
            {
                appDbContext.SaleProducts.RemoveRange(saleProducts);
            }

            var saleCombos = await appDbContext.SaleCombos.Where(sc => sc.Idsale == saleId).ToListAsync();
            if (saleCombos.Count > 0)
            {
                appDbContext.SaleCombos.RemoveRange(saleCombos);
            }

            // Sau đó mới xóa record cha
            appDbContext.Sales.Remove(sale);

            await appDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResponse
            {
                Status = 200,
                Message = "Xóa sự kiện khuyến mãi thành công."
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi xóa sự kiện khuyến mãi ID {SaleId}.", saleId);
            await transaction.RollbackAsync();
            return new ServiceResponse
            {
                Status = 500,
                Message = "Đã xảy ra lỗi khi xóa sự kiện khuyến mãi. Vui lòng thử lại sau."
            };
        }
    }
}
