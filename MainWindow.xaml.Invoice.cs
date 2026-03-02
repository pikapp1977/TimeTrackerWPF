using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TimeTrackerWPF
{
    public partial class MainWindow : Window
    {
        private void GenerateInvoice(Location location, string startDate, string endDate)
        {
            var entries = timeEntries.Where(e =>
                e.LocationId == location.Id &&
                string.Compare(e.Date, startDate) >= 0 &&
                string.Compare(e.Date, endDate) <= 0
            ).ToList();

            if (entries.Count == 0)
            {
                MessageBox.Show($"No time entries found for {location.FacilityName} in the selected date range.",
                    "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = "MDAnesthesia_" + location.FacilityName.Replace(" ", "_") + "_" + DateTime.Now.ToString("MMddyyyy") + ".xlsx"
            };

            if (saveDialog.ShowDialog() != true)
                return;

            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Invoice");

                int row = 1;
                worksheet.Cell(row, 1).Value = "INVOICE";
                worksheet.Cell(row, 1).Style.Font.FontSize = 24;
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                row = 3;
                worksheet.Cell(row, 1).Value = "From:";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                row++;
                worksheet.Cell(row, 2).Value = string.IsNullOrEmpty(businessSettings.BusinessName) ? "Your Company Name" : businessSettings.BusinessName;
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                row++;
                if (!string.IsNullOrEmpty(businessSettings.BusinessAddress))
                {
                    worksheet.Cell(row, 1).Value = businessSettings.BusinessAddress;
                    row++;
                }
                string cityStateZip = "";
                if (!string.IsNullOrEmpty(businessSettings.BusinessCity))
                    cityStateZip = businessSettings.BusinessCity;
                if (!string.IsNullOrEmpty(businessSettings.BusinessState))
                    cityStateZip += (cityStateZip.Length > 0 ? ", " : "") + businessSettings.BusinessState;
                if (!string.IsNullOrEmpty(businessSettings.BusinessZip))
                    cityStateZip += " " + businessSettings.BusinessZip;
                if (!string.IsNullOrEmpty(cityStateZip))
                {
                    worksheet.Cell(row, 1).Value = cityStateZip;
                    row++;
                }
                if (!string.IsNullOrEmpty(businessSettings.BusinessPhone))
                {
                    worksheet.Cell(row, 1).Value = businessSettings.BusinessPhone;
                    row++;
                }
                if (!string.IsNullOrEmpty(businessSettings.BusinessEmail))
                {
                    worksheet.Cell(row, 1).Value = businessSettings.BusinessEmail;
                    row++;
                }

                row++;
                worksheet.Cell(row, 1).Value = "Bill To:";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                row++;
                worksheet.Cell(row, 2).Value = location.FacilityName;
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                row++;
                if (!string.IsNullOrEmpty(location.ContactName))
                {
                    worksheet.Cell(row, 2).Value = location.ContactName;
                    row++;
                }
                if (!string.IsNullOrEmpty(location.Address))
                {
                    worksheet.Cell(row, 2).Value = location.Address;
                    row++;
                }
                string locCityStateZip = "";
                if (!string.IsNullOrEmpty(location.City))
                    locCityStateZip = location.City;
                if (!string.IsNullOrEmpty(location.State))
                    locCityStateZip += (locCityStateZip.Length > 0 ? ", " : "") + location.State;
                if (!string.IsNullOrEmpty(location.Zip))
                    locCityStateZip += " " + location.Zip;
                if (!string.IsNullOrEmpty(locCityStateZip))
                {
                    worksheet.Cell(row, 2).Value = locCityStateZip;
                    row++;
                }
                if (!string.IsNullOrEmpty(location.ContactPhone))
                {
                    worksheet.Cell(row, 2).Value = location.ContactPhone;
                    row++;
                }
                if (!string.IsNullOrEmpty(location.ContactEmail))
                {
                    worksheet.Cell(row, 2).Value = location.ContactEmail;
                    row++;
                }

                row++;
                worksheet.Cell(row, 1).Value = "Invoice Date:";
                worksheet.Cell(row, 2).Value = DateTime.Now.ToString("MM/dd/yyyy");
                row++;
                worksheet.Cell(row, 1).Value = "Period:";
                worksheet.Cell(row, 2).Value = startDate + " to " + endDate;

                row += 2;
                int headerRow = row;
                worksheet.Cell(row, 1).Value = "Date";
                worksheet.Cell(row, 2).Value = "Description";
                worksheet.Cell(row, 3).Value = "Quantity";
                worksheet.Cell(row, 4).Value = "Amount";
                worksheet.Range(headerRow, 1, headerRow, 4).Style.Font.Bold = true;
                worksheet.Range(headerRow, 1, headerRow, 4).Style.Fill.BackgroundColor = XLColor.LightGray;
                worksheet.Range(headerRow, 1, headerRow, 4).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                worksheet.Range(headerRow, 1, headerRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                row++;
                decimal total = 0;
                foreach (var entry in entries.OrderBy(e => e.Date))
                {
                    double hours = CalculateHoursWorked(entry.ArrivalTime, entry.DepartureTime);
                    string description = "Worked " + hours.ToString("F2") + " hours (" + entry.ArrivalTime + " - " + entry.DepartureTime + ")";

                    worksheet.Cell(row, 1).Value = entry.Date;
                    worksheet.Cell(row, 2).Value = description;
                    worksheet.Cell(row, 3).Value = 1;
                    worksheet.Cell(row, 4).Value = (double)entry.DailyPay;
                    worksheet.Cell(row, 4).Style.NumberFormat.Format = "$#,##0.00";

                    total += entry.DailyPay;
                    row++;
                }

                row++;
                worksheet.Cell(row, 3).Value = "TOTAL:";
                worksheet.Cell(row, 3).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Style.Font.FontSize = 12;
                worksheet.Cell(row, 4).Value = (double)total;
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "$#,##0.00";
                worksheet.Cell(row, 4).Style.Font.Bold = true;
                worksheet.Cell(row, 4).Style.Font.FontSize = 12;

                if (entries.Any(e => !string.IsNullOrWhiteSpace(e.Notes)))
                {
                    row += 2;
                    worksheet.Cell(row, 1).Value = "Notes:";
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    row++;
                    foreach (var entry in entries.Where(e => !string.IsNullOrWhiteSpace(e.Notes)))
                    {
                        worksheet.Cell(row, 1).Value = entry.Date + ": " + entry.Notes;
                        row++;
                    }
                }

                worksheet.Column(1).Width = 12;
                worksheet.Column(2).Width = 50;
                worksheet.Column(3).Width = 12;
                worksheet.Column(4).Width = 15;

                workbook.SaveAs(saveDialog.FileName);

                // Generate PDF with the same filename
                string pdfFileName = Path.ChangeExtension(saveDialog.FileName, ".pdf");
                GenerateInvoicePDF(location, startDate, endDate, entries, total, pdfFileName);

                MessageBox.Show($"Invoice saved to:\n{saveDialog.FileName}\n\nPDF saved to:\n{pdfFileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                UpdateInvoicePreview(location, startDate, endDate, entries, total);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating invoice: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateInvoicePDF(Location location, string startDate, string endDate, List<TimeEntry> entries, decimal total, string fileName)
        {
            try
            {
                QuestPDF.Settings.License = LicenseType.Community;

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter);
                        page.Margin(50);

                        page.Content().Column(column =>
                        {
                            // Invoice Title
                            column.Item().Text("INVOICE").FontSize(24).Bold();
                            column.Item().PaddingBottom(20);

                            // From Section
                            column.Item().Text("From:").Bold();
                            string businessName = string.IsNullOrEmpty(businessSettings.BusinessName) ? "Your Company Name" : businessSettings.BusinessName;
                            column.Item().Text(businessName).Bold();

                            if (!string.IsNullOrEmpty(businessSettings.BusinessAddress))
                                column.Item().Text(businessSettings.BusinessAddress);

                            string cityStateZip = "";
                            if (!string.IsNullOrEmpty(businessSettings.BusinessCity))
                                cityStateZip = businessSettings.BusinessCity;
                            if (!string.IsNullOrEmpty(businessSettings.BusinessState))
                                cityStateZip += (cityStateZip.Length > 0 ? ", " : "") + businessSettings.BusinessState;
                            if (!string.IsNullOrEmpty(businessSettings.BusinessZip))
                                cityStateZip += " " + businessSettings.BusinessZip;
                            if (!string.IsNullOrEmpty(cityStateZip))
                                column.Item().Text(cityStateZip);

                            if (!string.IsNullOrEmpty(businessSettings.BusinessPhone))
                                column.Item().Text(businessSettings.BusinessPhone);
                            if (!string.IsNullOrEmpty(businessSettings.BusinessEmail))
                                column.Item().Text(businessSettings.BusinessEmail);

                            column.Item().PaddingBottom(15);

                            // Bill To Section
                            column.Item().Text("Bill To:").Bold();
                            column.Item().Text(location.FacilityName).Bold();

                            if (!string.IsNullOrEmpty(location.ContactName))
                                column.Item().Text(location.ContactName);
                            if (!string.IsNullOrEmpty(location.Address))
                                column.Item().Text(location.Address);

                            string locCityStateZip = "";
                            if (!string.IsNullOrEmpty(location.City))
                                locCityStateZip = location.City;
                            if (!string.IsNullOrEmpty(location.State))
                                locCityStateZip += (locCityStateZip.Length > 0 ? ", " : "") + location.State;
                            if (!string.IsNullOrEmpty(location.Zip))
                                locCityStateZip += " " + location.Zip;
                            if (!string.IsNullOrEmpty(locCityStateZip))
                                column.Item().Text(locCityStateZip);

                            if (!string.IsNullOrEmpty(location.ContactPhone))
                                column.Item().Text(location.ContactPhone);
                            if (!string.IsNullOrEmpty(location.ContactEmail))
                                column.Item().Text(location.ContactEmail);

                            column.Item().PaddingBottom(15);

                            // Invoice Details
                            column.Item().Text($"Invoice Date: {DateTime.Now:MM/dd/yyyy}");
                            column.Item().Text($"Period: {startDate} to {endDate}");
                            column.Item().PaddingBottom(15);

                            // Table
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(5);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(2);
                                });

                                // Header
                                table.Header(header =>
                                {
                                    header.Cell().Background("#CCCCCC").Padding(5).Text("Date").Bold();
                                    header.Cell().Background("#CCCCCC").Padding(5).Text("Description").Bold();
                                    header.Cell().Background("#CCCCCC").Padding(5).Text("Quantity").Bold();
                                    header.Cell().Background("#CCCCCC").Padding(5).Text("Amount").Bold();
                                });

                                // Data rows
                                foreach (var entry in entries.OrderBy(e => e.Date))
                                {
                                    double hours = CalculateHoursWorked(entry.ArrivalTime, entry.DepartureTime);
                                    string description = $"Worked {hours:F2} hours ({entry.ArrivalTime} - {entry.DepartureTime})";

                                    table.Cell().BorderBottom(0.5f).Padding(5).Text(entry.Date);
                                    table.Cell().BorderBottom(0.5f).Padding(5).Text(description);
                                    table.Cell().BorderBottom(0.5f).Padding(5).Text("1");
                                    table.Cell().BorderBottom(0.5f).Padding(5).Text($"${entry.DailyPay:F2}");
                                }

                                // Total row
                                table.Cell().ColumnSpan(3).Padding(5).AlignRight().Text("TOTAL:").Bold().FontSize(12);
                                table.Cell().Padding(5).Text($"${total:F2}").Bold().FontSize(12);
                            });

                            // Notes section
                            if (entries.Any(e => !string.IsNullOrWhiteSpace(e.Notes)))
                            {
                                column.Item().PaddingTop(20);
                                column.Item().Text("Notes:").Bold();
                                foreach (var entry in entries.Where(e => !string.IsNullOrWhiteSpace(e.Notes)))
                                {
                                    column.Item().Text($"{entry.Date}: {entry.Notes}");
                                }
                            }
                        });
                    });
                });

                document.GeneratePdf(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating PDF: {ex.Message}\n\nThe Excel file was saved successfully.", "PDF Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateInvoicePreview(Location location, string startDate, string endDate, List<TimeEntry> entries, decimal total)
        {
            string businessName = string.IsNullOrEmpty(businessSettings.BusinessName) ? "Your Company Name" : businessSettings.BusinessName;
            string businessAddress = string.IsNullOrEmpty(businessSettings.BusinessAddress) ? "" : businessSettings.BusinessAddress;
            string businessCityStateZip = "";
            if (!string.IsNullOrEmpty(businessSettings.BusinessCity))
                businessCityStateZip = businessSettings.BusinessCity;
            if (!string.IsNullOrEmpty(businessSettings.BusinessState))
                businessCityStateZip += (businessCityStateZip.Length > 0 ? ", " : "") + businessSettings.BusinessState;
            if (!string.IsNullOrEmpty(businessSettings.BusinessZip))
                businessCityStateZip += " " + businessSettings.BusinessZip;

            string preview = $"INVOICE\n";
            preview += $"========\n\n";
            preview += $"From: {businessName}\n";
            if (!string.IsNullOrEmpty(businessAddress))
                preview += $"{businessAddress}\n";
            if (!string.IsNullOrEmpty(businessCityStateZip))
                preview += $"{businessCityStateZip}\n";
            preview += $"\n";

            string locCityStateZip = "";
            if (!string.IsNullOrEmpty(location.City))
                locCityStateZip = location.City;
            if (!string.IsNullOrEmpty(location.State))
                locCityStateZip += (locCityStateZip.Length > 0 ? ", " : "") + location.State;
            if (!string.IsNullOrEmpty(location.Zip))
                locCityStateZip += " " + location.Zip;

            preview += $"Bill To:\n{location.FacilityName}\n{location.ContactName}\n";
            if (!string.IsNullOrEmpty(location.Address))
                preview += $"{location.Address}\n";
            if (!string.IsNullOrEmpty(locCityStateZip))
                preview += $"{locCityStateZip}\n";
            if (!string.IsNullOrEmpty(location.ContactPhone))
                preview += $"{location.ContactPhone}\n";
            if (!string.IsNullOrEmpty(location.ContactEmail))
                preview += $"{location.ContactEmail}\n";
            preview += $"\n";
            preview += $"Period: {startDate} to {endDate}\n\n";
            preview += $"Date       | Arrival  | Departure | Hours | Amount\n";
            preview += new string('-', 55) + "\n";

            foreach (var entry in entries.OrderBy(e => e.Date))
            {
                double hours = CalculateHoursWorked(entry.ArrivalTime, entry.DepartureTime);
                preview += $"{entry.Date} | {entry.ArrivalTime,-8} | {entry.DepartureTime,-9} | {hours:F2} | ${entry.DailyPay:F2}\n";
            }

            preview += new string('-', 55) + "\n";
            preview += $"{"",45} TOTAL: ${total:F2}\n";

            txtInvoicePreview.Text = preview;
        }
    }
}
