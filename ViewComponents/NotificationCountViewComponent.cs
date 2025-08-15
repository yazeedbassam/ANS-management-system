using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess; // تأكد من هذا الـ namespace
using System.Security.Claims;
//using Oracle.ManagedDataAccess.Client;
using System.Data;
using Microsoft.Data.SqlClient; // تأكد من وجود هذا الـ namespace
using System.Data;             // مطلوب لـ DataTable
using WebApplication1.DataAccess; // تأكد أن SqlServerDb أو SqlDb هنا
using System;                  // مطلوب لـ Convert.ToInt32
using Microsoft.AspNetCore.Mvc; // مطلوب لـ IViewComponentResult و View
using System.Security.Claims; // مطلوب لـ ClaimTypes
using WebApplication1.DataAccess; // تأكد من أنك تستخدم SqlDb هنا
using Microsoft.Data.SqlClient; // تأكد من استخدام هذا الـ namespace
using System.Data;             // مطلوب لـ DataTable

public class NotificationCountViewComponent : ViewComponent
{
    private readonly PostgreSQLDb _db;

    public NotificationCountViewComponent(PostgreSQLDb db)
    {
        _db = db;
    }

    public IViewComponentResult Invoke()
    {
        // استخدام User بدلاً من UserClaimsPrincipal للوصول إلى بيانات المستخدم المصادق عليه
        if (!User.Identity.IsAuthenticated)
        {
            return View((object)0); // إرجاع 0 إذا لم يكن المستخدم مصادقًا
        }

        //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userIdClaim = ((ClaimsPrincipal)User).FindFirst(ClaimTypes.NameIdentifier); // <== تم التعديل هنا

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return View((object)0); // إرجاع 0 إذا لم يتم العثور على معرف المستخدم أو كان غير صالح
        }

        int notificationCount = 0; // تهيئة عدد الإشعارات

        // استخدام SqlConnection و SqlCommand و SqlParameter
        // نفترض أن _db.GetConnection() ترجع SqlConnection
        using (var connection = _db.GetConnection())
        {
            connection.Open();
            // تم تعديل SQL: :userId إلى @userId
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM notifications WHERE userid = @userId AND is_read = 0", connection))
            {
                // تعريف المعامل باستخدام Microsoft.Data.SqlClient.SqlParameter
                cmd.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@userId", SqlDbType.Int) { Value = userId }); // <== تم التعديل

                // استخدام ExecuteScalar لجلب قيمة واحدة (COUNT(*)) بكفاءة أكبر
                object result = cmd.ExecuteScalar();

                if (result != DBNull.Value && result != null)
                {
                    notificationCount = Convert.ToInt32(result);
                }
            }
        }

        return View((object)notificationCount); // إرجاع الـ View مع عدد الإشعارات
    }
}