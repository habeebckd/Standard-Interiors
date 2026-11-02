using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StandardInteriors.Api.Data;
using StandardInteriors.Api.Domain.Entities;
using StandardInteriors.Api.Domain.Enums;
using StandardInteriors.Api.Dtos.Customers;
using StandardInteriors.Api.Dtos.Settings;
using StandardInteriors.Api.Options;

namespace StandardInteriors.Api.Services.Implementations;

public interface IDocumentService
{
    Task<byte[]> GenerateEstimatePdfAsync(int estimateId, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateCustomerSummaryPdfAsync(int customerId, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateCustomerStatementPdfAsync(int customerId, CancellationToken cancellationToken = default);
    Task<byte[]> GeneratePaymentSheetPdfAsync(int customerId, CancellationToken cancellationToken = default);
    Task<byte[]> GeneratePaymentReceiptPdfAsync(int paymentId, CancellationToken cancellationToken = default);
}

public sealed class DocumentService : IDocumentService
{
    private readonly StandardInteriorsDbContext _db;
    private readonly ISettingsService _settings;
    private readonly ICustomerService _customerService;
    private readonly BrandingOptions _branding;

    public DocumentService(StandardInteriorsDbContext db, ISettingsService settings, ICustomerService customerService, IOptions<BrandingOptions> branding)
    {
        _db = db; _settings = settings; _customerService = customerService; _branding = branding.Value;
    }

    public async Task<byte[]> GenerateEstimatePdfAsync(int estimateId, CancellationToken cancellationToken = default)
    {
        var estimate = await _db.Estimates.AsNoTracking().Include(x => x.Customer).Include(x => x.Sections).ThenInclude(x => x.Items).ThenInclude(x => x.Service).FirstOrDefaultAsync(x => x.Id == estimateId, cancellationToken);
        if (estimate == null) throw new ArgumentException("Estimate not found.");
        var settings = await _settings.GetAsync(cancellationToken);
        return new StandardEstimateDocument(estimate, settings, ResolveLogoPath(settings)).GeneratePdf();
    }

    public async Task<byte[]> GenerateCustomerSummaryPdfAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == customerId, cancellationToken);
        if (customer == null) throw new ArgumentException("Customer not found.");
        var summary = await _customerService.GetSummaryAsync(customerId, cancellationToken);
        if (summary == null) throw new ArgumentException("Customer summary not found.");
        var settings = await _settings.GetAsync(cancellationToken);
        return new CustomerSummaryDocument(customer, summary, settings, ResolveLogoPath(settings)).GeneratePdf();
    }

    public async Task<byte[]> GenerateCustomerStatementPdfAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == customerId, cancellationToken);
        if (customer == null) throw new ArgumentException("Customer not found.");
        var statement = await _customerService.GetStatementAsync(customerId, cancellationToken);
        if (statement == null) throw new ArgumentException("Customer statement not found.");
        var settings = await _settings.GetAsync(cancellationToken);
        return new CustomerStatementDocument(customer, statement, settings, ResolveLogoPath(settings)).GeneratePdf();
    }

    public async Task<byte[]> GeneratePaymentSheetPdfAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == customerId, cancellationToken);
        if (customer == null) throw new ArgumentException("Customer not found.");
        var payments = await _db.Payments.AsNoTracking().Include(x => x.PaymentMethod).Include(x => x.Estimate).Where(x => x.CustomerId == customerId && x.Status == PaymentStatus.Posted).OrderBy(x => x.PaymentDate).ToListAsync(cancellationToken);
        var estimates = await _db.Estimates.AsNoTracking().Where(x => x.CustomerId == customerId && (x.Status == EstimateStatus.Sent || x.Status == EstimateStatus.Approved || x.Status == EstimateStatus.Completed)).ToListAsync(cancellationToken);
        var settings = await _settings.GetAsync(cancellationToken);
        return new PaymentSheetDocument(customer.Name, estimates.Sum(x => x.CalculatedTotalAmount), payments.Sum(x => x.Amount), payments, ResolveLogoPath(settings)).GeneratePdf();
    }

    public async Task<byte[]> GeneratePaymentReceiptPdfAsync(int paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _db.Payments.AsNoTracking().Include(x => x.Customer).Include(x => x.Estimate).Include(x => x.PaymentMethod).FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken);
        if (payment == null) throw new ArgumentException("Payment not found.");
        var targetOutstanding = await GetOutstandingAsync(payment.CustomerId, payment.EstimateId, payment.Id, cancellationToken);
        var settings = await _settings.GetAsync(cancellationToken);
        return new PaymentReceiptDocument(payment, targetOutstanding + payment.Amount, targetOutstanding, settings, ResolveLogoPath(settings)).GeneratePdf();
    }

    private string? ResolveLogoPath(CompanySettingsDto? settings)
    {
        var suppliedEmblem = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\..\\..\\public\\standard interiors Logo.png"));
        if (File.Exists(suppliedEmblem)) return suppliedEmblem;
        var path = settings?.LogoPath ?? _branding.LogoPath;
        if (string.IsNullOrWhiteSpace(path)) return null;
        var fullPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
        return File.Exists(fullPath) ? fullPath : null;
    }

    private async Task<decimal> GetOutstandingAsync(int customerId, int? estimateId, int paymentId, CancellationToken cancellationToken)
    {
        if (estimateId.HasValue)
        {
            var estimate = await _db.Estimates.AsNoTracking().FirstAsync(x => x.Id == estimateId.Value, cancellationToken);
            var received = await _db.Payments.AsNoTracking().Where(x => x.EstimateId == estimateId.Value && x.Status == PaymentStatus.Posted && x.Id != paymentId).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
            return Math.Max(estimate.CalculatedTotalAmount - received, 0m);
        }
        var estimates = await _db.Estimates.AsNoTracking().Where(x => x.CustomerId == customerId && (x.Status == EstimateStatus.Sent || x.Status == EstimateStatus.Approved || x.Status == EstimateStatus.Completed)).SumAsync(x => (decimal?)x.CalculatedTotalAmount, cancellationToken) ?? 0m;
        var receivedTotal = await _db.Payments.AsNoTracking().Where(x => x.CustomerId == customerId && x.Status == PaymentStatus.Posted && x.Id != paymentId).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        return Math.Max(estimates - receivedTotal, 0m);
    }

    private static string Money(decimal amount) => string.Format(new CultureInfo("en-IN"), "₹ {0:N2}", amount);
    private static string Number(decimal amount) => string.Format(new CultureInfo("en-IN"), "{0:N2}", amount);
    private static string Date(DateTime value) => value.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture);

    private static void Header(ColumnDescriptor col, string title, string? subtitle, string? logoPath, string? companyName)
    {
        col.Item().PaddingTop(10).AlignCenter().Width(310).Row(row =>
        {
            if (!string.IsNullOrWhiteSpace(logoPath)) row.ConstantItem(78).Height(78).Image(logoPath).FitArea();
            row.RelativeItem().PaddingLeft(14).AlignMiddle().Column(x =>
            {
                x.Item().Text("STANDARD").FontSize(21).Bold().FontColor(PdfStyle.Gold);
                x.Item().Text("INTERIORS").FontSize(21).Bold().FontColor(PdfStyle.Gold);
            });
        });
        col.Item().PaddingTop(14).AlignCenter().Text(title).FontSize(14).Bold().FontColor(PdfStyle.Ink);
        col.Item().PaddingTop(6).AlignCenter().Width(105).Height(3).Background(PdfStyle.Gold);
        if (!string.IsNullOrWhiteSpace(subtitle)) col.Item().PaddingTop(7).AlignCenter().Text(subtitle).FontSize(9).FontColor(PdfStyle.Muted);
    }

    private static void Footer(PageDescriptor page, string label)
    {
        page.Footer().PaddingTop(7).BorderTop(1).BorderColor(PdfStyle.Gold).Row(row =>
        {
            row.RelativeItem().Text("STANDARD INTERIORS  •  INTERIOR DESIGN & EXECUTION").FontSize(8).FontColor(PdfStyle.Muted);
            row.ConstantItem(150).AlignRight().Text(text => { text.Span(label).Bold().FontColor(PdfStyle.GoldDark); text.Span("  •  Page "); text.CurrentPageNumber(); text.Span(" / "); text.TotalPages(); });
        });
    }

    private static void Watermark(ColumnDescriptor col, string? logoPath, float height = 118)
    {
        var watermarkPath = Path.Combine(AppContext.BaseDirectory, "Assets", "standard-interiors-watermark.png");
        if (File.Exists(watermarkPath)) col.Item().PaddingVertical(7).Height(height).AlignCenter().AlignMiddle().Image(watermarkPath).FitArea();
        else col.Item().PaddingVertical(7).Height(height).AlignCenter().AlignMiddle().Text("STANDARD INTERIORS").FontSize(19).Bold().FontColor(PdfStyle.Watermark);
    }

    private static void Label(ColumnDescriptor container, string label, string value, float valueSize = 10)
    {
        container.Item().Text(label).FontSize(8).SemiBold().FontColor(PdfStyle.GoldDark);
        container.Item().PaddingTop(3).Text(value).FontSize(valueSize).FontColor(PdfStyle.Ink);
    }

    private static void LabelCell(IContainer container, string label)
    {
        container.Padding(8).Text(label).FontSize(9).SemiBold();
    }

    private static string RupeeIconSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 100 100\"><circle cx=\"50\" cy=\"50\" r=\"44\" fill=\"none\" stroke=\"#b08a38\" stroke-width=\"3\"/><circle cx=\"50\" cy=\"50\" r=\"37\" fill=\"none\" stroke=\"#b08a38\" stroke-width=\"1.5\"/><text x=\"50\" y=\"66\" text-anchor=\"middle\" font-family=\"Arial\" font-size=\"42\" font-weight=\"bold\" fill=\"#8b681f\">₹</text></svg>";

    private static void Balance(IContainer container, string label, decimal value)
    {
        container.Border(1).BorderColor(PdfStyle.Gold).Background(PdfStyle.Beige).Padding(14).Row(row =>
        {
            row.ConstantItem(48).AlignMiddle().Svg(RupeeIconSvg()).FitArea();
            row.RelativeItem().PaddingLeft(16).Column(x => { x.Item().Text(label).FontSize(10).Bold(); x.Item().PaddingTop(4).Text(Money(value)).FontSize(19).Bold().FontColor(PdfStyle.GoldDark); });
        });
    }

    private static void SummaryBand(IContainer container, decimal total, decimal paid, decimal balance)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn(); });
            Card(table.Cell(), "TOTAL ESTIMATED", Money(total)); Card(table.Cell(), "TOTAL RECEIVED", Money(paid)); Card(table.Cell(), "OUTSTANDING", Money(balance));
        });
    }

    private static void TotalLine(ColumnDescriptor col, string label, string value, bool emphasize = false)
    {
        col.Item().PaddingVertical(6).BorderBottom(1).BorderColor(PdfStyle.Line).Row(row =>
        {
            row.RelativeItem().Text(label).FontSize(9).Bold();
            row.ConstantItem(145).AlignRight().Text(value).FontSize(emphasize ? 10 : 9).Bold().FontColor(emphasize ? PdfStyle.GoldDark : PdfStyle.Ink);
        });
    }

    private static void Card(IContainer cell, string label, string value)
    {
        cell.Border(1).BorderColor(PdfStyle.Line).Background(PdfStyle.WhiteWarm).Padding(11).Column(x => { x.Item().Text(label).FontSize(8).SemiBold(); x.Item().PaddingTop(4).Text(value).FontSize(13).Bold().FontColor(PdfStyle.GoldDark); });
    }

    private static void TableHeader(IContainer cell, string text, bool right = false)
    {
        if (right) cell.Background(PdfStyle.Band).Padding(8).AlignRight().Text(text).FontSize(8).Bold();
        else cell.Background(PdfStyle.Band).Padding(8).Text(text).FontSize(8).Bold();
    }

    private static class PdfStyle
    {
        public const string Gold = "#b08a38", GoldDark = "#8b681f", Ink = "#202020", Muted = "#6f6f6f", Band = "#f5f2ed", Beige = "#f7f1e3", WhiteWarm = "#fffdf9", Line = "#e5d9c2", Watermark = "#f3eee5";
    }

    private sealed class StandardEstimateDocument : IDocument
    {
        private readonly Estimate _estimate; private readonly CompanySettingsDto? _settings; private readonly string? _logoPath;
        public StandardEstimateDocument(Estimate estimate, CompanySettingsDto? settings, string? logoPath) { _estimate = estimate; _settings = settings; _logoPath = logoPath; }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4); page.Margin(28); page.DefaultTextStyle(x => x.FontSize(10).FontColor(PdfStyle.Ink));
                page.Content().Column(col =>
                {
                    Header(col, "INTERIOR WORK ESTIMATE", null, _logoPath, _settings?.CompanyName);
                    col.Item().PaddingTop(18).Row(row => { row.RelativeItem().Column(x => { x.Item().Text("PREPARED FOR").FontSize(9).FontColor(PdfStyle.Muted); x.Item().PaddingTop(16).BorderBottom(1).BorderColor(PdfStyle.Ink).Text(_estimate.Customer.Name); }); row.ConstantItem(170).Column(x => { x.Item().Text("ESTIMATE DATE").FontSize(9).FontColor(PdfStyle.Muted); x.Item().PaddingTop(16).Text(Date(_estimate.EstimateDate)); }); });
                    col.Item().PaddingTop(10).LineHorizontal(1).LineColor(PdfStyle.Ink);
                    Watermark(col, _logoPath, 90);
                    col.Item().Background(PdfStyle.Band).Padding(10).Row(row => { row.RelativeItem().Text("DESCRIPTION / SCOPE OF WORK").Bold(); row.ConstantItem(165).AlignRight().Text("ESTIMATED AMOUNT (₹)").Bold(); });
                    foreach (var section in _estimate.Sections.OrderBy(x => x.DisplayOrder))
                    {
                        var showItems = _estimate.PricingMode == EstimatePricingMode.ItemBased;
                        var sectionAmount = _estimate.PricingMode == EstimatePricingMode.SectionBased ? section.ManualSectionAmount : null;
                        col.Item().PaddingTop(12).BorderBottom(1).BorderColor(PdfStyle.Line).PaddingBottom(11).Row(row =>
                        {
                            row.ConstantItem(42).AlignTop().Background(PdfStyle.Gold).PaddingVertical(7).AlignCenter().Text(section.SectionNumber.PadLeft(2, '0')).Bold().FontColor(Colors.White);
                            row.RelativeItem().PaddingLeft(12).Column(x =>
                            {
                                x.Item().Text($"SECTION {section.SectionNumber} - {section.Title.ToUpperInvariant()}").FontSize(11).Bold();
                                if (!string.IsNullOrWhiteSpace(section.Description)) x.Item().PaddingTop(3).Text(section.Description).FontSize(8).FontColor(PdfStyle.Muted);
                                foreach (var item in section.Items.OrderBy(x => x.DisplayOrder)) x.Item().PaddingTop(5).Text("•  " + item.Description).FontSize(9).FontColor(PdfStyle.Muted);
                            });
                            row.ConstantItem(150).AlignTop().AlignRight().Column(x =>
                            {
                                if (sectionAmount.HasValue) { x.Item().Text(Number(sectionAmount.Value)).FontSize(12).Bold(); x.Item().PaddingTop(7).Text("Estimated section total").FontSize(8).FontColor(PdfStyle.GoldDark); }
                                if (showItems) foreach (var item in section.Items.OrderBy(x => x.DisplayOrder)) x.Item().PaddingTop(5).Text(Money(item.ManualAmount ?? (item.Quantity.HasValue && item.Rate.HasValue ? item.Quantity.Value * item.Rate.Value : 0m))).FontSize(9);
                            });
                        });
                        if (showItems) { /* Item amounts are rendered in the same scope column only when that pricing mode is authoritative. */ }
                    }
                    col.Item().PaddingTop(10).AlignRight().Column(x =>
                    {
                        if (_estimate.PricingMode == EstimatePricingMode.SectionBased) foreach (var section in _estimate.Sections.OrderBy(x => x.DisplayOrder)) x.Item().PaddingTop(5).BorderBottom(1).BorderColor(PdfStyle.Line).PaddingBottom(5).Row(row => { row.RelativeItem().Text($"SECTION {section.SectionNumber} ESTIMATE").Bold(); row.ConstantItem(150).AlignRight().Text(Money(section.ManualSectionAmount ?? 0m)).Bold(); });
                    });
                    col.Item().PaddingTop(10).AlignCenter().Width(350).Column(x => { x.Item().Background(PdfStyle.Beige).Padding(14).Row(row => { row.ConstantItem(44).AlignMiddle().Svg(RupeeIconSvg()).FitArea(); row.RelativeItem().PaddingLeft(14).Column(y => { y.Item().Text("TOTAL ESTIMATED PROJECT COST").FontSize(9).Bold(); y.Item().PaddingTop(4).Text(Money(_estimate.CalculatedTotalAmount)).FontSize(19).Bold().FontColor(PdfStyle.GoldDark); }); }); });
                    col.Item().PaddingTop(15).Background(PdfStyle.Band).Padding(10).Column(x => { x.Item().Text("ESTIMATE NOTE").Bold(); x.Item().PaddingTop(6).Text(_estimate.EstimateNote ?? _settings?.DefaultEstimateNote ?? string.Empty).FontSize(8.5f).FontColor(PdfStyle.Muted); });
                });
                Footer(page, "ESTIMATE");
            });
        }
    }

    private sealed class PaymentSheetDocument : IDocument
    {
        private readonly string _customerName; private readonly decimal _total; private readonly decimal _paid; private readonly IReadOnlyList<Payment> _payments; private readonly string? _logoPath;
        public PaymentSheetDocument(string customerName, decimal total, decimal paid, IReadOnlyList<Payment> payments, string? logoPath) { _customerName = customerName; _total = total; _paid = paid; _payments = payments; _logoPath = logoPath; }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public void Compose(IDocumentContainer container)
        {
            container.Page(page => { page.Size(PageSizes.A4); page.Margin(28); page.DefaultTextStyle(x => x.FontSize(10).FontColor(PdfStyle.Ink)); page.Content().Column(col =>
            {
                Header(col, "PAYMENT SHEET", _customerName, _logoPath, "STANDARD INTERIORS"); Watermark(col, _logoPath);
                col.Item().PaddingTop(12).Background(PdfStyle.Band).Padding(10).Row(row => { row.RelativeItem().Text("DESCRIPTION").Bold(); row.ConstantItem(150).AlignRight().Text("AMOUNT (₹)").Bold(); });
                col.Item().PaddingVertical(9).BorderBottom(1).BorderColor(PdfStyle.Line).Row(row => { row.RelativeItem().Text("Interior Design & Execution Services"); row.ConstantItem(150).AlignRight().Text(Money(_total)); });
                col.Item().PaddingTop(18).AlignRight().Width(235).Column(x => { TotalLine(x, "TOTAL PRICE", Money(_total)); TotalLine(x, "TOTAL PAID", Money(_paid)); TotalLine(x, "REMAINING BALANCE", Money(Math.Max(_total - _paid, 0m)), true); });
                col.Item().PaddingTop(25).Background(PdfStyle.Band).Padding(10).Text("PAYMENT DETAILS").Bold();
                col.Item().Table(table => { table.ColumnsDefinition(c => { c.ConstantColumn(45); c.ConstantColumn(80); c.ConstantColumn(95); c.ConstantColumn(105); c.RelativeColumn(); }); table.Header(h => { TableHeader(h.Cell(), "S.NO."); TableHeader(h.Cell(), "DATE"); TableHeader(h.Cell(), "AMOUNT (₹)"); TableHeader(h.Cell(), "MODE OF PAYMENT"); TableHeader(h.Cell(), "TRANSACTION ID / NOTE"); }); var i = 1; foreach (var p in _payments) { table.Cell().BorderBottom(1).BorderColor(PdfStyle.Line).Padding(8).Text((i++).ToString()); table.Cell().BorderBottom(1).BorderColor(PdfStyle.Line).Padding(8).Text(p.PaymentDate.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)); table.Cell().BorderBottom(1).BorderColor(PdfStyle.Line).Padding(8).AlignRight().Text(Money(p.Amount)); table.Cell().BorderBottom(1).BorderColor(PdfStyle.Line).Padding(8).Text(p.PaymentMethod.Name); table.Cell().BorderBottom(1).BorderColor(PdfStyle.Line).Padding(8).Text(p.TransactionReference ?? p.Notes ?? string.Empty); } });
                col.Item().PaddingTop(17).AlignRight().Text($"TOTAL PAID  {Money(_paid)}").FontSize(12).Bold(); col.Item().PaddingTop(18).Element(x => Balance(x, "REMAINING BALANCE", Math.Max(_total - _paid, 0m)));
            }); Footer(page, "PAYMENT SHEET"); });
        }
    }

    private sealed class PaymentReceiptDocument : IDocument
    {
        private readonly Payment _payment; private readonly decimal _previous; private readonly decimal _remaining; private readonly CompanySettingsDto? _settings; private readonly string? _logoPath;
        public PaymentReceiptDocument(Payment payment, decimal previous, decimal remaining, CompanySettingsDto? settings, string? logoPath) { _payment = payment; _previous = previous; _remaining = remaining; _settings = settings; _logoPath = logoPath; }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public void Compose(IDocumentContainer container)
        {
            container.Page(page => { page.Size(PageSizes.A4); page.Margin(28); page.DefaultTextStyle(x => x.FontSize(10).FontColor(PdfStyle.Ink)); page.Content().Column(col =>
            {
                Header(col, "PAYMENT RECEIPT", _payment.Customer.Name, _logoPath, _settings?.CompanyName); Watermark(col, _logoPath);
                col.Item().PaddingTop(18).Background(PdfStyle.Band).Padding(12).Row(row => { row.RelativeItem().Column(x => { Label(x, "CUSTOMER", _payment.Customer.Name); Label(x, "PAYMENT DATE", Date(_payment.PaymentDate)); }); row.RelativeItem().Column(x => { Label(x, "PAYMENT NO.", _payment.PaymentNumber); Label(x, "ESTIMATE", _payment.Estimate?.EstimateNumber ?? "-"); }); });
                col.Item().PaddingTop(20).Table(table => { table.ColumnsDefinition(c => { c.RelativeColumn(); c.ConstantColumn(150); }); LabelCell(table.Cell(), "PREVIOUS BALANCE"); table.Cell().Padding(8).AlignRight().Text(Money(_previous)); LabelCell(table.Cell(), "AMOUNT RECEIVED"); table.Cell().Padding(8).AlignRight().Text(Money(_payment.Amount)).Bold().FontColor(PdfStyle.GoldDark); LabelCell(table.Cell(), "REMAINING BALANCE"); table.Cell().Padding(8).AlignRight().Text(Money(_remaining)).Bold().FontColor(PdfStyle.GoldDark); });
                col.Item().PaddingTop(22).Background(PdfStyle.Band).Padding(12).Column(x => { Label(x, "PAYMENT METHOD", _payment.PaymentMethod.Name); Label(x, "TRANSACTION ID / NOTE", _payment.TransactionReference ?? _payment.Notes ?? "-"); });
                col.Item().PaddingTop(22).Element(x => Balance(x, "REMAINING BALANCE", _remaining));
            }); Footer(page, "PAYMENT RECEIPT"); });
        }
    }

    private sealed class CustomerSummaryDocument : IDocument
    {
        private readonly Customer _customer; private readonly CustomerSummaryDto _summary; private readonly CompanySettingsDto? _settings; private readonly string? _logoPath;
        public CustomerSummaryDocument(Customer customer, CustomerSummaryDto summary, CompanySettingsDto? settings, string? logoPath) { _customer = customer; _summary = summary; _settings = settings; _logoPath = logoPath; }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public void Compose(IDocumentContainer container)
        {
            container.Page(page => { page.Size(PageSizes.A4); page.Margin(28); page.DefaultTextStyle(x => x.FontSize(10).FontColor(PdfStyle.Ink)); page.Content().Column(col =>
            {
                Header(col, "CUSTOMER SUMMARY", _customer.Name, _logoPath, _settings?.CompanyName); Watermark(col, _logoPath);
                col.Item().PaddingTop(12).Text("CUSTOMER INFORMATION").FontSize(10).Bold().FontColor(PdfStyle.GoldDark);
                col.Item().PaddingTop(7).Background(PdfStyle.Band).Padding(12).Row(row => { row.RelativeItem().Column(x => { Label(x, "PREPARED FOR", _customer.Name, 12); Label(x, "CUSTOMER CODE", _customer.CustomerCode); }); row.RelativeItem().Column(x => { Label(x, "PHONE", _customer.Phone); Label(x, "ADDRESS / LOCATION", _customer.Location ?? _customer.Address ?? "-"); }); });
                col.Item().PaddingTop(18).Text("FINANCIAL SUMMARY").FontSize(10).Bold().FontColor(PdfStyle.GoldDark); col.Item().PaddingTop(7).Element(x => SummaryBand(x, _summary.TotalEstimated, _summary.TotalReceived, _summary.Outstanding));
                col.Item().PaddingTop(12).Table(table => { table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); }); Card(table.Cell(), "TOTAL ESTIMATES", _summary.TotalEstimates.ToString()); Card(table.Cell(), "TOTAL PAYMENTS", _summary.TotalPayments.ToString()); });
                col.Item().PaddingTop(22).Element(x => Balance(x, "CURRENT BALANCE", _summary.Outstanding));
            }); Footer(page, "CUSTOMER SUMMARY"); });
        }
    }

    private sealed class CustomerStatementDocument : IDocument
    {
        private readonly Customer _customer; private readonly CustomerStatementDto _statement; private readonly CompanySettingsDto? _settings; private readonly string? _logoPath;
        public CustomerStatementDocument(Customer customer, CustomerStatementDto statement, CompanySettingsDto? settings, string? logoPath) { _customer = customer; _statement = statement; _settings = settings; _logoPath = logoPath; }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public void Compose(IDocumentContainer container)
        {
            container.Page(page => { page.Size(PageSizes.A4); page.Margin(28); page.DefaultTextStyle(x => x.FontSize(9).FontColor(PdfStyle.Ink)); page.Content().Column(col =>
            {
                Header(col, "CUSTOMER STATEMENT", _customer.Name, _logoPath, _settings?.CompanyName);
                col.Item().PaddingTop(27).Row(row => { row.RelativeItem().Column(x => { Label(x, "PREPARED FOR", _customer.Name, 12); x.Item().PaddingTop(3).Text(_customer.CustomerCode).FontSize(9).FontColor(PdfStyle.Muted); Label(x, "PHONE", _customer.Phone); }); row.RelativeItem().Column(x => { Label(x, "STATEMENT DATE", Date(DateTime.Today)); Label(x, "ADDRESS / LOCATION", _customer.Location ?? _customer.Address ?? "-"); }); });
                col.Item().PaddingTop(10).LineHorizontal(1).LineColor(PdfStyle.Ink); col.Item().PaddingTop(17).Text("ACCOUNT SUMMARY").FontSize(10).Bold().FontColor(PdfStyle.GoldDark); col.Item().PaddingTop(7).Element(x => SummaryBand(x, _statement.Summary.TotalEstimated, _statement.Summary.TotalReceived, _statement.Summary.Outstanding));
                Watermark(col, _logoPath); col.Item().PaddingTop(6).Text("ACCOUNT ACTIVITY").FontSize(10).Bold().FontColor(PdfStyle.GoldDark);
                col.Item().PaddingTop(7).Table(table => { table.ColumnsDefinition(c => { c.ConstantColumn(72); c.RelativeColumn(); c.ConstantColumn(68); c.ConstantColumn(79); c.ConstantColumn(79); c.ConstantColumn(85); }); table.Header(h => { TableHeader(h.Cell(), "DATE"); TableHeader(h.Cell(), "REFERENCE"); TableHeader(h.Cell(), "TYPE"); TableHeader(h.Cell(), "DEBIT (₹)", true); TableHeader(h.Cell(), "CREDIT (₹)", true); TableHeader(h.Cell(), "BALANCE (₹)", true); }); foreach (var line in _statement.Lines) { table.Cell().BorderBottom(1).BorderColor(PdfStyle.Line).Padding(8).Text(line.Date.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)); table.Cell().BorderBottom(1).BorderColor(PdfStyle.Line).Padding(8).Text(line.Reference); table.Cell().BorderBottom(1).BorderColor(PdfStyle.Line).Padding(8).Text(line.Type); table.Cell().BorderBottom(1).BorderColor(PdfStyle.Line).Padding(8).AlignRight().Text(line.Debit > 0 ? Money(line.Debit) : string.Empty); table.Cell().BorderBottom(1).BorderColor(PdfStyle.Line).Padding(8).AlignRight().Text(line.Credit > 0 ? Money(line.Credit) : string.Empty); table.Cell().BorderBottom(1).BorderColor(PdfStyle.Line).Padding(8).AlignRight().Text(Money(line.RunningBalance)).Bold(); } });
                col.Item().PaddingTop(18).Element(x => Balance(x, "CURRENT OUTSTANDING", _statement.Summary.Outstanding));
            }); Footer(page, "CUSTOMER STATEMENT"); });
        }
    }
}
