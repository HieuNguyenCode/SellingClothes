using System.Text;
using API.Global;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Services.Dto.Responses;
using Services.Features;
using Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// =======================================================================
// 1. CẤU HÌNH SERILOG (CONSOLE = INFO, FILE = ERROR)
// =======================================================================
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        "Logs/error-log.log",
        rollingInterval: RollingInterval.Month,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        retainedFileCountLimit: 12,
        restrictedToMinimumLevel: LogEventLevel.Error
    )
    .CreateLogger();

builder.Logging.AddSerilog(dispose: true);
// =======================================================================
// 2. KẾT NỐI DATABASE
// =======================================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
        mySqlOptions => { mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); });
});

// =======================================================================
// 3. CẤU HÌNH JWT (AUTHENTICATION)
// =======================================================================
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // 👇 SỬA LỖI CÚ PHÁP ASYNC TẠI ĐÂY
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context => // Thêm từ khóa 'async'
            {
                context.HandleResponse(); // Ngăn mặc định

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var response = new ServiceResponse
                {
                    Status = 401,
                    Message = "Bạn chưa đăng nhập hoặc Token không hợp lệ (Unauthorized)."
                };

                // Dùng System.Text.Json để viết response
                await context.Response.WriteAsJsonAsync(response);
            },
            OnForbidden = async context => // Thêm từ khóa 'async'
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";

                var response = new ServiceResponse
                {
                    Status = 403,
                    Message = "Bạn không có quyền truy cập chức năng này (Forbidden)."
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ITypeService, TypeService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IComboService, ComboService>();
builder.Services.AddScoped<ISaleService, SaleService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

builder.Services.AddRateLimiter(_ => { });

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response is { StatusCode: StatusCodes.Status404NotFound, HasStarted: false })
    {
        context.Response.ContentType = "application/json";
        var response = new ServiceResponse
        {
            Status = 404,
            Message = "Đường dẫn không tồn tại (URL Error). Vui lòng kiểm tra lại API Endpoint."
        };
        await context.Response.WriteAsJsonAsync(response);
    }
});

app.UseRateLimiter();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();

app.Run();