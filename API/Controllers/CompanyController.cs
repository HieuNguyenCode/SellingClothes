using API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Dto.Requests;
using Services.Interfaces;

namespace API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CompanyController(ICompanyService companyService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetCompany([FromQuery] string? search, [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        return Result(await companyService.GetAllCompanies(search, page, pageSize));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompanyById(Guid id)
    {
        return Result(await companyService.GetCompanyById(id));
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyUpdateDto companyCreateDto)
    {
        return Result(await companyService.CreateCompany(companyCreateDto));
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyUpdateDto companyUpdateDto)
    {
        return Result(await companyService.UpdateCompany(id, companyUpdateDto));
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        return Result(await companyService.DeleteCompany(id));
    }
}
