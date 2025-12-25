using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers;

public class MeetingController : Controller
{
    public IActionResult MeetingList()
    {
        return View();
    }
    
    public IActionResult MeetingAddEdit()
    {
        return View();
    }
    
}