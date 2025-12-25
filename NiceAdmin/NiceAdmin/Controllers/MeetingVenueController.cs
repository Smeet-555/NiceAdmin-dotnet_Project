using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers;

public class MeetingVenueController : Controller
{
    public IActionResult MeetingVenueList()
    {
        return View();
    }
    
    public IActionResult MeetingVenueAddEdit()
    {
        return View();
    }
    
    
}