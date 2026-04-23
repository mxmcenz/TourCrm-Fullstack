using FluentValidation;
using TourCrm.Application.DTOs.Leads;

namespace TourCrm.Application.Validations.Leads;

public class CreateLeadDtoValidator : AbstractValidator<CreateLeadDto>
{
    public CreateLeadDtoValidator()
    {
        RuleFor(x => x.CustomerFirstName).NotEmpty().WithMessage("Имя обязательно");
        RuleFor(x => x.CustomerLastName).NotEmpty().WithMessage("Фамилия обязательна");
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Телефон обязателен");
    }
}