namespace TimeTrackerWPF
{
    /// <summary>
    /// Helper class for calculations used in time tracking
    /// </summary>
    public static class CalculationHelpers
    {
        /// <summary>
        /// Calculates daily pay based on location pay rate and hours worked
        /// </summary>
        public static decimal CalculateDailyPay(Location location, string arrival, string departure)
        {
            if (location.PayRateType == "Per Day")
            {
                return location.PayRate;
            }
            
            double hours = CalculateHoursWorked(arrival, departure);
            return location.PayRate * (decimal)hours;
        }

        /// <summary>
        /// Calculates hours worked between arrival and departure times
        /// Handles overnight shifts (departure before arrival means next day)
        /// </summary>
        public static double CalculateHoursWorked(string arrival, string departure)
        {
            try
            {
                DateTime arrivalTime = DateTime.Parse(arrival);
                DateTime departureTime = DateTime.Parse(departure);

                if (departureTime < arrivalTime)
                {
                    departureTime = departureTime.AddDays(1);
                }

                TimeSpan diff = departureTime - arrivalTime;
                return diff.TotalHours;
            }
            catch
            {
                return 0;
            }
        }
    }
}
