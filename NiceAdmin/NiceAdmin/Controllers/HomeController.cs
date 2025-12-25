using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult PageTitle()
    {
        return View();
    }

    public IActionResult SectionDashboard()
    {
        return View();
    }
    
}