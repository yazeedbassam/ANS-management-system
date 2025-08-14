// File: Controllers/DataDetailsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Data; // لـ DataTable

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")] // يمكن تقييد الوصول للمدراء فقط
    public class DataDetailsController : Controller
    {
        private readonly SqlServerDb _db;

        public DataDetailsController(SqlServerDb db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var viewModel = new AllDetailsViewModel();

            // جلب جميع البيانات باستخدام الدوال الموجودة في SqlServerDb
            var controllersDt = _db.GetAllControllers();
            viewModel.AllControllers = controllersDt.AsEnumerable().Select(row => new ControllerUser
            {
                ControllerId = Convert.ToInt32(row["controllerid"]),
                FullName = row["fullname"].ToString(),
                Username = row["username"].ToString(),
                Email = row["email"]?.ToString(),
                PhoneNumber = row["phone_number"]?.ToString(),
                JobTitle = row["job_title"]?.ToString(),
                CurrentDepartment = row["current_department"]?.ToString(),
                EmploymentStatus = row["employment_status"]?.ToString(),
                // أضف بقية الخصائص التي تحتاجها
            }).ToList();

            viewModel.AllEmployees = _db.GetEmployees(null, null, null, null, null, null, null, null, null);

            // الآن نستخدم الدوال الجديدة التي تجمع البيانات
            viewModel.AllLicenses = _db.GetAllLicensesDetailsMix();
            viewModel.AllCertificates = _db.GetAllCertificatesDetailsMix();
            viewModel.AllObservations = _db.GetAllObservationsDetails();

            viewModel.AllProjects = _db.GetAllProjects();

            return View(viewModel);
        }
    }
}
