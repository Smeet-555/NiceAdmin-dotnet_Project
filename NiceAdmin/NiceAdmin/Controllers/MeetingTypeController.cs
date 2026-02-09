using System.Data;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;
using Microsoft.Data.SqlClient;

namespace NiceAdmin.Controllers
{
    public class MeetingTypeController : Controller
    {
        private readonly IConfiguration _configuration;

        public MeetingTypeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return MeetingTypeList();
        }

        public IActionResult MeetingTypeList()
        {
            List<MeetingTypeViewModel> meetingTypes = new();

            try
            {
                using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new("PR_MeetingType_SelectAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    meetingTypes.Add(new MeetingTypeViewModel
                    {
                        MeetingTypeId = Convert.ToInt32(reader["MeetingTypeID"]),
                        MeetingTypeName = reader["MeetingTypeName"]?.ToString() ?? string.Empty,
                        Remarks = reader["Remarks"]?.ToString() ?? string.Empty
                    });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading meeting types: " + ex.Message;
            }

            return View("MeetingTypeList", meetingTypes);
        }

        public IActionResult MeetingTypeAddEdit(int? id)
        {
            if (!id.HasValue || id == 0)
            {
                // Add mode - create new meeting type
                return View(new MeetingTypeViewModel
                {
                    MeetingTypeId = 0
                });
            }

            MeetingTypeViewModel model = null;

            try
            {
                using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new("PR_MeetingType_SelectByPK", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeetingTypeID", id.Value);

                con.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    model = new MeetingTypeViewModel
                    {
                        MeetingTypeId = Convert.ToInt32(reader["MeetingTypeID"]),
                        MeetingTypeName = reader["MeetingTypeName"]?.ToString() ?? string.Empty,
                        Remarks = reader["Remarks"]?.ToString() ?? string.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading meeting type: " + ex.Message;
                return RedirectToAction("MeetingTypeList");
            }

            if (model == null)
            {
                TempData["ErrorMessage"] = "Meeting type not found.";
                return RedirectToAction("MeetingTypeList");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Save(MeetingTypeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("MeetingTypeAddEdit", model);
            }

            try
            {
                using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
                SqlCommand cmd;

                if (model.MeetingTypeId == 0)
                {
                    // Insert new meeting type
                    cmd = new SqlCommand("PR_MeetingType_Insert", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeetingTypeName", model.MeetingTypeName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? string.Empty);
                }
                else
                {
                    // Update existing meeting type
                    cmd = new SqlCommand("PR_MeetingType_UpdateByPK", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeetingTypeID", model.MeetingTypeId);
                    cmd.Parameters.AddWithValue("@MeetingTypeName", model.MeetingTypeName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? string.Empty);
                }

                con.Open();
                cmd.ExecuteNonQuery();

                TempData["SuccessMessage"] = model.MeetingTypeId == 0
                    ? "Meeting type added successfully!"
                    : "Meeting type updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error saving meeting type: " + ex.Message;
                return View("MeetingTypeAddEdit", model);
            }

            return RedirectToAction("MeetingTypeList");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                TempData["ErrorMessage"] = "Database connection string is not configured. Please check appsettings.json";
                return RedirectToAction("MeetingTypeList");
            }

            try
            {
                using SqlConnection con = new(connectionString);
                using SqlCommand cmd = new("PR_MeetingType_DeleteByPK", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeetingTypeID", id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                TempData["SuccessMessage"] = rowsAffected > 0
                    ? "Meeting type deleted successfully!"
                    : "Meeting type not found.";
            }
            catch (SqlException sqlEx)
            {
                // Check for foreign key constraint violation (Error 547)
                if (sqlEx.Number == 547)
                {
                    TempData["ErrorMessage"] = "Cannot delete this meeting type because it is being used by one or more meetings. Please delete or update those meetings first.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Database error deleting meeting type: {sqlEx.Message} (Error Number: {sqlEx.Number}, Line: {sqlEx.LineNumber})";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting meeting type: {ex.Message}";
            }

            return RedirectToAction("MeetingTypeList");
        }
    }
}
