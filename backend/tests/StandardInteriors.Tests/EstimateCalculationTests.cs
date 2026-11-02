using StandardInteriors.Api.Domain.Entities;
using StandardInteriors.Api.Domain.Enums;
using StandardInteriors.Api.Services;

namespace StandardInteriors.Tests;

public sealed class EstimateCalculationTests
{
    private readonly CalculationService _calculator = new();

    [Fact]
    public void ItemBased_SumsItemAmounts()
    {
        var estimate = BuildEstimate(EstimatePricingMode.ItemBased, null, 30000m, 20000m, 25000m);
        var result = _calculator.CalculateEstimate(estimate);

        Assert.Equal(75000m, result.EstimatedTotal);
    }

    [Fact]
    public void SectionBased_SumsSectionAmounts()
    {
        var estimate = BuildEstimate(EstimatePricingMode.SectionBased, 100000m, 0m, 0m, 0m, 170000m, 0m);
        var result = _calculator.CalculateEstimate(estimate);

        Assert.Equal(270000m, result.EstimatedTotal);
    }

    [Fact]
    public void Overall_UsesManualAmount()
    {
        var estimate = BuildEstimate(EstimatePricingMode.Overall, 270000m, 0m, 0m);
        estimate.ManualOverallAmount = 270000m;
        var result = _calculator.CalculateEstimate(estimate);

        Assert.Equal(270000m, result.EstimatedTotal);
    }

    [Fact]
    public void PaymentTotal_IgnoresVoidedPayments()
    {
        var payments = new[]
        {
            new Payment { Amount = 50000m, Status = PaymentStatus.Posted },
            new Payment { Amount = 30000m, Status = PaymentStatus.Posted },
            new Payment { Amount = 10000m, Status = PaymentStatus.Voided }
        };

        Assert.Equal(80000m, _calculator.CalculatePaymentTotal(payments));
    }

    [Fact]
    public void Outstanding_SubtractsPaidAmount()
    {
        Assert.Equal(190000m, _calculator.CalculateOutstanding(270000m, 80000m));
    }

    private static Estimate BuildEstimate(EstimatePricingMode mode, decimal? section1Amount, decimal item1, decimal item2, decimal item3 = 0m, decimal? section2Amount = null, decimal item4 = 0m)
        => new()
        {
            PricingMode = mode,
            Sections =
            [
                new EstimateSection
                {
                    DisplayOrder = 1,
                    ManualSectionAmount = section1Amount,
                    Items =
                    [
                        new EstimateItem { DisplayOrder = 1, ManualAmount = item1, Description = "A" },
                        new EstimateItem { DisplayOrder = 2, ManualAmount = item2, Description = "B" },
                        new EstimateItem { DisplayOrder = 3, ManualAmount = item3, Description = "C" }
                    ]
                },
                new EstimateSection
                {
                    DisplayOrder = 2,
                    ManualSectionAmount = section2Amount,
                    Items =
                    [
                        new EstimateItem { DisplayOrder = 1, ManualAmount = item4, Description = "D" }
                    ]
                }
            ]
        };
}
