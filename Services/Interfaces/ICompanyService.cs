using Services.Dto.Requests;
using Services.Dto.Responses;

namespace Services.Interfaces;

public interface ICompanyService
{
    Task<ServiceResponse<List<CompanysDto>>> GetAllCompanies(string? search, int? page, int? pageSize);
    Task<ServiceResponse<CompanysDto>> GetCompanyById(Guid id);
    Task<ServiceResponse> CreateCompany(CompanyUpdateDto companyCreateDto);
    Task<ServiceResponse> UpdateCompany(Guid id, CompanyUpdateDto companyUpdateDto);
    Task<ServiceResponse> DeleteCompany(Guid id);
}
