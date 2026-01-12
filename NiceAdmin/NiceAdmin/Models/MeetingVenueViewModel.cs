using System;
using System.ComponentModel.DataAnnotations;

namespace NiceAdmin.Models
{
    public class MeetingVenueViewModel
    {
        [Key]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Venue name is required.")]
        [StringLength(150, MinimumLength = 2,
            ErrorMessage = "Venue name must be between 2 and 150 characters.")]
        public string VenueName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Modified { get; set; }
    }
}