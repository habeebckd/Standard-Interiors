using StandardInteriors.Api.Domain.Entities;
using StandardInteriors.Api.Domain.Enums;

namespace StandardInteriors.Api.Services;

public sealed record EstimateSectionCalculation(decimal SectionAmount, decimal ItemTotal, decimal DisplayAmount);
public sealed record EstimateCalculationResult(decimal EstimatedTotal, IReadOnlyDictionary<int, EstimateSectionCalculation> Sections);

public interface ICalculationService
{
    EstimateCalculationResult CalculateEstimate(Estimate estimate);
    decimal CalculatePaymentTotal(IEnumerable<Payment> payments);
    decimal CalculateOutstanding(decimal totalEstimate, decimal totalPaid);
    decimal GetItemAmount(EstimateItem item);
}

public sealed class CalculationService : ICalculationService
{
    public EstimateCalculationResult CalculateEstimate(Estimate estimate)
    {
        var results = new Dictionary<int, EstimateSectionCalculation>();
        decimal total = 0;

        foreach (var section in estimate.Sections.OrderBy(x => x.DisplayOrder))
        {
            var mode = section.PricingMode ?? estimate.PricingMode;
            decimal itemTotal = section.Items.OrderBy(x => x.DisplayOrder).Sum(GetItemAmount);
            decimal sectionAmount = mode switch
            {
                EstimatePricingMode.ItemBased => itemTotal,
                EstimatePricingMode.SectionBased => section.ManualSectionAmount ?? itemTotal,
                EstimatePricingMode.Overall => section.ManualSectionAmount ?? itemTotal,
                _ => itemTotal
            };

            if (mode == EstimatePricingMode.Overall)
            {
                sectionAmount = section.ManualSectionAmount ?? 0m;
            }

            results[section.Id == 0 ? section.DisplayOrder : section.Id] = new EstimateSectionCalculation(sectionAmount, itemTotal, sectionAmount);
            if (mode != EstimatePricingMode.Overall)
            {
                total += sectionAmount;
            }
        }

        if (estimate.PricingMode == EstimatePricingMode.Overall)
        {
            total = estimate.ManualOverallAmount ?? 0m;
        }

        return new EstimateCalculationResult(total, results);
    }

    public decimal CalculatePaymentTotal(IEnumerable<Payment> payments)
        => payments.Where(x => x.Status == PaymentStatus.Posted).Sum(x => x.Amount);

    public decimal CalculateOutstanding(decimal totalEstimate, decimal totalPaid)
        => Math.Max(totalEstimate - totalPaid, 0m);

    public decimal GetItemAmount(EstimateItem item)
    {
        if (item.ManualAmount.HasValue)
        {
            return item.ManualAmount.Value;
        }

        if (item.Quantity.HasValue && item.Rate.HasValue)
        {
            return item.Quantity.Value * item.Rate.Value;
        }

        return 0m;
    }
}
