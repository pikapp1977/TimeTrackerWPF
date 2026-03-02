# Time Tracker WPF

A modern Windows Presentation Foundation (WPF) desktop application for tracking work hours and generating professional invoices.

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)
![License](https://img.shields.io/badge/license-Personal-green)

## Overview

TimeTracker WPF is a complete rewrite of the original Windows Forms TimeTracker application, built with modern WPF technology. It provides all the same functionality with a cleaner architecture and better code organization.

### Key Features

- **📍 Location Management** - Track multiple client locations with full contact information
- **⏱️ Time Entry Tracking** - Log work hours with arrival/departure times
- **🔒 Lock/Archive System** - Protect completed entries from accidental changes
- **📄 Invoice Generation** - Create professional Excel and PDF invoices
- **⚙️ Business Settings** - Configure your company information for invoices
- **💾 SQLite Database** - Reliable local data storage

## Screenshots

*(Add screenshots here if desired)*

## Technology Stack

- **.NET 8.0** - Latest .NET framework
- **WPF (Windows Presentation Foundation)** - Modern UI framework
- **XAML** - Declarative UI markup
- **SQLite** - Embedded database (Microsoft.Data.Sqlite)
- **ClosedXML** - Excel file generation
- **QuestPDF** - PDF document generation

## Prerequisites

### For Running
- Windows 10 or later (64-bit)
- No additional software needed (self-contained)

### For Building
- .NET 8 SDK or later
- Windows 10 or later
- (Optional) Visual Studio 2022 or VS Code

## Installation

### Option 1: Download Release (Recommended)
1. Go to [Releases](../../releases)
2. Download `TimeTrackerWPF.exe` or `TimeTrackerWPFSetup_v1.0.41.exe`
3. Run the application

### Option 2: Build from Source
```bash
git clone https://github.com/yourusername/TimeTrackerWPF.git
cd TimeTrackerWPF
dotnet build -c Release
```

The executable will be in: `bin/Release/net8.0-windows/TimeTrackerWPF.exe`

### Option 3: Self-Contained Build
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

Output: `bin/Release/net8.0-windows/win-x64/publish/TimeTrackerWPF.exe`

## Quick Start

1. **Launch the application**
2. **Add a location** - Go to "Locations" tab, enter facility details
3. **Add time entries** - Go to "Time Entry" tab, select location and times
4. **Generate invoice** - Go to "Generate Invoice" tab, select date range
5. **Configure settings** - Go to "Settings" tab, enter your business info

## Project Structure

```
TimeTrackerWPF/
├── App.xaml                         # Application resources & styles
├── App.xaml.cs                      # Application entry point
├── MainWindow.xaml                  # Main window UI (XAML)
├── MainWindow.xaml.cs               # Core logic & database
├── MainWindow.xaml.EventHandlers.cs # UI event handlers
├── MainWindow.xaml.Invoice.cs       # Invoice generation
├── Models.cs                        # Data models
├── TimeEntryService.cs              # Business logic service
└── TimeTrackerWPF.csproj           # Project configuration
```

## Database

The application stores data locally using SQLite:

```
%LOCALAPPDATA%\TimeTracker\timetracker.db
```

**Note:** This is the same database location as the Windows Forms version, allowing both applications to share data.

### Database Schema

- **Locations** - Client facilities and contact information
- **TimeEntries** - Work hours with lock/archive flags
- **BusinessSettings** - Your company information for invoices

## Features in Detail

### Location Management
- Add, edit, and delete client locations
- Store complete contact information
- Configure pay rates (per hour or per day)
- Full address support

### Time Entry Tracking
- Log arrival and departure times
- Automatic hours calculation
- Support for overnight shifts
- Optional notes for each entry
- Lock entries to prevent changes
- Archive completed work

### Invoice Generation
- Generate Excel spreadsheets (.xlsx)
- Generate PDF documents
- Support for date ranges
- Batch generation for all locations
- Preview before saving
- Professional formatting

### Business Settings
- Configure your company name and address
- Set contact information
- Customize invoice appearance
- Toggle advanced features

## Building

### Debug Build
```bash
dotnet build
```

### Release Build
```bash
dotnet build -c Release
```

### Publish (Self-Contained)
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### Create Installer (Requires Inno Setup)
```batch
build-installer.bat
```

Download Inno Setup: https://jrsoftware.org/isdown.php

## Testing

Unit tests are located in the sibling `timetracker` repository:

```bash
cd ../timetracker/TimeTracker.Tests
dotnet test
```

All 23 unit tests cover the `TimeEntryService` business logic used by both applications.

## Comparison with Windows Forms Version

| Feature | Windows Forms | WPF |
|---------|:-------------:|:---:|
| UI Framework | Windows Forms | WPF/XAML |
| Code Organization | Single file (2,032 lines) | Multiple files (~2,000 lines) |
| Data Binding | Manual | Automatic |
| Styling | Per-control | Global styles |
| DPI Scaling | Manual | Automatic |
| Maintenance | Good | Better |

**Both versions:**
- Use the same database
- Have identical features
- Are fully functional
- Can be used interchangeably

## Architecture

### MVVM-Ready
While currently using code-behind, the architecture is designed to easily transition to MVVM:
- Models already separated (`Models.cs`)
- Business logic isolated (`TimeEntryService.cs`)
- UI and logic cleanly separated

### Partial Classes
The `MainWindow` is organized across three files:
- **MainWindow.xaml.cs** - Core logic, database, helpers
- **MainWindow.xaml.EventHandlers.cs** - UI event handlers
- **MainWindow.xaml.Invoice.cs** - Invoice generation

This improves maintainability and code navigation.

## Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Development Guidelines
- Keep business logic in `TimeEntryService.cs`
- Use XAML for UI when possible
- Follow WPF naming conventions
- Add unit tests for new features
- Update documentation

## Roadmap

Possible future enhancements:
- [ ] MVVM refactoring
- [ ] ICommand implementation
- [ ] Dark/light theme support
- [ ] Custom reports
- [ ] Data export/import
- [ ] Multi-user support
- [ ] Cloud sync option
- [ ] Mobile companion app

## Known Issues

- SmartScreen warning on first run (unsigned executable)
- Edit location dialog is code-generated (could be XAML)

## License

This project is for personal use. See LICENSE file for details.

## Support

For issues, questions, or suggestions:
- Open an [Issue](../../issues)
- Check existing [Discussions](../../discussions)

## Related Projects

- [TimeTracker (Windows Forms)](../timetracker) - Original Windows Forms version
- Both share the same database and can be used interchangeably

## Changelog

### Version 1.0.41 (Current)
- Initial WPF implementation
- Feature parity with Windows Forms version
- All core functionality complete
- Professional invoice generation
- Comprehensive documentation

## Acknowledgments

- Built with .NET 8 and WPF
- Uses ClosedXML for Excel generation
- Uses QuestPDF for PDF generation
- Database powered by SQLite

---

**⭐ If you find this project useful, please consider giving it a star!**

## Quick Links

- [Build Guide](BUILD_AND_INSTALL_GUIDE.md)
- [Quick Start](BUILD_QUICKSTART.md)
- [Project Summary](WPF_PROJECT_SUMMARY.md)
- [Download Latest Release](../../releases/latest)

---

Made with ❤️ using WPF and .NET 8
