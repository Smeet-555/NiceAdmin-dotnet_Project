using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers;

public class DepartmentController : Controller
{
    // Static list to simulate database (in real app, this would be a database)
    private static List<DepartmentViewModel> _departments = new List<DepartmentViewModel>
    {
        new DepartmentViewModel()
        {
            DepartmentId = 1,
            DepartmentName = "CSE",
            Created = DateTime.Now.AddDays(-30),
            Modified = DateTime.Now.AddDays(-5),
        },
        new DepartmentViewModel()
        {
            DepartmentId = 2,
            DepartmentName = "Mechanical",
            Created = DateTime.Now.AddDays(-25),
            Modified = DateTime.Now.AddDays(-3),
        },
        new DepartmentViewModel()
        {
            DepartmentId = 3,
            DepartmentName = "Civil",
            Created = DateTime.Now.AddDays(-20),
            Modified = DateTime.Now.AddDays(-1),
        }
    };

    public IActionResult Index()
    {
        return View("DepartmentList", _departments);
    }
    
    public IActionResult DepartmentAddEdit(int? id)
    {
        if (id.HasValue)
        {
            // Edit mode - find the department
            var department = _departments.FirstOrDefault(d => d.DepartmentId == id.Value);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }
        else
        {
            // Add mode - create new department
            var newDepartment = new DepartmentViewModel
            {
                DepartmentId = 0, // 0 indicates new record
                Created = DateTime.Now,
                Modified = DateTime.Now
            };
            return View(newDepartment);
        }
    }

    [HttpPost]
    public IActionResult Save(DepartmentViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.DepartmentId == 0)
            {
                // Add new department
                model.DepartmentId = _departments.Max(d => d.DepartmentId) + 1;
                model.Created = DateTime.Now;
                model.Modified = DateTime.Now;
                _departments.Add(model);
                
                TempData["SuccessMessage"] = "Department added successfully!";
            }
            else
            {
                // Update existing department
                var existingDepartment = _departments.FirstOrDefault(d => d.DepartmentId == model.DepartmentId);
                if (existingDepartment != null)
                {
                    existingDepartment.DepartmentName = model.DepartmentName;
                    existingDepartment.Modified = DateTime.Now;
                    
                    TempData["SuccessMessage"] = "Department updated successfully!";
                }
            }
            
            return RedirectToAction("Index");
        }
        
        // If model is not valid, return to form with errors
        return View("DepartmentAddEdit", model);
    }

    [HttpPost]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var department = _departments.FirstOrDefault(d => d.DepartmentId == id);
        if (department != null)
        {
            _departments.Remove(department);
            TempData["SuccessMessage"] = "Department deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Department not found!";
        }
    
        return RedirectToAction("Index");
    }

}