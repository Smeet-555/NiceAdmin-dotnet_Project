using System.Data;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;
using Microsoft.Data.SqlClient;

namespace NiceAdmin.Controllers
{
    public class MeetingController : Controller
    {
        private readonly IConfiguration _configuration;

        public MeetingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return MeetingList();
        }

        public IActionResult MeetingList()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                TempData["ErrorMessage"] = "Database connection string is not configured. Please check appsettings.json";
                return View(new List<MeetingViewModel>());
            }

            List<MeetingViewModel> meetings = new();

            try
            {
                using SqlConnection con = new(connectionString);
                using SqlCommand cmd = new("PR_Meetings_SelectAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    meetings.Add(new MeetingViewModel
                    {
                        MeetingId = Convert.ToInt32(reader["MeetingID"]),
                        MeetingDate = reader["MeetingDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["MeetingDate"]),
                        MeetingVenueName = reader["MeetingVenueName"] == DBNull.Value ? string.Empty : reader["MeetingVenueName"].ToString(),
                        MeetingTypeName = reader["MeetingTypeName"] == DBNull.Value ? string.Empty : reader["MeetingTypeName"].ToString(),
                        DepartmentName = reader["DepartmentName"] == DBNull.Value ? string.Empty : reader["DepartmentName"].ToString(),
                        IsCancelled = reader["IsCancelled"] == DBNull.Value ? false : Convert.ToBoolean(reader["IsCancelled"])
                    });
                }
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = $"Database error loading meetings: {sqlEx.Message} (Error Number: {sqlEx.Number}, Line: {sqlEx.LineNumber})";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading meetings: {ex.Message}";
            }

            return View(meetings);
        }

        [HttpGet]
        public IActionResult MeetingAddEdit(int? id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                TempData["ErrorMessage"] = "Database connection string is not configured. Please check appsettings.json";
                return RedirectToAction("MeetingList");
            }

            // Load dropdown data
            LoadDropdownData();

            if (!id.HasValue || id == 0)
            {
                return View(new MeetingViewModel
                {
                    MeetingDate = DateTime.Today,
                    IsCancelled = false
                });
            }

            MeetingViewModel model = null;

            try
            {
                using SqlConnection con = new(connectionString);
                using SqlCommand cmd = new("PR_Meetings_SelectByPK", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeetingID", id.Value);

                con.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    model = new MeetingViewModel
                    {
                        MeetingId = Convert.ToInt32(reader["MeetingID"]),
                        MeetingDate = reader["MeetingDate"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(reader["MeetingDate"]),
                        MeetingVenueId = reader["MeetingVenueID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MeetingVenueID"]),
                        MeetingTypeId = reader["MeetingTypeID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MeetingTypeID"]),
                        DepartmentId = reader["DepartmentID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DepartmentID"]),
                        MeetingDescription = reader["MeetingDescription"]?.ToString() ?? string.Empty,
                        IsCancelled = reader["IsCancelled"] == DBNull.Value ? false : Convert.ToBoolean(reader["IsCancelled"]),
                        // Display names for showing in form
                        MeetingVenueName = reader["MeetingVenueName"]?.ToString() ?? string.Empty,
                        MeetingTypeName = reader["MeetingTypeName"]?.ToString() ?? string.Empty,
                        DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty
                    };
                }
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = $"Database error loading meeting: {sqlEx.Message} (Error Number: {sqlEx.Number}, Line: {sqlEx.LineNumber})";
                return RedirectToAction("MeetingList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading meeting: {ex.Message}";
                return RedirectToAction("MeetingList");
            }

            if (model == null)
            {
                TempData["ErrorMessage"] = "Meeting not found.";
                return RedirectToAction("MeetingList");
            }

            return View(model);
        }

        private void LoadDropdownData()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                TempData["ErrorMessage"] = "Database connection string is not configured. Please check appsettings.json";
                ViewBag.MeetingVenues = new List<object>();
                ViewBag.MeetingTypes = new List<object>();
                ViewBag.Departments = new List<object>();
                return;
            }

            try
            {
                using SqlConnection con = new(connectionString);
                con.Open();

                // Load Meeting Venues
                var venues = new List<object>();
                using (var cmd = new SqlCommand("PR_MeetingVenue_SelectAll", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            venues.Add(new { Value = reader["MeetingVenueID"], Text = reader["MeetingVenueName"] });
                        }
                    }
                }
                ViewBag.MeetingVenues = venues;

                // Load Meeting Types
                var meetingTypes = new List<object>();
                using (var cmd = new SqlCommand("PR_MeetingType_SelectAll", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            meetingTypes.Add(new { Value = reader["MeetingTypeID"], Text = reader["MeetingTypeName"] });
                        }
                    }
                }
                ViewBag.MeetingTypes = meetingTypes;

                // Load Departments
                var departments = new List<object>();
                using (var cmd = new SqlCommand("PR_Department_SelectAll", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            departments.Add(new { Value = reader["DepartmentID"], Text = reader["DepartmentName"] });
                        }
                    }
                }
                ViewBag.Departments = departments;
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = $"Database error loading dropdown data: {sqlEx.Message} (Error Number: {sqlEx.Number}, Line: {sqlEx.LineNumber})";
                ViewBag.MeetingVenues = new List<object>();
                ViewBag.MeetingTypes = new List<object>();
                ViewBag.Departments = new List<object>();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading dropdown data: {ex.Message}";
                ViewBag.MeetingVenues = new List<object>();
                ViewBag.MeetingTypes = new List<object>();
                ViewBag.Departments = new List<object>();
            }
        }

        [HttpPost]
        public IActionResult Save(MeetingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdownData();
                return View("MeetingAddEdit", model);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                TempData["ErrorMessage"] = "Database connection string is not configured. Please check appsettings.json";
                LoadDropdownData();
                return View("MeetingAddEdit", model);
            }

            try
            {
                using SqlConnection con = new(connectionString);
                SqlCommand cmd;

                if (model.MeetingId == 0)
                {
                    cmd = new SqlCommand("PR_Meetings_Insert", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeetingDate", model.MeetingDate);
                    cmd.Parameters.AddWithValue("@MeetingVenueID", model.MeetingVenueId);
                    cmd.Parameters.AddWithValue("@MeetingTypeID", model.MeetingTypeId);
                    cmd.Parameters.AddWithValue("@DepartmentID", model.DepartmentId);
                    cmd.Parameters.AddWithValue("@MeetingDescription", model.MeetingDescription ?? string.Empty);
                    cmd.Parameters.AddWithValue("@DocumentPath", DBNull.Value);
                }
                else
                {
                    cmd = new SqlCommand("PR_Meetings_UpdateByPK", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeetingID", model.MeetingId);
                    cmd.Parameters.AddWithValue("@MeetingDate", model.MeetingDate);
                    cmd.Parameters.AddWithValue("@MeetingTypeID", model.MeetingTypeId);
                    cmd.Parameters.AddWithValue("@MeetingVenueID", model.MeetingVenueId);
                    cmd.Parameters.AddWithValue("@DepartmentID", model.DepartmentId);
                    cmd.Parameters.AddWithValue("@MeetingDescription", model.MeetingDescription ?? string.Empty);
                    cmd.Parameters.AddWithValue("@DocumentPath", DBNull.Value);
                }

                con.Open();
                cmd.ExecuteNonQuery();

                TempData["SuccessMessage"] = model.MeetingId == 0
                    ? "Meeting added successfully!"
                    : "Meeting updated successfully!";
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = $"Database error saving meeting: {sqlEx.Message} (Error Number: {sqlEx.Number}, Line: {sqlEx.LineNumber})";
                LoadDropdownData();
                return View("MeetingAddEdit", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error saving meeting: {ex.Message}";
                LoadDropdownData();
                return View("MeetingAddEdit", model);
            }

            return RedirectToAction("MeetingList");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                TempData["ErrorMessage"] = "Database connection string is not configured. Please check appsettings.json";
                return RedirectToAction("MeetingList");
            }

            try
            {
                using SqlConnection con = new(connectionString);
                using SqlCommand cmd = new("PR_Meetings_DeleteByPK", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeetingID", id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                TempData["SuccessMessage"] = rowsAffected > 0
                    ? "Meeting deleted successfully!"
                    : "Meeting not found.";
            }
            catch (SqlException sqlEx)
            {
                // Check for foreign key constraint violation (Error 547)
                if (sqlEx.Number == 547)
                {
                    TempData["ErrorMessage"] = "Cannot delete this meeting because it has meeting members assigned. Please delete the meeting members first.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Database error deleting meeting: {sqlEx.Message} (Error Number: {sqlEx.Number}, Line: {sqlEx.LineNumber})";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting meeting: {ex.Message}";
            }

            return RedirectToAction("MeetingList");
        }
    }
}