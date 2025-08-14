using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Models; // تأكد من أن هذا هو اسم الـ namespace الصحيح لمشروعك

public class Employee
{
    public int EmployeeID { get; set; }
    public string? EmployeeOfficialID { get; set; }
    public int? UserID { get; set; } // وضعنا علامة الاستفهام لأنه يمكن أن يكون فارغاً (NULL)
    public required string FullName { get; set; }
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public bool IsActive { get; set; }
    public string? Address { get; set; }
    public string? Location { get; set; } // الحقل الجديد الذي أضفته
    public string? EmergencyContactPhone { get; set; }

    // ==> أضف الخاصية الجديدة هنا <==
    public string? Gender { get; set; }
    // هذه خاصية إضافية سنستخدمها لاحقاً لعرض اسم المستخدم بجانب الموظف
    public string? Username { get; set; }



    public string? EducationLevel { get; set; }
    public string? Status { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? MaritalStatus { get; set; }
    public string? PhotoPath { get; set; }
}
