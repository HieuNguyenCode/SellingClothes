using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services.Dto.Requests;
using Services.Dto.Responses;
using Services.Interfaces;

namespace Services.Features;

public class ProductService(AppDbContext appDbContext) : IProductService
{
    public async Task<ServiceResponse<List<ProductsDto>>> GetProductsAsync(string? search, int? page, int? pageSize)
    {
        var now = DateTime.Now;

        var query = appDbContext.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Name.Contains(search) ||
                                     c.IdtypeNavigation.Name.Contains(search) ||
                                     c.IdcompanyNavigation.Name.Contains(search));
        }

        query = query.OrderBy(c => c.Name);

        var validPage = (page ?? 1) > 0 ? page ?? 1 : 1;
        var validPageSize = (pageSize ?? 10) > 0 ? pageSize ?? 10 : 10;

        var products = await query
            .Skip((validPage - 1) * validPageSize)
            .Take(validPageSize)
            .Select(c => new ProductsDto
            {
                Id = c.Idproduct,
                Name = c.Name,
                Price = c.Price,

                PriceSale = c.Saleproducts
                    .Where(s => s.StartDate <= now && s.EndDate >= now)
                    .Select(s => (int?)s.Price)
                    .Min(),

                Image = c.Images.Select(i => i.Url).FirstOrDefault() ?? ""
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
                PriceSale = p.Saleproducts
                    .Where(s => s.StartDate <= now && s.EndDate >= now)
                    .Select(s => (int?)s.Price)
                    .Min(),
                Images = p.Images.Select(i => i.Url).ToList(),

                // Sizes = p.Sizes.Select(s => s.Name).ToList(),
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

    public async Task<ServiceResponse> CreateProductAsync(ProductUpdateDto dto)
    {
        var isNameExist = await appDbContext.Products.AnyAsync(p => p.Name == dto.Name);
        if (isNameExist)
        {
            return new ServiceResponse { Status = 400, Message = "Tên sản phẩm đã tồn tại." };
        }

        // 2. Truy xuất ID của Loại sản phẩm và Thương hiệu từ Name
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

        // 3. Xử lý lưu ảnh đại diện (Image chính)
        var mainImageUrl = await SaveFileAsync(dto.Image);
        if (string.IsNullOrEmpty(mainImageUrl))
        {
            return new ServiceResponse { Status = 400, Message = "Lỗi khi tải ảnh chính lên hệ thống." };
        }

        var newProductId = Guid.NewGuid();

        var product = new Product
        {
            Idproduct = newProductId,
            Name = dto.Name,
            Price = dto.Price,
            Describe = dto.Description,
            Idcompany = company.Idcompany,
            Idtype = type.Idtype,
            Image = mainImageUrl
        };

        await appDbContext.Products.AddAsync(product);

        // 5. Xử lý lưu danh sách ảnh phụ (nếu có)
        if (dto.Images != null && dto.Images.Any())
        {
            var productImages = new List<Image>();
            foreach (var file in dto.Images)
            {
                var imageUrl = await SaveFileAsync(file);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    productImages.Add(new Image
                    {
                        Url = imageUrl,
                        Idproduct = newProductId
                    });
                }
            }

            if (productImages.Count != 0)
            {
                await appDbContext.Images.AddRangeAsync(productImages);
            }
        }

        await appDbContext.SaveChangesAsync();

        return new ServiceResponse
        {
            Status = 201,
            Message = "Thêm sản phẩm thành công."
        };
    }

    public async Task<ServiceResponse> UpdateProductAsync(Guid id, ProductUpdateDto productUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteProductAsync(Guid id)
    {
        throw new NotImplementedException();
    }

// ====================================================================
// HÀM HỖ TRỢ LƯU FILE 
// ====================================================================
    private static async Task<string> SaveFileAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return string.Empty;
        }

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/images/{fileName}";
    }
}
