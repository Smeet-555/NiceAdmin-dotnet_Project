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

        public IActionResult MeetingVenueAddEdit(int? id)
        {
            if (id.HasValue)
            {
                // Edit mode - find the venue
                var venue = _venues.FirstOrDefault(v => v.VenueId == id.Value);
                if (venue == null)
                {
                    return NotFound();
                }
                return View(venue);
            }
            else
            {
                // Add mode - create new venue
                var newVenue = new MeetingVenueViewModel
                {
                    VenueId = 0 // 0 indicates new record
                };
                return View(newVenue);
            }
        }

        [HttpPost]
        public IActionResult Save(MeetingVenueViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.VenueId == 0)
                {
                    // Add new venue
                    model.VenueId = _venues.Any() ? _venues.Max(v => v.VenueId) + 1 : 1;
                    // Created and Modified will be handled by database
                    _venues.Add(model);
                    
                    TempData["SuccessMessage"] = "Meeting venue added successfully!";
                }
                else
                {
                    // Update existing venue
                    var existingVenue = _venues.FirstOrDefault(v => v.VenueId == model.VenueId);
                    if (existingVenue != null)
                    {
                        existingVenue.VenueName = model.VenueName;
                        // Modified will be handled by database
                        
                        TempData["SuccessMessage"] = "Meeting venue updated successfully!";
                    }
                }
                
                return RedirectToAction("MeetingVenueList");
            }
            
            // If model is not valid, return to form with errors
            return View("MeetingVenueAddEdit", model);
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