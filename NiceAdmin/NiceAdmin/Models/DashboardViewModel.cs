namespace NiceAdmin.Models
{
    public class DashboardViewModel
    {

        public int TotalMeetings { get; set; }
        public int TotalStaff { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalMeetingTypes { get; set; }
        public int TotalMeetingVenues { get; set; }


        public int TodayMeetings { get; set; }
        public int TodayCompletedMeetings { get; set; }
        public int TodayScheduledMeetings { get; set; }
        public int TodayCancelledMeetings { get; set; }


        public int ThisMonthMeetings { get; set; }
        public int ThisMonthStaffAdded { get; set; }
        public int ThisMonthDepartmentsAdded { get; set; }


        public List<MeetingViewModel> RecentMeetings { get; set; } = new();
        public List<StaffModelView> RecentStaff { get; set; } = new();
        public List<DepartmentStatsViewModel> DepartmentStats { get; set; } = new();
        public List<MeetingTypeStatsViewModel> MeetingTypeStats { get; set; } = new();
        public List<DashboardActivityViewModel> RecentActivities { get; set; } = new();
        public List<ChartDataViewModel> WeeklyMeetingData { get; set; } = new();
    }

    public class DepartmentStatsViewModel
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int MeetingCount { get; set; }
        public int StaffCount { get; set; }
        public int ActiveMeetings { get; set; }
    }

    public class MeetingTypeStatsViewModel
    {
        public string MeetingTypeName { get; set; } = string.Empty;
        public int Count { get; set; }
        public double SuccessRate { get; set; }
        public string AverageDuration { get; set; } = string.Empty;
    }

    public class DashboardActivityViewModel
    {
        public string Activity { get; set; } = string.Empty;
        public string TimeAgo { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class ChartDataViewModel
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
        public DateTime Date { get; set; }
    }
}
