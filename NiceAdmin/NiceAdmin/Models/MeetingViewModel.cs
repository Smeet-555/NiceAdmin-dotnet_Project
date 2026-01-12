using System;
using System.ComponentModel.DataAnnotations;

namespace NiceAdmin.Models
{
    public class MeetingViewModel
    {
        [Key]
        public int MeetingId { get; set; }

        [Required(ErrorMessage = "Meeting date is required.")]
        [DataType(DataType.Date)]
        public DateTime MeetingDate { get; set; }

        [Required(ErrorMessage = "Meeting venue is required.")]
        [StringLength(150, ErrorMessage = "Meeting venue name cannot exceed 150 characters.")]
        public string MeetingVenueName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Meeting type is required.")]
        [StringLength(100, ErrorMessage = "Meeting type name cannot exceed 100 characters.")]
        public string MeetingTypeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department name is required.")]
        [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters.")]
        public string DepartmentName { get; set; } = string.Empty;

        [Required]
        public bool IsCancelled { get; set; }
    }
}