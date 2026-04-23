using System.ComponentModel.DataAnnotations.Schema;

namespace TourCrm.Core.Entities;

public class Employee : User
{
    public int OfficeId { get; set; } 
    public Office Office { get; set; }
    public int LegalEntityId { get; set; } 
    public LegalEntity LegalEntity { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int LeadLimit { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
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

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SalaryAmount { get; set; }            

    public string? WorkConditions { get; set; }           
    public string? Note { get; set; }                     
}