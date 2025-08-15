using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebApplication1.DataAccess;
using WebApplication1.Models;
using Microsoft.Data.SqlClient; // تأكد من استخدام هذا الـ namespace
using System.Data;             // مطلوب لـ DataTable
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// إضافة خدمات قاعدة البيانات مع دعم متغيرات البيئة
builder.Services.AddSingleton<SqlServerDb>(provider =>
{
    // استخدام متغير البيئة مباشرة
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
        ?? builder.Configuration.GetConnectionString("DefaultConnection");
    
    // طباعة للتحقق
    Console.WriteLine($"Using connection string: {connectionString}");
    
    return new SqlServerDb(connectionString);
});
builder.Services.AddSingleton<IPasswordHasher<ControllerUser>, PasswordHasher<ControllerUser>>();

// إضافة الخدمة فقط في بيئة التطوير
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<LicenseExpiryNotificationService>();
    builder.Services.AddScoped<LicenseExpiryNotificationService>();
}

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // مثال لمدة انتهاء الصلاحية
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAuthorization();

builder.Services.AddAuthorization(opt => {
    opt.FallbackPolicy = new AuthorizationPolicyBuilder()
                         .RequireAuthenticatedUser()
                         .Build();
    opt.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
    opt.AddPolicy("RequireController", p => p.RequireRole("Controller", "Admin"));
});
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 2) Seed Admin user - فقط في بيئة التطوير
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<SqlServerDb>();
            if (db.GetUserByUsername("admin") == null)
            {
                db.CreateUser("admin", "123", "Admin");
                Console.WriteLine("Admin user created successfully");
            }
            else
            {
                Console.WriteLine("Admin user already exists");
            }
        }
        catch (Exception ex)
        {
            // تسجيل الخطأ ولكن لا توقف التطبيق
            Console.WriteLine($"Warning: Could not seed admin user: {ex.Message}");
            Console.WriteLine($"Connection string used: {Environment.GetEnvironmentVariable("DATABASE_URL")}");
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// إضافة route للصفحة الرئيسية بدون مصادقة - بسيط جداً
app.MapGet("/", () => "OK");

// إضافة route للـ health check
app.MapGet("/health", () => "OK");

app.Run();

