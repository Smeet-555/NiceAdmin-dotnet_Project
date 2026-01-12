using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers
{
    public class MeetingVenueController : Controller
    {
        // Static list to simulate database
        private static List<MeetingVenueViewModel> _venues = new List<MeetingVenueViewModel>
        {
            new MeetingVenueViewModel
            {
                VenueId = 1,
                VenueName = "Conference Room A",
                Created = DateTime.Now.AddDays(-10),
                Modified = DateTime.Now.AddDays(-2)
            },
            new MeetingVenueViewModel
            {
                VenueId = 2,
                VenueName = "Main Auditorium",
                Created = DateTime.Now.AddDays(-20),
                Modified = DateTime.Now.AddDays(-5)
            },
            new MeetingVenueViewModel
            {
                VenueId = 3,
                VenueName = "Training Hall",
                Created = DateTime.Now.AddDays(-15),
                Modified = DateTime.Now
            }
        };

        public IActionResult MeetingVenueList()
        {
            return View(_venues);
        }

        public IActionResult MeetingVenueAddEdit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var venue = _venues.FirstOrDefault(v => v.VenueId == id);
            if (venue != null)
            {
                _venues.Remove(venue);
                TempData["SuccessMessage"] = "Meeting venue deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Meeting venue not found!";
            }
            
            return RedirectToAction("MeetingVenueList");
        }
    }
}