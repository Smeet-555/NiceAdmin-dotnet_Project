using System.ComponentModel.DataAnnotations;

namespace NiceAdmin.Models;

public class Department
{
    public int Id { get; set; }
    
    [Required]
    [Display(Name = "Department Name")]
    public string DepartmentName { get; set; } = string.Empty;
    
    [Display(Name = "Description")]
    public string? Description { get; set; }
    
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}