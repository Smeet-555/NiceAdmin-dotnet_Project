using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers;

public class MeetingMemberController : Controller
{
    public IActionResult MeetingMembersList()
    {
        var meetingMembersList = new List<MeetingMemberViewModel>
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
        return View("MeetingMembersList" , meetingMembersList);
    }
    
    public IActionResult MeetingMembersAddEdit()
    {
        return View();
    }
}