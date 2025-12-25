using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers;

public class MeetingMemberController : Controller
{
    public IActionResult MeetingMembersList()
    {
        return View();
    }
    
    public IActionResult MeetingMembersAddEdit()
    {
        return View();
    }
}