using System.ComponentModel.DataAnnotations;

namespace NiceAdmin.Models
{
    public class MeetingTypeViewModel
    {
        [Key]
        public int MeetingTypeId { get; set; }

        [Required(ErrorMessage = "Meeting type name is required.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Meeting type name must be between 2 and 100 characters.")]
        public string MeetingTypeName { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Remarks cannot exceed 300 characters.")]
        public string? Remarks { get; set; }
    }
}