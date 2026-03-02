@echo off
echo ================================================
echo Building Time Tracker WPF Installer
echo ================================================
echo.

REM Clean previous builds
echo [1/4] Cleaning previous builds...
if exist bin\Release rmdir /s /q bin\Release
if exist obj\Release rmdir /s /q obj\Release
if exist TimeTrackerWPFSetup_*.exe del /q TimeTrackerWPFSetup_*.exe
echo Done.
echo.

REM Build Release version
echo [2/4] Building Release version...
dotnet publish TimeTrackerWPF.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
if errorlevel 1 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)
echo Done.
echo.

REM Check if Inno Setup is installed
echo [3/4] Checking for Inno Setup...
set ISCC="C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if not exist %ISCC% (
    echo WARNING: Inno Setup not found!
    echo.
    echo Inno Setup is not installed. You can:
    echo   1. Download from: https://jrsoftware.org/isdown.php
    echo   2. Install with default settings
    echo   3. Run this script again to create the installer
    echo.
    echo The executable has been built successfully at:
    echo bin\Release\net8.0-windows\win-x64\publish\TimeTrackerWPF.exe
    echo.
    echo You can distribute this EXE file directly without an installer.
    pause
    exit /b 0
)
echo Done.
echo.

REM Create installer
echo [4/4] Creating installer with Inno Setup...
%ISCC% TimeTrackerWPFSetup.iss
if errorlevel 1 (
    echo ERROR: Installer creation failed!
    pause
    exit /b 1
)
echo Done.
echo.

echo ================================================
echo Build Complete!
echo ================================================
echo.
echo Executable: bin\Release\net8.0-windows\win-x64\publish\TimeTrackerWPF.exe
echo Installer:  TimeTrackerWPFSetup_v1.0.41.exe
echo.
pause
