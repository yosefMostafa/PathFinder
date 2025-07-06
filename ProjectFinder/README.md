# Everything App - .NET MAUI File Search Tool

A .NET MAUI application that replicates the functionality of the "Everything" file search tool, providing fast file and folder searching with advanced filtering capabilities.

## Features

### Core Functionality
- **Fast File Search**: Real-time search across file system with instant results
- **Advanced Filtering**: Filter by file type (Audio, Video, Images, Documents)
- **Regular Expression Support**: Use regex patterns for complex searches
- **Case Sensitivity**: Toggle case-sensitive search
- **Whole Word Matching**: Match complete words only

### User Interface
- **Menu Bar**: Complete menu system with File, Edit, View, Search, Bookmarks, Tools, and Help menus
- **Search Bar**: Intuitive search interface with filter dropdown
- **File List**: Detailed file listing with columns for Name, Path, Size, and Date Modified
- **Status Bar**: Real-time status updates and filter indicators
- **File Icons**: Visual file type indicators

### File Operations
- **Copy/Cut/Paste**: Standard clipboard operations
- **Delete**: Remove files and folders
- **Rename**: Rename files and folders
- **Open**: Launch files with default applications
- **Select All**: Select all visible files

### Bookmarks
- **Add Bookmarks**: Save frequently accessed locations
- **Organize Bookmarks**: Manage bookmark collection
- **Quick Access**: Fast navigation to bookmarked folders

## Project Structure

```
EverythingApp/
├── Models/
│   └── FileItem.cs                 # File/folder data model
├── ViewModels/
│   └── MainPageViewModel.cs        # Main window view model (MVVM pattern)
├── Services/
│   ├── FileService.cs              # File system operations and search
│   ├── FileOperationService.cs     # File operations (copy, cut, paste, delete)
│   └── BookmarkService.cs          # Bookmark management
├── Commands/
│   └── RelayCommand.cs             # Command pattern implementation
├── Resources/
│   ├── Styles/
│   │   ├── Colors.xaml             # Color definitions
│   │   └── Styles.xaml             # UI styles
│   ├── Images/                     # File type icons
│   ├── Fonts/                      # Custom fonts
│   └── Raw/                        # Raw assets
├── MainPage.xaml                   # Main window UI layout
├── MainPage.xaml.cs                # Main window code-behind
├── App.xaml                        # Application resources
├── App.xaml.cs                     # Application startup
├── AppShell.xaml                   # Shell navigation
├── AppShell.xaml.cs                # Shell code-behind
├── MauiProgram.cs                  # Application configuration
└── EverythingApp.csproj            # Project file
```

## Technical Details

### Architecture
- **MVVM Pattern**: Clean separation of concerns using Model-View-ViewModel
- **Command Pattern**: Proper command handling with RelayCommand implementation
- **Async/Await**: Non-blocking file operations and search
- **Data Binding**: Two-way binding between UI and view models

### Key Components

#### FileService
- Scans file system with configurable depth limits
- Implements search with regex support
- Provides file type filtering
- Handles large result sets efficiently

#### FileOperationService
- Manages clipboard operations
- Handles file/folder operations
- Provides async file operations
- Supports both copy and move operations

#### BookmarkService
- Persistent bookmark storage using JSON
- Default bookmarks for common locations
- Add/remove/update bookmark operations

### Performance Optimizations
- **Limited Depth Scanning**: Prevents excessive recursion
- **Result Limiting**: Caps search results for performance
- **Async Operations**: Non-blocking UI during file operations
- **Sample Data Fallback**: Graceful handling when file system access is limited

## Build Requirements

### Prerequisites
- .NET 8.0 SDK or later
- .NET MAUI workload installed
- Visual Studio 2022 (recommended) or Visual Studio Code with C# extension

### Supported Platforms
- Windows 10/11 (Primary target)
- macOS (with .NET MAUI for Mac)
- Android (mobile adaptation)
- iOS (mobile adaptation)

## Build Instructions

### Using Visual Studio 2022
1. Open `EverythingApp.sln` in Visual Studio 2022
2. Ensure .NET MAUI workload is installed
3. Select target platform (Windows recommended)
4. Build and run the project (F5)

### Using Command Line
```bash
# Clone or extract the project
cd EverythingApp

# Restore NuGet packages
dotnet restore

# Build the project
dotnet build

# Run the application (Windows)
dotnet run --framework net8.0-windows10.0.19041.0
```

### Installing .NET MAUI Workload
```bash
# Install MAUI workload
dotnet workload install maui

# Verify installation
dotnet workload list
```

## Configuration

### File Type Extensions
File type filtering is configured in `FileService.cs`:
- **Audio**: .mp3, .wav, .flac, .aac, .ogg, .wma, .m4a
- **Video**: .mp4, .avi, .mkv, .mov, .wmv, .flv, .webm
- **Images**: .jpg, .jpeg, .png, .gif, .bmp, .tiff, .svg
- **Documents**: .pdf, .doc, .docx, .txt, .rtf, .xls, .xlsx, .ppt, .pptx

### Performance Settings
- **Max Scan Depth**: 3 levels (configurable in FileService)
- **Max Results**: 10,000 files (configurable)
- **Search Results**: 1,000 items (configurable)

## Usage

### Basic Search
1. Type search terms in the search box
2. Results appear instantly as you type
3. Use the filter dropdown to limit by file type

### Advanced Search
- **Regex**: Select "REGEX" filter and use regular expressions
- **Case Sensitive**: Use Search menu → Match Case
- **Whole Words**: Use Search menu → Match Whole Word

### File Operations
- **Select Files**: Click to select, Ctrl+Click for multiple selection
- **Copy**: Ctrl+C or Edit menu → Copy
- **Cut**: Ctrl+X or Edit menu → Cut
- **Paste**: Ctrl+V or Edit menu → Paste
- **Delete**: Delete key or Edit menu → Delete
- **Rename**: F2 or right-click → Rename

### Bookmarks
- **Add**: Bookmarks menu → Add Bookmark
- **Manage**: Bookmarks menu → Organize Bookmarks

## Troubleshooting

### Common Issues

#### Build Errors
- Ensure .NET 8.0 SDK is installed
- Verify MAUI workload is properly installed
- Check target framework in project file

#### File Access Issues
- Run with appropriate permissions
- Check antivirus software interference
- Verify file system permissions

#### Performance Issues
- Reduce scan depth in FileService
- Limit result count
- Exclude large directories from search

### Sample Data Mode
If file system access is limited, the application falls back to sample data demonstrating the interface and functionality.

## Customization

### Adding File Types
Edit the extension lists in `FileService.cs`:
```csharp
private readonly List<string> _customExtensions = new() { ".custom", ".ext" };
```

### Changing UI Colors
Modify `Resources/Styles/Colors.xaml` to customize the color scheme.

### Adding Menu Items
Extend the MenuBar in `MainPage.xaml` and add corresponding commands in `MainPageViewModel.cs`.

## License

This project is provided as a code example and educational resource. Modify and use as needed for your projects.

## Contributing

This is a demonstration project. For production use, consider:
- Enhanced error handling
- Platform-specific optimizations
- Additional file operation features
- Improved UI/UX design
- Comprehensive testing

## Support

For issues related to .NET MAUI development:
- [.NET MAUI Documentation](https://docs.microsoft.com/en-us/dotnet/maui/)
- [.NET MAUI GitHub Repository](https://github.com/dotnet/maui)
- [Microsoft Developer Community](https://developercommunity.visualstudio.com/)

