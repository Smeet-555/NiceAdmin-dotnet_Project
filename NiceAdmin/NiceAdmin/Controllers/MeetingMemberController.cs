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
    
    public IActionResult MeetingMembersAddEdit()
    {
        return View();
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