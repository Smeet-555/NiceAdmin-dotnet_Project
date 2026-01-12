using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers
{
    public class MeetingTypeController : Controller
    {
        // Static list to simulate database
        private static List<MeetingTypeViewModel> _meetingTypes = new List<MeetingTypeViewModel>
        {
            new MeetingTypeViewModel
            {
                MeetingTypeId = 1,
                MeetingTypeName = "Project Meeting",
                Remarks = "Weekly project discussion"
            },
            new MeetingTypeViewModel
            {
                MeetingTypeId = 2,
                MeetingTypeName = "Management Meeting",
                Remarks = "Monthly review meeting"
            },
            new MeetingTypeViewModel
            {
                MeetingTypeId = 3,
                MeetingTypeName = "Training Session",
                Remarks = "Employee skill enhancement"
            }
        };

        public IActionResult MeetingTypeList()
        {
            return View(_meetingTypes);
        }

        public IActionResult MeetingTypeAddEdit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var meetingType = _meetingTypes.FirstOrDefault(mt => mt.MeetingTypeId == id);
            if (meetingType != null)
            {
                _meetingTypes.Remove(meetingType);
                TempData["SuccessMessage"] = "Meeting type deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Meeting type not found!";
            }
            
            return RedirectToAction("MeetingTypeList");
        }
    }
}