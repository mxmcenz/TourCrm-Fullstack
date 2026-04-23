namespace TourCrm.Application.DTOs.Employees;

public class EmployeeUpdateDto
{
    public int OfficeId { get; set; }
    public int LegalEntityId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string Phone { get; set; } = string.Empty;
    public int LeadLimit { get; set; }
    public string? Password { get; set; }
    public bool IsDeleted { get; set; }
    public List<int> RoleIds { get; set; } = new();
    
    public string? Position { get; set; }
    public string? PositionGenitive { get; set; }
    public string? PowerOfAttorneyNumber { get; set; }

    public string? LastNameGenitive { get; set; }
    public string? FirstNameGenitive { get; set; }
    public string? MiddleNameGenitive { get; set; }

    public string? MobilePhone { get; set; }
    public string? AdditionalPhone { get; set; }

    public DateTime? BirthDate { get; set; }
    public string? TimeZone { get; set; }

    public string? ContactInfo { get; set; }
    public DateTime? HireDate { get; set; }
    public decimal? SalaryAmount { get; set; }
    public string? WorkConditions { get; set; }
    public string? Note { get; set; }
}