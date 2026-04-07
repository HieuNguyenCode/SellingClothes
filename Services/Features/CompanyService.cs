using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Services.Dto.Requests;
using Services.Dto.Responses;
using Services.Interfaces;

namespace Services.Features;

public class CompanyService(AppDbContext appDbContext) : ICompanyService
{
    public async Task<ServiceResponse<List<CompanysDto>>> GetAllCompanies(string? search, int? page, int? pageSize)
    {
        var query = appDbContext.Companies.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(c => c.Name.Contains(search));

        query = query.OrderBy(c => c.Name);

        var validPage = (page ?? 1) > 0 ? page ?? 1 : 1;
        var validPageSize = (pageSize ?? 10) > 0 ? pageSize ?? 10 : 10;

        var companies = await query
            .Skip((validPage - 1) * validPageSize)
            .Take(validPageSize)
            .Select(c => new CompanysDto
            {
                Id = c.Idcompany,
                Name = c.Name
            })
            .ToListAsync();

        return new ServiceResponse<List<CompanysDto>>
        {
            Status = 200,
            Message = "Lấy danh sách thương hiệu/công ty thành công.",
            Data = companies
        };
    }

    public async Task<ServiceResponse<CompanysDto>> GetCompanyById(Guid id)
    {
        var company = await appDbContext.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Idcompany == id);
        if (company == null)
            return new ServiceResponse<CompanysDto>
            {
                Status = 404,
                Message = "Không tìm thấy công ty."
            };

        var companyDto = new CompanysDto
        {
            Id = company.Idcompany,
            Name = company.Name
        };

        return new ServiceResponse<CompanysDto>
        {
            Status = 200,
            Data = companyDto
        };
    }

    public async Task<ServiceResponse> CreateCompany(CompanyUpdateDto companyCreateDto)
    {
        var company = await appDbContext.Companies.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == companyCreateDto.Name)
            .ConfigureAwait(false);
        if (company != null)
            return new ServiceResponse
            {
                Status = 400,
                Message = "Tên công ty đã tồn tại."
            };

        var newCompany = new Company
        {
            Name = companyCreateDto.Name
        };
        await appDbContext.Companies.AddAsync(newCompany);
        await appDbContext.SaveChangesAsync().ConfigureAwait(false);
        return new ServiceResponse
        {
            Status = 200,
            Message = "Tạo công ty thành công."
        };
    }

    public async Task<ServiceResponse> UpdateCompany(Guid id, CompanyUpdateDto companyUpdateDto)
    {
        var company = await appDbContext.Companies.FirstOrDefaultAsync(c => c.Idcompany == id);
        if (company == null)
            return new ServiceResponse
            {
                Status = 404,
                Message = "Không tìm thấy công ty."
            };

        var isNameExist = await appDbContext.Companies.AsNoTracking()
            .AnyAsync(c => c.Name == companyUpdateDto.Name && c.Idcompany != id);
        if (isNameExist)
            return new ServiceResponse
            {
                Status = 400,
                Message = "Tên công ty đã tồn tại."
            };

        company.Name = companyUpdateDto.Name;
        appDbContext.Companies.Update(company);
        await appDbContext.SaveChangesAsync().ConfigureAwait(false);
        return new ServiceResponse
        {
            Status = 200,
            Message = "Cập nhật công ty thành công."
        };
    }

    public async Task<ServiceResponse> DeleteCompany(Guid id)
    {
        var company = await appDbContext.Companies
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Idcompany == id);
        if (company == null)
            return new ServiceResponse
            {
                Status = 404,
                Message = "Không tìm thấy công ty."
            };

        if (company.Products.Count != 0)
            return new ServiceResponse
            {
                Status = 400,
                Message = "Không thể xóa công ty vì vẫn còn sản phẩm liên quan."
            };

        appDbContext.Companies.Remove(company);
        await appDbContext.SaveChangesAsync().ConfigureAwait(false);
        return new ServiceResponse
        {
            Status = 200,
            Message = "Xóa công ty thành công."
        };
    }
}