using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using WebApplication1.DataAccess;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using OfficeOpenXml;
using SendGrid.Helpers.Mail;
using System.Drawing; // لعمليات الصورة إذا أردت
using System.Linq; // Required for .Any()

public class NotificationController : Controller
{
    private readonly SqlServerDb _db;

    public NotificationController(SqlServerDb db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult ExportNotificationsToPDF(string filter = "")
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        // --- FIX V2: Pass null for empty/whitespace filters ---
        // If the filter is empty, pass null to GetNotifications to signify "no filter".
        string effectiveFilter = string.IsNullOrWhiteSpace(filter) ? null : filter;
        var notifications = _db.GetNotifications(effectiveFilter);

        // --- DEBUGGING CHECK ---
        // Check if any data was returned from the database.
        if (notifications == null || !notifications.Any())
        {
            return NotFound("No notifications found to export. The database query returned no results for the given filter.");
        }

        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "carc.png");

        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape()); // صفحة عرضية
                page.Margin(30);

                // رأس الصفحة مع الشعار والعنوان
                page.Header()
                    .AlignCenter()
                    .Column(col =>
                    {
                        if (System.IO.File.Exists(logoPath))
                            col.Item().AlignCenter().Height(70).Image(logoPath, QuestPDF.Infrastructure.ImageScaling.FitHeight);

                        col.Item().PaddingTop(8);
                        col.Item().AlignCenter().Text("تقرير الإشعارات")
                            .Bold().FontSize(28).FontColor(QuestPDF.Helpers.Colors.Blue.Medium);
                        col.Item().AlignCenter().Text(DateTime.Now.ToString("yyyy/MM/dd HH:mm")).FontSize(14).FontColor(QuestPDF.Helpers.Colors.Grey.Medium);
                    });

                // جدول الإشعارات أعرض ومرتب
                page.Content().PaddingTop(22).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);       // #
                        columns.RelativeColumn(1.6f);     // UserId
                        columns.RelativeColumn(2.2f);     // ControllerName
                        // columns.RelativeColumn(3f);       // Message
                        // columns.RelativeColumn(1.6f);     // Link
                        // columns.RelativeColumn(1.8f);     // CreatedAt
                        columns.RelativeColumn(1.6f);     // Note
                        columns.RelativeColumn(1.8f);     // LicenseType
                        columns.RelativeColumn(1.8f);     // LicenseExpiry
                        columns.RelativeColumn(1.8f);     // phone No
                        columns.RelativeColumn(1.9f);     // Email
                        columns.RelativeColumn(1.8f);     // Location
                    });

                    // رأس الجدول
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyleHeader).Text("#");
                        header.Cell().Element(CellStyleHeader).Text("User");
                        header.Cell().Element(CellStyleHeader).Text("Controller Name");
                        //header.Cell().Element(CellStyleHeader).Text("Message");
                        // header.Cell().Element(CellStyleHeader).Text("Link");
                        // header.Cell().Element(CellStyleHeader).Text("Created At");
                        header.Cell().Element(CellStyleHeader).Text("Note");
                        header.Cell().Element(CellStyleHeader).Text("License Type");
                        header.Cell().Element(CellStyleHeader).Text("License Expiry");
                        header.Cell().Element(CellStyleHeader).Text("Phone No");
                        header.Cell().Element(CellStyleHeader).Text("Email");
                        header.Cell().Element(CellStyleHeader).Text("Location");
                    });

                    int idx = 1;
                    foreach (var n in notifications)
                    {
                        table.Cell().Element(CellStyleBody).Text(idx++);
                        table.Cell().Element(CellStyleBody).Text(n.UserId ?? "");
                        // هنا ضع اسم المراقب وليس رقم الـController
                        table.Cell().Element(CellStyleBody).Text(n.FullName ?? "");  // تأكد أن جلب الاسم في الموديل
                        // table.Cell().Element(CellStyleBody).Text(n.Message ?? "");
                        //table.Cell().Element(CellStyleBody).Text(n.Link ?? "");
                        // table.Cell().Element(CellStyleBody).Text(n.CreatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "");
                        table.Cell().Element(CellStyleBody).Text(n.Note ?? "");
                        table.Cell().Element(CellStyleBody).Text(n.LicenseType ?? "");
                        table.Cell().Element(CellStyleBody).Text(n.LicenseExpiryDate?.ToString("yyyy-MM-dd") ?? "");
                        table.Cell().Element(CellStyleBody).Text(n.phonenumber ?? "");
                        table.Cell().Element(CellStyleBody).Text(n.Email ?? "");
                        table.Cell().Element(CellStyleBody).Text(n.Location ?? "");
                    }
                });

                page.Footer()
                    .AlignCenter()
                    .PaddingTop(8)
                    .Text("تقرير إشعارات - نظام إدارة المراقبة الجوية - " + DateTime.Now.ToString("yyyy/MM/dd"))
                    .FontSize(10)
                    .FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
            });
        });

        var pdfBytes = document.GeneratePdf();
        return File(pdfBytes, "application/pdf", "notifications.pdf");

        // تنسيقات الجدول
        IContainer CellStyleHeader(IContainer container) =>
            container
                .Background(QuestPDF.Helpers.Colors.Blue.Medium)
                .PaddingVertical(6).PaddingHorizontal(2)
                .DefaultTextStyle(x => x.FontColor(QuestPDF.Helpers.Colors.White).FontSize(12).SemiBold());

        IContainer CellStyleBody(IContainer container) =>
            container
                .BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2)
                .PaddingVertical(7).PaddingHorizontal(2)
                .DefaultTextStyle(x => x.FontSize(11).FontColor(QuestPDF.Helpers.Colors.Grey.Darken3));
    }

    [HttpGet]
    public IActionResult ExportNotificationsToExcel(string filter = "")
    {
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        // --- FIX V2: Pass null for empty/whitespace filters ---
        // If the filter is empty, pass null to GetNotifications to signify "no filter".
        string effectiveFilter = string.IsNullOrWhiteSpace(filter) ? null : filter;
        var notifications = _db.GetNotifications(effectiveFilter);

        // --- DEBUGGING CHECK ---
        // Check if any data was returned from the database.
        if (notifications == null || !notifications.Any())
        {
            return NotFound("No notifications found to export. The database query returned no results for the given filter.");
        }

        using (var package = new OfficeOpenXml.ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Notifications");

            // إضافة الشعار إذا وجد
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "carc.png");
            if (System.IO.File.Exists(logoPath))
            {
                var excelImage = worksheet.Drawings.AddPicture("Logo", logoPath);
                excelImage.SetPosition(0, 0, 2, 0);
                excelImage.SetSize(130, 70);
            }

            // رأس التقرير
            worksheet.Cells[2, 2, 2, 8].Merge = true;
            worksheet.Cells[2, 2].Value = "تقرير الإشعارات";
            worksheet.Cells[2, 2].Style.Font.Size = 16;
            worksheet.Cells[2, 2].Style.Font.Bold = true;
            worksheet.Cells[2, 2].Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(33, 150, 243));
            worksheet.Cells[2, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.Cells[3, 2, 3, 8].Merge = true;
            worksheet.Cells[3, 2].Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            worksheet.Cells[3, 2].Style.Font.Size = 10;
            worksheet.Cells[3, 2].Style.Font.Color.SetColor(System.Drawing.Color.Gray);
            worksheet.Cells[3, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            // عناوين الأعمدة
            worksheet.Cells[5, 1].Value = "#";
            worksheet.Cells[5, 2].Value = "User";
            worksheet.Cells[5, 3].Value = "Controller Name";
            worksheet.Cells[5, 4].Value = "Message";
            worksheet.Cells[5, 5].Value = "Created At";
            worksheet.Cells[5, 6].Value = "Note";
            worksheet.Cells[5, 7].Value = "License Type";
            worksheet.Cells[5, 8].Value = "License Expiry";
            worksheet.Cells[5, 9].Value = "Phone No";
            worksheet.Cells[5, 10].Value = "Email";
            worksheet.Cells[5, 11].Value = "Location";

            using (var range = worksheet.Cells[5, 1, 5, 11])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(33, 150, 243));
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }

            int row = 6;
            int idx = 1;
            foreach (var n in notifications)
            {
                worksheet.Cells[row, 1].Value = idx++;
                worksheet.Cells[row, 2].Value = n.UserId;
                worksheet.Cells[row, 3].Value = n.FullName; // اسم المراقب الجوي (تأكد موجود في الموديل)
                worksheet.Cells[row, 4].Value = n.Message;
                worksheet.Cells[row, 5].Value = n.CreatedAt?.ToString("yyyy-MM-dd HH:mm");
                worksheet.Cells[row, 6].Value = n.Note;
                worksheet.Cells[row, 7].Value = n.LicenseType;
                worksheet.Cells[row, 8].Value = n.LicenseExpiryDate?.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 9].Value = n.phonenumber;
                worksheet.Cells[row, 10].Value = n.Email;
                worksheet.Cells[row, 11].Value = n.Location;
                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet.View.FreezePanes(6, 1);

            var excelBytes = package.GetAsByteArray();
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "notifications.xlsx");
        }
    }
}
