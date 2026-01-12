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

        [Required(ErrorMessage = "Department name is required.")]
        [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters.")]
        public string DepartmentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [Phone(ErrorMessage = "Invalid mobile number format.")]
        [StringLength(15, ErrorMessage = "Mobile number cannot exceed 15 characters.")]
        public string MobileNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(150, ErrorMessage = "Email address cannot exceed 150 characters.")]
        public string EmailAddress { get; set; } = string.Empty;
    }
}