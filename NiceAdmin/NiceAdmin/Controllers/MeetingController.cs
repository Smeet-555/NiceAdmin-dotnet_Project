using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers
{
    public class MeetingController : Controller
    {
        // Static list to simulate database
        private static List<MeetingViewModel> _meetings = new List<MeetingViewModel>
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

        public IActionResult MeetingList()
        {
            return View(_meetings);
        }

        public IActionResult MeetingAddEdit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var meeting = _meetings.FirstOrDefault(m => m.MeetingId == id);
            if (meeting != null)
            {
                _meetings.Remove(meeting);
                TempData["SuccessMessage"] = "Meeting deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Meeting not found!";
            }
            
            return RedirectToAction("MeetingList");
        }
    }
}