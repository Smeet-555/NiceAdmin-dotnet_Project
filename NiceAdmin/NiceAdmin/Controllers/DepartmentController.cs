using System.Data;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;
using Microsoft.Data.SqlClient;

namespace NiceAdmin.Controllers;

public class DepartmentController : Controller
{
    private readonly IConfiguration _configuration;

    public DepartmentController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        List<DepartmentViewModel> departments = new();

        try
        {
            using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new("PR_Department_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                departments.Add(new DepartmentViewModel
                {
                    DepartmentId = Convert.ToInt32(reader["DepartmentID"]),
                    DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty,
                    Created = reader["Created"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["Created"]),
                    Modified = reader["Modified"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["Modified"])
                });
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error loading departments: " + ex.Message;
        }

        return View("DepartmentList", departments);
    }
    
    public IActionResult DepartmentAddEdit(int? id)
    {
        if (!id.HasValue || id == 0)
        {
            // Add mode - create new department
            return View(new DepartmentViewModel
            {
                DepartmentId = 0,
                Created = DateTime.Now,
                Modified = DateTime.Now
            });
        }

        DepartmentViewModel model = null;

        try
        {
            using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new("PR_Department_SelectByPK", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DepartmentID", id.Value);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                model = new DepartmentViewModel
                {
                    DepartmentId = Convert.ToInt32(reader["DepartmentID"]),
                    DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty,
                    Created = reader["Created"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["Created"]),
                    Modified = reader["Modified"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["Modified"])
                };
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error loading department: " + ex.Message;
            return RedirectToAction("Index");
        }

        if (model == null)
        {
            TempData["ErrorMessage"] = "Department not found.";
            return RedirectToAction("Index");
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult Save(DepartmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("DepartmentAddEdit", model);
        }

        try
        {
            using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
            SqlCommand cmd;

            if (model.DepartmentId == 0)
            {
                // Insert new department
                cmd = new SqlCommand("PR_Department_Insert", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DepartmentName", model.DepartmentName ?? string.Empty);
            }
            else
            {
                // Update existing department
                cmd = new SqlCommand("PR_Department_UpdateByPK", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DepartmentID", model.DepartmentId);
                cmd.Parameters.AddWithValue("@DepartmentName", model.DepartmentName ?? string.Empty);
            }

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = model.DepartmentId == 0
                ? "Department added successfully!"
                : "Department updated successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error saving department: " + ex.Message;
            return View("DepartmentAddEdit", model);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            TempData["ErrorMessage"] = "Database connection string is not configured. Please check appsettings.json";
            return RedirectToAction("Index");
        }

        try
        {
            using SqlConnection con = new(connectionString);
            using SqlCommand cmd = new("PR_Department_DeleteByPK", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DepartmentID", id);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = rowsAffected > 0
                ? "Department deleted successfully!"
                : "Department not found.";
        }
        catch (SqlException sqlEx)
        {
            // Check for foreign key constraint violation (Error 547)
            if (sqlEx.Number == 547)
            {
                TempData["ErrorMessage"] = "Cannot delete this department because it is being used by staff members or meetings. Please delete or update those records first.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Database error deleting department: {sqlEx.Message} (Error Number: {sqlEx.Number}, Line: {sqlEx.LineNumber})";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting department: {ex.Message}";
        }

        return RedirectToAction("Index");
    }
}
