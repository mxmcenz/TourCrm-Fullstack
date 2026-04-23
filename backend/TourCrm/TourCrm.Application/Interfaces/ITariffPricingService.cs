using TourCrm.Core.Enums;

namespace TourCrm.Application.Interfaces;

public interface ITariffPricingService
{
    Task<decimal?> GetPriceAsync(int tariffId, BillingPeriod period, CancellationToken ct);
    Task<decimal?> GetMonthlyEquivalentAsync(int tariffId, BillingPeriod period, CancellationToken ct);
}