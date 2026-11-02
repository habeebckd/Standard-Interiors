using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using StandardInteriors.Api.Data;
using StandardInteriors.Api.Controllers;
using StandardInteriors.Api.Domain.Entities;
using StandardInteriors.Api.Middleware;
using StandardInteriors.Api.Options;
using StandardInteriors.Api.Services;
using StandardInteriors.Api.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
builder.Services.Configure<JwtOptions>(options =>
{
    options.Key = Environment.GetEnvironmentVariable("STANDARD_INTERIORS_JWT_KEY") ?? builder.Configuration["Jwt:Key"] ?? string.Empty;
    options.Issuer = builder.Configuration["Jwt:Issuer"] ?? "StandardInteriors.Api";
    options.Audience = builder.Configuration["Jwt:Audience"] ?? "StandardInteriors.Frontend";
    options.ExpirationMinutes = int.TryParse(builder.Configuration["Jwt:ExpirationMinutes"], out var minutes) ? minutes : 480;
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var key = Environment.GetEnvironmentVariable("STANDARD_INTERIORS_JWT_KEY") ?? builder.Configuration["Jwt:Key"] ?? string.Empty;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key.PadRight(32, '0'))),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "StandardInteriors.Api",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "StandardInteriors.Frontend",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationHandler, PaymentVoidHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PaymentVoid", policy => policy.RequireAuthenticatedUser().AddRequirements(new PaymentVoidRequirement()));
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

builder.Services.Configure<BrandingOptions>(builder.Configuration.GetSection("Branding"));
builder.Services.AddDbContext<StandardInteriorsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ICalculationService, CalculationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IServiceCatalogService, ServiceCatalogService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddScoped<IEstimateService, EstimateService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IWorkStatusService, WorkStatusService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("frontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StandardInteriorsDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.EnsureSeededAsync(db);
    await DbSeeder.EnsureAdminAsync(db, scope.ServiceProvider.GetRequiredService<IPasswordHasher<AppUser>>(), app.Configuration, app.Logger);
}

app.Run();
