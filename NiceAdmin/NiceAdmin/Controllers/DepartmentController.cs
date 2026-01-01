using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers;

public class DepartmentController : Controller
{
    public IActionResult Index()
    {
        var department = new List<DepartmentViewModel>
        {
            new DepartmentViewModel()
            {
                DepartmentId = 1,
                DepartmentName = "CSE",
                Created = DateTime.Now,
                Modified = DateTime.Now,
            },
            new DepartmentViewModel()
            {
                DepartmentId = 2,
                DepartmentName = "Mechanical",
                Created = DateTime.Now,
                Modified = DateTime.Now,
            },
            new DepartmentViewModel()
            {
                DepartmentId = 3,
                DepartmentName = "Civil",
                Created = DateTime.Now,
                Modified = DateTime.Now,
            }
        };
        
        return View("DepartmentList" , department);
        
    }
    
    public IActionResult DepartmentAddEdit()
    {
        return View();
    } 
}