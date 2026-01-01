using System.Runtime.InteropServices.JavaScript;

namespace NiceAdmin.Models;

public class MeetingVenueViewModel
{
    public int VenueId { get; set; } 
    public string VenueName { get; set; } 
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    
}