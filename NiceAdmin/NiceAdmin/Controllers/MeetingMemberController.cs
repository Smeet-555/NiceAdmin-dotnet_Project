using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers;

public class MeetingMemberController : Controller
{
    // Static list to simulate database
    private static List<MeetingMemberViewModel> _meetingMembersList = new List<MeetingMemberViewModel>
    {
        new MeetingMemberViewModel
        {
            MeetingId = 1,
            MeetingDescription = "Meeting for Something",
            StaffName = "StaffA",
            DepartmentName = "DepartmentA",
            IsPresent = true,
            Remarks = "Remarks"
        },
        new MeetingMemberViewModel
        {
            MeetingId = 2,
            MeetingDescription = "Meeting for Something@",
            StaffName = "StaffB",
            DepartmentName = "DepartmentB",
            IsPresent = false,
            Remarks = "RemarksB"
        },
        new MeetingMemberViewModel
        {
            MeetingId = 3,
            MeetingDescription = "Meeting for Something#",
            StaffName = "StaffC",
            DepartmentName = "DepartmentC",
            IsPresent = true,
            Remarks = "RemarksC"
        }
    };

    public IActionResult MeetingMembersList()
    {
        return View("MeetingMembersList", _meetingMembersList);
    }
    
    public IActionResult MeetingMembersAddEdit(int? id)
    {
        if (id.HasValue)
        {
            // Edit mode - find the meeting member
            var meetingMember = _meetingMembersList.FirstOrDefault(mm => mm.MeetingId == id.Value);
            if (meetingMember == null)
            {
                return NotFound();
            }
            return View(meetingMember);
        }
        else
        {
            // Add mode - create new meeting member
            var newMeetingMember = new MeetingMemberViewModel
            {
                MeetingId = 0, // 0 indicates new record
                IsPresent = false
            };
            return View(newMeetingMember);
        }
    }

    [HttpPost]
    public IActionResult Save(MeetingMemberViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.MeetingId == 0)
            {
                // Add new meeting member
                model.MeetingId = _meetingMembersList.Any() ? _meetingMembersList.Max(mm => mm.MeetingId) + 1 : 1;
                _meetingMembersList.Add(model);
                
                TempData["SuccessMessage"] = "Meeting member added successfully!";
            }
            else
            {
                // Update existing meeting member
                var existingMember = _meetingMembersList.FirstOrDefault(mm => mm.MeetingId == model.MeetingId);
                if (existingMember != null)
                {
                    existingMember.MeetingDescription = model.MeetingDescription;
                    existingMember.StaffName = model.StaffName;
                    existingMember.DepartmentName = model.DepartmentName;
                    existingMember.IsPresent = model.IsPresent;
                    existingMember.Remarks = model.Remarks;
                    
                    TempData["SuccessMessage"] = "Meeting member updated successfully!";
                }
            }
            
            return RedirectToAction("MeetingMembersList");
        }
        
        // If model is not valid, return to form with errors
        return View("MeetingMembersAddEdit", model);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var meetingMember = _meetingMembersList.FirstOrDefault(mm => mm.MeetingId == id);
        if (meetingMember != null)
        {
            _meetingMembersList.Remove(meetingMember);
            TempData["SuccessMessage"] = "Meeting member deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Meeting member not found!";
        }
        
        return RedirectToAction("MeetingMembersList");
    }
}