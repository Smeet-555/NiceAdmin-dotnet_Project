using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers
{
    public class StaffController : Controller
    {
        // Static list to simulate database
        private static List<StaffModelView> _staffList = new List<StaffModelView>
        {
            new StaffModelView
            {
                StaffId = 1,
                StaffName = "John Doe",
                DepartmentName = "IT",
                MobileNo = "9876543210",
                EmailAddress = "john.doe@company.com"
            },
            new StaffModelView
            {
                StaffId = 2,
                StaffName = "Jane Smith",
                DepartmentName = "HR",
                MobileNo = "9123456780",
                EmailAddress = "jane.smith@company.com"
            },
            new StaffModelView
            {
                StaffId = 3,
                StaffName = "Michael Brown",
                DepartmentName = "Finance",
                MobileNo = "9988776655",
                EmailAddress = "michael.brown@company.com"
            }
        };

        public IActionResult StaffList()
        {
            return View(_staffList);
        }

        public IActionResult StaffAddEdit(int? id)
        {
            if (id.HasValue)
            {
                // Edit mode - find the staff member
                var staff = _staffList.FirstOrDefault(s => s.StaffId == id.Value);
                if (staff == null)
                {
                    return NotFound();
                }
                return View(staff);
            }
            else
            {
                // Add mode - create new staff member
                var newStaff = new StaffModelView
                {
                    StaffId = 0 // 0 indicates new record
                };
                return View(newStaff);
            }
        }

        [HttpPost]
        public IActionResult Save(StaffModelView model)
        {
            if (ModelState.IsValid)
            {
                if (model.StaffId == 0)
                {
                    // Add new staff
                    model.StaffId = _staffList.Any() ? _staffList.Max(s => s.StaffId) + 1 : 1;
                    _staffList.Add(model);
                    
                    TempData["SuccessMessage"] = "Staff member added successfully!";
                }
                else
                {
                    // Update existing staff
                    var existingStaff = _staffList.FirstOrDefault(s => s.StaffId == model.StaffId);
                    if (existingStaff != null)
                    {
                        existingStaff.StaffName = model.StaffName;
                        existingStaff.DepartmentName = model.DepartmentName;
                        existingStaff.MobileNo = model.MobileNo;
                        existingStaff.EmailAddress = model.EmailAddress;
                        
                        TempData["SuccessMessage"] = "Staff member updated successfully!";
                    }
                }
                
                return RedirectToAction("StaffList");
            }
            
            // If model is not valid, return to form with errors
            return View("StaffAddEdit", model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var staff = _staffList.FirstOrDefault(s => s.StaffId == id);
            if (staff != null)
            {
                _staffList.Remove(staff);
                TempData["SuccessMessage"] = "Staff member deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Staff member not found!";
            }
            
            return RedirectToAction("StaffList");
        }
    }
}