using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace NiceAdmin.Controllers;

public class HomeController : Controller
{
    private readonly IConfiguration _configuration;

    public HomeController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IActionResult> Index()
    {
        var dashboardData = await GetDashboardDataAsync();
        return View(dashboardData);
    }
    
    public IActionResult PageTitle()
    {
        return View();
    }

    public IActionResult SectionDashboard()
    {
        return View();
    }

    private async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        var dashboard = new DashboardViewModel();

        try
        {
            using var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await con.OpenAsync();

            // Get summary statistics
            await GetSummaryStatistics(con, dashboard);
            
            // Get today's statistics
            await GetTodayStatistics(con, dashboard);
            
            // Get this month's statistics
            await GetThisMonthStatistics(con, dashboard);
            
            // Get recent meetings
            await GetRecentMeetings(con, dashboard);
            
            // Get recent staff
            await GetRecentStaff(con, dashboard);
            
            // Get department statistics
            await GetDepartmentStatistics(con, dashboard);
            
            // Get meeting type statistics
            await GetMeetingTypeStatistics(con, dashboard);
            
            // Get recent activities
            await GetRecentActivities(con, dashboard);
            
            // Get chart data
            await GetChartData(con, dashboard);
        }
        catch (Exception ex)
        {
            // Log error and return empty dashboard
            TempData["ErrorMessage"] = "Error loading dashboard data: " + ex.Message;
        }

        return dashboard;
    }

    private async Task GetSummaryStatistics(SqlConnection con, DashboardViewModel dashboard)
    {
        // Total Meetings - using PR_Meetings_SelectAll
        using (var cmd = new SqlCommand("PR_Meetings_SelectAll", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            using var reader = await cmd.ExecuteReaderAsync();
            int count = 0;
            while (await reader.ReadAsync()) count++;
            dashboard.TotalMeetings = count;
        }

        // Total Staff - using PR_Staff_SelectAll
        using (var cmd = new SqlCommand("PR_Staff_SelectAll", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            using var reader = await cmd.ExecuteReaderAsync();
            int count = 0;
            while (await reader.ReadAsync()) count++;
            dashboard.TotalStaff = count;
        }

        // Total Departments - using PR_Department_SelectAll
        using (var cmd = new SqlCommand("PR_Department_SelectAll", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            using var reader = await cmd.ExecuteReaderAsync();
            int count = 0;
            while (await reader.ReadAsync()) count++;
            dashboard.TotalDepartments = count;
        }

        // Total Meeting Types - using PR_MeetingType_SelectAll
        using (var cmd = new SqlCommand("PR_MeetingType_SelectAll", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            using var reader = await cmd.ExecuteReaderAsync();
            int count = 0;
            while (await reader.ReadAsync()) count++;
            dashboard.TotalMeetingTypes = count;
        }

        // Total Meeting Venues - using PR_MeetingVenue_SelectAll
        using (var cmd = new SqlCommand("PR_MeetingVenue_SelectAll", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            using var reader = await cmd.ExecuteReaderAsync();
            int count = 0;
            while (await reader.ReadAsync()) count++;
            dashboard.TotalMeetingVenues = count;
        }
    }

    private async Task GetTodayStatistics(SqlConnection con, DashboardViewModel dashboard)
    {
        var today = DateTime.Today;

        // Get all meetings using PR_Meetings_SelectAll and filter in code
        using (var cmd = new SqlCommand("PR_Meetings_SelectAll", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            using var reader = await cmd.ExecuteReaderAsync();
            
            int todayMeetings = 0;
            int todayCompleted = 0;
            int todayScheduled = 0;
            int todayCancelled = 0;
            
            while (await reader.ReadAsync())
            {
                var meetingDate = Convert.ToDateTime(reader["MeetingDate"]);
                var isCancelled = reader["IsCancelled"] == DBNull.Value ? false : Convert.ToBoolean(reader["IsCancelled"]);
                
                if (meetingDate.Date == today)
                {
                    todayMeetings++;
                    
                    if (isCancelled)
                    {
                        todayCancelled++;
                    }
                    else if (meetingDate < DateTime.Now)
                    {
                        todayCompleted++;
                    }
                    else
                    {
                        todayScheduled++;
                    }
                }
            }
            
            dashboard.TodayMeetings = todayMeetings;
            dashboard.TodayCompletedMeetings = todayCompleted;
            dashboard.TodayScheduledMeetings = todayScheduled;
            dashboard.TodayCancelledMeetings = todayCancelled;
        }
    }

    private async Task GetThisMonthStatistics(SqlConnection con, DashboardViewModel dashboard)
    {
        var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        // This Month's Meetings - using PR_Meetings_SelectAll and filter in code
        using (var cmd = new SqlCommand("PR_Meetings_SelectAll", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            using var reader = await cmd.ExecuteReaderAsync();
            
            int count = 0;
            while (await reader.ReadAsync())
            {
                var meetingDate = Convert.ToDateTime(reader["MeetingDate"]);
                if (meetingDate >= firstDayOfMonth)
                {
                    count++;
                }
            }
            dashboard.ThisMonthMeetings = count;
        }

        // This Month's Staff Added - using PR_Staff_SelectAll and filter in code
        using (var cmd = new SqlCommand("PR_Staff_SelectAll", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            using var reader = await cmd.ExecuteReaderAsync();
            
            int count = 0;
            while (await reader.ReadAsync())
            {
                var modified = reader["Modified"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Modified"]);
                if (modified >= firstDayOfMonth)
                {
                    count++;
                }
            }
            dashboard.ThisMonthStaffAdded = count;
        }

        // This Month's Departments Added - using PR_Department_SelectAll and filter in code
        using (var cmd = new SqlCommand("PR_Department_SelectAll", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            using var reader = await cmd.ExecuteReaderAsync();
            
            int count = 0;
            while (await reader.ReadAsync())
            {
                var modified = reader["Modified"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Modified"]);
                if (modified >= firstDayOfMonth)
                {
                    count++;
                }
            }
            dashboard.ThisMonthDepartmentsAdded = count;
        }
    }

    private async Task GetRecentMeetings(SqlConnection con, DashboardViewModel dashboard)
    {
        // Using PR_Meetings_SelectAll which already orders by MeetingDate DESC
        using var cmd = new SqlCommand("PR_Meetings_SelectAll", con);
        cmd.CommandType = CommandType.StoredProcedure;

        using var reader = await cmd.ExecuteReaderAsync();
        int count = 0;
        while (await reader.ReadAsync() && count < 5)
        {
            dashboard.RecentMeetings.Add(new MeetingViewModel
            {
                MeetingId = Convert.ToInt32(reader["MeetingID"]),
                MeetingDate = Convert.ToDateTime(reader["MeetingDate"]),
                MeetingTypeName = reader["MeetingTypeName"]?.ToString() ?? string.Empty,
                MeetingVenueName = reader["MeetingVenueName"]?.ToString() ?? string.Empty,
                DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty,
                IsCancelled = reader["IsCancelled"] == DBNull.Value ? false : Convert.ToBoolean(reader["IsCancelled"]),
                MeetingDescription = reader["MeetingDescription"]?.ToString() ?? string.Empty
            });
            count++;
        }
    }

    private async Task GetRecentStaff(SqlConnection con, DashboardViewModel dashboard)
    {
        // Using PR_Staff_SelectAll which includes department name
        using var cmd = new SqlCommand("PR_Staff_SelectAll", con);
        cmd.CommandType = CommandType.StoredProcedure;

        using var reader = await cmd.ExecuteReaderAsync();
        
        // Read all staff and sort by Modified date
        var allStaff = new List<(StaffModelView staff, DateTime modified)>();
        
        while (await reader.ReadAsync())
        {
            var staff = new StaffModelView
            {
                StaffId = Convert.ToInt32(reader["StaffID"]),
                StaffName = reader["StaffName"]?.ToString() ?? string.Empty,
                EmailAddress = reader["EmailAddress"]?.ToString() ?? string.Empty,
                DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty
            };
            
            var modified = reader["Modified"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Modified"]);
            allStaff.Add((staff, modified));
        }
        
        // Sort by modified date descending and take top 5
        dashboard.RecentStaff = allStaff
            .OrderByDescending(x => x.modified)
            .Take(5)
            .Select(x => x.staff)
            .ToList();
    }

    private async Task GetDepartmentStatistics(SqlConnection con, DashboardViewModel dashboard)
    {
        // Get all departments using PR_Department_SelectAll
        var departments = new Dictionary<int, string>();
        using (var cmd = new SqlCommand("PR_Department_SelectAll", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            using var reader = await cmd.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                var deptId = Convert.ToInt32(reader["DepartmentID"]);
                var deptName = reader["DepartmentName"]?.ToString() ?? string.Empty;
                departments[deptId] = deptName;
            }
        }

        // Get all staff using PR_Staff_SelectAll
        var staffByDept = new Dictionary<int, int>();
        using (var cmd = new SqlCommand("PR_Staff_SelectAll", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            using var reader = await cmd.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                var deptId = Convert.ToInt32(reader["DepartmentID"]);
                if (!staffByDept.ContainsKey(deptId))
                    staffByDept[deptId] = 0;
                staffByDept[deptId]++;
            }
        }

        // Get all meetings using PR_Meetings_SelectAll
        var meetingsByDept = new Dictionary<int, int>();
        var activeMeetingsByDept = new Dictionary<int, int>();
        
        using (var cmd = new SqlCommand("PR_Meetings_SelectAll", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            using var reader = await cmd.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                var deptId = Convert.ToInt32(reader["DepartmentID"]);
                var meetingDate = Convert.ToDateTime(reader["MeetingDate"]);
                var isCancelled = reader["IsCancelled"] == DBNull.Value ? false : Convert.ToBoolean(reader["IsCancelled"]);
                
                if (!meetingsByDept.ContainsKey(deptId))
                    meetingsByDept[deptId] = 0;
                meetingsByDept[deptId]++;
                
                if (meetingDate > DateTime.Now && !isCancelled)
                {
                    if (!activeMeetingsByDept.ContainsKey(deptId))
                        activeMeetingsByDept[deptId] = 0;
                    activeMeetingsByDept[deptId]++;
                }
            }
        }

        // Combine all statistics
        foreach (var dept in departments)
        {
            dashboard.DepartmentStats.Add(new DepartmentStatsViewModel
            {
                DepartmentName = dept.Value,
                MeetingCount = meetingsByDept.ContainsKey(dept.Key) ? meetingsByDept[dept.Key] : 0,
                StaffCount = staffByDept.ContainsKey(dept.Key) ? staffByDept[dept.Key] : 0,
                ActiveMeetings = activeMeetingsByDept.ContainsKey(dept.Key) ? activeMeetingsByDept[dept.Key] : 0
            });
        }

        // Sort by meeting count descending
        dashboard.DepartmentStats = dashboard.DepartmentStats.OrderByDescending(d => d.MeetingCount).ToList();
    }

    private async Task GetMeetingTypeStatistics(SqlConnection con, DashboardViewModel dashboard)
    {
        // Get all meeting types using PR_MeetingType_SelectAll
        var meetingTypes = new Dictionary<int, string>();
        using (var cmd = new SqlCommand("PR_MeetingType_SelectAll", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            using var reader = await cmd.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                var typeId = Convert.ToInt32(reader["MeetingTypeID"]);
                var typeName = reader["MeetingTypeName"]?.ToString() ?? string.Empty;
                meetingTypes[typeId] = typeName;
            }
        }

        // Get all meetings and calculate statistics
        var meetingCountByType = new Dictionary<int, int>();
        var successfulMeetingsByType = new Dictionary<int, int>();
        
        using (var cmd = new SqlCommand("PR_Meetings_SelectAll", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            using var reader = await cmd.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                var typeId = Convert.ToInt32(reader["MeetingTypeID"]);
                var isCancelled = reader["IsCancelled"] == DBNull.Value ? false : Convert.ToBoolean(reader["IsCancelled"]);
                
                if (!meetingCountByType.ContainsKey(typeId))
                    meetingCountByType[typeId] = 0;
                meetingCountByType[typeId]++;
                
                if (!isCancelled)
                {
                    if (!successfulMeetingsByType.ContainsKey(typeId))
                        successfulMeetingsByType[typeId] = 0;
                    successfulMeetingsByType[typeId]++;
                }
            }
        }

        // Combine statistics
        foreach (var type in meetingTypes)
        {
            var count = meetingCountByType.ContainsKey(type.Key) ? meetingCountByType[type.Key] : 0;
            var successful = successfulMeetingsByType.ContainsKey(type.Key) ? successfulMeetingsByType[type.Key] : 0;
            var successRate = count > 0 ? (successful * 100.0 / count) : 0;
            
            dashboard.MeetingTypeStats.Add(new MeetingTypeStatsViewModel
            {
                MeetingTypeName = type.Value,
                Count = count,
                SuccessRate = successRate,
                AverageDuration = "2h 30m" // Placeholder since we don't have duration data
            });
        }

        // Sort by count descending
        dashboard.MeetingTypeStats = dashboard.MeetingTypeStats.OrderByDescending(m => m.Count).ToList();
    }

    private async Task GetRecentActivities(SqlConnection con, DashboardViewModel dashboard)
    {
        // Generate recent activities based on recent data changes
        var activities = new List<DashboardActivityViewModel>();

        // Recent meetings
        foreach (var meeting in dashboard.RecentMeetings.Take(3))
        {
            var timeAgo = GetTimeAgo(meeting.MeetingDate);
            activities.Add(new DashboardActivityViewModel
            {
                Activity = $"Meeting '{meeting.MeetingDescription}' scheduled in {meeting.MeetingVenueName}",
                TimeAgo = timeAgo,
                Type = meeting.IsCancelled ? "danger" : "success",
                CreatedDate = meeting.MeetingDate
            });
        }

        // Recent staff
        foreach (var staff in dashboard.RecentStaff.Take(2))
        {
            activities.Add(new DashboardActivityViewModel
            {
                Activity = $"New staff member {staff.StaffName} added to {staff.DepartmentName}",
                TimeAgo = "Recently",
                Type = "primary",
                CreatedDate = DateTime.Now
            });
        }

        dashboard.RecentActivities = activities.OrderByDescending(a => a.CreatedDate).Take(5).ToList();
    }

    private async Task GetChartData(SqlConnection con, DashboardViewModel dashboard)
    {
        // Get meetings from the last 7 days using PR_Meetings_SelectAll
        var sevenDaysAgo = DateTime.Now.AddDays(-7);
        var meetingsByDay = new Dictionary<DateTime, int>();
        
        using var cmd = new SqlCommand("PR_Meetings_SelectAll", con);
        cmd.CommandType = CommandType.StoredProcedure;

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var meetingDate = Convert.ToDateTime(reader["MeetingDate"]);
            if (meetingDate >= sevenDaysAgo)
            {
                var dateOnly = meetingDate.Date;
                if (!meetingsByDay.ContainsKey(dateOnly))
                    meetingsByDay[dateOnly] = 0;
                meetingsByDay[dateOnly]++;
            }
        }

        // Create chart data for the last 7 days
        for (int i = 6; i >= 0; i--)
        {
            var date = DateTime.Today.AddDays(-i);
            var count = meetingsByDay.ContainsKey(date) ? meetingsByDay[date] : 0;
            
            dashboard.WeeklyMeetingData.Add(new ChartDataViewModel
            {
                Label = date.ToString("MM/dd"),
                Value = count,
                Date = date
            });
        }
    }

    private string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.Now - dateTime;
        
        if (timeSpan.TotalMinutes < 1)
            return "Just now";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} min ago";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} hr ago";
        if (timeSpan.TotalDays < 7)
            return $"{(int)timeSpan.TotalDays} day(s) ago";
        
        return dateTime.ToString("MMM dd");
    }
}