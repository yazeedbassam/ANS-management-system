using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess;
using System.Security.Claims;
using System.Data;
using System;
using Npgsql;

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

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return View((object)0); // إرجاع 0 إذا لم يتم العثور على معرف المستخدم أو كان غير صالح
        }

        int notificationCount = 0; // تهيئة عدد الإشعارات

        // استخدام PostgreSQL connection
        using (var connection = _db.GetConnection())
        {
            connection.Open();
            using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM notifications WHERE userid = @userId AND is_read = false", connection))
            {
                cmd.Parameters.Add(new NpgsqlParameter("@userId", userId));

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