using System.Runtime.InteropServices.JavaScript;

namespace NiceAdmin.Models;

public class MeetingMemberViewModel
{
    public int MeetingId { get; set; } 
    public string MeetingDescription { get; set; } 
    public string StaffName { get; set; } 
    public string DepartmentName { get; set; } 
    public bool IsPresent { get; set; } 
    public string Remarks { get; set; } 
    
}