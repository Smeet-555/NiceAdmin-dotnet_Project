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

        public IActionResult MeetingTypeAddEdit(int? id)
        {
            if (id.HasValue)
            {
                // Edit mode - find the meeting type
                var meetingType = _meetingTypes.FirstOrDefault(mt => mt.MeetingTypeId == id.Value);
                if (meetingType == null)
                {
                    return NotFound();
                }
                return View(meetingType);
            }
            else
            {
                // Add mode - create new meeting type
                var newMeetingType = new MeetingTypeViewModel
                {
                    MeetingTypeId = 0 // 0 indicates new record
                };
                return View(newMeetingType);
            }
        }

        [HttpPost]
        public IActionResult Save(MeetingTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.MeetingTypeId == 0)
                {
                    // Add new meeting type
                    model.MeetingTypeId = _meetingTypes.Any() ? _meetingTypes.Max(mt => mt.MeetingTypeId) + 1 : 1;
                    _meetingTypes.Add(model);
                    
                    TempData["SuccessMessage"] = "Meeting type added successfully!";
                }
                else
                {
                    // Update existing meeting type
                    var existingMeetingType = _meetingTypes.FirstOrDefault(mt => mt.MeetingTypeId == model.MeetingTypeId);
                    if (existingMeetingType != null)
                    {
                        existingMeetingType.MeetingTypeName = model.MeetingTypeName;
                        existingMeetingType.Remarks = model.Remarks;
                        
                        TempData["SuccessMessage"] = "Meeting type updated successfully!";
                    }
                }
                
                return RedirectToAction("MeetingTypeList");
            }
            
            // If model is not valid, return to form with errors
            return View("MeetingTypeAddEdit", model);
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