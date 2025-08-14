using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class CreateEmployeeViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    [Display(Name = "Employee Official ID")]
    public string? EmployeeOfficialID { get; set; }

    [Display(Name = "Job Title")]
    public string? JobTitle { get; set; }

    public string? Department { get; set; }

    public string? Location { get; set; }

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    // ===== الحقول الجديدة المضافة =====
    [DataType(DataType.Date)]
    [Display(Name = "Hire Date")]
    public DateTime? HireDate { get; set; } = DateTime.Today; // قيمة افتراضية بتاريخ اليوم

    public string? Address { get; set; }

    [Display(Name = "Emergency Contact Phone")]
    public string? EmergencyContactPhone { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true; // قيمة افتراضية أن الموظف فعال
                                               // ===== نهاية الحقول الجديدة =====
    public string? Gender { get; set; }

    // --- حقول خاصة بإنشاء المستخدم ---
    [Required]
    public string Username { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [Display(Name = "Role")]
    public string RoleName { get; set; }
}