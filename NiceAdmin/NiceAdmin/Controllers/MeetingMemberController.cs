using System.Data;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;
using Microsoft.Data.SqlClient;

namespace NiceAdmin.Controllers
{
    public class MeetingMemberController : Controller
    {
        private readonly IConfiguration _configuration;

        public MeetingMemberController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return MeetingMembersList();
        }

        public IActionResult MeetingMembersList()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                TempData["ErrorMessage"] = "Database connection string is not configured. Please check appsettings.json";
                return View(new List<MeetingMemberViewModel>());
            }

            List<MeetingMemberViewModel> meetingMembers = new();

            try
            {
                using SqlConnection con = new(connectionString);
                using SqlCommand cmd = new("PR_MeetingMember_SelectAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var meetingDate = reader["MeetingDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["MeetingDate"]);
                    var meetingDisplay = meetingDate != DateTime.MinValue ? meetingDate.ToString("MMM dd, yyyy") : "Unknown";

                    meetingMembers.Add(new MeetingMemberViewModel
                    {
                        MeetingMemberId = Convert.ToInt32(reader["MeetingMemberID"]),
                        MeetingId = Convert.ToInt32(reader["MeetingID"]),
                        StaffId = Convert.ToInt32(reader["StaffID"]),
                        IsPresent = reader["IsPresent"] == DBNull.Value ? false : Convert.ToBoolean(reader["IsPresent"]),
                        Remarks = reader["Remarks"] == DBNull.Value ? string.Empty : reader["Remarks"].ToString(),
                        StaffName = reader["StaffName"] == DBNull.Value ? string.Empty : reader["StaffName"].ToString(),
                        MeetingDescription = meetingDisplay,
                        DepartmentName = string.Empty
                    });
                }
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = $"Database error loading meeting members: {sqlEx.Message} (Error Number: {sqlEx.Number}, Line: {sqlEx.LineNumber})";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading meeting members: {ex.Message}";
            }

            // Get department names for each staff member
            if (meetingMembers.Any())
            {
                try
                {
                    using SqlConnection con = new(connectionString);
                    con.Open();

                    foreach (var member in meetingMembers)
                    {
                        using var cmd = new SqlCommand(@"
                            SELECT d.DepartmentName 
                            FROM MOM_Staff s 
                            INNER JOIN MOM_Department d ON s.DepartmentID = d.DepartmentID 
                            WHERE s.StaffID = @StaffID", con);
                        cmd.Parameters.AddWithValue("@StaffID", member.StaffId);
                        
                        var result = cmd.ExecuteScalar();
                        member.DepartmentName = result?.ToString() ?? "Unknown";
                    }
                }
                catch (SqlException sqlEx)
                {
                    TempData["WarningMessage"] = $"Could not load department information: {sqlEx.Message} (Error Number: {sqlEx.Number})";
                }
                catch (Exception ex)
                {
                    TempData["WarningMessage"] = $"Could not load department information: {ex.Message}";
                }
            }

            return View(meetingMembers);
        }

        [HttpGet]
        public IActionResult MeetingMembersAddEdit(int? id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                TempData["ErrorMessage"] = "Database connection string is not configured. Please check appsettings.json";
                return RedirectToAction("MeetingMembersList");
            }

            // Load dropdown data
            LoadDropdownData();

            if (!id.HasValue || id == 0)
            {
                return View(new MeetingMemberViewModel
                {
                    IsPresent = false
                });
            }

            MeetingMemberViewModel model = null;

            try
            {
                using SqlConnection con = new(connectionString);
                using SqlCommand cmd = new("PR_MeetingMember_SelectByPK", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeetingMemberID", id.Value);

                con.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    model = new MeetingMemberViewModel
                    {
                        MeetingMemberId = Convert.ToInt32(reader["MeetingMemberID"]),
                        MeetingId = Convert.ToInt32(reader["MeetingID"]),
                        StaffId = Convert.ToInt32(reader["StaffID"]),
                        IsPresent = reader["IsPresent"] == DBNull.Value ? false : Convert.ToBoolean(reader["IsPresent"]),
                        Remarks = reader["Remarks"] == DBNull.Value ? string.Empty : reader["Remarks"].ToString(),
                        StaffName = string.Empty,
                        DepartmentName = string.Empty
                    };
                }
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = $"Database error loading meeting member: {sqlEx.Message} (Error Number: {sqlEx.Number}, Line: {sqlEx.LineNumber})";
                return RedirectToAction("MeetingMembersList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading meeting member: {ex.Message}";
                return RedirectToAction("MeetingMembersList");
            }

            if (model == null)
            {
                TempData["ErrorMessage"] = "Meeting member not found.";
                return RedirectToAction("MeetingMembersList");
            }

            return View(model);
        }

        private void LoadDropdownData()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                TempData["ErrorMessage"] = "Database connection string is not configured. Please check appsettings.json";
                ViewBag.Meetings = new List<object>();
                ViewBag.Staff = new List<object>();
                return;
            }

            try
            {
                using SqlConnection con = new(connectionString);
                con.Open();

                // Load Meetings
                var meetings = new List<object>();
                using (var cmd = new SqlCommand("PR_Meetings_SelectAll", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var meetingDate = Convert.ToDateTime(reader["MeetingDate"]).ToString("yyyy-MM-dd");
                            var meetingTypeName = reader["MeetingTypeName"] == DBNull.Value ? "Unknown" : reader["MeetingTypeName"].ToString();
                            var meetingVenueName = reader["MeetingVenueName"] == DBNull.Value ? "Unknown" : reader["MeetingVenueName"].ToString();
                            var meetingText = $"{meetingDate} - {meetingTypeName} ({meetingVenueName})";
                            meetings.Add(new { Value = reader["MeetingID"], Text = meetingText });
                        }
                    }
                }
                ViewBag.Meetings = meetings;

                // Load Staff
                var staff = new List<object>();
                using (var cmd = new SqlCommand("PR_Staff_SelectAll", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var staffName = reader["StaffName"] == DBNull.Value ? "Unknown" : reader["StaffName"].ToString();
                            var departmentName = reader["DepartmentName"] == DBNull.Value ? "Unknown" : reader["DepartmentName"].ToString();
                            var staffText = $"{staffName} ({departmentName})";
                            staff.Add(new { Value = reader["StaffID"], Text = staffText });
                        }
                    }
                }
                ViewBag.Staff = staff;
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = $"Database error loading dropdown data: {sqlEx.Message} (Error Number: {sqlEx.Number}, Line: {sqlEx.LineNumber})";
                ViewBag.Meetings = new List<object>();
                ViewBag.Staff = new List<object>();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading dropdown data: {ex.Message}";
                ViewBag.Meetings = new List<object>();
                ViewBag.Staff = new List<object>();
            }
        }

        [HttpPost]
        public IActionResult Save(MeetingMemberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdownData();
                return View("MeetingMembersAddEdit", model);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                TempData["ErrorMessage"] = "Database connection string is not configured. Please check appsettings.json";
                LoadDropdownData();
                return View("MeetingMembersAddEdit", model);
            }

            try
            {
                using SqlConnection con = new(connectionString);
                SqlCommand cmd;

                if (model.MeetingMemberId == 0)
                {
                    cmd = new SqlCommand("PR_MeetingMember_Insert", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeetingID", model.MeetingId);
                    cmd.Parameters.AddWithValue("@StaffID", model.StaffId);
                    cmd.Parameters.AddWithValue("@IsPresent", model.IsPresent);
                    cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? string.Empty);
                }
                else
                {
                    cmd = new SqlCommand("PR_MeetingMember_UpdateByPK", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeetingMemberID", model.MeetingMemberId);
                    cmd.Parameters.AddWithValue("@MeetingID", model.MeetingId);
                    cmd.Parameters.AddWithValue("@StaffID", model.StaffId);
                    cmd.Parameters.AddWithValue("@IsPresent", model.IsPresent);
                    cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? string.Empty);
                }

                con.Open();
                cmd.ExecuteNonQuery();

                TempData["SuccessMessage"] = model.MeetingMemberId == 0
                    ? "Meeting member added successfully!"
                    : "Meeting member updated successfully!";
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = $"Database error saving meeting member: {sqlEx.Message} (Error Number: {sqlEx.Number}, Line: {sqlEx.LineNumber})";
                LoadDropdownData();
                return View("MeetingMembersAddEdit", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error saving meeting member: {ex.Message}";
                LoadDropdownData();
                return View("MeetingMembersAddEdit", model);
            }

            return RedirectToAction("MeetingMembersList");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                TempData["ErrorMessage"] = "Database connection string is not configured. Please check appsettings.json";
                return RedirectToAction("MeetingMembersList");
            }

            try
            {
                using SqlConnection con = new(connectionString);
                using SqlCommand cmd = new("PR_MeetingMember_DeleteByPK", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeetingMemberID", id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                TempData["SuccessMessage"] = rowsAffected > 0
                    ? "Meeting member deleted successfully!"
                    : "Meeting member not found.";
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = $"Database error deleting meeting member: {sqlEx.Message} (Error Number: {sqlEx.Number}, Line: {sqlEx.LineNumber})";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting meeting member: {ex.Message}";
            }

            return RedirectToAction("MeetingMembersList");
        }
    }
}