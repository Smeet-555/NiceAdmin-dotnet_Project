using System.ComponentModel.DataAnnotations;

namespace NiceAdmin.Models
{
    public class MeetingMemberViewModel
    {
        [Key]
        public int MeetingMemberId { get; set; }

        [Required]
        public int MeetingId { get; set; }

        [Required]
        public int StaffId { get; set; }

        // Display properties - not required for form submission
        [StringLength(200, ErrorMessage = "Meeting description cannot exceed 200 characters.")]
        public string MeetingDescription { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Staff name cannot exceed 100 characters.")]
        public string StaffName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters.")]
        public string DepartmentName { get; set; } = string.Empty;

        [Required]
        public bool IsPresent { get; set; }

        [StringLength(300, ErrorMessage = "Remarks cannot exceed 300 characters.")]
        public string? Remarks { get; set; }
    }
}