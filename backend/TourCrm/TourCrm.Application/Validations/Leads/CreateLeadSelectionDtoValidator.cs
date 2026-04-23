using FluentValidation;
using TourCrm.Application.DTOs.Leads;

namespace TourCrm.Application.Validations.Leads;

public class CreateLeadSelectionDtoValidator : AbstractValidator<CreateLeadSelectionDto>
{
    public CreateLeadSelectionDtoValidator()
    {
        RuleFor(x => x.Price)
            .NotNull().WithMessage("Стоимость обязательна")
            .GreaterThanOrEqualTo(0).WithMessage("Стоимость не может быть отрицательной");
    }
}