# TimeTracker WPF

A modern Windows Presentation Foundation (WPF) version of the TimeTracker application.

## Overview

This is a **new, separate project** that reimplements the TimeTracker application using WPF instead of Windows Forms. It provides the same functionality with a more modern UI framework.

## Features

All features from the original Windows Forms version:

- **Location Management** - Add, edit, and delete client locations with full contact information
- **Time Entry Tracking** - Track work hours with arrival/departure times
- **Lock/Archive System** - Protect entries from accidental deletion
- **Invoice Generation** - Create professional Excel and PDF invoices
- **Business Settings** - Configure your business information for invoices

## Technology Stack

- **.NET 8.0** - Latest .NET framework
- **WPF (Windows Presentation Foundation)** - Modern Windows desktop UI framework
- **XAML** - Declarative UI markup
- **SQLite** - Same database as Windows Forms version (shared database file)
- **ClosedXML** - Excel generation
- **QuestPDF** - PDF generation

## Project Structure

```
TimeTrackerWPF/
├── App.xaml                              # Application entry point
├── App.xaml.cs                           # Application code-behind
├── MainWindow.xaml                       # Main window UI (XAML)
├── MainWindow.xaml.cs                    # Main window code-behind
├── MainWindow.xaml.EventHandlers.cs      # Event handlers (partial class)
├── MainWindow.xaml.Invoice.cs            # Invoice generation (partial class)
├── Models.cs                             # Data models (Location, TimeEntry, etc.)
├── TimeEntryService.cs                   # Business logic service
└── TimeTrackerWPF.csproj                 # Project configuration
```

## Differences from Windows Forms Version

### Architecture
- **XAML-based UI** - Declarative markup instead of procedural code
- **Better separation of concerns** - UI and logic more cleanly separated
- **Partial classes** - Code organized into logical files
- **Data binding** - ListView items bound directly to collections

### UI Improvements
- **Modern styling** - Global styles defined in App.xaml
- **Better DPI scaling** - WPF handles high-DPI displays automatically
- **Smoother animations** - Native WPF animations and transitions
- **Resizable controls** - Better layout management

### Code Organization
- **Partial classes** - MainWindow split across multiple files for maintainability
- **Cleaner event handlers** - Separated from main logic
- **Reusable models** - Shared data classes

## Database Compatibility

**Important:** This WPF version uses the **same database** as the Windows Forms version. Both applications store data in:

```
%LOCALAPPDATA%\TimeTracker\timetracker.db
```

This means:
- ✅ You can run both applications side-by-side
- ✅ Data is shared between both versions
- ✅ No data migration needed
- ⚠️ Don't run both simultaneously (SQLite file locking)

## Building the Project

### Prerequisites
- .NET 8 SDK
- Windows 10 or later

### Build Commands

**Debug build:**
```bash
dotnet build TimeTrackerWPF.csproj
```

**Release build:**
```bash
dotnet build TimeTrackerWPF.csproj -c Release
```

**Publish self-contained executable:**
```bash
dotnet publish TimeTrackerWPF.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### Output Location
```
TimeTrackerWPF/bin/Release/net8.0-windows/win-x64/publish/TimeTrackerWPF.exe
```

## Running the Application

**From Visual Studio:**
1. Open `TimeTrackerWPF.csproj` in Visual Studio
2. Press F5 to run

**From Command Line:**
```bash
cd TimeTrackerWPF
dotnet run
```

**From Executable:**
```bash
./bin/Release/net8.0-windows/win-x64/publish/TimeTrackerWPF.exe
```

## Development Notes

### XAML Benefits
- **Visual designer support** - Can use Visual Studio's XAML designer
- **Styles and templates** - Easy to customize appearance
- **Data binding** - Less boilerplate code
- **Resource dictionaries** - Reusable styles and templates

### Code Organization
The MainWindow class is split into partial classes:
- `MainWindow.xaml.cs` - Core initialization and database methods
- `MainWindow.xaml.EventHandlers.cs` - UI event handlers
- `MainWindow.xaml.Invoice.cs` - Invoice generation logic

This improves maintainability and makes the code easier to navigate.

### Future Enhancements
Possible WPF-specific improvements:
- **MVVM Pattern** - Could refactor to Model-View-ViewModel
- **Commands** - Replace event handlers with ICommand
- **Custom controls** - Create reusable UI components
- **Themes** - Add light/dark theme support
- **Animations** - Add smooth transitions between views

## Testing

The WPF version shares the same business logic (`TimeEntryService.cs`) as the Windows Forms version, so the existing unit tests in `TimeTracker.Tests` apply to both applications.

## Comparison with Windows Forms

| Aspect | Windows Forms | WPF |
|--------|---------------|-----|
| **UI Declaration** | Procedural code | XAML markup |
| **Styling** | Manual per-control | Global styles |
| **Data Binding** | Manual updates | Automatic binding |
| **DPI Scaling** | Manual handling | Automatic |
| **Modern Look** | Windows 95 style | Modern Windows |
| **Learning Curve** | Easy | Moderate |
| **Performance** | Good | Excellent |
| **Maintenance** | More verbose | Cleaner |

## Migration from Windows Forms

This project demonstrates how to migrate a Windows Forms application to WPF:

1. **Business logic stays the same** - `TimeEntryService.cs` is identical
2. **Database code unchanged** - All SQLite operations work as-is
3. **UI rebuilt in XAML** - Forms converted to XAML markup
4. **Event handlers adapted** - Similar structure, WPF event signatures
5. **Data binding added** - ListView uses ItemsSource instead of manual population

## Known Differences

1. **Edit dialog** - WPF version creates dialog programmatically (could be XAML in future)
2. **Color coding** - Works differently but achieves same result
3. **Menu** - WPF uses different menu structure
4. **Message boxes** - Different MessageBox API (MessageBoxButton vs MessageBoxButtons)

## Contributing

When making changes:
1. Keep business logic in `TimeEntryService.cs`
2. Use XAML for UI when possible
3. Follow WPF naming conventions (PascalCase for XAML elements)
4. Test with existing database from Windows Forms version

## Version

Inherits version from `../timetracker/Version.props` (currently 1.0.41)

## License

Same as Windows Forms version - Personal use

---

## Quick Start

1. **Build the project:**
   ```bash
   cd TimeTrackerWPF
   dotnet build
   ```

2. **Run it:**
   ```bash
   dotnet run
   ```

3. **Test it:**
   - Add a location
   - Add a time entry
   - Generate an invoice
   - Compare with Windows Forms version!

Both apps use the same database, so you can switch between them seamlessly!
