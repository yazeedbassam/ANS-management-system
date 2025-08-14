// File: Controllers/DashboardController.cs (مثال، قد يكون اسم الكنترولر مختلفًا لديك)
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Linq;
using System; // لـ DateTime
using System.Data; // لـ DataRow
using WebApplication1.DataAccess; // تأكد من أنك تستخدم SqlDb هنا
using Microsoft.Data.SqlClient; // تأكد من استخدام هذا الـ namespace
using System.Data;
using WebApplication1.ViewModels;             // مطلوب لـ DataTable

public class DashboardController : Controller
{
    private readonly SqlServerDb _db;

    public DashboardController(SqlServerDb db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var viewModel = new DashboardViewModel
        {
            TotalControllers = _db.GetControllersCount(),
            TotalLicenses = _db.GetTotalLicensesCount(),
            ExpiredLicenses = _db.GetExpiredLicensesCount(),
            SoonExpiringLicenses = _db.GetSoonExpiringLicensesCount(),
            CertificateStats = _db.GetCertificatesStats(),
            ObservationsData = _db.GetAllObservationsbyDashboard(),

            ControllerDetails = _db.GetControllerDetails(),
            AllLicensesDetails = _db.GetAllLicensesDetails(), // الآن ستتضمن اسم المراقب
            ExpiredLicensesDetails = _db.GetExpiredLicensesDetails(),
            SoonExpiringLicensesDetails = _db.GetSoonExpiringLicensesDetails(),

            AllControllersFullDetails = new List<ControllerFullDetail>() // تهيئة القائمة الجديدة
        };

        // ملء AllControllersFullDetails
        var allControllersDt = _db.GetAllControllers(); // جلب جميع المراقبين
        foreach (DataRow controllerRow in allControllersDt.Rows)
        {
            // تأكد أن الأعمدة موجودة في DataTable التي ترجعها GetAllControllers()
            int controllerId = Convert.ToInt32(controllerRow["controllerid"]);
            string controllerFullName = controllerRow["fullname"].ToString();
            string controllerUsername = controllerRow["username"].ToString(); // نحتاج اسم المستخدم لجلب الرخص والشهادات

            // جلب الرخص لهذا المراقب
            var licensesForController = _db.GetLicensesByController(controllerUsername);
            var licenseDetailsList = licensesForController.Select(l => $"{l.TypeName ?? "غير معروف"} (تنتهي في {(l.ExpiryDate.HasValue ? l.ExpiryDate.Value.ToShortDateString() : "غير محدد")})").ToList();
            var licenseExpiryDatesList = licensesForController.Select(l => (DateTime?)l.ExpiryDate).ToList();

            // جلب الشهادات لهذا المراقب
            var certificatesForController = _db.GetCertificatesByController(controllerUsername);
            var certificateDetailsList = certificatesForController.Select(c => $"{c.Title} (صادرة في {c.IssueDate.ToShortDateString()})").ToList();

            viewModel.AllControllersFullDetails.Add(new ControllerFullDetail
            {
                FullName = controllerFullName,
                LicenseDetails = licenseDetailsList,
                CertificateDetails = certificateDetailsList,
                LicenseExpiryDates = licenseExpiryDatesList
            });
        }

        return View(viewModel);
    }
}