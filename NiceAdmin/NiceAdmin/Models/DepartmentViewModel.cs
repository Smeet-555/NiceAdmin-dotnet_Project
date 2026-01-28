using System;
using System.ComponentModel.DataAnnotations;

namespace NiceAdmin.Models
{
    public class DepartmentViewModel
    {
        [Key]
        public int DepartmentId { get; set; }

        // [Required(ErrorMessage = "Department name is required.")]
        // [StringLength(100, MinimumLength = 2, 
            // ErrorMessage = "Department name must be between 2 and 100 characters.")]
        public string DepartmentName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Modified { get; set; }
    }
}