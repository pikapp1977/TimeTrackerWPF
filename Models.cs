namespace TimeTrackerWPF
{
    public class Location
    {
        public int Id { get; set; }
        public string FacilityName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public decimal PayRate { get; set; }
        public string PayRateType { get; set; } = string.Empty;

        public string PayRateDisplay => $"${PayRate:F2} ({PayRateType})";
    }

    public class TimeEntry
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public string DepartureTime { get; set; } = string.Empty;
        public decimal DailyPay { get; set; }
        public string Notes { get; set; } = string.Empty;
        public bool Locked { get; set; }
        public bool Archived { get; set; }

        public string HoursWorked
        {
            get
            {
                try
                {
                    DateTime arrival = DateTime.Parse(ArrivalTime);
                    DateTime departure = DateTime.Parse(DepartureTime);
                    if (departure < arrival) departure = departure.AddDays(1);
                    return ((departure - arrival).TotalHours).ToString("F2");
                }
                catch { return "0.00"; }
            }
        }

        public string DailyPayDisplay => $"${DailyPay:F2}";
        
        // Separate display properties for Locked and Archived columns
        public string LockedDisplay => Locked ? "🔒" : "";
        public string ArchivedDisplay => Archived ? "📦" : "";
    }

    public class BusinessSettings
    {
        public string BusinessName { get; set; } = string.Empty;
        public string BusinessAddress { get; set; } = string.Empty;
        public string BusinessCity { get; set; } = string.Empty;
        public string BusinessState { get; set; } = string.Empty;
        public string BusinessZip { get; set; } = string.Empty;
        public string BusinessPhone { get; set; } = string.Empty;
        public string BusinessEmail { get; set; } = string.Empty;
        public bool ShowClearAllButton { get; set; }
    }
}
