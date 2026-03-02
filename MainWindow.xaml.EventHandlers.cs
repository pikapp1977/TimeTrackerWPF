using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace TimeTrackerWPF
{
    public partial class MainWindow : Window
    {
        #region Menu Event Handlers

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string versionString = version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "Unknown";

            MessageBox.Show(
                $"Time Tracker WPF\n\n" +
                $"Version {versionString}\n\n" +
                $"A WPF application for tracking work hours and generating invoices\n\n" +
                $"© {DateTime.Now.Year} Personal",
                "About Time Tracker",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        #endregion

        #region Location Event Handlers

        private void BtnAddLocation_Click(object sender, RoutedEventArgs e)
        {
            string facilityName = txtFacilityName.Text.Trim();
            string contactName = txtContactName.Text.Trim();
            string email = txtContactEmail.Text.Trim();
            string phone = txtContactPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string city = txtCity.Text.Trim();
            string state = txtState.Text.Trim();
            string zip = txtZip.Text.Trim();

            if (string.IsNullOrWhiteSpace(facilityName))
            {
                MessageBox.Show("Please enter a facility name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPayRate.Text, out decimal payRate))
            {
                MessageBox.Show("Please enter a valid pay rate.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string payRateType = ((ComboBoxItem)cmbPayRateType.SelectedItem)?.Content.ToString() ?? "Per Hour";

            AddLocation(facilityName, contactName, email, phone, address, city, state, zip, payRate, payRateType);

            txtFacilityName.Clear();
            txtContactName.Clear();
            txtContactEmail.Clear();
            txtContactPhone.Clear();
            txtAddress.Clear();
            txtCity.Clear();
            txtState.Clear();
            txtZip.Clear();
            txtPayRate.Clear();
            cmbPayRateType.SelectedIndex = 0;

            LoadLocations();
            RefreshLocationsList();
            MessageBox.Show("Location added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEditLocation_Click(object sender, RoutedEventArgs e)
        {
            if (lstLocations.SelectedItem is Location location)
            {
                ShowEditLocationDialog(location);
            }
        }

        private void BtnDeleteLocation_Click(object sender, RoutedEventArgs e)
        {
            if (lstLocations.SelectedItem is Location location)
            {
                var result = MessageBox.Show(
                    "Are you sure you want to delete this location? All time entries for this location will also be deleted.",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteLocation(location.Id);
                    LoadLocations();
                    RefreshLocationsList();
                }
            }
        }

        private void ShowEditLocationDialog(Location location)
        {
            var dialog = new Window
            {
                Title = "Edit Location",
                Width = 600,
                Height = 700,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize
            };

            var stack = new StackPanel { Margin = new Thickness(20) };

            // Create textboxes for each field
            var txtFac = new TextBox { Text = location.FacilityName, Height = 30, Margin = new Thickness(0, 5, 0, 10) };
            var txtCon = new TextBox { Text = location.ContactName, Height = 30, Margin = new Thickness(0, 5, 0, 10) };
            var txtEm = new TextBox { Text = location.ContactEmail, Height = 30, Margin = new Thickness(0, 5, 0, 10) };
            var txtPh = new TextBox { Text = location.ContactPhone, Height = 30, Margin = new Thickness(0, 5, 0, 10) };
            var txtAddr = new TextBox { Text = location.Address, Height = 30, Margin = new Thickness(0, 5, 0, 10) };
            var txtCit = new TextBox { Text = location.City, Height = 30, Margin = new Thickness(0, 5, 0, 10) };
            var txtSt = new TextBox { Text = location.State, Height = 30, Margin = new Thickness(0, 5, 0, 10) };
            var txtZp = new TextBox { Text = location.Zip, Height = 30, Margin = new Thickness(0, 5, 0, 10) };
            var txtRate = new TextBox { Text = location.PayRate.ToString("F2"), Height = 30, Margin = new Thickness(0, 5, 0, 10) };

            var lblType = new Label { Content = "Pay Rate Type:", Margin = new Thickness(0, 5, 0, 0) };
            var cmbType = new ComboBox { Margin = new Thickness(0, 5, 0, 10), Height = 30 };
            cmbType.Items.Add(new ComboBoxItem { Content = "Per Hour" });
            cmbType.Items.Add(new ComboBoxItem { Content = "Per Day" });
            cmbType.SelectedIndex = location.PayRateType == "Per Hour" ? 0 : 1;

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 20, 0, 0) };
            var btnSave = new Button { Content = "Save", Width = 100, Margin = new Thickness(5) };
            var btnCancel = new Button { Content = "Cancel", Width = 100, Margin = new Thickness(5) };

            btnSave.Click += (s, e) =>
            {
                string facilityName = txtFac.Text.Trim();
                if (string.IsNullOrWhiteSpace(facilityName))
                {
                    MessageBox.Show("Please enter a facility name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(txtRate.Text, out decimal payRate))
                {
                    MessageBox.Show("Please enter a valid pay rate.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string payRateType = ((ComboBoxItem)cmbType.SelectedItem)?.Content.ToString() ?? "Per Hour";

                UpdateLocation(location.Id, facilityName, txtCon.Text.Trim(), txtEm.Text.Trim(),
                    txtPh.Text.Trim(), txtAddr.Text.Trim(), txtCit.Text.Trim(), txtSt.Text.Trim(), txtZp.Text.Trim(), payRate, payRateType);

                LoadLocations();
                RefreshLocationsList();
                MessageBox.Show("Location updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                dialog.Close();
            };

            btnCancel.Click += (s, e) => dialog.Close();

            stack.Children.Add(new Label { Content = "Facility Name:" });
            stack.Children.Add(txtFac);
            stack.Children.Add(new Label { Content = "Contact Name:" });
            stack.Children.Add(txtCon);
            stack.Children.Add(new Label { Content = "Contact Email:" });
            stack.Children.Add(txtEm);
            stack.Children.Add(new Label { Content = "Contact Phone:" });
            stack.Children.Add(txtPh);
            stack.Children.Add(new Label { Content = "Street Address:" });
            stack.Children.Add(txtAddr);
            stack.Children.Add(new Label { Content = "City:" });
            stack.Children.Add(txtCit);
            stack.Children.Add(new Label { Content = "State:" });
            stack.Children.Add(txtSt);
            stack.Children.Add(new Label { Content = "Zip:" });
            stack.Children.Add(txtZp);
            stack.Children.Add(new Label { Content = "Pay Rate ($):" });
            stack.Children.Add(txtRate);
            stack.Children.Add(lblType);
            stack.Children.Add(cmbType);
            buttonPanel.Children.Add(btnSave);
            buttonPanel.Children.Add(btnCancel);
            stack.Children.Add(buttonPanel);

            dialog.Content = stack;
            dialog.ShowDialog();
        }

        #endregion

        #region Time Entry Event Handlers

        private void BtnAddEntry_Click(object sender, RoutedEventArgs e)
        {
            if (cmbLocation.SelectedItem is not Location selectedLocation)
            {
                MessageBox.Show("Please select a location.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!dtpDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a date.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string date = dtpDate.SelectedDate.Value.ToString("MM/dd/yyyy");

            int arrivalHour = (int)cmbArrivalHour.SelectedItem;
            string arrivalMin = cmbArrivalMin.SelectedItem.ToString() ?? "00";
            string arrivalAMPM = ((ComboBoxItem)cmbArrivalAMPM.SelectedItem)?.Content.ToString() ?? "AM";
            if (arrivalAMPM == "PM" && arrivalHour != 12) arrivalHour += 12;
            if (arrivalAMPM == "AM" && arrivalHour == 12) arrivalHour = 0;
            string arrival = $"{arrivalHour:D2}:{arrivalMin}:00";

            int departureHour = (int)cmbDepartureHour.SelectedItem;
            string departureMin = cmbDepartureMin.SelectedItem.ToString() ?? "00";
            string departureAMPM = ((ComboBoxItem)cmbDepartureAMPM.SelectedItem)?.Content.ToString() ?? "PM";
            if (departureAMPM == "PM" && departureHour != 12) departureHour += 12;
            if (departureAMPM == "AM" && departureHour == 12) departureHour = 0;
            string departure = $"{departureHour:D2}:{departureMin}:00";

            string notes = txtNotes.Text.Trim();

            decimal dailyPay = CalculateDailyPay(selectedLocation, arrival, departure);
            AddTimeEntry(selectedLocation.Id, date, arrival, departure, dailyPay, notes);

            cmbArrivalHour.SelectedIndex = 6;
            cmbArrivalMin.SelectedIndex = 0;
            cmbArrivalAMPM.SelectedIndex = 0;
            cmbDepartureHour.SelectedIndex = 4;
            cmbDepartureMin.SelectedIndex = 0;
            cmbDepartureAMPM.SelectedIndex = 1;
            txtNotes.Clear();

            LoadTimeEntries();
            RefreshTimeEntriesList();
            UpdateTotals();
        }

        private void BtnToggleLock_Click(object sender, RoutedEventArgs e)
        {
            if (lstTimeEntries.SelectedItem is TimeEntry entry)
            {
                timeEntryService.ToggleLock(entry.Id);
                LoadTimeEntries();
                RefreshTimeEntriesList();
            }
        }

        private void BtnArchive_Click(object sender, RoutedEventArgs e)
        {
            if (lstTimeEntries.SelectedItem is TimeEntry entry)
            {
                var result = timeEntryService.ToggleArchive(entry.Id);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Cannot Archive", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                LoadTimeEntries();
                RefreshTimeEntriesList();
                UpdateTotals();
            }
        }

        private void BtnDeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            if (lstTimeEntries.SelectedItem is TimeEntry entry)
            {
                DeleteTimeEntry(entry.Id);
                LoadTimeEntries();
                RefreshTimeEntriesList();
                UpdateTotals();
            }
        }

        private void BtnClearAll_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete all unlocked time entries? Locked entries will be preserved.",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                timeEntryService.DeleteUnlockedEntries();
                LoadTimeEntries();
                RefreshTimeEntriesList();
                UpdateTotals();
            }
        }

        private void ChkShowArchived_Changed(object sender, RoutedEventArgs e)
        {
            LoadTimeEntries();
            RefreshTimeEntriesList();
            UpdateTotals();
        }

        #endregion

        #region Invoice Event Handlers

        private void DtpStartDate_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (dtpStartDate.SelectedDate.HasValue)
            {
                DateTime startDate = dtpStartDate.SelectedDate.Value;

                if (startDate.Day == 1)
                {
                    // If start date is 1st of month, set end date to 14th of same month
                    dtpEndDate.SelectedDate = new DateTime(startDate.Year, startDate.Month, 14);
                }
                else if (startDate.Day == 15)
                {
                    // If start date is 15th of month, set end date to last day of same month
                    dtpEndDate.SelectedDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
                }
            }
        }

        private void BtnGenerateInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (cmbInvoiceLocation.SelectedItem is not Location selectedLocation)
            {
                MessageBox.Show("Please select a location.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!dtpStartDate.SelectedDate.HasValue || !dtpEndDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select start and end dates.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string startDate = dtpStartDate.SelectedDate.Value.ToString("MM/dd/yyyy");
            string endDate = dtpEndDate.SelectedDate.Value.ToString("MM/dd/yyyy");

            GenerateInvoice(selectedLocation, startDate, endDate);
        }

        private void BtnGenerateAll_Click(object sender, RoutedEventArgs e)
        {
            if (!dtpStartDate.SelectedDate.HasValue || !dtpEndDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select start and end dates.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string startDate = dtpStartDate.SelectedDate.Value.ToString("MM/dd/yyyy");
            string endDate = dtpEndDate.SelectedDate.Value.ToString("MM/dd/yyyy");

            foreach (var location in locations)
            {
                GenerateInvoice(location, startDate, endDate);
            }
        }

        #endregion

        #region Settings Event Handlers

        private void BtnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            businessSettings.BusinessName = txtBusinessName.Text.Trim();
            businessSettings.BusinessAddress = txtBusinessAddress.Text.Trim();
            businessSettings.BusinessCity = txtBusinessCity.Text.Trim();
            businessSettings.BusinessState = txtBusinessState.Text.Trim();
            businessSettings.BusinessZip = txtBusinessZip.Text.Trim();
            businessSettings.BusinessPhone = txtBusinessPhone.Text.Trim();
            businessSettings.BusinessEmail = txtBusinessEmail.Text.Trim();
            businessSettings.ShowClearAllButton = chkShowClearAll.IsChecked ?? false;

            SaveBusinessSettings();
            UpdateClearAllButtonVisibility();
            MessageBox.Show("Settings saved successfully! The button visibility has been updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion
    }
}
