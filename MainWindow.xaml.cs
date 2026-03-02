using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TimeTrackerWPF
{
    public partial class MainWindow : Window
    {
        private readonly string dbPath;
        private List<Location> locations;
        private List<TimeEntry> timeEntries;
        private BusinessSettings businessSettings;
        private readonly TimeEntryService timeEntryService;

        public MainWindow()
        {
            InitializeComponent();

            string appDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TimeTracker");
            if (!Directory.Exists(appDataDir))
                Directory.CreateDirectory(appDataDir);
            dbPath = Path.Combine(appDataDir, "timetracker.db");

            locations = new List<Location>();
            timeEntries = new List<TimeEntry>();
            businessSettings = new BusinessSettings();
            timeEntryService = new TimeEntryService(dbPath);

            InitializeDatabase();
            LoadLocations();
            LoadTimeEntries();
            LoadBusinessSettings();
            InitializeControls();
            RefreshLocationsList();
            RefreshTimeEntriesList();
            UpdateTotals();
            UpdateClearAllButtonVisibility();
            
            txtDbPath.Text = dbPath;
            UpdateDatabaseSize();
            dtpDate.SelectedDate = DateTime.Today;
            dtpStartDate.SelectedDate = DateTime.Today;
            dtpEndDate.SelectedDate = DateTime.Today;
        }

        private void UpdateDatabaseSize()
        {
            if (File.Exists(dbPath))
            {
                FileInfo fileInfo = new FileInfo(dbPath);
                double sizeInMB = fileInfo.Length / (1024.0 * 1024.0);
                txtDbSize.Text = $"Database Size: {sizeInMB:F2} MB";
            }
            else
            {
                txtDbSize.Text = "Database Size: N/A";
            }
        }

        private void InitializeControls()
        {
            // Populate hour dropdowns
            for (int i = 1; i <= 12; i++)
            {
                cmbArrivalHour.Items.Add(i);
                cmbDepartureHour.Items.Add(i);
            }
            cmbArrivalHour.SelectedIndex = 6; // 7 AM
            cmbDepartureHour.SelectedIndex = 4; // 5 PM

            // Populate minute dropdowns
            cmbArrivalMin.Items.Add("00");
            cmbArrivalMin.Items.Add("15");
            cmbArrivalMin.Items.Add("30");
            cmbArrivalMin.Items.Add("45");
            cmbDepartureMin.Items.Add("00");
            cmbDepartureMin.Items.Add("15");
            cmbDepartureMin.Items.Add("30");
            cmbDepartureMin.Items.Add("45");
            cmbArrivalMin.SelectedIndex = 0;
            cmbDepartureMin.SelectedIndex = 0;

            // Load business settings into form
            txtBusinessName.Text = businessSettings.BusinessName;
            txtBusinessAddress.Text = businessSettings.BusinessAddress;
            txtBusinessCity.Text = businessSettings.BusinessCity;
            txtBusinessState.Text = businessSettings.BusinessState;
            txtBusinessZip.Text = businessSettings.BusinessZip;
            txtBusinessPhone.Text = businessSettings.BusinessPhone;
            txtBusinessEmail.Text = businessSettings.BusinessEmail;
            chkShowClearAll.IsChecked = businessSettings.ShowClearAllButton;
        }

        #region Database Methods

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Locations (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FacilityName TEXT NOT NULL,
                    ContactName TEXT NOT NULL,
                    ContactEmail TEXT,
                    ContactPhone TEXT,
                    Address TEXT,
                    City TEXT,
                    State TEXT,
                    Zip TEXT,
                    PayRate REAL NOT NULL,
                    PayRateType TEXT NOT NULL
                )";
            command.ExecuteNonQuery();

            command.CommandText = "ALTER TABLE Locations ADD COLUMN City TEXT";
            try { command.ExecuteNonQuery(); } catch { }

            command.CommandText = "ALTER TABLE Locations ADD COLUMN State TEXT";
            try { command.ExecuteNonQuery(); } catch { }

            command.CommandText = "ALTER TABLE Locations ADD COLUMN Zip TEXT";
            try { command.ExecuteNonQuery(); } catch { }

            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS TimeEntries (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    LocationId INTEGER NOT NULL,
                    Date TEXT NOT NULL,
                    ArrivalTime TEXT NOT NULL,
                    DepartureTime TEXT NOT NULL,
                    DailyPay REAL NOT NULL,
                    Notes TEXT,
                    FOREIGN KEY (LocationId) REFERENCES Locations(Id)
                )";
            command.ExecuteNonQuery();

            command.CommandText = "ALTER TABLE TimeEntries ADD COLUMN Locked INTEGER DEFAULT 0";
            try { command.ExecuteNonQuery(); } catch { }

            command.CommandText = "ALTER TABLE TimeEntries ADD COLUMN Archived INTEGER DEFAULT 0";
            try { command.ExecuteNonQuery(); } catch { }

            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS BusinessSettings (
                    Id INTEGER PRIMARY KEY CHECK (Id = 1),
                    BusinessName TEXT,
                    BusinessAddress TEXT,
                    BusinessCity TEXT,
                    BusinessState TEXT,
                    BusinessZip TEXT,
                    BusinessPhone TEXT,
                    BusinessEmail TEXT
                )";
            command.ExecuteNonQuery();

            command.CommandText = "ALTER TABLE BusinessSettings ADD COLUMN BusinessCity TEXT";
            try { command.ExecuteNonQuery(); } catch { }

            command.CommandText = "ALTER TABLE BusinessSettings ADD COLUMN BusinessState TEXT";
            try { command.ExecuteNonQuery(); } catch { }

            command.CommandText = "ALTER TABLE BusinessSettings ADD COLUMN BusinessZip TEXT";
            try { command.ExecuteNonQuery(); } catch { }

            command.CommandText = "INSERT OR IGNORE INTO BusinessSettings (Id) VALUES (1)";
            try { command.ExecuteNonQuery(); } catch { }

            command.CommandText = "ALTER TABLE BusinessSettings ADD COLUMN ShowClearAllButton INTEGER DEFAULT 0";
            try { command.ExecuteNonQuery(); } catch { }

            // Create indexes for improved query performance
            command.CommandText = "CREATE INDEX IF NOT EXISTS idx_timeentries_date ON TimeEntries(Date)";
            try { command.ExecuteNonQuery(); } catch { }

            command.CommandText = "CREATE INDEX IF NOT EXISTS idx_timeentries_location ON TimeEntries(LocationId)";
            try { command.ExecuteNonQuery(); } catch { }

            command.CommandText = "CREATE INDEX IF NOT EXISTS idx_timeentries_status ON TimeEntries(Locked, Archived)";
            try { command.ExecuteNonQuery(); } catch { }

            command.CommandText = "CREATE INDEX IF NOT EXISTS idx_timeentries_invoice ON TimeEntries(LocationId, Date, Locked, Archived)";
            try { command.ExecuteNonQuery(); } catch { }
        }

        private void LoadLocations()
        {
            locations.Clear();
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, FacilityName, ContactName, ContactEmail, ContactPhone, Address, City, State, Zip, PayRate, PayRateType FROM Locations ORDER BY FacilityName";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                locations.Add(new Location
                {
                    Id = reader.GetInt32(0),
                    FacilityName = reader.GetString(1),
                    ContactName = reader.GetString(2),
                    ContactEmail = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    ContactPhone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Address = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    City = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    State = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    Zip = reader.IsDBNull(8) ? "" : reader.GetString(8),
                    PayRate = (decimal)reader.GetDouble(9),
                    PayRateType = reader.GetString(10)
                });
            }
        }

        private void LoadTimeEntries()
        {
            timeEntries.Clear();
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();

            bool showArchived = chkShowArchived.IsChecked ?? false;
            string whereClause = showArchived ? "" : "WHERE (t.Archived = 0 OR t.Archived IS NULL)";

            command.CommandText = $@"
                SELECT t.Id, t.LocationId, l.FacilityName, t.Date, t.ArrivalTime, t.DepartureTime, t.DailyPay, t.Notes, t.Locked, t.Archived
                FROM TimeEntries t
                JOIN Locations l ON t.LocationId = l.Id
                {whereClause}
                ORDER BY t.Date DESC";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                timeEntries.Add(new TimeEntry
                {
                    Id = reader.GetInt32(0),
                    LocationId = reader.GetInt32(1),
                    LocationName = reader.GetString(2),
                    Date = reader.GetString(3),
                    ArrivalTime = reader.GetString(4),
                    DepartureTime = reader.GetString(5),
                    DailyPay = (decimal)reader.GetDouble(6),
                    Notes = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    Locked = !reader.IsDBNull(8) && reader.GetInt32(8) == 1,
                    Archived = !reader.IsDBNull(9) && reader.GetInt32(9) == 1
                });
            }
        }

        private void LoadBusinessSettings()
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT BusinessName, BusinessAddress, BusinessCity, BusinessState, BusinessZip, BusinessPhone, BusinessEmail, ShowClearAllButton FROM BusinessSettings WHERE Id = 1";
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                businessSettings = new BusinessSettings
                {
                    BusinessName = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    BusinessAddress = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    BusinessCity = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    BusinessState = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    BusinessZip = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    BusinessPhone = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    BusinessEmail = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    ShowClearAllButton = !reader.IsDBNull(7) && reader.GetInt32(7) == 1
                };
            }
        }

        private void SaveBusinessSettings()
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR REPLACE INTO BusinessSettings (Id, BusinessName, BusinessAddress, BusinessCity, BusinessState, BusinessZip, BusinessPhone, BusinessEmail, ShowClearAllButton)
                VALUES (1, $name, $address, $city, $state, $zip, $phone, $email, $showClearAll)";
            command.Parameters.AddWithValue("$name", businessSettings.BusinessName);
            command.Parameters.AddWithValue("$address", businessSettings.BusinessAddress);
            command.Parameters.AddWithValue("$city", businessSettings.BusinessCity);
            command.Parameters.AddWithValue("$state", businessSettings.BusinessState);
            command.Parameters.AddWithValue("$zip", businessSettings.BusinessZip);
            command.Parameters.AddWithValue("$phone", businessSettings.BusinessPhone);
            command.Parameters.AddWithValue("$email", businessSettings.BusinessEmail);
            command.Parameters.AddWithValue("$showClearAll", businessSettings.ShowClearAllButton ? 1 : 0);
            command.ExecuteNonQuery();
        }

        private void AddLocation(string facilityName, string contactName, string email, string phone,
            string address, string city, string state, string zip, decimal payRate, string payRateType)
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Locations (FacilityName, ContactName, ContactEmail, ContactPhone, Address, City, State, Zip, PayRate, PayRateType)
                VALUES ($facilityName, $contactName, $contactEmail, $contactPhone, $address, $city, $state, $zip, $payRate, $payRateType)";
            command.Parameters.AddWithValue("$facilityName", facilityName);
            command.Parameters.AddWithValue("$contactName", contactName);
            command.Parameters.AddWithValue("$contactEmail", email);
            command.Parameters.AddWithValue("$contactPhone", phone);
            command.Parameters.AddWithValue("$address", address);
            command.Parameters.AddWithValue("$city", city);
            command.Parameters.AddWithValue("$state", state);
            command.Parameters.AddWithValue("$zip", zip);
            command.Parameters.AddWithValue("$payRate", (double)payRate);
            command.Parameters.AddWithValue("$payRateType", payRateType);
            command.ExecuteNonQuery();
        }

        private void UpdateLocation(int id, string facilityName, string contactName, string email, string phone,
            string address, string city, string state, string zip, decimal payRate, string payRateType)
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Locations 
                SET FacilityName = $facilityName, ContactName = $contactName, ContactEmail = $contactEmail, 
                    ContactPhone = $contactPhone, Address = $address, City = $city, State = $state, Zip = $zip, PayRate = $payRate, PayRateType = $payRateType
                WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$facilityName", facilityName);
            command.Parameters.AddWithValue("$contactName", contactName);
            command.Parameters.AddWithValue("$contactEmail", email);
            command.Parameters.AddWithValue("$contactPhone", phone);
            command.Parameters.AddWithValue("$address", address);
            command.Parameters.AddWithValue("$city", city);
            command.Parameters.AddWithValue("$state", state);
            command.Parameters.AddWithValue("$zip", zip);
            command.Parameters.AddWithValue("$payRate", (double)payRate);
            command.Parameters.AddWithValue("$payRateType", payRateType);
            command.ExecuteNonQuery();
        }

        private void DeleteLocation(int id)
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM TimeEntries WHERE LocationId = $id";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();

            command.CommandText = "DELETE FROM Locations WHERE Id = $id";
            command.ExecuteNonQuery();
        }

        private void AddTimeEntry(int locationId, string date, string arrivalTime, string departureTime, decimal dailyPay, string notes)
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO TimeEntries (LocationId, Date, ArrivalTime, DepartureTime, DailyPay, Notes, Locked, Archived)
                VALUES ($locationId, $date, $arrivalTime, $departureTime, $dailyPay, $notes, 0, 0)";
            command.Parameters.AddWithValue("$locationId", locationId);
            command.Parameters.AddWithValue("$date", date);
            command.Parameters.AddWithValue("$arrivalTime", arrivalTime);
            command.Parameters.AddWithValue("$departureTime", departureTime);
            command.Parameters.AddWithValue("$dailyPay", (double)dailyPay);
            command.Parameters.AddWithValue("$notes", notes);
            command.ExecuteNonQuery();
        }

        private void DeleteTimeEntry(int id)
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();

            // Check if entry is locked
            command.CommandText = "SELECT Locked FROM TimeEntries WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);
            var result = command.ExecuteScalar();

            if (result != null && Convert.ToInt32(result) == 1)
            {
                MessageBox.Show("This entry is locked and cannot be deleted. Unlock it first.", "Cannot Delete", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            command.CommandText = "DELETE FROM TimeEntries WHERE Id = $id";
            command.ExecuteNonQuery();
        }

        #endregion

        #region UI Refresh Methods

        private void RefreshLocationsList()
        {
            lstLocations.ItemsSource = null;
            lstLocations.ItemsSource = locations;

            cmbLocation.ItemsSource = null;
            cmbLocation.ItemsSource = locations;

            cmbInvoiceLocation.ItemsSource = null;
            cmbInvoiceLocation.ItemsSource = locations;
        }

        private void RefreshTimeEntriesList()
        {
            lstTimeEntries.ItemsSource = null;
            lstTimeEntries.ItemsSource = timeEntries;

            // Apply row styling
            lstTimeEntries.Items.Refresh();
        }

        private void UpdateTotals()
        {
            decimal totalPay = 0;
            double totalHours = 0;

            foreach (var entry in timeEntries.Where(e => !e.Archived))
            {
                totalPay += entry.DailyPay;
                totalHours += double.Parse(entry.HoursWorked);
            }

            lblTotalHours.Content = $"Total Hours: {totalHours:F2}";
            lblTotalPay.Content = $"Total Pay: ${totalPay:F2}";
        }

        private void UpdateClearAllButtonVisibility()
        {
            btnClearAll.Visibility = businessSettings.ShowClearAllButton ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Helper Methods

        private decimal CalculateDailyPay(Location location, string arrival, string departure)
        {
            return CalculationHelpers.CalculateDailyPay(location, arrival, departure);
        }

        private double CalculateHoursWorked(string arrival, string departure)
        {
            return CalculationHelpers.CalculateHoursWorked(arrival, departure);
        }

        #endregion

        // Continue in next message due to length...
    }
}
