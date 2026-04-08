using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Dto.Requests;
using Services.Dto.Responses;
using Services.Helpers;
using Services.Interfaces;

namespace Services.Features;

public class ProductService(
    AppDbContext appDbContext,
    ILogger<ProductService> logger
) : IProductService
{
    public async Task<ServiceResponse<List<ProductsDto>>> GetProductsAsync(string? role, string? search, int? page,
        int? pageSize)
    {
        var now = DateTime.Now;
        var query = appDbContext.Products.AsNoTracking().AsQueryable()
            .Where(p => !p.IsDeleted && (role == "admin" || p.IsPublished));

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Name.Contains(search) ||
                                     c.IdtypeNavigation.Name.Contains(search) ||
                                     c.IdcompanyNavigation.Name.Contains(search));
        }

        query = query.OrderBy(c => c.Name);
        List<ProductsDto> products;
        if (page.HasValue && pageSize.HasValue)
        {
            var validPage = page.Value > 0 ? page.Value : 1;
            var validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

            var totalCount = await query.CountAsync();

            products = await query
                .Skip((validPage - 1) * validPageSize)
                .Take(validPageSize)
                .Select(c => new ProductsDto
                {
                    Id = c.Idproduct,
                    Name = c.Name,
                    Price = c.Price,
                    PriceSale = c.SaleProducts
                        .Where(s => s.StartDate <= now && (s.EndDate == null || s.EndDate >= now))
                        .Select(s => (int?)s.Price)
                        .Min(),
                    Image = c.Image,
                    IsPublished = c.IsPublished
                })
                .ToListAsync();

            return new ServiceResponse<List<ProductsDto>>
            {
                Status = 200,
                Message = "Lấy danh sách sản phẩm thành công.",
                Data = products,
                PageSize = validPageSize,
                PageNumber = validPage,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / validPageSize)
            };
        }

        products = await query
            .Select(c => new ProductsDto
            {
                Id = c.Idproduct,
                Name = c.Name,
                Price = c.Price,
                PriceSale = c.SaleProducts
                    .Where(s => s.StartDate <= now && (s.EndDate == null || s.EndDate >= now))
                    .Select(s => (int?)s.Price)
                    .Min(),
                Image = c.Image,
                IsPublished = c.IsPublished
            })
            .ToListAsync();

        return new ServiceResponse<List<ProductsDto>>
        {
            Status = 200,
            Message = "Lấy danh sách sản phẩm thành công.",
            Data = products
        };
    }

    public async Task<ServiceResponse<ProductDto>> GetProductByIdAsync(Guid id)
    {
        var now = DateTime.Now;

        var product = await appDbContext.Products
            .AsNoTracking()
            .Where(p => p.Idproduct == id)
            .Select(p => new ProductDto
            {
                Id = p.Idproduct,
                Name = p.Name,
                Price = p.Price,
                PriceSale = p.SaleProducts
                    .Where(s => s.StartDate <= now && (s.EndDate == null || s.EndDate >= now))
                    .Select(s => (int?)s.Price)
                    .Min(),
                Image = p.Image,
                Images = p.Images.Select(i => i.Url).ToList(),

                Sizes = p.Sizes.Select(s => s.Name).ToList(),
                Colors = p.Colors.Select(c => c.Name).ToList(),

                TypeName = p.IdtypeNavigation.Name,
                CompanyName = p.IdcompanyNavigation.Name,
                Description = p.Describe
            })
            .FirstOrDefaultAsync();
        if (product == null)
        {
            return new ServiceResponse<ProductDto>
            {
                Status = 404,
                Message = "Không tìm thấy sản phẩm."
            };
        }

        return new ServiceResponse<ProductDto>
        {
            Status = 200,
            Message = "Lấy thông tin chi tiết sản phẩm thành công.",
            Data = product
        };
    }

    public async Task<ServiceResponse> CreateProductAsync(ProductUpdateDto dto, string? sub)
    {
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
        {
            return new ServiceResponse
            {
                Status = 400,
                Message = "Thông tin người dùng không hợp lệ"
            };
        }

        var uploadedImageUrls = new List<string>();

        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        try
        {
            var isNameExist = await appDbContext.Products.AnyAsync(p => p.Name == dto.Name);
            if (isNameExist)
            {
                return new ServiceResponse { Status = 400, Message = "Tên sản phẩm đã tồn tại." };
            }

            var type = await appDbContext.Types
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Name == dto.TypeName);
            if (type == null)
            {
                return new ServiceResponse { Status = 404, Message = "Không tìm thấy loại sản phẩm." };
            }

            var company = await appDbContext.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == dto.CompanyName);
            if (company == null)
            {
                return new ServiceResponse { Status = 404, Message = "Không tìm thấy thương hiệu." };
            }

            var mainImageUrl = await SaveFile.SaveFileAsync(dto.Image);
            if (string.IsNullOrEmpty(mainImageUrl))
            {
                return new ServiceResponse { Status = 400, Message = "Lỗi khi tải ảnh chính lên hệ thống." };
            }

            uploadedImageUrls.Add(mainImageUrl);

            var newProductId = Guid.NewGuid();

            var product = new Product
            {
                Idproduct = newProductId,
                Name = dto.Name,
                Price = dto.Price,
                Describe = dto.Description,
                Idcompany = company.Idcompany,
                Idtype = type.Idtype,
                Image = mainImageUrl,
                CreateBy = userId
            };

            await appDbContext.Products.AddAsync(product);
            Console.WriteLine(dto.Images?.Count ?? 0);

            if (dto.Images != null && dto.Images.Count != 0)
            {
                var productImages = new List<Image>();
                foreach (var file in dto.Images)
                {
                    var imageUrl = await SaveFile.SaveFileAsync(file);
                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        continue;
                    }

                    uploadedImageUrls.Add(imageUrl);

                    productImages.Add(new Image
                    {
                        Url = imageUrl,
                        Idproduct = newProductId,
                        CreateBy = userId
                    });
                }

                if (productImages.Count != 0)
                {
                    await appDbContext.Images.AddRangeAsync(productImages);
                }
            }

            if (dto.Colors.Count != 0)
            {
                var productColors = (from color in dto.Colors
                    where !string.IsNullOrEmpty(color)
                    select new Color { Idproduct = newProductId, Name = color }).ToList();

                if (productColors.Count != 0)
                {
                    await appDbContext.Colors.AddRangeAsync(productColors);
                }
            }

            if (dto.Sizes.Count != 0)
            {
                var productSizes = (from size in dto.Sizes
                    where !string.IsNullOrEmpty(size)
                    select new Size { Idproduct = newProductId, Name = size }).ToList();

                if (productSizes.Count != 0)
                {
                    await appDbContext.Sizes.AddRangeAsync(productSizes);
                }
            }

            await appDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResponse
            {
                Status = 201,
                Message = "Thêm sản phẩm thành công."
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            SaveFile.DeleteFileRange(uploadedImageUrls);
            logger.LogError(ex, "Lỗi khi tạo sản phẩm. Đã hoàn tác toàn bộ thay đổi và xóa các file đã tải lên.");
            return new ServiceResponse
            {
                Status = 500,
                Message = "Có lỗi xảy ra trong quá trình lưu dữ liệu. Đã hoàn tác toàn bộ thay đổi."
            };
        }
    }

    public async Task<ServiceResponse> UpdateProductAsync(Guid id, ProductUpdateDto dto, string? sub)
    {
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
        {
            return new ServiceResponse
            {
                Status = 400,
                Message = "Thông tin người dùng không hợp lệ"
            };
        }

        var uploadedImageUrls = new List<string>();
        var oldImageUrlsToDeleteOnSuccess = new List<string>();

        var product = await appDbContext.Products
            .Include(p => p.Images)
            .Include(p => p.Colors)
            .Include(p => p.Sizes)
            .FirstOrDefaultAsync(p => p.Idproduct == id);

        if (product == null)
        {
            return new ServiceResponse { Status = 404, Message = "Không tìm thấy sản phẩm." };
        }

        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        try
        {
            var isNameExist = await appDbContext.Products.AnyAsync(p => p.Name == dto.Name && p.Idproduct != id);
            if (isNameExist)
            {
                return new ServiceResponse { Status = 400, Message = "Tên sản phẩm đã tồn tại." };
            }

            var type = await appDbContext.Types
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Name == dto.TypeName);
            if (type == null)
            {
                return new ServiceResponse { Status = 404, Message = "Không tìm thấy loại sản phẩm." };
            }

            var company = await appDbContext.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == dto.CompanyName);
            if (company == null)
            {
                return new ServiceResponse { Status = 404, Message = "Không tìm thấy thương hiệu." };
            }

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Describe = dto.Description;
            product.Idcompany = company.Idcompany;
            product.Idtype = type.Idtype;
            product.UpdateBy = userId;

            var newMainImageUrl = await SaveFile.SaveFileAsync(dto.Image);
            if (string.IsNullOrEmpty(newMainImageUrl))
            {
                return new ServiceResponse { Status = 400, Message = "Lỗi khi tải ảnh chính lên hệ thống." };
            }

            uploadedImageUrls.Add(newMainImageUrl);
            oldImageUrlsToDeleteOnSuccess.Add(product.Image);
            product.Image = newMainImageUrl;


            if (dto.Images != null && dto.Images.Count != 0)
            {
                if (product.Images.Count != 0)
                {
                    oldImageUrlsToDeleteOnSuccess.AddRange(product.Images.Select(i => i.Url));
                    appDbContext.Images.RemoveRange(product.Images);
                }

                var productImages = new List<Image>();
                foreach (var file in dto.Images)
                {
                    var imageUrl = await SaveFile.SaveFileAsync(file);
                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        continue;
                    }

                    uploadedImageUrls.Add(imageUrl);

                    productImages.Add(new Image
                    {
                        Url = imageUrl,
                        Idproduct = id,
                        CreateBy = userId
                    });
                }

                if (productImages.Count != 0)
                {
                    await appDbContext.Images.AddRangeAsync(productImages);
                }
            }

            if (product.Colors.Count != 0)
            {
                appDbContext.Colors.RemoveRange(product.Colors);
            }

            if (dto.Colors.Count != 0)
            {
                var productColors = (from color in dto.Colors
                    where !string.IsNullOrEmpty(color)
                    select new Color { Idproduct = id, Name = color }).ToList();

                if (productColors.Count != 0)
                {
                    await appDbContext.Colors.AddRangeAsync(productColors);
                }
            }

            if (product.Sizes.Count != 0)
            {
                appDbContext.Sizes.RemoveRange(product.Sizes);
            }

            if (dto.Sizes.Count != 0)
            {
                var productSizes = (from size in dto.Sizes
                    where !string.IsNullOrEmpty(size)
                    select new Size { Idproduct = id, Name = size }).ToList();

                if (productSizes.Count != 0)
                {
                    await appDbContext.Sizes.AddRangeAsync(productSizes);
                }
            }

            await appDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            SaveFile.DeleteFileRange(oldImageUrlsToDeleteOnSuccess);

            return new ServiceResponse
            {
                Status = 200,
                Message = "Cập nhật sản phẩm thành công."
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            SaveFile.DeleteFileRange(uploadedImageUrls);

            logger.LogError(ex, "Lỗi khi cập nhật sản phẩm. Đã hoàn tác toàn bộ thay đổi và xóa các file đã tải lên.");
            return new ServiceResponse
            {
                Status = 500,
                Message = "Có lỗi xảy ra trong quá trình lưu dữ liệu. Đã hoàn tác toàn bộ thay đổi."
            };
        }
    }

    public async Task<ServiceResponse> DeleteProductAsync(Guid id, string? sub)
    {
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
        {
            return new ServiceResponse
            {
                Status = 400,
                Message = "Thông tin người dùng không hợp lệ"
            };
        }

        var product = await appDbContext.Products
            .FirstOrDefaultAsync(p => p.Idproduct == id);

        if (product == null)
        {
            return new ServiceResponse { Status = 404, Message = "Không tìm thấy sản phẩm." };
        }

        product.IsDeleted = !product.IsDeleted;
        product.UpdateBy = userId;
        appDbContext.Products.Update(product);
        await appDbContext.SaveChangesAsync();

        return new ServiceResponse
        {
            Status = 200,
            Message = "Xóa sản phẩm thành công."
        };
    }

    public async Task<ServiceResponse> PublishProductAsync(Guid id, string? sub)
    {
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
        {
            return new ServiceResponse
            {
                Status = 400,
                Message = "Thông tin người dùng không hợp lệ"
            };
        }

        var product = await appDbContext.Products
            .Include(p => p.ComboProducts)
            .FirstOrDefaultAsync(p => p.Idproduct == id);

        if (product == null)
        {
            return new ServiceResponse { Status = 404, Message = "Không tìm thấy sản phẩm." };
        }

        if (product.IsDeleted)
        {
            return new ServiceResponse
            {
                Status = 400,
                Message = "Không thể thay đổi trạng thái xuất bản của sản phẩm đã bị xóa."
            };
        }

        product.IsPublished = !product.IsPublished;
        product.UpdateBy = userId;

        appDbContext.Products.Update(product);
        await appDbContext.SaveChangesAsync();

        return new ServiceResponse
        {
            Status = 200,
            Message = product.IsPublished ? "Xuất bản sản phẩm thành công." : "Hủy xuất bản sản phẩm thành công."
        };
    }
}
