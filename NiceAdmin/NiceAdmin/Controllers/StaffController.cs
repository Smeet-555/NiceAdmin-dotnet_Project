using System.Data;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;
using Microsoft.Data.SqlClient;

namespace NiceAdmin.Controllers
{
    public class StaffController : Controller
    {
        private readonly IConfiguration _configuration;

        public StaffController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return StaffList();
        }

        public IActionResult StaffList()
        {
            List<StaffModelView> staffList = new();

            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    TempData["ErrorMessage"] = "Connection string is not configured!";
                    return View("StaffList", staffList);
                }

                using SqlConnection con = new(connectionString);
                using SqlCommand cmd = new("PR_Staff_SelectAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    staffList.Add(new StaffModelView
                    {
                        StaffId = Convert.ToInt32(reader["StaffID"]),
                        StaffName = reader["StaffName"]?.ToString() ?? string.Empty,
                        DepartmentId = Convert.ToInt32(reader["DepartmentID"]),
                        MobileNo = reader["MobileNo"]?.ToString() ?? string.Empty,
                        EmailAddress = reader["EmailAddress"]?.ToString() ?? string.Empty,
                        Remarks = reader["Remarks"]?.ToString() ?? string.Empty,
                        DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty
                    });
                }
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = $"SQL Error: {sqlEx.Message} | Error Number: {sqlEx.Number} | Line: {sqlEx.LineNumber}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.GetType().Name} - {ex.Message}";
            }

            return View("StaffList", staffList);
        }

        [HttpGet]
        public IActionResult StaffAddEdit(int? id)
        {
            // Load dropdown data
            LoadDropdownData();

            if (!id.HasValue || id == 0)
            {
                return View(new StaffModelView
                {
                    StaffId = 0
                });
            }

            StaffModelView model = null;

            try
            {
                using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new("PR_Staff_SelectByPK", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StaffID", id.Value);

                con.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    model = new StaffModelView
                    {
                        StaffId = Convert.ToInt32(reader["StaffID"]),
                        StaffName = reader["StaffName"]?.ToString() ?? string.Empty,
                        DepartmentId = Convert.ToInt32(reader["DepartmentID"]),
                        MobileNo = reader["MobileNo"]?.ToString() ?? string.Empty,
                        EmailAddress = reader["EmailAddress"]?.ToString() ?? string.Empty,
                        Remarks = reader["Remarks"]?.ToString() ?? string.Empty,
                        DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty
                    };
                }
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = $"SQL Error loading staff: {sqlEx.Message} | Error Number: {sqlEx.Number}";
                return RedirectToAction("StaffList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading staff member: {ex.Message}";
                return RedirectToAction("StaffList");
            }

            if (model == null)
            {
                TempData["ErrorMessage"] = "Staff member not found.";
                return RedirectToAction("StaffList");
            }

            return View(model);
        }

        private void LoadDropdownData()
        {
            try
            {
                using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
                con.Open();

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
                TempData["ErrorMessage"] = $"SQL Error loading departments: {sqlEx.Message} | Error Number: {sqlEx.Number}";
                ViewBag.Departments = new List<object>();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading dropdown data: {ex.Message}";
                ViewBag.Departments = new List<object>();
            }
        }

        [HttpPost]
        public IActionResult Save(StaffModelView model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdownData();
                return View("StaffAddEdit", model);
            }

            try
            {
                using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
                SqlCommand cmd;

                if (model.StaffId == 0)
                {
                    cmd = new SqlCommand("PR_Staff_Insert", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DepartmentID", model.DepartmentId);
                    cmd.Parameters.AddWithValue("@StaffName", model.StaffName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@MobileNo", model.MobileNo ?? string.Empty);
                    cmd.Parameters.AddWithValue("@EmailAddress", model.EmailAddress ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? string.Empty);
                }
                else
                {
                    cmd = new SqlCommand("PR_Staff_UpdateByPK", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StaffID", model.StaffId);
                    cmd.Parameters.AddWithValue("@DepartmentID", model.DepartmentId);
                    cmd.Parameters.AddWithValue("@StaffName", model.StaffName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@MobileNo", model.MobileNo ?? string.Empty);
                    cmd.Parameters.AddWithValue("@EmailAddress", model.EmailAddress ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? string.Empty);
                }

                con.Open();
                cmd.ExecuteNonQuery();

                TempData["SuccessMessage"] = model.StaffId == 0
                    ? "Staff member added successfully!"
                    : "Staff member updated successfully!";
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = $"SQL Error saving staff: {sqlEx.Message} | Error Number: {sqlEx.Number}";
                LoadDropdownData();
                return View("StaffAddEdit", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error saving staff member: {ex.Message}";
                LoadDropdownData();
                return View("StaffAddEdit", model);
            }

            return RedirectToAction("StaffList");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                TempData["ErrorMessage"] = "Database connection string is not configured. Please check appsettings.json";
                return RedirectToAction("StaffList");
            }

            try
            {
                using SqlConnection con = new(connectionString);
                using SqlCommand cmd = new("PR_Staff_DeleteByPK", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StaffID", id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                TempData["SuccessMessage"] = rowsAffected > 0
                    ? "Staff member deleted successfully!"
                    : "Staff member not found.";
            }
            catch (SqlException sqlEx)
            {
                // Check for foreign key constraint violation (Error 547)
                if (sqlEx.Number == 547)
                {
                    TempData["ErrorMessage"] = "Cannot delete this staff member because they are assigned to one or more meetings. Please remove them from those meetings first.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Database error deleting staff: {sqlEx.Message} (Error Number: {sqlEx.Number}, Line: {sqlEx.LineNumber})";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting staff member: {ex.Message}";
            }

            return RedirectToAction("StaffList");
        }
    }
}
