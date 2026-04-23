namespace TourCrm.Application.DTOs.Tariffs;

public record UpdateTariffDto(
    string Name,
    int MinEmployees,
    int MaxEmployees,
    decimal MonthlyPrice,
    decimal? HalfYearPrice,
    decimal? YearlyPrice,
    IReadOnlyList<TariffPermissionDto> Permissions
);