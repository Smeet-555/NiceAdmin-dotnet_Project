using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers
{
    public class MeetingController : Controller
    {
        public IActionResult MeetingList()
        {
            var meetings = new List<MeetingViewModel>
            {
                new MeetingViewModel
                {
                    MeetingId = 1,
                    MeetingDate = DateTime.Today,
                    MeetingVenueName = "Conference Room A",
                    MeetingTypeName = "Project Meeting",
                    DepartmentName = "IT",
                    IsCancelled = false
                },
                new MeetingViewModel
                {
                    MeetingId = 2,
                    MeetingDate = DateTime.Today.AddDays(1),
                    MeetingVenueName = "Main Hall",
                    MeetingTypeName = "Management Meeting",
                    DepartmentName = "HR",
                    IsCancelled = true
                },
                new MeetingViewModel
                {
                    MeetingId = 3,
                    MeetingDate = DateTime.Today.AddDays(3),
                    MeetingVenueName = "Training Room",
                    MeetingTypeName = "Training Session",
                    DepartmentName = "Finance",
                    IsCancelled = false
                }
            };

            return View(meetings);
        }

        public IActionResult MeetingAddEdit()
        {
            return View();
        }
    }
}