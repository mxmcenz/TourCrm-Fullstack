using TourCrm.Application.Interfaces;
using TourCrm.Core.Enums;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services;

public class TariffPricingService(IUnitOfWork uow) : ITariffPricingService
{
    public async Task<decimal?> GetPriceAsync(int tariffId, BillingPeriod period, CancellationToken ct)
    {
        var t = await uow.Tariffs.GetByIdAsync(tariffId, ct);
        if (t is null) return null;

        return period switch
        {
            BillingPeriod.Month => R(t.MonthlyPrice),
            BillingPeriod.HalfYear => R(t.HalfYearPrice ?? t.MonthlyPrice * 6m),
            BillingPeriod.Year => R(t.YearlyPrice ?? t.MonthlyPrice * 12m),
            _ => R(t.MonthlyPrice * (int)period)
        };
    }

    public async Task<decimal?> GetMonthlyEquivalentAsync(int tariffId, BillingPeriod period, CancellationToken ct)
    {
        var total = await GetPriceAsync(tariffId, period, ct);
        if (total is null) return null;

        var months = period switch
        {
            BillingPeriod.Month  => 1,
            BillingPeriod.HalfYear => 6,
            BillingPeriod.Year   => 12,
            _ => 1
        };

        return decimal.Round(total.Value / months, 2, MidpointRounding.AwayFromZero);
    }

    static decimal R(decimal v) => Math.Round(v, 2, MidpointRounding.AwayFromZero);
}