# GitHub Actions Guide - TimeTrackerWPF

## Overview

Two GitHub Actions workflows have been set up to automatically build and release your WPF application.

---

## Workflows Created

### 1. Build Workflow (`build.yml`)
**Triggers:**
- Every push to `main` branch
- Every pull request to `main` branch
- Manual trigger (workflow_dispatch)

**What it does:**
- ✅ Builds the WPF application
- ✅ Creates the installer with Inno Setup
- ✅ Uploads both portable EXE and installer as artifacts

**Artifacts available after build:**
- `TimeTrackerWPF-Portable` - Standalone executable
- `TimeTrackerWPF-Installer` - Windows installer

### 2. Release Workflow (`release.yml`)
**Triggers:**
- When you create a tag like `v1.0.41`
- Manual trigger (workflow_dispatch)

**What it does:**
- ✅ Builds the application
- ✅ Creates the installer
- ✅ Creates a GitHub Release
- ✅ Uploads installer and portable EXE to the release
- ✅ Generates release notes automatically

---

## How to Use (From Ubuntu/WSL)

### Push the Workflows to GitHub

```bash
cd /mnt/c/users/admin/documents/TimeTrackerWPF
git add .github/
git commit -m "Add GitHub Actions workflows for automated builds"
git push
```

### Trigger Automatic Build

Every time you push code changes:
```bash
cd /mnt/c/users/admin/documents/TimeTrackerWPF
git add .
git commit -m "Your changes"
git push
```

The build workflow will automatically run!

### Create a Release

To create an official release with installer:

```bash
cd /mnt/c/users/admin/documents/TimeTrackerWPF

# Create and push a version tag
git tag -a v1.0.41 -m "Release version 1.0.41"
git push origin v1.0.41
```

This will:
1. Trigger the release workflow
2. Build the application
3. Create the installer
4. Create a GitHub Release
5. Upload files to the release

---

## Viewing Build Results

### Check Workflow Status

1. Go to: https://github.com/pikapp1977/TimeTrackerWPF
2. Click the "Actions" tab
3. See all workflow runs

### Download Artifacts

After a build completes:
1. Go to Actions tab
2. Click on the workflow run
3. Scroll down to "Artifacts"
4. Download:
   - `TimeTrackerWPF-Portable` (standalone EXE)
   - `TimeTrackerWPF-Installer` (installer)

### View Releases

1. Go to: https://github.com/pikapp1977/TimeTrackerWPF
2. Click "Releases"
3. All tagged releases will appear here with downloads

---

## Manual Trigger

You can manually trigger workflows without pushing code:

1. Go to: https://github.com/pikapp1977/TimeTrackerWPF/actions
2. Click on the workflow name (e.g., "Build WPF Installer")
3. Click "Run workflow" button
4. Select branch
5. Click "Run workflow"

---

## Workflow Details

### Build Process

```yaml
1. Checkout code from repository
2. Setup .NET 8 SDK
3. Restore NuGet packages
4. Publish as self-contained single-file
5. Download and install Inno Setup
6. Create installer with Inno Setup
7. Upload artifacts
```

### What Gets Built

**Portable Executable:**
- Location: `bin/Release/net8.0-windows/win-x64/publish/TimeTrackerWPF.exe`
- Size: ~181 MB
- Self-contained (includes .NET runtime)

**Installer:**
- Location: `TimeTrackerWPFSetup_v1.0.41.exe`
- Size: ~100 MB (compressed)
- Professional Windows installer

---

## Creating Your First Release

### Step 1: Push the Workflows

```bash
cd /mnt/c/users/admin/documents/TimeTrackerWPF
git add .github/
git commit -m "Add GitHub Actions for automated builds and releases"
git push
```

### Step 2: Create a Release Tag

```bash
# Make sure everything is committed
git status

# Create annotated tag
git tag -a v1.0.41 -m "Initial WPF release - full feature parity with Windows Forms version"

# Push the tag
git push origin v1.0.41
```

### Step 3: Watch the Magic Happen

1. Go to: https://github.com/pikapp1977/TimeTrackerWPF/actions
2. Watch the "Create Release" workflow run
3. After ~5-10 minutes, check: https://github.com/pikapp1977/TimeTrackerWPF/releases
4. Your release will be there with both files ready to download!

---

## Future Releases

When you want to release a new version:

### Update Version Number

Edit `Version.props` in the timetracker folder (shared version):
```xml
<Version>1.0.42</Version>
```

### Commit and Tag

```bash
cd /mnt/c/users/admin/documents/TimeTrackerWPF
git add ../timetracker/Version.props
git commit -m "Bump version to 1.0.42"
git push

# Create new release
git tag -a v1.0.42 -m "Release version 1.0.42 - bug fixes and improvements"
git push origin v1.0.42
```

Done! GitHub Actions will automatically create the release.

---

## Advantages of GitHub Actions

✅ **Automated builds** - No need for local Inno Setup installation  
✅ **Consistent environment** - Same build every time  
✅ **Release automation** - Tag and release in one step  
✅ **Artifact storage** - All builds saved for 90 days  
✅ **Cross-platform development** - Build from Ubuntu/WSL, builds run on Windows  
✅ **Free for public repos** - Unlimited build minutes  

---

## Comparison with Original TimeTracker

Both repositories now have the same automated build setup:

| Feature | TimeTracker (WinForms) | TimeTrackerWPF |
|---------|:----------------------:|:--------------:|
| Automated builds | ✅ | ✅ |
| Inno Setup installer | ✅ | ✅ |
| Artifact uploads | ✅ | ✅ |
| Release automation | ✅ | ✅ |
| Manual trigger | ✅ | ✅ |

---

## Troubleshooting

### Workflow Fails

1. Check the Actions tab for error messages
2. Common issues:
   - Syntax error in YAML
   - Missing files (e.g., .iss file)
   - Version mismatch in filenames

### Installer Not Created

- Check that `TimeTrackerWPFSetup.iss` exists
- Verify path to published EXE is correct
- Check Inno Setup installation step succeeded

### Release Not Created

- Make sure tag starts with `v` (e.g., `v1.0.41`)
- Tag must be pushed to GitHub
- Check GITHUB_TOKEN has permissions

---

## Quick Commands Reference

```bash
# Push workflows to GitHub
cd /mnt/c/users/admin/documents/TimeTrackerWPF
git add .github/
git commit -m "Add GitHub Actions workflows"
git push

# Create a release
git tag -a v1.0.41 -m "Release v1.0.41"
git push origin v1.0.41

# Push code changes (triggers build)
git add .
git commit -m "Your changes"
git push

# View workflows
# https://github.com/pikapp1977/TimeTrackerWPF/actions

# View releases
# https://github.com/pikapp1977/TimeTrackerWPF/releases
```

---

## Next Steps

1. ✅ Push the workflow files to GitHub
2. ✅ Watch the first build run
3. ✅ Create your first release tag
4. ✅ Download and test the installer
5. ✅ Share the release with users

---

## Benefits for You (Ubuntu User)

Since you're working from Ubuntu/WSL:
- ✅ No need to install Inno Setup locally
- ✅ No need for Windows-specific tools
- ✅ Push from Linux, build happens in cloud on Windows
- ✅ Download ready-to-distribute installers
- ✅ Professional CI/CD pipeline

**You can develop on Linux, and GitHub Actions handles the Windows build!** 🚀
