using System.Runtime.InteropServices.JavaScript;

namespace NiceAdmin.Models;

public class DepartmentViewModel
{
    public int DepartmentId { get; set; } 
    public string DepartmentName { get; set; } 
    public DateTime Created{ get; set; }
    public DateTime Modified { get; set; }
}

