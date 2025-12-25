using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers;

public class StaffController : Controller
{
    public IActionResult StaffList()
    {
        return View();
    }

    public IActionResult StaffAddEdit()
    {
        return View();
    }
    
    
    
}