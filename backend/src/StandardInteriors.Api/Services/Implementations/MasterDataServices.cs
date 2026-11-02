using Microsoft.EntityFrameworkCore;
using StandardInteriors.Api.Data;
using StandardInteriors.Api.Domain.Entities;
using StandardInteriors.Api.Dtos.Services;
using StandardInteriors.Api.Dtos.Settings;

namespace StandardInteriors.Api.Services.Implementations;

public interface IServiceCatalogService
{
    Task<IReadOnlyList<ServiceDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> CreateAsync(ServiceUpsertDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, ServiceUpsertDto dto, CancellationToken cancellationToken = default);
    Task<bool> ToggleAsync(int id, bool isActive, CancellationToken cancellationToken = default);
}

public interface IPaymentMethodService
{
    Task<IReadOnlyList<PaymentMethodDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> CreateAsync(PaymentMethodUpsertDto dto, string code, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, PaymentMethodUpsertDto dto, CancellationToken cancellationToken = default);
    Task<bool> ToggleAsync(int id, bool isActive, CancellationToken cancellationToken = default);
}

public interface ISettingsService
{
    Task<CompanySettingsDto?> GetAsync(CancellationToken cancellationToken = default);
    Task<CompanySettingsDto> SaveAsync(CompanySettingsDto setting, CancellationToken cancellationToken = default);
}

public sealed class ServiceCatalogService : IServiceCatalogService
{
    private readonly StandardInteriorsDbContext _db;

    public ServiceCatalogService(StandardInteriorsDbContext db) => _db = db;

    public async Task<IReadOnlyList<ServiceDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _db.Services.AsNoTracking().OrderBy(x => x.DisplayOrder).ThenBy(x => x.ServiceName)
            .Select(x => new ServiceDto(x.Id, x.ServiceCode, x.ServiceName, x.Description, x.DefaultUnit, x.DefaultPrice, x.DisplayOrder, x.IsActive))
            .ToListAsync(cancellationToken);

    public async Task<int> CreateAsync(ServiceUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var next = await _db.Services.CountAsync(cancellationToken) + 1;
        var entity = new ServiceMaster
        {
            ServiceCode = $"SRV-{next:000}",
            ServiceName = dto.ServiceName.Trim(),
            Description = dto.Description?.Trim(),
            DefaultUnit = dto.DefaultUnit?.Trim(),
            DefaultPrice = dto.DefaultPrice,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive
        };

        _db.Services.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(int id, ServiceUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Services.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity == null) return false;

        entity.ServiceName = dto.ServiceName.Trim();
        entity.Description = dto.Description?.Trim();
        entity.DefaultUnit = dto.DefaultUnit?.Trim();
        entity.DefaultPrice = dto.DefaultPrice;
        entity.DisplayOrder = dto.DisplayOrder;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ToggleAsync(int id, bool isActive, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Services.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity == null) return false;
        entity.IsActive = isActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class PaymentMethodService : IPaymentMethodService
{
    private readonly StandardInteriorsDbContext _db;

    public PaymentMethodService(StandardInteriorsDbContext db) => _db = db;

    public async Task<IReadOnlyList<PaymentMethodDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _db.PaymentMethods.AsNoTracking().OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
            .Select(x => new PaymentMethodDto(x.Id, x.Code, x.Name, x.IsActive, x.DisplayOrder))
            .ToListAsync(cancellationToken);

    public async Task<int> CreateAsync(PaymentMethodUpsertDto dto, string code, CancellationToken cancellationToken = default)
    {
        var entity = new PaymentMethod { Code = code, Name = dto.Name.Trim(), IsActive = dto.IsActive, DisplayOrder = dto.DisplayOrder };
        _db.PaymentMethods.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(int id, PaymentMethodUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _db.PaymentMethods.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity == null) return false;
        entity.Name = dto.Name.Trim();
        entity.IsActive = dto.IsActive;
        entity.DisplayOrder = dto.DisplayOrder;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ToggleAsync(int id, bool isActive, CancellationToken cancellationToken = default)
    {
        var entity = await _db.PaymentMethods.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity == null) return false;
        entity.IsActive = isActive;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class SettingsService : ISettingsService
{
    private readonly StandardInteriorsDbContext _db;

    public SettingsService(StandardInteriorsDbContext db) => _db = db;

    public async Task<CompanySettingsDto?> GetAsync(CancellationToken cancellationToken = default)
        => await _db.CompanySettings.AsNoTracking().OrderBy(x => x.Id)
            .Select(x => new CompanySettingsDto
            {
                Id = x.Id,
                CompanyName = x.CompanyName,
                Address = x.Address,
                Phone = x.Phone,
                Email = x.Email,
                EstimatePrefix = x.EstimatePrefix,
                PaymentPrefix = x.PaymentPrefix,
                Currency = x.Currency,
                DefaultEstimateNote = x.DefaultEstimateNote,
                LogoPath = x.LogoPath
            })
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<CompanySettingsDto> SaveAsync(CompanySettingsDto setting, CancellationToken cancellationToken = default)
    {
        var existing = await _db.CompanySettings.FirstOrDefaultAsync(x => x.Id == setting.Id, cancellationToken);
        if (existing == null)
        {
            existing = new CompanySetting
            {
                CompanyName = setting.CompanyName,
                Address = setting.Address,
                Phone = setting.Phone,
                Email = setting.Email,
                EstimatePrefix = setting.EstimatePrefix,
                PaymentPrefix = setting.PaymentPrefix,
                Currency = setting.Currency,
                DefaultEstimateNote = setting.DefaultEstimateNote,
                LogoPath = setting.LogoPath
            };
            _db.CompanySettings.Add(existing);
            await _db.SaveChangesAsync(cancellationToken);
        }

        existing.CompanyName = setting.CompanyName;
        existing.Address = setting.Address;
        existing.Phone = setting.Phone;
        existing.Email = setting.Email;
        existing.EstimatePrefix = setting.EstimatePrefix;
        existing.PaymentPrefix = setting.PaymentPrefix;
        existing.Currency = setting.Currency;
        existing.DefaultEstimateNote = setting.DefaultEstimateNote;
        existing.LogoPath = setting.LogoPath;
        existing.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        return new CompanySettingsDto
        {
            Id = existing.Id,
            CompanyName = existing.CompanyName,
            Address = existing.Address,
            Phone = existing.Phone,
            Email = existing.Email,
            EstimatePrefix = existing.EstimatePrefix,
            PaymentPrefix = existing.PaymentPrefix,
            Currency = existing.Currency,
            DefaultEstimateNote = existing.DefaultEstimateNote,
            LogoPath = existing.LogoPath
        };
    }
}
