using TourCrm.Core.Entities.Leads;

namespace TourCrm.Core.Entities.Dictionaries;

public class LeadLabel
{
    public int LeadId { get; set; }
    public Lead Lead { get; set; } = default!;

    public int LabelId { get; set; }
    public Label Label { get; set; } = default!;

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
}