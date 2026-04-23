namespace TourCrm.Application.DTOs.Tariffs;

public record CreateTariffDto(
    string Name,
    int MinEmployees,
    int MaxEmployees,
    decimal MonthlyPrice,
    decimal? HalfYearPrice,
    decimal? YearlyPrice,
    IReadOnlyList<TariffPermissionDto> Permissions
);