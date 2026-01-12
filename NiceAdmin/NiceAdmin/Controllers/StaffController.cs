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

        public IActionResult StaffAddEdit()
        {
            return View();
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