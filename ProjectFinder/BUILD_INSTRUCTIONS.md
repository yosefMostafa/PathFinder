# Build Instructions for Everything App

## Prerequisites

### Required Software
1. **.NET 8.0 SDK** or later
   - Download from: https://dotnet.microsoft.com/download
   - Verify installation: `dotnet --version`

2. **.NET MAUI Workload**
   ```bash
   dotnet workload install maui
   ```

3. **Development Environment** (Choose one):
   - **Visual Studio 2022** (Recommended for Windows)
     - Community, Professional, or Enterprise edition
     - Install with ".NET Multi-platform App UI development" workload
   - **Visual Studio Code**
     - Install C# extension
     - Install .NET MAUI extension

### Platform-Specific Requirements

#### Windows Development
- Windows 10 version 1809 or higher
- Windows 11 (recommended)
- Windows SDK (automatically installed with Visual Studio)

#### macOS Development
- macOS 10.15 or higher
- Xcode (latest stable version)
- Xcode Command Line Tools

## Step-by-Step Build Process

### Method 1: Visual Studio 2022 (Recommended)

1. **Open the Project**
   ```
   File → Open → Project/Solution
   Navigate to EverythingApp folder
   Select EverythingApp.csproj
   ```

2. **Verify Target Framework**
   - Right-click project → Properties
   - Ensure target frameworks include `net8.0-windows10.0.19041.0`

3. **Restore Packages**
   - Right-click solution → Restore NuGet Packages
   - Or use: `Tools → NuGet Package Manager → Package Manager Console`
   ```
   dotnet restore
   ```

4. **Select Target Platform**
   - In the toolbar, select target platform (Windows Machine recommended)
   - For Windows: Choose "Windows Machine"

5. **Build and Run**
   - Press F5 to build and run
   - Or use: `Build → Start Debugging`

### Method 2: Command Line

1. **Navigate to Project Directory**
   ```bash
   cd path/to/EverythingApp
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the Project**
   ```bash
   # Debug build
   dotnet build
   
   # Release build
   dotnet build --configuration Release
   ```

4. **Run the Application**
   ```bash
   # For Windows
   dotnet run --framework net8.0-windows10.0.19041.0
   
   # For other platforms (if configured)
   dotnet run --framework net8.0-android
   dotnet run --framework net8.0-ios
   dotnet run --framework net8.0-maccatalyst
   ```

### Method 3: Visual Studio Code

1. **Open Project**
   ```bash
   code path/to/EverythingApp
   ```

2. **Install Required Extensions**
   - C# (Microsoft)
   - .NET MAUI (Microsoft)

3. **Open Terminal in VS Code**
   - Terminal → New Terminal

4. **Build and Run**
   ```bash
   dotnet restore
   dotnet build
   dotnet run --framework net8.0-windows10.0.19041.0
   ```

## Platform-Specific Build Instructions

### Windows Desktop Application

1. **Target Framework Configuration**
   Ensure `EverythingApp.csproj` includes:
   ```xml
   <TargetFrameworks>net8.0-windows10.0.19041.0</TargetFrameworks>
   ```

2. **Build Command**
   ```bash
   dotnet build --framework net8.0-windows10.0.19041.0 --configuration Release
   ```

3. **Publish for Distribution**
   ```bash
   dotnet publish --framework net8.0-windows10.0.19041.0 --configuration Release --self-contained true
   ```

### Android Application

1. **Prerequisites**
   - Android SDK (installed via Visual Studio or Android Studio)
   - Java Development Kit (JDK) 11 or later

2. **Build Command**
   ```bash
   dotnet build --framework net8.0-android --configuration Release
   ```

3. **Deploy to Device/Emulator**
   ```bash
   dotnet run --framework net8.0-android
   ```

### iOS Application

1. **Prerequisites** (macOS only)
   - Xcode with iOS SDK
   - iOS device or simulator

2. **Build Command**
   ```bash
   dotnet build --framework net8.0-ios --configuration Release
   ```

### macOS Application

1. **Build Command**
   ```bash
   dotnet build --framework net8.0-maccatalyst --configuration Release
   ```

## Troubleshooting Build Issues

### Common Problems and Solutions

#### 1. MAUI Workload Not Found
**Error**: `Workload ID maui isn't supported on this platform`

**Solution**:
```bash
# Update .NET SDK
dotnet --version

# Install MAUI workload
dotnet workload install maui

# If still failing, try:
dotnet workload update
dotnet workload repair
```

#### 2. Missing Dependencies
**Error**: Package restore failed

**Solution**:
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore --force
```

#### 3. Target Framework Issues
**Error**: Target framework not supported

**Solution**:
- Verify .NET 8.0 SDK installation
- Check project file target frameworks
- Ensure platform-specific SDKs are installed

#### 4. Windows SDK Missing
**Error**: Windows SDK not found

**Solution**:
- Install Visual Studio with Windows development workload
- Or download Windows SDK separately
- Verify SDK path in project properties

#### 5. Build Performance Issues
**Symptoms**: Slow build times

**Solutions**:
```bash
# Use parallel builds
dotnet build --verbosity minimal

# Clean before build
dotnet clean
dotnet build
```

### Debug Build vs Release Build

#### Debug Build (Development)
```bash
dotnet build --configuration Debug
```
- Includes debug symbols
- No optimizations
- Larger file size
- Better for debugging

#### Release Build (Production)
```bash
dotnet build --configuration Release
```
- Optimized code
- Smaller file size
- No debug symbols
- Better performance

## Publishing for Distribution

### Windows Executable
```bash
# Self-contained executable
dotnet publish --framework net8.0-windows10.0.19041.0 --configuration Release --self-contained true --runtime win-x64

# Framework-dependent (requires .NET runtime on target machine)
dotnet publish --framework net8.0-windows10.0.19041.0 --configuration Release --self-contained false
```

### Android APK
```bash
dotnet publish --framework net8.0-android --configuration Release
```

### iOS App Store Package
```bash
dotnet publish --framework net8.0-ios --configuration Release
```

## Verification Steps

### After Successful Build

1. **Check Output Directory**
   ```
   bin/Debug/net8.0-windows10.0.19041.0/
   bin/Release/net8.0-windows10.0.19041.0/
   ```

2. **Verify Executable**
   - Windows: `EverythingApp.exe`
   - Other platforms: Platform-specific packages

3. **Test Basic Functionality**
   - Application launches without errors
   - UI elements render correctly
   - Search functionality works
   - File operations respond

### Performance Verification
- Application startup time < 3 seconds
- Search results appear instantly
- UI remains responsive during file operations
- Memory usage remains reasonable

## Additional Resources

### Documentation
- [.NET MAUI Documentation](https://docs.microsoft.com/en-us/dotnet/maui/)
- [.NET CLI Reference](https://docs.microsoft.com/en-us/dotnet/core/tools/)

### Community Support
- [.NET MAUI GitHub](https://github.com/dotnet/maui)
- [Microsoft Developer Community](https://developercommunity.visualstudio.com/)
- [Stack Overflow - .NET MAUI](https://stackoverflow.com/questions/tagged/.net-maui)

### Tools
- [.NET SDK Downloads](https://dotnet.microsoft.com/download)
- [Visual Studio Downloads](https://visualstudio.microsoft.com/downloads/)
- [Visual Studio Code](https://code.visualstudio.com/)

## Next Steps

After successful build:
1. Test the application thoroughly
2. Consider adding unit tests
3. Implement additional features as needed
4. Prepare for deployment to target platforms
5. Set up continuous integration/deployment if required

