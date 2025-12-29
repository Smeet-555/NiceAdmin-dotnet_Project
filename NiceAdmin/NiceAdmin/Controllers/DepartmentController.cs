using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers;

public class DepartmentController : Controller
{
    public IActionResult Index()
    {
        return View("DepartmentList");
    }
    
    public IActionResult DepartmentAddEdit()
    {
        return View();
    }

    
}