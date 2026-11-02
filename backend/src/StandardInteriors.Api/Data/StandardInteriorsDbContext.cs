using Microsoft.EntityFrameworkCore;
using StandardInteriors.Api.Domain.Entities;

namespace StandardInteriors.Api.Data;

public sealed class StandardInteriorsDbContext : DbContext
{
    public StandardInteriorsDbContext(DbContextOptions<StandardInteriorsDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<ServiceMaster> Services => Set<ServiceMaster>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Estimate> Estimates => Set<Estimate>();
    public DbSet<EstimateSection> EstimateSections => Set<EstimateSection>();
    public DbSet<EstimateItem> EstimateItems => Set<EstimateItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<CompanySetting> CompanySettings => Set<CompanySetting>();
    public DbSet<EstimateStatusHistory> EstimateStatusHistory => Set<EstimateStatusHistory>();
    public DbSet<WorkStatusHistory> WorkStatusHistories => Set<WorkStatusHistory>();
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(x => x.CustomerCode).IsUnique();
            entity.HasIndex(x => x.Phone);
            entity.HasIndex(x => x.CreatedByUserId);
            entity.HasIndex(x => x.UpdatedByUserId);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(50).IsRequired();
            entity.Property(x => x.CustomerCode).HasMaxLength(30).IsRequired();
        });

        modelBuilder.Entity<ServiceMaster>(entity =>
        {
            entity.HasIndex(x => x.ServiceCode).IsUnique();
            entity.HasIndex(x => x.ServiceName);
            entity.Property(x => x.ServiceName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.ServiceCode).HasMaxLength(30).IsRequired();
            entity.Property(x => x.DefaultPrice).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(30).IsRequired();
        });

        modelBuilder.Entity<Estimate>(entity =>
        {
            entity.HasIndex(x => x.EstimateNumber).IsUnique();
            entity.HasIndex(x => x.CustomerId);
            entity.HasIndex(x => x.EstimateDate);
            entity.HasIndex(x => x.CreatedByUserId);
            entity.HasIndex(x => x.UpdatedByUserId);
            entity.Property(x => x.EstimateNumber).HasMaxLength(30).IsRequired();
            entity.Property(x => x.ManualOverallAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.CalculatedTotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.CalculatedReceivedAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.CalculatedBalance).HasColumnType("decimal(18,2)");
            entity.Property(x => x.EstimateNote).HasMaxLength(2000);
            entity.Property(x => x.Notes).HasMaxLength(2000);
            entity.HasOne(x => x.Customer).WithMany(x => x.Estimates).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            entity.Property(x => x.WorkStatusDescription).HasMaxLength(2000);
            entity.Property(x => x.StopReason).HasMaxLength(2000);
            entity.Property(x => x.ResumeNote).HasMaxLength(2000);
            entity.HasIndex(x => x.WorkStatus);
        });

        modelBuilder.Entity<EstimateSection>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(250).IsRequired();
            entity.Property(x => x.SectionNumber).HasMaxLength(20).IsRequired();
            entity.Property(x => x.ManualSectionAmount).HasColumnType("decimal(18,2)");
            entity.HasOne(x => x.Estimate).WithMany(x => x.Sections).HasForeignKey(x => x.EstimateId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EstimateItem>(entity =>
        {
            entity.Property(x => x.Description).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Quantity).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Rate).HasColumnType("decimal(18,2)");
            entity.Property(x => x.ManualAmount).HasColumnType("decimal(18,2)");
            entity.HasOne(x => x.EstimateSection).WithMany(x => x.Items).HasForeignKey(x => x.EstimateSectionId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Service).WithMany(x => x.EstimateItems).HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(x => x.PaymentNumber).IsUnique();
            entity.HasIndex(x => x.PaymentDate);
            entity.HasIndex(x => x.EstimateId);
            entity.HasIndex(x => x.CreatedByUserId);
            entity.HasIndex(x => x.UpdatedByUserId);
            entity.HasIndex(x => x.VoidedByUserId);
            entity.Property(x => x.PaymentNumber).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.TransactionReference).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(2000);
            entity.HasOne(x => x.Customer).WithMany(x => x.Payments).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Estimate).WithMany(x => x.Payments).HasForeignKey(x => x.EstimateId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.PaymentMethod).WithMany(x => x.Payments).HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasIndex(x => x.Username).IsUnique();
            entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
            entity.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<Customer>().HasOne<AppUser>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Customer>().HasOne<AppUser>().WithMany().HasForeignKey(x => x.UpdatedByUserId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Estimate>().HasOne<AppUser>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Estimate>().HasOne<AppUser>().WithMany().HasForeignKey(x => x.UpdatedByUserId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Payment>().HasOne<AppUser>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Payment>().HasOne<AppUser>().WithMany().HasForeignKey(x => x.UpdatedByUserId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Payment>().HasOne<AppUser>().WithMany().HasForeignKey(x => x.VoidedByUserId).OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CompanySetting>(entity =>
        {
            entity.Property(x => x.CompanyName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.EstimatePrefix).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PaymentPrefix).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Currency).HasMaxLength(10).IsRequired();
            entity.Property(x => x.DefaultEstimateNote).HasMaxLength(2000).IsRequired();
        });

        modelBuilder.Entity<EstimateStatusHistory>(entity =>
        {
            entity.HasOne(x => x.Estimate).WithMany(x => x.StatusHistory).HasForeignKey(x => x.EstimateId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WorkStatusHistory>(entity =>
        {
            entity.HasIndex(x => new { x.EstimateId, x.ChangedAt });
            entity.Property(x => x.Description).HasMaxLength(2000);
            entity.Property(x => x.StopReason).HasMaxLength(2000);
            entity.HasOne(x => x.Estimate).WithMany(x => x.WorkStatusHistories).HasForeignKey(x => x.EstimateId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.ChangedByUser).WithMany().HasForeignKey(x => x.ChangedByUserId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<PaymentMethod>().HasData(
            new PaymentMethod { Id = 1, Code = "CASH", Name = "Cash", DisplayOrder = 1, IsActive = true },
            new PaymentMethod { Id = 2, Code = "GPAY", Name = "GPay", DisplayOrder = 2, IsActive = true },
            new PaymentMethod { Id = 3, Code = "PHONEPE", Name = "PhonePe", DisplayOrder = 3, IsActive = true },
            new PaymentMethod { Id = 4, Code = "UPI", Name = "UPI", DisplayOrder = 4, IsActive = true },
            new PaymentMethod { Id = 5, Code = "BANK", Name = "Bank Transfer", DisplayOrder = 5, IsActive = true },
            new PaymentMethod { Id = 6, Code = "CHEQUE", Name = "Cheque", DisplayOrder = 6, IsActive = true },
            new PaymentMethod { Id = 7, Code = "CARD", Name = "Card", DisplayOrder = 7, IsActive = true },
            new PaymentMethod { Id = 8, Code = "OTHER", Name = "Other", DisplayOrder = 8, IsActive = true }
        );

        modelBuilder.Entity<ServiceMaster>().HasData(
            new ServiceMaster { Id = 1, ServiceCode = "SRV-001", ServiceName = "Ceiling", DisplayOrder = 1, IsActive = true },
            new ServiceMaster { Id = 2, ServiceCode = "SRV-002", ServiceName = "Electrical Work", DisplayOrder = 2, IsActive = true },
            new ServiceMaster { Id = 3, ServiceCode = "SRV-003", ServiceName = "Putty", DisplayOrder = 3, IsActive = true },
            new ServiceMaster { Id = 4, ServiceCode = "SRV-004", ServiceName = "Painting", DisplayOrder = 4, IsActive = true },
            new ServiceMaster { Id = 5, ServiceCode = "SRV-005", ServiceName = "Lights", DisplayOrder = 5, IsActive = true },
            new ServiceMaster { Id = 6, ServiceCode = "SRV-006", ServiceName = "Curtains", DisplayOrder = 6, IsActive = true },
            new ServiceMaster { Id = 7, ServiceCode = "SRV-007", ServiceName = "Wardrobe & Dressing Room", DisplayOrder = 7, IsActive = true },
            new ServiceMaster { Id = 8, ServiceCode = "SRV-008", ServiceName = "Bed Set", DisplayOrder = 8, IsActive = true },
            new ServiceMaster { Id = 9, ServiceCode = "SRV-009", ServiceName = "Glass Partition", DisplayOrder = 9, IsActive = true }
        );

        modelBuilder.Entity<CompanySetting>().HasData(new CompanySetting
        {
            Id = 1,
            CompanyName = "STANDARD INTERIORS",
            EstimatePrefix = "EST",
            PaymentPrefix = "PAY",
            Currency = "INR",
            DefaultEstimateNote = "This estimate is prepared for the listed interior works. Final cost may vary based on site measurements, material/brand selection, design changes and additional work approved during execution.",
            LogoPath = "..\\..\\..\\..\\..\\..\\assets\\logo\\standard-interiors-logo.png"
        });
    }
}
