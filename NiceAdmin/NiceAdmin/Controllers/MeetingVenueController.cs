using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;

namespace NiceAdmin.Controllers
{
    public class MeetingVenueController : Controller
    {
        public IActionResult MeetingVenueList()
        {
            var venues = new List<MeetingVenueViewModel>
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

            return View(venues);
        }

        public IActionResult MeetingVenueAddEdit()
        {
            return View();
        }
    }
}