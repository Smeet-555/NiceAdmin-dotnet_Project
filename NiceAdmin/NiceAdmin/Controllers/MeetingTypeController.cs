using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers
{
    public class MeetingTypeController : Controller
    {
        public IActionResult MeetingTypeList()
        {
            var meetingTypes = new List<MeetingTypeViewModel>
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

            return View(meetingTypes);
        }

        public IActionResult MeetingTypeAddEdit()
        {
            return View();
        }
    }
}