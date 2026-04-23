using System.Text.Encodings.Web;
using System.Text.Json;
using TourCrm.Application.DTOs.Clients;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Entities.Client;
using TourCrm.Core.Enums;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services;

public class ClientService(IUnitOfWork uow, IAuditLogger audit) : IClientService
{
    public async Task<(IReadOnlyList<ClientListItemDto> items, int total)> SearchAsync(
        int companyId, string? query, int page, int pageSize, bool includeDeleted, CancellationToken ct)
    {
        var (entities, total) = await uow.Clients
            .SearchWithDetailsAsync(companyId, query, page, pageSize, includeDeleted, ct);

        return (entities.Select(MapList).ToList(), total);
    }

    public async Task<(IReadOnlyList<ClientListItemDto> items, int total)> SearchDeletedAsync(
        int companyId, string? query, int page, int pageSize, CancellationToken ct)
    {
        var (entities, total) = await uow.Clients.SearchDeletedAsync(companyId, query, page, pageSize, ct);
        return (entities.Select(MapList).ToList(), total);
    }

    public Task<ClientDetailsDto?> GetAsync(int id, int companyId, bool includeDeleted, CancellationToken ct)
    {
        return GetInternalAsync(id, companyId, includeDeleted, ct);

        async Task<ClientDetailsDto?> GetInternalAsync(int i, int c, bool incDel, CancellationToken t)
        {
            var e = await uow.Clients.GetWithDetailsAsync(c, i, incDel, t);
            return e is null ? null : MapDetails(e);
        }
    }

    public async Task<ClientDetailsDto> CreateAsync(int companyId, int? userId, CreateClientDto dto,
        CancellationToken ct)
    {
        var e = new Client
        {
            CompanyId = companyId,
            ClientType = dto.ClientType,
            ManagerId = dto.ManagerId,
            FirstName = dto.FirstName?.Trim() ?? "",
            LastName = dto.LastName?.Trim() ?? "",
            MiddleName = dto.MiddleName?.Trim(),
            FirstNameGenitive = dto.FirstNameGenitive,
            LastNameGenitive = dto.LastNameGenitive,
            MiddleNameGenitive = dto.MiddleNameGenitive,
            BirthDay = dto.BirthDay,
            Gender = dto.Gender,
            PhoneE164 = dto.PhoneE164?.Trim(),
            Email = NormalizeEmail(dto.Email),
            IsSubscribedToMailing = dto.IsSubscribedToMailing,
            IsEmailNotificationEnabled = dto.IsEmailNotificationEnabled,
            ReferredBy = dto.ReferredBy,
            Note = dto.Note,
            DiscountPercent = dto.DiscountPercent,
            IsTourist = dto.IsTourist
        };

        if (dto.Passport is { } p)
            e.Passport = new Passport
            {
                CompanyId = companyId,
                FirstNameLatin = p.FirstNameLatin,
                LastNameLatin = p.LastNameLatin,
                SerialNumber = p.SerialNumber,
                IssueDate = p.IssueDate,
                ExpireDate = p.ExpireDate,
                IssuingAuthority = p.IssuingAuthority
            };

        if (dto.IdentityDocument is { } id)
            e.IdentityDocument = new IdentityDocument
            {
                CompanyId = companyId,
                CitizenshipCountryId = id.CitizenshipCountryId,
                ResidenceCountryId = id.ResidenceCountryId,
                ResidenceCityId = id.ResidenceCityId,
                BirthPlace = id.BirthPlace,
                SerialNumber = id.SerialNumber,
                IssuedBy = id.IssuedBy,
                IssueDate = id.IssueDate,
                DocumentNumber = id.DocumentNumber,
                PersonalNumber = id.PersonalNumber,
                RegistrationAddress = id.RegistrationAddress,
                ResidentialAddress = id.ResidentialAddress,
                MotherFullName = id.MotherFullName,
                FatherFullName = id.FatherFullName,
                ContactInfo = id.ContactInfo
            };

        if (dto.BirthCertificate is { } bc)
            e.BirthCertificate = new BirthCertificate
            {
                CompanyId = companyId,
                SerialNumber = bc.SerialNumber,
                IssuedBy = bc.IssuedBy,
                IssueDate = bc.IssueDate
            };

        if (dto.Insurances is { Count: > 0 })
            e.Insurances = dto.Insurances!
                .Select(i => new InsurancePolicy
                {
                    CompanyId = companyId,
                    IssueDate = i.IssueDate,
                    ExpireDate = i.ExpireDate,
                    CountryId = i.CountryId,
                    Note = i.Note
                }).ToList();

        if (dto.Visas is { Count: > 0 })
            e.Visas = dto.Visas!
                .Select(v => new VisaRecord
                {
                    CompanyId = companyId,
                    IssueDate = v.IssueDate,
                    ExpireDate = v.ExpireDate,
                    CountryId = v.CountryId,
                    IsSchengen = v.IsSchengen,
                    Note = v.Note
                }).ToList();

        await uow.Clients.AddAsync(e, ct);
        await uow.SaveChangesAsync(ct);
        await audit.LogAsync(companyId, nameof(Client), e.Id.ToString(), AuditAction.Insert, Snapshot(e), userId, ct);
        return MapDetails(e);
    }

    public async Task UpdateAsync(int id, int companyId, int? userId, UpdateClientDto dto, CancellationToken ct)
    {
        var e = await uow.Clients.GetForUpdateAsync(companyId, id, ct)
                ?? throw new KeyNotFoundException();

        e.ClientType = dto.ClientType;
        e.ManagerId = dto.ManagerId;
        e.FirstName = dto.FirstName?.Trim() ?? "";
        e.LastName = dto.LastName?.Trim() ?? "";
        e.MiddleName = dto.MiddleName?.Trim();
        e.FirstNameGenitive = dto.FirstNameGenitive;
        e.LastNameGenitive = dto.LastNameGenitive;
        e.MiddleNameGenitive = dto.MiddleNameGenitive;
        e.BirthDay = dto.BirthDay;
        e.Gender = dto.Gender;
        e.PhoneE164 = dto.PhoneE164?.Trim();
        e.Email = NormalizeEmail(dto.Email);
        e.IsSubscribedToMailing = dto.IsSubscribedToMailing;
        e.IsEmailNotificationEnabled = dto.IsEmailNotificationEnabled;
        e.ReferredBy = dto.ReferredBy;
        e.Note = dto.Note;
        e.DiscountPercent = dto.DiscountPercent;
        e.IsTourist = dto.IsTourist;
        e.UpdatedAtUtc = DateTime.UtcNow;

        if (dto.Passport is null)
        {
            e.Passport = null;
        }
        else
        {
            e.Passport ??= new Passport { CompanyId = companyId };
            e.Passport.FirstNameLatin = dto.Passport.FirstNameLatin;
            e.Passport.LastNameLatin = dto.Passport.LastNameLatin;
            e.Passport.SerialNumber = dto.Passport.SerialNumber;
            e.Passport.IssueDate = dto.Passport.IssueDate;
            e.Passport.ExpireDate = dto.Passport.ExpireDate;
            e.Passport.IssuingAuthority = dto.Passport.IssuingAuthority;
        }

        if (dto.IdentityDocument is null)
        {
            e.IdentityDocument = null;
        }
        else
        {
            e.IdentityDocument ??= new IdentityDocument { CompanyId = companyId };
            e.IdentityDocument.CitizenshipCountryId = dto.IdentityDocument.CitizenshipCountryId;
            e.IdentityDocument.ResidenceCountryId = dto.IdentityDocument.ResidenceCountryId;
            e.IdentityDocument.ResidenceCityId = dto.IdentityDocument.ResidenceCityId;
            e.IdentityDocument.BirthPlace = dto.IdentityDocument.BirthPlace;
            e.IdentityDocument.SerialNumber = dto.IdentityDocument.SerialNumber;
            e.IdentityDocument.IssuedBy = dto.IdentityDocument.IssuedBy;
            e.IdentityDocument.IssueDate = dto.IdentityDocument.IssueDate;
            e.IdentityDocument.DocumentNumber = dto.IdentityDocument.DocumentNumber;
            e.IdentityDocument.PersonalNumber = dto.IdentityDocument.PersonalNumber;
            e.IdentityDocument.RegistrationAddress = dto.IdentityDocument.RegistrationAddress;
            e.IdentityDocument.ResidentialAddress = dto.IdentityDocument.ResidentialAddress;
            e.IdentityDocument.MotherFullName = dto.IdentityDocument.MotherFullName;
            e.IdentityDocument.FatherFullName = dto.IdentityDocument.FatherFullName;
            e.IdentityDocument.ContactInfo = dto.IdentityDocument.ContactInfo;
        }

        if (dto.BirthCertificate is null)
        {
            e.BirthCertificate = null;
        }
        else
        {
            e.BirthCertificate ??= new BirthCertificate { CompanyId = companyId };
            e.BirthCertificate.SerialNumber = dto.BirthCertificate.SerialNumber;
            e.BirthCertificate.IssuedBy = dto.BirthCertificate.IssuedBy;
            e.BirthCertificate.IssueDate = dto.BirthCertificate.IssueDate;
        }

        if (dto.Insurances is null)
        {
            e.Insurances.Clear();
        }
        else
        {
            e.Insurances.Clear();
            foreach (var i in dto.Insurances)
                e.Insurances.Add(new InsurancePolicy
                {
                    CompanyId = companyId,
                    IssueDate = i.IssueDate,
                    ExpireDate = i.ExpireDate,
                    CountryId = i.CountryId,
                    Note = i.Note
                });
        }

        if (dto.Visas is null)
        {
            e.Visas.Clear();
        }
        else
        {
            e.Visas.Clear();
            foreach (var v in dto.Visas)
                e.Visas.Add(new VisaRecord
                {
                    CompanyId = companyId,
                    IssueDate = v.IssueDate,
                    ExpireDate = v.ExpireDate,
                    CountryId = v.CountryId,
                    IsSchengen = v.IsSchengen,
                    Note = v.Note
                });
        }

        await uow.SaveChangesAsync(ct);
        await audit.LogAsync(companyId, nameof(Client), id.ToString(), AuditAction.Update, Snapshot(e), userId, ct);
    }

    public async Task SoftDeleteAsync(int id, int companyId, int? userId, CancellationToken ct)
    {
        var e = await uow.Clients.GetForUpdateAsync(companyId, id, ct)
                ?? throw new KeyNotFoundException();

        var data = Snapshot(e); // снимок ДО удаления
        e.IsDeleted = true;
        await uow.SaveChangesAsync(ct);

        await audit.LogAsync(companyId, nameof(Client), id.ToString(),
            AuditAction.Delete, data, userId, ct);
    }

    public async Task RestoreAsync(int id, int companyId, int? userId, CancellationToken ct)
    {
        var e = await uow.Clients.GetForUpdateAsync(companyId, id, includeDeleted: true, ct)
                ?? throw new KeyNotFoundException();

        if (!e.IsDeleted) return;

        e.IsDeleted = false;
        await uow.SaveChangesAsync(ct);

        await audit.LogAsync(companyId, nameof(Client), id.ToString(),
            AuditAction.Restore, Snapshot(e), userId, ct);
    }

    private static ClientListItemDto MapList(Client e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        PhoneE164 = e.PhoneE164,
        Email = e.Email,
        IsTourist = e.IsTourist,
        ClientType = e.ClientType,
        Passport = e.Passport is null
            ? null
            : new PassportDto(
                e.Passport.FirstNameLatin, e.Passport.LastNameLatin, e.Passport.SerialNumber,
                e.Passport.IssueDate, e.Passport.ExpireDate, e.Passport.IssuingAuthority),
        IdentityDocument = e.IdentityDocument is null
            ? null
            : new IdentityDocumentDto(
                e.IdentityDocument.CitizenshipCountryId, e.IdentityDocument.ResidenceCountryId,
                e.IdentityDocument.ResidenceCityId,
                e.IdentityDocument.BirthPlace, e.IdentityDocument.SerialNumber, e.IdentityDocument.IssuedBy,
                e.IdentityDocument.IssueDate,
                e.IdentityDocument.DocumentNumber, e.IdentityDocument.PersonalNumber,
                e.IdentityDocument.RegistrationAddress,
                e.IdentityDocument.ResidentialAddress, e.IdentityDocument.MotherFullName,
                e.IdentityDocument.FatherFullName,
                e.IdentityDocument.ContactInfo),
        BirthCertificate = e.BirthCertificate is null
            ? null
            : new BirthCertificateDto(
                e.BirthCertificate.SerialNumber, e.BirthCertificate.IssuedBy, e.BirthCertificate.IssueDate),
        Insurances = e.Insurances.Select(i => new InsurancePolicyDto(i.IssueDate, i.ExpireDate, i.CountryId, i.Note))
            .ToList(),
        Visas = e.Visas.Select(v => new VisaRecordDto(v.IssueDate, v.ExpireDate, v.CountryId, v.IsSchengen, v.Note))
            .ToList(),
        IsDeleted = e.IsDeleted
    };

    private static ClientDetailsDto MapDetails(Client e)
    {
        var baseDto = MapList(e);
        return new ClientDetailsDto
        {
            Id = baseDto.Id,
            FirstName = baseDto.FirstName,
            LastName = baseDto.LastName,
            PhoneE164 = baseDto.PhoneE164,
            Email = baseDto.Email,
            IsTourist = baseDto.IsTourist,
            Passport = baseDto.Passport,
            IdentityDocument = baseDto.IdentityDocument,
            BirthCertificate = baseDto.BirthCertificate,
            Insurances = baseDto.Insurances,
            Visas = baseDto.Visas,
            IsDeleted = baseDto.IsDeleted,
            CompanyId = e.CompanyId,
            ClientType = e.ClientType,
            ManagerId = e.ManagerId,
            MiddleName = e.MiddleName,
            FirstNameGenitive = e.FirstNameGenitive,
            LastNameGenitive = e.LastNameGenitive,
            MiddleNameGenitive = e.MiddleNameGenitive,
            BirthDay = e.BirthDay,
            Gender = e.Gender,
            IsSubscribedToMailing = e.IsSubscribedToMailing,
            IsEmailNotificationEnabled = e.IsEmailNotificationEnabled,
            ReferredBy = e.ReferredBy,
            Note = e.Note,
            DiscountPercent = e.DiscountPercent
        };
    }

    private static readonly JsonSerializerOptions AuditJsonOptions =
        new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    private static string Snapshot(Client c) => JsonSerializer.Serialize(new
    {
        c.Id, c.CompanyId, c.ClientType, c.ManagerId,
        c.FirstName, c.LastName, c.MiddleName,
        c.FirstNameGenitive, c.LastNameGenitive, c.MiddleNameGenitive,
        c.BirthDay, c.Gender, c.PhoneE164, c.Email,
        c.IsSubscribedToMailing, c.IsEmailNotificationEnabled,
        c.ReferredBy, c.Note, c.DiscountPercent, c.IsTourist,
        Passport = c.Passport is null
            ? null
            : new
            {
                c.Passport.FirstNameLatin, c.Passport.LastNameLatin, c.Passport.SerialNumber,
                c.Passport.IssueDate, c.Passport.ExpireDate, c.Passport.IssuingAuthority
            },
        IdentityDocument = c.IdentityDocument is null
            ? null
            : new
            {
                c.IdentityDocument.CitizenshipCountryId, c.IdentityDocument.ResidenceCountryId,
                c.IdentityDocument.ResidenceCityId,
                c.IdentityDocument.BirthPlace, c.IdentityDocument.SerialNumber, c.IdentityDocument.IssuedBy,
                c.IdentityDocument.IssueDate, c.IdentityDocument.DocumentNumber, c.IdentityDocument.PersonalNumber,
                c.IdentityDocument.RegistrationAddress, c.IdentityDocument.ResidentialAddress,
                c.IdentityDocument.MotherFullName, c.IdentityDocument.FatherFullName, c.IdentityDocument.ContactInfo
            },
        BirthCertificate = c.BirthCertificate is null
            ? null
            : new
            {
                c.BirthCertificate.SerialNumber, c.BirthCertificate.IssuedBy, c.BirthCertificate.IssueDate
            },
        Insurances = c.Insurances.Select(i => new { i.IssueDate, i.ExpireDate, i.CountryId, i.Note }).ToList(),
        Visas = c.Visas.Select(v => new { v.IssueDate, v.ExpireDate, v.CountryId, v.IsSchengen, v.Note }).ToList()
    }, AuditJsonOptions);

    private static string? NormalizeEmail(string? s)
        => string.IsNullOrWhiteSpace(s) ? null : s.Trim().ToLowerInvariant();
}