using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers;

public class MeetingTypeController : Controller
{
    public IActionResult MeetingTypeList()
    {
        return View();
    }
    
    public IActionResult MeetingTypeAddEdit()
    {
        return View();
    }

    
}