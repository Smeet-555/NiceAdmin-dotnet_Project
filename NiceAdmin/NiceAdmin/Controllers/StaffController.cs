using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult StaffList()
        {
            var staffList = new List<StaffModelView>
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

            return View(staffList);
        }

        public IActionResult StaffAddEdit()
        {
            return View();
        }
    }
}