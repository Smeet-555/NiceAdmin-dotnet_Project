namespace NiceAdmin.Models;

public class MeetingViewModel
{
    public int MeetingId { get; set; } 
    public DateTime MeetingDate { get; set; } 
    public string MeetingVenueName { get; set; } 
    public string MeetingTypeName { get; set; } 
    public string DepartmentName { get; set; } 
    public Boolean IsCancelled { get; set; }
    
}