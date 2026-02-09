using System.Data;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;
using Microsoft.Data.SqlClient;

namespace NiceAdmin.Controllers
{
    public class MeetingVenueController : Controller
    {
        private readonly IConfiguration _configuration;

        public MeetingVenueController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return MeetingVenueList();
        }

        public IActionResult MeetingVenueList()
        {
            List<MeetingVenueViewModel> venues = new();

            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    TempData["ErrorMessage"] = "Connection string is not configured!";
                    return View("MeetingVenueList", venues);
                }

                using SqlConnection con = new(connectionString);
                using SqlCommand cmd = new("PR_MeetingVenue_SelectAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    venues.Add(new MeetingVenueViewModel
                    {
                        VenueId = Convert.ToInt32(reader["MeetingVenueID"]),
                        VenueName = reader["MeetingVenueName"]?.ToString() ?? string.Empty,
                        Created = reader["Created"] == DBNull.Value
                            ? DateTime.Now
                            : Convert.ToDateTime(reader["Created"]),
                        Modified = reader["Modified"] == DBNull.Value
                            ? DateTime.Now
                            : Convert.ToDateTime(reader["Modified"])
                    });
                }

                TempData["SuccessMessage"] = $"Loaded {venues.Count} meeting venues successfully!";
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] =
                    $"SQL Error: {sqlEx.Message} | Error Number: {sqlEx.Number} | Line: {sqlEx.LineNumber}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    $"Error: {ex.GetType().Name} - {ex.Message} | Stack: {ex.StackTrace?.Substring(0, Math.Min(200, ex.StackTrace.Length))}";
            }

            return View("MeetingVenueList", venues);
        }

        public IActionResult MeetingVenueAddEdit(int? id)
        {
            if (!id.HasValue || id == 0)
            {
                // Add mode - create new meeting venue
                return View(new MeetingVenueViewModel
                {
                    VenueId = 0,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                });
            }

            MeetingVenueViewModel model = null;

            try
            {
                using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new("PR_MeetingVenue_SelectByPK", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeetingVenueID", id.Value);

                con.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    model = new MeetingVenueViewModel
                    {
                        VenueId = Convert.ToInt32(reader["MeetingVenueID"]),
                        VenueName = reader["MeetingVenueName"]?.ToString() ?? string.Empty,
                        Created = reader["Created"] == DBNull.Value
                            ? DateTime.Now
                            : Convert.ToDateTime(reader["Created"]),
                        Modified = reader["Modified"] == DBNull.Value
                            ? DateTime.Now
                            : Convert.ToDateTime(reader["Modified"])
                    };
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading meeting venue: " + ex.Message;
                return RedirectToAction("MeetingVenueList");
            }

            if (model == null)
            {
                TempData["ErrorMessage"] = "Meeting venue not found.";
                return RedirectToAction("MeetingVenueList");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Save(MeetingVenueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("MeetingVenueAddEdit", model);
            }

            try
            {
                using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
                SqlCommand cmd;

                if (model.VenueId == 0)
                {
                    // Insert new meeting venue
                    cmd = new SqlCommand("PR_MeetingVenue_Insert", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeetingVenueName", model.VenueName ?? string.Empty);
                }
                else
                {
                    // Update existing meeting venue
                    cmd = new SqlCommand("PR_MeetingVenue_UpdateByPK", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeetingVenueID", model.VenueId);
                    cmd.Parameters.AddWithValue("@MeetingVenueName", model.VenueName ?? string.Empty);
                }

                con.Open();
                cmd.ExecuteNonQuery();

                TempData["SuccessMessage"] = model.VenueId == 0
                    ? "Meeting venue added successfully!"
                    : "Meeting venue updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error saving meeting venue: " + ex.Message;
                return View("MeetingVenueAddEdit", model);
            }

            return RedirectToAction("MeetingVenueList");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                TempData["ErrorMessage"] =
                    "Database connection string is not configured. Please check appsettings.json";
                return RedirectToAction("MeetingVenueList");
            }

            try
            {
                using SqlConnection con = new(connectionString);
                using SqlCommand cmd = new("PR_MeetingVenue_DeleteByPK", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeetingVenueID", id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                TempData["SuccessMessage"] = rowsAffected > 0
                    ? "Meeting venue deleted successfully!"
                    : "Meeting venue not found.";
            }
            catch (SqlException sqlEx)
            {
                // Check for foreign key constraint violation (Error 547)
                if (sqlEx.Number == 547)
                {
                    TempData["ErrorMessage"] =
                        "Cannot delete this meeting venue because it is being used by one or more meetings. Please delete or update those meetings first.";
                }
                else
                {
                    TempData["ErrorMessage"] =
                        $"Database error deleting meeting venue: {sqlEx.Message} (Error Number: {sqlEx.Number}, Line: {sqlEx.LineNumber})";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting meeting venue: {ex.Message}";
            }

            return RedirectToAction("MeetingVenueList");
        }
    }
}
