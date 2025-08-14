using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data.SqlClient;
using System.Drawing;
using System.IO; // <-- إضافة مهمة لاستخدام Path و File
using WebApplication1.DataAccess;
using WebApplication1.Models;
using Color = System.Drawing.Color;

namespace WebApplication1.Controllers;

[Authorize(Policy = "RequireAdmin")] // فقط الأدمن يستطيع الوصول لهذا الكنترولر
public class EmployeesController : Controller
{
    private readonly SqlServerDb _db;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public EmployeesController(SqlServerDb db, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _webHostEnvironment = webHostEnvironment; // قم بتعيينها هنا

    }

    // GET: /Employees
    public IActionResult Index()
    {
        // نستدعي الدالة الجديدة بـ 8 قيم null لتتوافق مع تعريفها الجديد
        var employees = _db.GetEmployees(null, null, null, null, null, null, null, null, null);

        return View(employees);
    }

    // POST: /Employees/Create
    // هذه الدالة وظيفتها استقبال البيانات بعد الضغط على زر "Create Employee"
    // في ملف EmployeesController.cs

    // =============================================================
    // الدالة الأولى: لعرض الفورم الفارغ للمستخدم (GET)
    // لاحظ: لا يوجد متغيرات (parameters) في هذه الدالة
    // =============================================================
    public IActionResult Create()
    {
        // 1. نجهز القوائم المنسدلة
        ViewBag.JobTitles = new List<SelectListItem>
    {
        new() { Value = "AIS Officer", Text = "AIS Officer" },
        new() { Value = "AIS Technician", Text = "AIS Technician" },
        new() { Value = "AFTN Technician", Text = "AFTN Technician" },
        new() { Value = "Assistant", Text = "Assistant" },
        new() { Value = "Supervisor", Text = "Supervisor" },
        new() { Value = "Section Head", Text = "Section Head" },
        new() { Value = "Manager", Text = "Manager" },
        new() { Value = "Data Analyst", Text = "Data Analyst" },
        new() { Value = "Coordinator", Text = "Coordinator" },
        new() { Value = "Data Quality Specialist", Text = "Data Quality Specialist" }
    };

        ViewBag.Departments = new List<SelectListItem>
    {
        new() { Value = "AIS - Aeronautical Information Services", Text = "AIS - Aeronautical Information Services" },
        new() { Value = "CNS - Communication, Navigation, and Surveillance", Text = "CNS - Communication, Navigation, and Surveillance" },
        new() { Value = "Airspace Management - ASM", Text = "Airspace Management - ASM" },
        new() { Value = "ATFM - Air Traffic Flow Management", Text = "ATFM - Air Traffic Flow Management" },
        new() { Value = "Safety & Quality Management", Text = "Safety & Quality Management" }
    };

        ViewBag.Roles = new SelectList(_db.GetAllRoles(), "RoleName", "RoleName");

        // 2. نعرض الصفحة مع موديل فارغ
        return View(new CreateEmployeeViewModel());
    }


    // =================================================================
    // الدالة الثانية: لمعالجة البيانات بعد الضغط على زر الحفظ (POST)
    // لاحظ: هذه الدالة تستقبل الموديل ولديها [HttpPost]
    // =================================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateEmployeeViewModel model)
    {
        // التحقق من اسم المستخدم والإيميل
        if (_db.GetUserByUsername(model.Username) != null)
        {
            ModelState.AddModelError("Username", "This username is already taken.");
        }
        //if (_db.EmployeeEmailExists(model.Email))
        //{
        //    ModelState.AddModelError("Email", "This email address is already in use.");
        //}

        if (ModelState.IsValid) // هذا الشرط سيتحقق الآن من الأخطاء التي أضفتها يدوياً
        {
            try
            {
                _db.CreateEmployeeAndUser(model);
                TempData["SuccessMessage"] = "Employee created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred: " + ex.Message);
            }
        }

        // في حالة الفشل، نعيد تعبئة القوائم المنسدلة ونعرض الصفحة من جديد
        // ... (باقي الكود كما هو لإعادة تعبئة ViewBag) ...
        ViewBag.JobTitles = new List<SelectListItem>
    {
        new() { Value = "AIS Officer", Text = "AIS Officer" },
        new() { Value = "AIS Technician", Text = "AIS Technician" },
        new() { Value = "Assistant", Text = "Assistant" },
        new() { Value = "Supervisor", Text = "Supervisor" },
        new() { Value = "Section Head", Text = "Section Head" },
        new() { Value = "Manager", Text = "Manager" },
        new() { Value = "Data Analyst", Text = "Data Analyst" },
        new() { Value = "Coordinator", Text = "Coordinator" },
        new() { Value = "Data Quality Specialist", Text = "Data Quality Specialist" }
    };
        ViewBag.Departments = new List<SelectListItem>
    {
        new() { Value = "AIS - Aeronautical Information Services", Text = "AIS - Aeronautical Information Services" },
        new() { Value = "CNS - Communication, Navigation, and Surveillance", Text = "CNS - Communication, Navigation, and Surveillance" },
        new() { Value = "Airspace Management - ASM", Text = "Airspace Management - ASM" },
        new() { Value = "ATFM - Air Traffic Flow Management", Text = "ATFM - Air Traffic Flow Management" },
        new() { Value = "Safety & Quality Management", Text = "Safety & Quality Management" }
    };
        ViewBag.Roles = new SelectList(_db.GetAllRoles(), "RoleName", "RoleName");

        return View(model);
    }
    // GET: /Employees/Edit/5
    public IActionResult Edit(int id)
    {// 1. نجهز قائمة المسميات الوظيفية
        ViewBag.JobTitles = new List<SelectListItem>
    {
        new() { Value = "Officer", Text = "Officer" },
        new() { Value = "Technician", Text = "Technician" },
        new() { Value = "Assistant", Text = "Assistant" },
        new() { Value = "Supervisor", Text = "Supervisor" },
        new() { Value = "Section Head", Text = "Section Head" },
        new() { Value = "Manager", Text = "Manager" },
        new() { Value = "Data Analyst", Text = "Data Analyst" },
        new() { Value = "Coordinator", Text = "Coordinator" },
        new() { Value = "Data Quality Specialist", Text = "Data Quality Specialist" }
    };

        // 2. نجهز قائمة الأقسام
        ViewBag.Departments = new List<SelectListItem>
    {
        new() { Value = "AIS - Aeronautical Information Services", Text = "AIS - Aeronautical Information Services" },
        new() { Value = "CNS - Communication, Navigation, and Surveillance", Text = "CNS - Communication, Navigation, and Surveillance" },
        new() { Value = "Airspace Management - ASM", Text = "Airspace Management - ASM" },
        new() { Value = "ATFM - Air Traffic Flow Management", Text = "ATFM - Air Traffic Flow Management" },
        new() { Value = "Safety & Quality Management", Text = "Safety & Quality Management" }
    };
        ViewBag.Gender = new List<SelectListItem>
    {
        new() { Value = "Male", Text = "Male" },
        new() { Value = "Female", Text = "Female" }
    };
        var employee = _db.GetEmployeeById(id);
        if (employee == null)
        {
            return NotFound();
        }
        return View(employee);
    }

    // POST: /Employees/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Employee employee)
    {
        if (id != employee.EmployeeID)
        {
            return BadRequest();
        }

        // إزالة حقول معينة من التحقق لأنها غير موجودة في الفورم
        ModelState.Remove("EmployeeOfficialID");
        ModelState.Remove("Email");


        if (ModelState.IsValid)
        {
            try
            {
                _db.UpdateEmployee(employee);
                TempData["SuccessMessage"] = "Employee updated successfully!";
            }
            catch (Exception)
            {
                // يمكنك معالجة الخطأ هنا
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(employee);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        try
        {
            // نتأكد أولاً أن الموظف موجود (خطوة اختيارية لكنها جيدة)
            var employee = _db.GetEmployeeById(id);
            if (employee == null)
            {
                TempData["Error"] = "Employee not found.";
                return RedirectToAction(nameof(Index));
            }

            // استدعاء ميثود الحذف الجديدة
            _db.DeleteEmployee(id);

            TempData["SuccessMessage"] = "Employee has been deleted successfully.";
        }
        catch (Exception ex)
        {
            // في حال حدوث أي خطأ، نعرض رسالة خطأ
            TempData["Error"] = $"Error deleting employee: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
    // أضف هذه الدوال داخل EmployeesController.cs

    public IActionResult ExportToPDF(string fullName, string employeeOfficialID, string jobTitle, string department, string username)
    {
        // 1. تحديد الترخيص
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        // 2. جلب البيانات المفلترة
        var filteredEmployees = _db.GetEmployees(fullName, employeeOfficialID, jobTitle, department, username, null, null, null, null);
        var recordCount = filteredEmployees.Count;

        // 3. تعريف دوال التنسيق (Styles)
        IContainer HeaderStyle(IContainer container) => container
            .Background(Colors.Blue.Medium)
            .PaddingVertical(4).PaddingHorizontal(6)
            .AlignCenter()
            .DefaultTextStyle(x => x.FontColor(Colors.White).FontSize(9).Bold()); // تصغير خط الهيدر

        IContainer BodyCellStyle(IContainer container) => container
            .BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(4).PaddingHorizontal(6)
            .DefaultTextStyle(x => x.FontSize(8)); // تصغير خط المحتوى

        // 4. إنشاء المستند
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre); // تقليل الهوامش قليلاً
                page.DefaultTextStyle(x => x.FontFamily("Arial"));

                // تصميم رأس الصفحة (Header)
                page.Header().Column(headerCol =>
                {
                    headerCol.Item().Row(row =>
                    {
                        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "carc.png");
                        if (System.IO.File.Exists(logoPath))
                        {
                            var logoBytes = System.IO.File.ReadAllBytes(logoPath);
                            row.ConstantColumn(70).Image(logoBytes); // تصغير حجم الشعار قليلاً
                        }

                        row.RelativeColumn().Column(col =>
                        {
                            col.Item().AlignCenter().Text("هيئة تنظيم الطيران المدني الأردني").Bold().FontSize(12); // تصغير الخط
                            col.Item().AlignCenter().Text("JORDAN CIVIL AVIATION REGULATORY COMMISSION").FontSize(9).FontColor(Colors.Grey.Darken1); // تصغير الخط
                            col.Item().PaddingTop(5).AlignCenter().Text($"Employees Report - {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(8).FontColor(Colors.Grey.Darken2); // تصغير الخط
                        });
                    });
                    headerCol.Item().PaddingTop(10); // تقليل المسافة
                    headerCol.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    headerCol.Item().PaddingTop(5);
                });

                // محتوى الصفحة (Content)
                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(25);         // #
                        columns.RelativeColumn(2.5f);       // Full Name
                        columns.RelativeColumn(1.5f);       // Employee ID
                        columns.RelativeColumn(2f);         // Job Title
                        columns.RelativeColumn(2.5f);       // Department
                        columns.RelativeColumn(1.5f);       // Username
                        columns.RelativeColumn(1f);         // Status
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("#");
                        header.Cell().Element(HeaderStyle).Text("Full Name");
                        header.Cell().Element(HeaderStyle).Text("User ID");
                        header.Cell().Element(HeaderStyle).Text("Job Title");
                        header.Cell().Element(HeaderStyle).Text("Department");
                        header.Cell().Element(HeaderStyle).Text("Username");
                        header.Cell().Element(HeaderStyle).Text("Status");
                    });

                    int index = 1;
                    foreach (var emp in filteredEmployees)
                    {
                        table.Cell().Element(BodyCellStyle).AlignCenter().Text(index++.ToString());
                        table.Cell().Element(BodyCellStyle).Text(emp.FullName ?? "-");
                        table.Cell().Element(BodyCellStyle).Text(emp.EmployeeOfficialID ?? "-");
                        table.Cell().Element(BodyCellStyle).Text(emp.JobTitle ?? "-");
                        table.Cell().Element(BodyCellStyle).Text(emp.Department ?? "-");
                        table.Cell().Element(BodyCellStyle).Text(emp.Username ?? "-");
                        table.Cell().Element(BodyCellStyle).Text(emp.IsActive ? "Active" : "Inactive");
                    }
                });

                // تصميم تذييل الصفحة (Footer)
                page.Footer().Row(row =>
                {
                    row.RelativeColumn().Text(txt =>
                    {
                        txt.DefaultTextStyle(x => x.FontSize(7).FontColor(Colors.Grey.Darken1)); // تصغير الخط
                        txt.Span($"Total Records: {recordCount}");
                    });

                    row.RelativeColumn().AlignRight().Text(txt =>
                    {
                        txt.DefaultTextStyle(x => x.FontSize(7).FontColor(Colors.Grey.Darken1)); // تصغير الخط
                        txt.Span("Page ");
                        txt.CurrentPageNumber();
                        txt.Span(" of ");
                        txt.TotalPages();
                    });
                });
            });
        });

        var pdfBytes = document.GeneratePdf();
        return File(pdfBytes, "application/pdf", $"Employees_List_{DateTime.Now:yyyyMMdd}.pdf");
    }


    public IActionResult ExportToExcel(
        string fullName, string employeeOfficialID, string jobTitle, string department,
        string username, string phoneNumber, string email, string location, string? gender)
    {
        // 1. تحديد ترخيص استخدام المكتبة
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        // 2. جلب البيانات المفلترة بنفس الطريقة
        var employees = _db.GetEmployees(
            fullName, employeeOfficialID, jobTitle, department, username,
            phoneNumber, email, location, gender
        );

        // 3. إنشاء ملف الإكسل
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Employees");

            // --- إعدادات وتصميم الهيدر والشعار ---
            worksheet.Cells.Style.Font.Name = "Arial";
            worksheet.View.RightToLeft = false; // للتأكد من أن الورقة من اليسار لليمين

            // إضافة الشعار
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "carc.png");
            if (System.IO.File.Exists(logoPath))
            {
                var excelImage = worksheet.Drawings.AddPicture("Logo", logoPath);
                excelImage.SetPosition(0, 0, 0, 15); // (row, row offset, col, col offset)
                excelImage.SetSize(120, 65); // تعديل حجم الشعار ليكون مناسبًا
            }

            // إضافة العناوين الرئيسية
            worksheet.Cells["C1"].Value = "هيئة تنظيم الطيران المدني الأردني";
            worksheet.Cells["C1"].Style.Font.Bold = true;
            worksheet.Cells["C1"].Style.Font.Size = 14;
            worksheet.Cells["C1:H1"].Merge = true;
            worksheet.Cells["C1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells["C2"].Value = "JORDAN CIVIL AVIATION REGULATORY COMMISSION";
            worksheet.Cells["C2"].Style.Font.Size = 10;
            worksheet.Cells["C2:H2"].Merge = true;
            worksheet.Cells["C2:H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells["C3"].Value = $"Employees Report - {DateTime.Now:yyyy-MM-dd}";
            worksheet.Cells["C3"].Style.Font.Size = 9;
            worksheet.Cells["C3"].Style.Font.Italic = true;
            worksheet.Cells["C3:H3"].Merge = true;
            worksheet.Cells["C3:H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // --- تحديد عناوين الجدول ---
            var headers = new string[]
            {
            "#", "Full Name", "User ID", "Job Title", "Department", "Hire Date", "Gender",
            "Email", "Phone Number", "Emergency Contact", "Location", "Address", "Status", "Username"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[5, i + 1].Value = headers[i];
            }

            // تنسيق صف العناوين
            using (var range = worksheet.Cells[5, 1, 5, headers.Length])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#4F81BD")); // لون أزرق
                range.Style.Font.Color.SetColor(Color.White);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // --- إضافة البيانات ---
            int row = 6;
            int index = 1;
            foreach (var emp in employees)
            {
                worksheet.Cells[row, 1].Value = index++;
                worksheet.Cells[row, 2].Value = emp.FullName;
                worksheet.Cells[row, 3].Value = emp.EmployeeOfficialID;
                worksheet.Cells[row, 4].Value = emp.JobTitle;
                worksheet.Cells[row, 5].Value = emp.Department;
                worksheet.Cells[row, 6].Value = emp.HireDate?.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 7].Value = emp.Gender;
                worksheet.Cells[row, 8].Value = emp.Email;
                worksheet.Cells[row, 9].Value = emp.PhoneNumber;
                worksheet.Cells[row, 10].Value = emp.EmergencyContactPhone;
                worksheet.Cells[row, 11].Value = emp.Location;
                worksheet.Cells[row, 12].Value = emp.Address;
                worksheet.Cells[row, 13].Value = emp.IsActive ? "Active" : "Inactive";
                worksheet.Cells[row, 14].Value = emp.Username;
                row++;
            }

            // تنسيق الخلايا الرقمية والتاريخية
            worksheet.Cells[6, 6, row - 1, 6].Style.Numberformat.Format = "yyyy-mm-dd";
            worksheet.Cells[6, 1, row - 1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // جعل الأعمدة تتناسب مع المحتوى تلقائيًا
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var excelBytes = package.GetAsByteArray();
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Employees_List_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }

    [HttpGet]
    public IActionResult ViewEmployeeDetails(int id)
    {
        try
        {
            // جلب بيانات الموظف الأساسية
            var employee = _db.GetEmployeeById(id);
            if (employee == null)
            {
                return Json(new { success = false, message = "Employee not found" });
            }

            // جلب الرخص الخاصة بالموظف
            var licenses = _db.GetLicensesByEmployeeId(id).Select(l => new
            {
                typeName = l.TypeName,
                issueDate = l.IssueDate?.ToString("yyyy-MM-dd"),
                expiryDate = l.ExpiryDate?.ToString("yyyy-MM-dd"),
                status = l.Status,
                filePath = !string.IsNullOrEmpty(l.FilePath) ? l.FilePath : "#"
            }).ToList();

            // جلب الشهادات الخاصة بالموظف
            var certificates = _db.GetCertificatesByEmployeeId(id).Select(c => new
            {
                typeName = c.TypeName,
                title = c.Title,
                issueDate = c.IssueDate?.ToString("yyyy-MM-dd"),
                expiryDate = c.ExpiryDate?.ToString("yyyy-MM-dd"),
                status = c.Status,
                filePath = !string.IsNullOrEmpty(c.FilePath) ? c.FilePath : "#"
            }).ToList();

            // جلب الملاحظات/السفرات الخاصة بالموظف
            var observations = _db.GetObservationsByEmployeeId(id).Select(o => new
            {
                travelCountry = o.TravelCountry,
                durationDays = o.DurationDays,
                departDate = o.DepartDate?.ToString("yyyy-MM-dd"),
                returnDate = o.ReturnDate?.ToString("yyyy-MM-dd"),
                licenseNumber = o.LicenseNumber,
                notes = o.Notes
            }).ToList();

            // جلب المشاريع التي يشارك فيها الموظف
            var projects = _db.GetProjectsByEmployeeId(id).Select(p => new
            {
                id = p.ProjectId,
                projectName = p.ProjectName,
                description = p.Description,
                startDate = p.StartDate?.ToString("yyyy-MM-dd"),
                endDate = p.EndDate?.ToString("yyyy-MM-dd"),
                location = p.Location,
                status = p.Status,
                participants = _db.GetParticipantsByProjectId(p.ProjectId).Select(participant => new
                {
                    name = participant.Name,
                    role = participant.Role
                }).ToList(),
                divisions = _db.GetDivisionsByProjectId(p.ProjectId),
                files = GetProjectFiles(p.FolderPath)
            }).ToList();

            // إعداد البيانات للإرسال
            var result = new
            {
                success = true,
                employee = new
                {
                    fullName = employee.FullName,
                    username = employee.Username,
                    email = employee.Email,
                    phoneNumber = employee.PhoneNumber,
                    // dateOfBirth = employee.DateOfBirth?.ToString("yyyy-MM-dd"), // تم إزالتها
                    // maritalStatus = employee.MaritalStatus, // تم إزالتها
                    currentDepartment = employee.Department,
                    employmentStatus = employee.IsActive ? "Active" : "Inactive",
                    hireDate = employee.HireDate?.ToString("yyyy-MM-dd"),
                    // educationLevel = employee.EducationLevel, // تم إزالتها
                    address = employee.Address,
                    emergencyContact = employee.EmergencyContactPhone,
                    jobTitle = employee.JobTitle,
                    gender = employee.Gender, // إضافة Gender
                    employeeOfficialID = employee.EmployeeOfficialID, // إضافة EmployeeOfficialID
                    photoPath = !string.IsNullOrEmpty(employee.PhotoPath) ? employee.PhotoPath : "/images/default-avatar.png"
                },
                licenses = licenses,
                certificates = certificates,
                observations = observations,
                projects = projects
            };

            return Json(result);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // دالة مساعدة لجلب ملفات المشروع
    private List<object> GetProjectFiles(string folderPath)
    {
        var files = new List<object>();

        if (string.IsNullOrEmpty(folderPath))
            return files;

        try
        {
            string physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, folderPath.TrimStart('/', '\\'));
            if (Directory.Exists(physicalPath))
            {
                var directoryInfo = new DirectoryInfo(physicalPath);
                foreach (var file in directoryInfo.GetFiles().OrderBy(f => f.Name))
                {
                    files.Add(new
                    {
                        name = file.Name,
                        url = $"{folderPath}/{file.Name}".Replace('\\', '/'),
                        size = FormatFileSize(file.Length)
                    });
                }
            }
        }
        catch (Exception)
        {
            // في حالة حدوث خطأ، نعيد قائمة فارغة
        }

        return files;
    }

    // دالة مساعدة لتنسيق حجم الملف
    private string FormatFileSize(long bytes)
    {
        var unit = 1024;
        if (bytes < unit) return $"{bytes} B";
        var exp = (int)(Math.Log(bytes) / Math.Log(unit));
        var pre = "KMGTPE"[exp - 1];
        return $"{bytes / Math.Pow(unit, exp):F1} {pre}B";
    }
    public IActionResult Details(int id)
    {
        var viewModel = _db.GetEmployeeDetailsById(id);
        if (viewModel == null)
        {
            return NotFound();
        }
        return View(viewModel);
    }



}