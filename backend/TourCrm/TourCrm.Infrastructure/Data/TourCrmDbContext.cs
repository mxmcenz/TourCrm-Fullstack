using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TourCrm.Core.Abstractions;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Client;
using TourCrm.Core.Entities.Deals;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Entities.Leads;
using TourCrm.Core.Entities.Roles;
using TourCrm.Core.Entities.Tariffs;

namespace TourCrm.Infrastructure.Data;

public class TourCrmDbContext : DbContext
{
    private readonly int? _companyId;
    public int? CurrentCompanyId => _companyId;

    public TourCrmDbContext(DbContextOptions<TourCrmDbContext> options) : base(options)
    {
        _companyId = null;
    }

    public TourCrmDbContext(DbContextOptions<TourCrmDbContext> options, ICompanyContext companyContext) : base(options)
    {
        _companyId = companyContext?.CompanyId;
    }

    public DbSet<User> Users { get; init; } = null!;
    public DbSet<Employee> Employees { get; init; } = null!;
    public DbSet<Tourist> Tourists { get; init; } = null!;
    public DbSet<Lead> Leads { get; init; } = null!;
    public DbSet<LeadSelection> LeadSelections { get; init; } = null!;
    public DbSet<LeadHistory> LeadHistories { get; init; } = null!;
    public DbSet<Deal> Deals { get; init; } = null!;
    public DbSet<DealStatus> DealStatuses { get; init; } = null!;
    public DbSet<DealHistory> DealHistories { get; init; } = null!;
    public DbSet<Country> Countries { get; init; } = null!;
    public DbSet<Company> Companies { get; init; } = null!;
    public DbSet<LegalEntity> LegalEntities { get; init; } = null!;
    public DbSet<Office> Offices { get; init; } = null!;
    public DbSet<Role> Roles { get; init; } = null!;
    public DbSet<RolePermission> RolePermissions { get; init; } = null!;
    public DbSet<UserRole> UserRoles { get; init; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; init; } = null!;
    public DbSet<ServiceType> ServiceTypes { get; init; } = null!;
    public DbSet<City> Cities { get; init; } = null!;
    public DbSet<NumberType> NumberTypes { get; init; } = null!;
    public DbSet<AccommodationType> AccommodationTypes { get; init; } = null!;
    public DbSet<MealType> MealTypes { get; init; } = null!;
    public DbSet<Partner> Partners { get; init; } = null!;
    public DbSet<TourOperator> TourOperators { get; init; } = null!;
    public DbSet<Citizenship> Citizenships { get; init; } = null!;
    public DbSet<PartnerMark> PartnerMarks { get; init; } = null!;
    public DbSet<PartnerType> PartnerTypes { get; init; } = null!;
    public DbSet<Client> Clients { get; init; } = null!;
    public DbSet<Passport> Passports { get; init; } = null!;
    public DbSet<IdentityDocument> IdentityDocuments { get; init; } = null!;
    public DbSet<BirthCertificate> BirthCertificates { get; init; } = null!;
    public DbSet<InsurancePolicy> InsurancePolicies { get; init; } = null!;
    public DbSet<VisaRecord> VisaRecords { get; init; } = null!;
    public DbSet<AuditLog> AuditLogs { get; init; } = null!;
    public DbSet<Hotel> Hotels { get; init; } = null!;
    public DbSet<Currency> Currencies { get; init; } = null!;
    public DbSet<LeadStatus> LeadStatuses { get; init; } = null!;
    public DbSet<LeadSource> LeadSources { get; init; } = null!;
    public DbSet<LeadRequestType> LeadRequestTypes { get; init; } = null!;
    public DbSet<Label> Labels { get; init; } = null!;
    public DbSet<VisaType> VisaTypes { get; init; } = null!;
    public DbSet<DealServiceItem> DealServices { get; init; } = null!;
    public DbSet<DealClientPayment> DealClientPayments { get; init; } = null!;
    public DbSet<DealPartnerPayment> DealPartnerPayments { get; init; } = null!;
    public DbSet<Tariff> Tariffs { get; init; } = null!;
    public DbSet<TariffPermission> TariffPermissions { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var utcDateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Employee>().ToTable("Employees");
        modelBuilder.Entity<Tourist>().ToTable("Tourists");

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Company>(e =>
        {
            e.ToTable("Companies");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(150);
            e.Property(x => x.OwnerUserId).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();
            e.HasIndex(x => x.OwnerUserId).IsUnique();

            e.HasOne(x => x.LegalEntity)
                .WithOne()
                .HasForeignKey<Company>(x => x.LegalEntityId)
                .OnDelete(DeleteBehavior.SetNull);
            
            e.HasOne(x => x.Tariff)
                .WithMany(t => t.Companies)
                .HasForeignKey(x => x.TariffId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<LegalEntity>(e =>
        {
            e.ToTable("LegalEntities");
            e.HasKey(x => x.Id);

            e.Property(x => x.Name).IsRequired().HasMaxLength(150);
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();
            e.Property(x => x.NameRu).HasMaxLength(150);
            e.Property(x => x.NameEn).HasMaxLength(150);
            e.Property(x => x.NameFull).HasMaxLength(200);
            e.HasOne(x => x.CountryRef)
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.CityRef)
                .WithMany()
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(x => x.LegalAddress).HasMaxLength(300);
            e.Property(x => x.ActualAddress).HasMaxLength(300);
            e.Property(x => x.DirectorFio).HasMaxLength(150);
            e.Property(x => x.DirectorFioGen).HasMaxLength(150);
            e.Property(x => x.DirectorPost).HasMaxLength(100).HasDefaultValue("Директор");
            e.Property(x => x.DirectorPostGen).HasMaxLength(100).HasDefaultValue("Директора");
            e.Property(x => x.DirectorBasis).HasMaxLength(100).HasDefaultValue("Устава");
            e.Property(x => x.CfoFio).HasMaxLength(150);
            e.Property(x => x.Phones).HasMaxLength(300);
            e.Property(x => x.Website).HasMaxLength(200);
            e.Property(x => x.Email).HasMaxLength(150);
            e.Property(x => x.BinIin).HasMaxLength(20);
            e.Property(x => x.BankDetailsJson);
            e.HasOne(x => x.Company)
                .WithMany(c => c.LegalEntities)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.CompanyId, x.Name })
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false AND \"Name\" IS NOT NULL AND \"Name\" <> ''");
        });

        modelBuilder.Entity<Office>(b =>
        {
            b.ToTable("Offices");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(120);
            b.Property(x => x.Address).HasMaxLength(300);
            b.Property(x => x.Phone).HasMaxLength(32);
            b.Property(x => x.Email).HasMaxLength(150);

            b.HasOne(x => x.LegalEntity)
                .WithMany(le => le.Offices)
                .HasForeignKey(x => x.LegalEntityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Employee>(e =>
        {
            e.HasOne(x => x.Office)
                .WithMany(o => o.Employees)
                .HasForeignKey(x => x.OfficeId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.LegalEntity)
                .WithMany()
                .HasForeignKey(x => x.LegalEntityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Lead>(e =>
        {
            e.ToTable("Leads");
            e.HasKey(x => x.Id);
            e.Property(x => x.LeadNumber).HasMaxLength(32).IsRequired();
            e.Property(x => x.ManagerFullName).HasMaxLength(200);
            e.Property(x => x.CustomerType).HasMaxLength(32).IsRequired();
            e.Property(x => x.CustomerFirstName).HasMaxLength(120);
            e.Property(x => x.CustomerLastName).HasMaxLength(120);
            e.Property(x => x.CustomerMiddleName).HasMaxLength(120);
            e.Property(x => x.Phone).HasMaxLength(64);
            e.Property(x => x.Email).HasMaxLength(150);
            e.Property(x => x.Country).HasMaxLength(120);
            e.Property(x => x.Accommodation).HasMaxLength(64);
            e.Property(x => x.MealPlan).HasMaxLength(32);
            e.Property(x => x.Note).HasMaxLength(2000);
            e.Property(x => x.DesiredArrival).HasColumnType("date");
            e.Property(x => x.DesiredDeparture).HasColumnType("date");
            e.Property(x => x.DocsPackageDate).HasColumnType("date");
            e.Property(x => x.Budget).HasColumnType("numeric(18,2)");
            e.Property(x => x.CreatedByUserId).HasMaxLength(64).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();
            e.HasOne(l => l.Office)
                .WithMany(o => o.Leads)
                .HasForeignKey(l => l.OfficeId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(l => l.Manager)
                .WithMany()
                .HasForeignKey(l => l.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(l => l.LeadStatus)
                .WithMany()
                .HasForeignKey(l => l.LeadStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(l => l.Source)
                .WithMany()
                .HasForeignKey(l => l.SourceId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(l => l.RequestType)
                .WithMany()
                .HasForeignKey(l => l.RequestTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(l => l.Company)
                .WithMany()
                .HasForeignKey(l => l.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            if (_companyId != null)
                e.HasQueryFilter(x =>
                    x.CompanyId == _companyId && !x.Office.IsDeleted && !x.Office.LegalEntity.IsDeleted);
            else
                e.HasQueryFilter(x => !x.Office.IsDeleted && !x.Office.LegalEntity.IsDeleted);
        });

        modelBuilder.Entity<LeadLabel>(b =>
        {
            b.ToTable("LeadLabels");
            b.HasKey(x => new { x.LeadId, x.LabelId });

            b.HasOne(x => x.Lead)
                .WithMany(l => l.LeadLabels)
                .HasForeignKey(x => x.LeadId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Label)
                .WithMany()
                .HasForeignKey(x => x.LabelId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            if (_companyId != null)
                b.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<LeadSelection>(e =>
        {
            e.ToTable("LeadSelections");
            e.HasKey(x => x.Id);

            e.HasOne(ls => ls.Lead)
                .WithMany(l => l.Selections)
                .HasForeignKey(ls => ls.LeadId)
                .OnDelete(DeleteBehavior.Cascade);

            e.Property(x => x.DepartureCity).HasMaxLength(120).IsRequired();
            e.Property(x => x.Country).HasMaxLength(120).IsRequired();
            e.Property(x => x.City).HasMaxLength(120).IsRequired();
            e.Property(x => x.Hotel).HasMaxLength(200);
            e.Property(x => x.RoomType).HasMaxLength(120);
            e.Property(x => x.Accommodation).HasMaxLength(64).IsRequired();
            e.Property(x => x.MealPlan).HasMaxLength(32).IsRequired();
            e.Property(x => x.StartDate).HasColumnType("date");
            e.Property(x => x.Link).HasMaxLength(500);
            e.Property(x => x.Note).HasMaxLength(2000);
            e.Property(x => x.PartnerName).HasMaxLength(200);
            e.Property(x => x.Currency).HasMaxLength(8).IsRequired();
            e.Property(x => x.Price).HasColumnType("numeric(18,2)");
            e.Property(x => x.CreatedByUserId).HasMaxLength(64).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<LeadHistory>(b =>
        {
            b.ToTable("LeadHistories");
            b.HasKey(x => x.Id);
            b.Property(x => x.Action).HasMaxLength(32);
            b.Property(x => x.ActorUserId).HasMaxLength(64);
            b.Property(x => x.ActorFullName).HasMaxLength(200);

            b.HasOne(x => x.Lead)
                .WithMany()
                .HasForeignKey(x => x.LeadId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            if (_companyId != null)
                b.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<Deal>(e =>
        {
            e.ToTable("Deals");
            e.HasKey(d => d.Id);
            e.Property(d => d.DealNumber).HasMaxLength(32).IsRequired();
            e.Property(d => d.InternalNumber).HasMaxLength(32);
            e.Property(d => d.StatusId).IsRequired();
            e.HasOne(d => d.Status).WithMany().HasForeignKey(d => d.StatusId).OnDelete(DeleteBehavior.Restrict);
            e.Property(d => d.TourName).HasMaxLength(300).IsRequired();
            e.Property(d => d.Price).HasColumnType("numeric(18,2)");
            e.Property(d => d.BookingNumbers).HasMaxLength(300);
            e.Property(d => d.Note).HasMaxLength(2000);
            e.Property(d => d.StartDate).HasColumnType("date");
            e.Property(d => d.EndDate).HasColumnType("date");
            e.Property(d => d.ClientPaymentDeadline).HasColumnType("date");
            e.Property(d => d.PartnerPaymentDeadline).HasColumnType("date");
            e.Property(d => d.DocsPackageDate).HasColumnType("date");
            e.Property(d => d.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("timezone('utc', now())")
                .ValueGeneratedOnAdd()
                .IsRequired();

            e.Property(d => d.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .ValueGeneratedOnAddOrUpdate();
            e.HasOne(d => d.Lead).WithMany().HasForeignKey(d => d.LeadId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.Manager).WithMany().HasForeignKey(d => d.ManagerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.Company).WithMany().HasForeignKey(d => d.CompanyId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.IssuerLegalEntity).WithMany().HasForeignKey(d => d.IssuerLegalEntityId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.RequestType).WithMany().HasForeignKey(d => d.RequestTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.Source).WithMany().HasForeignKey(d => d.SourceId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.TourOperator).WithMany().HasForeignKey(d => d.TourOperatorId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(d => new { d.CompanyId, d.DealNumber })
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false AND \"DealNumber\" IS NOT NULL AND \"DealNumber\" <> ''");

            if (CurrentCompanyId != null)
                e.HasQueryFilter(d => d.CompanyId == CurrentCompanyId && !d.IsDeleted);
            else
                e.HasQueryFilter(d => !d.IsDeleted);
        });

        modelBuilder.Entity<Deal>()
            .HasMany(d => d.Customers)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "DealCustomers",
                right => right.HasOne<Client>()
                    .WithMany()
                    .HasForeignKey("ClientId")
                    .OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<Deal>()
                    .WithMany()
                    .HasForeignKey("DealId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("DealCustomers");
                    join.HasKey("DealId", "ClientId");
                });

        modelBuilder.Entity<Deal>()
            .HasMany(d => d.Tourists)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "DealTourists",
                right => right.HasOne<Client>()
                    .WithMany()
                    .HasForeignKey("ClientId")
                    .OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<Deal>()
                    .WithMany()
                    .HasForeignKey("DealId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("DealTourists");
                    join.HasKey("DealId", "ClientId");
                });

        modelBuilder.Entity<DealStatus>(e =>
        {
            e.ToTable("DealStatuses");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(128);
            e.Property(x => x.IsFinal).IsRequired();
            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<DealHistory>(b =>
        {
            b.ToTable("DealHistories");
            b.HasKey(x => x.Id);

            b.Property(x => x.Action).HasMaxLength(32);
            b.Property(x => x.ActorUserId).HasMaxLength(64);
            b.Property(x => x.ActorFullName).HasMaxLength(200);
            b.Property(x => x.Note).HasMaxLength(2000);
            b.Property(x => x.CreatedAt).IsRequired();

            b.HasOne(x => x.Deal)
                .WithMany()
                .HasForeignKey(x => x.DealId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            if (_companyId != null)
                b.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<Role>(e =>
        {
            e.ToTable("Roles");
            e.HasKey(r => r.Id);
            e.Property(r => r.Name).IsRequired().HasMaxLength(100);
            e.HasOne(r => r.Company)
                .WithMany(c => c.Roles)
                .HasForeignKey(r => r.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        });

        modelBuilder.Entity<RolePermission>(e =>
        {
            e.ToTable("RolePermissions");
            e.HasKey(rp => new { rp.RoleId, rp.PermissionKey });
            e.Property(rp => rp.PermissionKey).IsRequired().HasMaxLength(200);

            e.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserRole>(e =>
        {
            e.ToTable("UserRoles");
            e.HasKey(ur => new { ur.UserId, ur.RoleId });

            e.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Client>(e =>
        {
            e.ToTable("Clients");
            e.Property(x => x.RowVersion).IsConcurrencyToken();
            e.Property(x => x.DiscountPercent).HasPrecision(5, 2);
            e.HasIndex(x => new { x.CompanyId, x.Email })
                .HasFilter("\"Email\" IS NOT NULL")
                .IsUnique();
            e.HasIndex(x => new { x.CompanyId, x.PhoneE164 })
                .HasFilter("\"PhoneE164\" IS NOT NULL")
                .IsUnique();
            e.HasOne(x => x.Passport)
                .WithOne(p => p.Client)
                .HasForeignKey<Passport>(p => p.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.IdentityDocument)
                .WithOne(d => d.Client)
                .HasForeignKey<IdentityDocument>(d => d.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.BirthCertificate)
                .WithOne(b => b.Client)
                .HasForeignKey<BirthCertificate>(b => b.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Visas)
                .WithOne(v => v.Client)
                .HasForeignKey(v => v.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Insurances)
                .WithOne(i => i.Client)
                .HasForeignKey(i => i.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId && !x.IsDeleted);
            else
                e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Passport>(e =>
        {
            e.ToTable("Passports");
            e.Property(x => x.RowVersion).IsConcurrencyToken();
            e.HasIndex(x => x.ClientId).IsUnique();
            e.HasIndex(x => new { x.CompanyId, x.SerialNumber })
                .HasFilter("\"SerialNumber\" IS NOT NULL AND \"SerialNumber\" <> ''")
                .IsUnique();
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<IdentityDocument>(e =>
        {
            e.ToTable("IdentityDocuments");
            e.Property(x => x.RowVersion).IsConcurrencyToken();
            e.HasIndex(x => x.ClientId).IsUnique();
            e.HasIndex(x => new { x.CompanyId, x.DocumentNumber })
                .HasFilter("\"DocumentNumber\" IS NOT NULL AND \"DocumentNumber\" <> ''")
                .IsUnique();
            e.HasIndex(x => new { x.CompanyId, x.PersonalNumber })
                .HasFilter("\"PersonalNumber\" IS NOT NULL AND \"PersonalNumber\" <> ''")
                .IsUnique();
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<BirthCertificate>(e =>
        {
            e.ToTable("BirthCertificates");
            e.Property(x => x.RowVersion).IsConcurrencyToken();
            e.HasIndex(x => x.ClientId).IsUnique();
            e.HasIndex(x => new { x.CompanyId, x.SerialNumber })
                .HasFilter("\"SerialNumber\" IS NOT NULL AND \"SerialNumber\" <> ''")
                .IsUnique();
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<InsurancePolicy>(e =>
        {
            e.ToTable("InsurancePolicies");
            e.Property(x => x.RowVersion).IsConcurrencyToken();
            e.HasIndex(x => new { x.CompanyId, x.ClientId, x.ExpireDate });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<VisaRecord>(e =>
        {
            e.ToTable("VisaRecords");
            e.Property(x => x.RowVersion).IsConcurrencyToken();
            e.HasIndex(x => new { x.CompanyId, x.ClientId, x.ExpireDate });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<AuditLog>(e =>
        {
            e.ToTable("AuditLogs");
            e.HasKey(x => x.Id);
            e.Property(x => x.Action).HasConversion<string>();
            e.HasIndex(x => new { x.CompanyId, x.Entity, x.EntityId, x.AtUtc });
        });

        modelBuilder.Entity<Country>(e =>
        {
            e.ToTable("Countries");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(150);

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<City>(e =>
        {
            e.ToTable("Cities");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(150);

            e.HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.CountryId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<Hotel>(e =>
        {
            e.ToTable("Hotels");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Stars).IsRequired(false);

            e.HasOne<City>()
                .WithMany()
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.CityId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<NumberType>(e =>
        {
            e.ToTable("NumberTypes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(120);

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<AccommodationType>(e =>
        {
            e.ToTable("AccommodationTypes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(120);

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<MealType>(e =>
        {
            e.ToTable("MealTypes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(120);

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<Currency>(e =>
        {
            e.ToTable("Currencies");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(60);

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<Partner>(e =>
        {
            e.ToTable("Partners");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<PartnerType>(e =>
        {
            e.ToTable("PartnerTypes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(120);

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<ServiceType>(e =>
        {
            e.ToTable("ServiceTypes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(120);

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<LeadStatus>(e =>
        {
            e.ToTable("LeadStatuses");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(128);

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<LeadSource>(e =>
        {
            e.ToTable("LeadSources");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(128);

            e.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<LeadRequestType>(e =>
        {
            e.ToTable("LeadRequestTypes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(128);

            e.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<Label>(e =>
        {
            e.ToTable("Labels");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(128);
            e.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<VisaType>(e =>
        {
            e.ToTable("VisaTypes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(120);

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            if (_companyId != null)
                e.HasQueryFilter(x => x.CompanyId == _companyId);
        });

        modelBuilder.Entity<DealServiceItem>(e =>
        {
            e.ToTable("DealServices");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Note).HasMaxLength(2000);

            e.HasOne(x => x.Deal)
                .WithMany(d => d.Services)
                .HasForeignKey(x => x.DealId)
                .OnDelete(DeleteBehavior.Cascade);

            if (_companyId != null)
                e.HasQueryFilter(x => x.Deal.CompanyId == _companyId);
        });

        modelBuilder.Entity<DealClientPayment>(e =>
        {
            e.ToTable("DealClientPayments");
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).IsRequired().HasMaxLength(200);
            e.Property(x => x.Amount).HasColumnType("numeric(18,2)");

            e.HasOne(x => x.Deal)
                .WithMany(d => d.ClientPayments)
                .HasForeignKey(x => x.DealId)
                .OnDelete(DeleteBehavior.Cascade);

            if (_companyId != null)
                e.HasQueryFilter(x => x.Deal.CompanyId == _companyId);
        });

        modelBuilder.Entity<DealPartnerPayment>(e =>
        {
            e.ToTable("DealPartnerPayments");
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).IsRequired().HasMaxLength(200);
            e.Property(x => x.Amount).HasColumnType("numeric(18,2)");

            e.HasOne(x => x.Deal)
                .WithMany(d => d.PartnerPayments)
                .HasForeignKey(x => x.DealId)
                .OnDelete(DeleteBehavior.Cascade);

            if (_companyId != null)
                e.HasQueryFilter(x => x.Deal.CompanyId == _companyId);
        });

        modelBuilder.Entity<TourOperator>(e =>
        {
            e.ToTable("TourOperators");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            e.HasQueryFilter(x => CurrentCompanyId != null && x.CompanyId == CurrentCompanyId);
        });

        modelBuilder.Entity<Citizenship>(e =>
        {
            e.ToTable("Citizenships");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(120);

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            e.HasQueryFilter(x => CurrentCompanyId != null && x.CompanyId == CurrentCompanyId);
        });

        modelBuilder.Entity<PartnerMark>(e =>
        {
            e.ToTable("PartnerMarks");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(120);

            e.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            e.HasQueryFilter(x => CurrentCompanyId != null && x.CompanyId == CurrentCompanyId);
        });
        
        modelBuilder.Entity<Tariff>(e =>
        {
            e.ToTable("Tariffs");
            e.HasKey(x => x.Id);

            e.Property(x => x.Name).IsRequired().HasMaxLength(120);
            e.Property(x => x.MinEmployees).IsRequired();
            e.Property(x => x.MaxEmployees).IsRequired();
            e.Property(x => x.MonthlyPrice).HasColumnType("numeric(18,2)").IsRequired();
            e.Property(x => x.HalfYearPrice).HasColumnType("numeric(18,2)");
            e.Property(x => x.YearlyPrice).HasColumnType("numeric(18,2)");
            e.Property(x => x.IsPublic).IsRequired().HasDefaultValue(true);
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
        });
        
        modelBuilder.Entity<TariffPermission>(e =>
        {
            e.ToTable("TariffPermissions");
            e.HasKey(x => x.Id);
            e.Property(x => x.PermissionKey).IsRequired().HasMaxLength(200);
            e.Property(x => x.IsGranted).IsRequired();
            e.HasOne(x => x.Tariff)
                .WithMany(t => t.Permissions)
                .HasForeignKey(x => x.TariffId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.TariffId, x.PermissionKey }).IsUnique();
        });

    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
        {
            var companyProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CompanyId");
            if (companyProp is not null && (companyProp.CurrentValue is null or 0) && _companyId.HasValue)
                companyProp.CurrentValue = _companyId.Value;
        }

        return base.SaveChangesAsync(ct);
    }

    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
        {
            var companyProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CompanyId");
            if (companyProp is not null && (companyProp.CurrentValue is null or 0) && _companyId.HasValue)
                companyProp.CurrentValue = _companyId.Value;
        }

        return base.SaveChanges();
    }
}