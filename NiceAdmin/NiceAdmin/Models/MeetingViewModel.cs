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

        // Foreign Keys
        [Required(ErrorMessage = "Meeting venue is required.")]
        public int MeetingVenueId { get; set; }
        
        [Required(ErrorMessage = "Meeting type is required.")]
        public int MeetingTypeId { get; set; }
        
        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        // Display Names - not required for form submission
        [StringLength(150, ErrorMessage = "Meeting venue name cannot exceed 150 characters.")]
        public string MeetingVenueName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Meeting type name cannot exceed 100 characters.")]
        public string MeetingTypeName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters.")]
        public string DepartmentName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Meeting description cannot exceed 500 characters.")]
        public string MeetingDescription { get; set; } = string.Empty;

        [Required]
        public bool IsCancelled { get; set; }
    }
}