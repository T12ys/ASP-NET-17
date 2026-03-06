using ASP_09._Swagger_documentation.Data;
using ASP_09._Swagger_documentation.Models;
using ASP_09._Swagger_documentation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ASP_09._Swagger_documentation.Services;

public class InvoicePdfService : IInvoicePdfService
{
    private readonly AppDbContext _context;

    public InvoicePdfService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]?> GenerateAsync(int userId, int invoiceId)
    {
        // Получаем инвойс только если принадлежит юзеру
        var invoice = await _context.Invoices
            .Include(i => i.Rows)
            .FirstOrDefaultAsync(i =>
                i.Id == invoiceId &&
                i.DeletedAt == null &&
                _context.Customers.Any(c => c.Id == i.CustomerId && c.UserId == userId));

        if (invoice is null) return null;

        // Получаем клиента
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == invoice.CustomerId);

        // Получаем юзера
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        //лицензия QuestPDF (бесплатная)
        QuestPDF.Settings.License = LicenseType.Community;

        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text($"ИНВОЙС #{invoice.Id}")
                            .FontSize(24).Bold();
                        col.Item().Text($"Статус: {invoice.Status}")
                            .FontSize(11).FontColor(GetStatusColor(invoice.Status));
                    });

                    row.ConstantItem(150).Column(col =>
                    {
                        col.Item().AlignRight().Text($"Дата создания:")
                            .FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().AlignRight().Text(invoice.CreatedAt.ToString("dd.MM.yyyy"))
                            .FontSize(11);
                        col.Item().AlignRight().Text($"Период работ:")
                            .FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().AlignRight().Text(
                            $"{invoice.StartDate:dd.MM.yyyy} — {invoice.EndDate:dd.MM.yyyy}")
                            .FontSize(11);
                    });
                });

                page.Content().PaddingTop(20).Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        // Исполнитель (юзер)
                        row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2)
                            .Padding(10).Column(c =>
                            {
                                c.Item().Text("ОТ:").Bold().FontSize(10)
                                    .FontColor(Colors.Grey.Medium);
                                c.Item().Text(user?.Name ?? "—").Bold();
                                if (!string.IsNullOrEmpty(user?.Email))
                                    c.Item().Text(user.Email).FontColor(Colors.Grey.Darken1);
                                if (!string.IsNullOrEmpty(user?.PhoneNumber))
                                    c.Item().Text(user.PhoneNumber).FontColor(Colors.Grey.Darken1);
                                if (!string.IsNullOrEmpty(user?.Address))
                                    c.Item().Text(user.Address).FontColor(Colors.Grey.Darken1);
                            });

                        row.ConstantItem(20); 

                        row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2)
                            .Padding(10).Column(c =>
                            {
                                c.Item().Text("КОМУ:").Bold().FontSize(10)
                                    .FontColor(Colors.Grey.Medium);
                                c.Item().Text(customer?.Name ?? "—").Bold();
                                if (!string.IsNullOrEmpty(customer?.Email))
                                    c.Item().Text(customer.Email).FontColor(Colors.Grey.Darken1);
                                if (!string.IsNullOrEmpty(customer?.PhoneNumber))
                                    c.Item().Text(customer.PhoneNumber).FontColor(Colors.Grey.Darken1);
                                if (!string.IsNullOrEmpty(customer?.Address))
                                    c.Item().Text(customer.Address).FontColor(Colors.Grey.Darken1);
                            });
                    });

                    col.Item().PaddingTop(20);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4); // Услуга
                            columns.RelativeColumn(1); // Кол-во
                            columns.RelativeColumn(1.5f); // Цена за ед.
                            columns.RelativeColumn(1.5f); // Сумма
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Darken3).Padding(8)
                                .Text("Услуга").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Grey.Darken3).Padding(8)
                                .AlignCenter().Text("Кол-во").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Grey.Darken3).Padding(8)
                                .AlignRight().Text("Цена за ед.").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Grey.Darken3).Padding(8)
                                .AlignRight().Text("Сумма").FontColor(Colors.White).Bold();
                        });

                        var isEven = false;
                        foreach (var row in invoice.Rows)
                        {
                            var bgColor = isEven ? Colors.Grey.Lighten4 : Colors.White;
                            isEven = !isEven;

                            table.Cell().Background(bgColor).Padding(8).Text(row.Service);
                            table.Cell().Background(bgColor).Padding(8).AlignCenter()
                                .Text(row.Quantity.ToString("G29"));
                            table.Cell().Background(bgColor).Padding(8).AlignRight()
                                .Text($"{row.Rate:N2}");
                            table.Cell().Background(bgColor).Padding(8).AlignRight()
                                .Text($"{row.Sum:N2}");
                        }
                    });

                    col.Item().PaddingTop(10);

                    col.Item().AlignRight().Row(row =>
                    {
                        row.ConstantItem(200).Border(1).BorderColor(Colors.Grey.Lighten2)
                            .Padding(10).Row(r =>
                            {
                                r.RelativeItem().Text("ИТОГО:").Bold().FontSize(13);
                                r.RelativeItem().AlignRight()
                                    .Text($"{invoice.TotalSum:N2}").Bold().FontSize(13);
                            });
                    });

                    if (!string.IsNullOrEmpty(invoice.Comment))
                    {
                        col.Item().PaddingTop(20).Column(c =>
                        {
                            c.Item().Text("Комментарий:").Bold()
                                .FontColor(Colors.Grey.Medium).FontSize(10);
                            c.Item().Text(invoice.Comment).FontColor(Colors.Grey.Darken1);
                        });
                    }
                });

                page.Footer().AlignCenter()
                    .Text($"Сгенерировано {DateTime.UtcNow:dd.MM.yyyy HH:mm} UTC")
                    .FontSize(9).FontColor(Colors.Grey.Medium);
            });
        }).GeneratePdf();

        return pdfBytes;
    }

    private static string GetStatusColor(InvoiceStatus status) => status switch
    {
        InvoiceStatus.Paid => Colors.Green.Darken2,
        InvoiceStatus.Cancelled => Colors.Red.Darken2,
        InvoiceStatus.Rejected => Colors.Red.Darken2,
        InvoiceStatus.Sent => Colors.Blue.Darken2,
        InvoiceStatus.Received => Colors.Blue.Darken1,
        _ => Colors.Grey.Darken2
    };
}