using System.ComponentModel.DataAnnotations;

namespace NiceAdmin.Models
{
    public class StaffModelView
    {
        [Key]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "Staff name is required.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Staff name must be between 2 and 100 characters.")]
        public string StaffName { get; set; } = string.Empty;

        // Foreign Key
        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        // Display Name - not required for form submission
        [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters.")]
        public string DepartmentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        [StringLength(15, ErrorMessage = "Mobile number cannot exceed 15 characters.")]
        public string MobileNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(150, ErrorMessage = "Email address cannot exceed 150 characters.")]
        public string EmailAddress { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Remarks cannot exceed 300 characters.")]
        public string? Remarks { get; set; }
    }
}