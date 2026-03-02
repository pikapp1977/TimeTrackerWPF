# Time Tracker WPF - Build and Installation Guide

## Build Status

✅ **Release build completed successfully!**

**Executable location:**
```
TimeTrackerWPF/bin/Release/net8.0-windows/win-x64/publish/TimeTrackerWPF.exe
```

**File size:** ~181 MB (self-contained with .NET runtime)

---

## Distribution Options

You have 3 ways to distribute the WPF application:

### Option 1: Standalone Executable (Easiest)
Just distribute the single EXE file - no installation needed!

**File to distribute:**
```
bin/Release/net8.0-windows/win-x64/publish/TimeTrackerWPF.exe
```

**User instructions:**
1. Copy `TimeTrackerWPF.exe` to any folder
2. Double-click to run
3. That's it!

**Pros:**
- ✅ No installer needed
- ✅ User can run from anywhere
- ✅ No registry changes
- ✅ Easy to update (just replace file)

**Cons:**
- ❌ No Start Menu entry
- ❌ No uninstaller
- ❌ Less "professional" appearance

---

### Option 2: ZIP Archive (Portable)
Create a ZIP file for easy distribution.

**Steps:**
1. Navigate to: `bin/Release/net8.0-windows/win-x64/publish/`
2. Select `TimeTrackerWPF.exe`
3. Right-click → "Send to" → "Compressed (zipped) folder"
4. Rename to: `TimeTrackerWPF_v1.0.41_Portable.zip`

**User instructions:**
1. Extract ZIP to desired location
2. Run `TimeTrackerWPF.exe`
3. (Optional) Create desktop shortcut

**Pros:**
- ✅ Easy to distribute
- ✅ Users can extract anywhere
- ✅ Portable - no installation

**Cons:**
- ❌ Same as Option 1

---

### Option 3: Professional Installer (Recommended)
Create a Windows installer using Inno Setup.

#### Install Inno Setup

**Download:**
https://jrsoftware.org/isdown.php

**Installation steps:**
1. Download `innosetup-6.x.x.exe`
2. Run installer
3. Click "Next" through wizard (default settings are fine)
4. Installation takes ~2 minutes

#### Create the Installer

**After installing Inno Setup, run:**
```batch
build-installer.bat
```

**Or manually:**
```batch
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" TimeTrackerWPFSetup.iss
```

**Output:**
```
TimeTrackerWPFSetup_v1.0.41.exe (~100 MB compressed)
```

**User instructions:**
1. Run `TimeTrackerWPFSetup_v1.0.41.exe`
2. Follow installation wizard
3. Application appears in Start Menu
4. Option to create desktop icon

**Pros:**
- ✅ Professional appearance
- ✅ Start Menu integration
- ✅ Proper uninstaller
- ✅ Desktop icon option
- ✅ Smaller download (compressed)

**Cons:**
- ❌ Requires Inno Setup to build
- ❌ More complex distribution

---

## Quick Build Commands

### Rebuild the Application
```bash
cd /mnt/c/users/admin/documents/TimeTrackerWPF
dotnet clean
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### Build with Inno Setup Installer
```batch
cd C:\users\admin\documents\TimeTrackerWPF
build-installer.bat
```

---

## Current Build Information

| Item | Details |
|------|---------|
| **Executable** | `TimeTrackerWPF.exe` |
| **Size** | ~181 MB |
| **Version** | 1.0.41 |
| **Framework** | .NET 8.0 |
| **Platform** | Windows 10+ (64-bit) |
| **Self-contained** | Yes (includes .NET runtime) |
| **Single file** | Yes |

---

## Distribution Comparison

| Feature | Standalone EXE | ZIP Archive | Installer |
|---------|:-------------:|:-----------:|:---------:|
| **Easy to create** | ✅ | ✅ | ⚠️ |
| **Easy to distribute** | ✅ | ✅ | ✅ |
| **Professional** | ❌ | ⚠️ | ✅ |
| **Start Menu entry** | ❌ | ❌ | ✅ |
| **Uninstaller** | ❌ | ❌ | ✅ |
| **File size** | 181 MB | 181 MB | ~100 MB |
| **User setup time** | Instant | 1 min | 2 min |

---

## Recommended Distribution Methods

### For Testing/Personal Use
→ **Standalone EXE** (Option 1)
- Just copy and run
- Fastest for you

### For Colleagues/Small Team
→ **ZIP Archive** (Option 2)
- Email the ZIP file
- They extract and run
- Still simple

### For Professional Distribution
→ **Installer** (Option 3)
- Install Inno Setup once
- Create professional installers
- Best user experience

---

## Files Created for Distribution

### Already Built ✅
```
bin/Release/net8.0-windows/win-x64/publish/
└── TimeTrackerWPF.exe (181 MB) - READY TO USE!
```

### Build Scripts Created ✅
```
TimeTrackerWPF/
├── build-installer.bat          - Automated build script
└── TimeTrackerWPFSetup.iss      - Inno Setup configuration
```

---

## System Requirements

**For Users:**
- Windows 10 or later (64-bit)
- 200 MB free disk space
- No .NET installation required (included in EXE)

**For Building:**
- .NET 8 SDK
- (Optional) Inno Setup 6 for installer creation

---

## Database Location

The WPF application uses the **same database** as the Windows Forms version:

```
%LOCALAPPDATA%\TimeTracker\timetracker.db
```

This means users can:
- ✅ Install WPF version alongside Windows Forms
- ✅ Data is automatically shared
- ✅ Switch between versions seamlessly

---

## Next Steps

### Immediate Distribution (No Installer)
1. Navigate to: `bin/Release/net8.0-windows/win-x64/publish/`
2. Copy `TimeTrackerWPF.exe`
3. Send to users
4. Done! ✅

### Create Professional Installer
1. Install Inno Setup: https://jrsoftware.org/isdown.php
2. Run: `build-installer.bat`
3. Distribute `TimeTrackerWPFSetup_v1.0.41.exe`
4. Done! ✅

---

## Comparison with Windows Forms Version

Both applications:
- Use same database
- Have same features
- Same version number (1.0.41)
- Similar file sizes (~181 MB)

**Windows Forms Installer:**
```
timetracker/TimeTrackerSetup_v1.0.41.exe
```

**WPF Installer (when built):**
```
TimeTrackerWPF/TimeTrackerWPFSetup_v1.0.41.exe
```

You can distribute both and let users choose!

---

## Troubleshooting

### Build fails
```bash
cd TimeTrackerWPF
dotnet clean
dotnet restore
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### Inno Setup not found
- Download from: https://jrsoftware.org/isdown.php
- Or use Option 1/2 (no installer needed)

### Executable won't run
- Requires Windows 10 or later
- Must be 64-bit Windows
- SmartScreen warning is normal (unsigned app)

---

## Current Status

✅ **Application built successfully!**  
✅ **Ready to distribute immediately!**  
⚠️ **Installer requires Inno Setup** (optional)

**Recommended action:**
Use the standalone EXE for now (`TimeTrackerWPF.exe`), or install Inno Setup to create a professional installer.

---

## Quick Reference

```
Current executable:
C:\users\admin\documents\TimeTrackerWPF\bin\Release\net8.0-windows\win-x64\publish\TimeTrackerWPF.exe

Rebuild:
cd C:\users\admin\documents\TimeTrackerWPF
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

Create installer (requires Inno Setup):
build-installer.bat
```

**Your WPF application is ready to distribute!** 🎉
