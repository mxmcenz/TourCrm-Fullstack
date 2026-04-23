namespace TourCrm.Core.Specifications;

public record PagedResult<T>(IReadOnlyList<T> Items, int Total);