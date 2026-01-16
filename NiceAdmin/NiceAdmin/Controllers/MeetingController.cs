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

        public IActionResult MeetingAddEdit(int? id)
        {
            if (id.HasValue)
            {
                // Edit mode - find the meeting
                var meeting = _meetings.FirstOrDefault(m => m.MeetingId == id.Value);
                if (meeting == null)
                {
                    return NotFound();
                }
                return View(meeting);
            }
            else
            {
                // Add mode - create new meeting
                var newMeeting = new MeetingViewModel
                {
                    MeetingId = 0, // 0 indicates new record
                    MeetingDate = DateTime.Today,
                    IsCancelled = false
                };
                return View(newMeeting);
            }
        }

        [HttpPost]
        public IActionResult Save(MeetingViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.MeetingId == 0)
                {
                    // Add new meeting
                    model.MeetingId = _meetings.Any() ? _meetings.Max(m => m.MeetingId) + 1 : 1;
                    _meetings.Add(model);
                    
                    TempData["SuccessMessage"] = "Meeting added successfully!";
                }
                else
                {
                    // Update existing meeting
                    var existingMeeting = _meetings.FirstOrDefault(m => m.MeetingId == model.MeetingId);
                    if (existingMeeting != null)
                    {
                        existingMeeting.MeetingDate = model.MeetingDate;
                        existingMeeting.MeetingVenueName = model.MeetingVenueName;
                        existingMeeting.MeetingTypeName = model.MeetingTypeName;
                        existingMeeting.DepartmentName = model.DepartmentName;
                        existingMeeting.IsCancelled = model.IsCancelled;
                        
                        TempData["SuccessMessage"] = "Meeting updated successfully!";
                    }
                }
                
                return RedirectToAction("MeetingList");
            }
            
            // If model is not valid, return to form with errors
            return View("MeetingAddEdit", model);
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