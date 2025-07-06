# Everything App - Architecture Documentation

## Overview

The Everything App is built using .NET MAUI (Multi-platform App UI) following the MVVM (Model-View-ViewModel) architectural pattern. This document outlines the application's architecture, design patterns, and component interactions.

## Architectural Patterns

### MVVM (Model-View-ViewModel)

The application strictly follows the MVVM pattern to ensure separation of concerns and maintainability:

- **Model**: Data structures and business logic (`Models/`, `Services/`)
- **View**: User interface components (`*.xaml` files)
- **ViewModel**: Presentation logic and data binding (`ViewModels/`)

### Command Pattern

All user interactions are handled through the Command pattern using `RelayCommand` implementation:
- Encapsulates user actions
- Enables/disables UI elements based on state
- Supports parameterized commands

### Service Layer Pattern

Business logic is encapsulated in service classes:
- `FileService`: File system operations
- `FileOperationService`: File manipulation operations
- `BookmarkService`: Bookmark management

## Component Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        Presentation Layer                   │
├─────────────────────────────────────────────────────────────┤
│  MainPage.xaml (View)                                      │
│  ├── MenuBar                                               │
│  ├── SearchBar                                             │
│  ├── FileListView                                          │
│  └── StatusBar                                             │
├─────────────────────────────────────────────────────────────┤
│  MainPageViewModel (ViewModel)                             │
│  ├── Data Binding Properties                               │
│  ├── Command Implementations                               │
│  └── Event Handling                                        │
├─────────────────────────────────────────────────────────────┤
│                        Service Layer                        │
├─────────────────────────────────────────────────────────────┤
│  FileService              FileOperationService             │
│  ├── File Scanning        ├── Copy/Cut/Paste              │
│  ├── Search Logic         ├── Delete Operations           │
│  └── Filtering            └── Rename Operations            │
│                                                            │
│  BookmarkService                                           │
│  ├── Bookmark Storage                                      │
│  ├── CRUD Operations                                       │
│  └── JSON Persistence                                      │
├─────────────────────────────────────────────────────────────┤
│                         Data Layer                          │
├─────────────────────────────────────────────────────────────┤
│  FileItem (Model)         Bookmark (Model)                │
│  ├── File Properties      ├── Bookmark Data               │
│  ├── Display Properties   └── Metadata                     │
│  └── INotifyPropertyChanged                               │
└─────────────────────────────────────────────────────────────┘
```

## Core Components

### 1. Models

#### FileItem
```csharp
public class FileItem : INotifyPropertyChanged
{
    // Display properties
    public string Name { get; set; }
    public string Path { get; set; }
    public string Size { get; set; }
    public string DateModified { get; set; }
    public string Icon { get; set; }
    public string BackgroundColor { get; set; }
    
    // Metadata
    public string FullPath { get; set; }
    public bool IsDirectory { get; set; }
    public long SizeInBytes { get; set; }
    public DateTime LastModified { get; set; }
    public bool IsSelected { get; set; }
}
```

**Responsibilities**:
- Represents file/folder data
- Implements property change notifications
- Manages selection state
- Provides formatted display values

#### Bookmark
```csharp
public class Bookmark
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public DateTime CreatedDate { get; set; }
}
```

**Responsibilities**:
- Represents bookmark data
- Provides unique identification
- Stores creation metadata

### 2. ViewModels

#### MainPageViewModel
```csharp
public class MainPageViewModel : INotifyPropertyChanged
{
    // Collections
    public ObservableCollection<FileItem> Files { get; }
    public ObservableCollection<object> SelectedFiles { get; }
    
    // Properties
    public string SearchText { get; set; }
    public string SelectedFilter { get; set; }
    public string StatusText { get; set; }
    
    // Commands
    public ICommand CopyCommand { get; }
    public ICommand PasteCommand { get; }
    // ... other commands
}
```

**Responsibilities**:
- Manages UI state and data
- Handles user commands
- Coordinates service operations
- Implements data binding
- Manages command states

### 3. Services

#### FileService
```csharp
public class FileService
{
    public async Task<List<FileItem>> GetFilesAsync(string? rootPath = null)
    public async Task<List<FileItem>> SearchFilesAsync(string searchTerm, string filter, bool useRegex)
    private void ScanDirectory(string directoryPath, List<FileItem> files, int currentDepth, int maxDepth)
    private FileItem CreateFileItem(FileSystemInfo info)
}
```

**Responsibilities**:
- File system scanning and enumeration
- Search functionality with regex support
- File type filtering
- Performance optimization (depth limiting, result capping)
- Error handling for inaccessible directories

#### FileOperationService
```csharp
public class FileOperationService
{
    public async Task<bool> CopyFilesAsync(IEnumerable<FileItem> files)
    public async Task<bool> CutFilesAsync(IEnumerable<FileItem> files)
    public async Task<bool> PasteFilesAsync(string destinationPath)
    public async Task<bool> DeleteFilesAsync(IEnumerable<FileItem> files)
    public async Task<bool> RenameFileAsync(FileItem file, string newName)
}
```

**Responsibilities**:
- Clipboard operations (copy/cut/paste)
- File and directory operations
- Async operation handling
- Error management and reporting

#### BookmarkService
```csharp
public class BookmarkService
{
    public async Task<bool> AddBookmarkAsync(string name, string path)
    public async Task<bool> RemoveBookmarkAsync(Guid bookmarkId)
    public async Task<bool> UpdateBookmarkAsync(Guid bookmarkId, string name, string path)
    private void LoadBookmarks()
    private async Task SaveBookmarksAsync()
}
```

**Responsibilities**:
- Bookmark persistence using JSON
- CRUD operations for bookmarks
- Default bookmark initialization
- File I/O error handling

### 4. Commands

#### RelayCommand
```csharp
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;
    
    public bool CanExecute(object? parameter)
    public void Execute(object? parameter)
    public void RaiseCanExecuteChanged()
}
```

**Responsibilities**:
- Encapsulates command logic
- Manages command state (enabled/disabled)
- Supports parameterized commands
- Implements ICommand interface

## Data Flow

### 1. Application Startup
```
App.xaml.cs → MauiProgram.cs → AppShell.xaml → MainPage.xaml → MainPageViewModel
```

1. `MauiProgram` configures the application
2. `App` sets the main page to `AppShell`
3. `AppShell` navigates to `MainPage`
4. `MainPage` creates and binds to `MainPageViewModel`
5. `MainPageViewModel` initializes services and loads initial data

### 2. File Loading Process
```
MainPageViewModel.LoadInitialFiles() → FileService.GetFilesAsync() → UI Update
```

1. ViewModel calls FileService to get files
2. FileService scans file system asynchronously
3. Results are added to ObservableCollection
4. UI automatically updates via data binding
5. Status bar shows progress and results count

### 3. Search Process
```
User Input → SearchText Property → PerformSearch() → FileService.SearchFilesAsync() → UI Update
```

1. User types in search box
2. SearchText property setter triggers search
3. ViewModel calls FileService with search parameters
4. FileService filters results based on criteria
5. UI updates with filtered results

### 4. File Operations
```
User Action → Command → ViewModel Handler → FileOperationService → Status Update
```

1. User triggers action (menu, keyboard, context menu)
2. Command executes corresponding ViewModel method
3. ViewModel calls appropriate service method
4. Service performs file operation
5. ViewModel updates status and refreshes if needed

## Threading and Async Patterns

### Async/Await Usage
- All file operations use async/await pattern
- UI thread remains responsive during operations
- Background tasks for file scanning and search
- Proper exception handling in async methods

### Thread Safety
- ObservableCollection updates on UI thread
- Service operations run on background threads
- Property change notifications on UI thread
- Command state updates synchronized

## Error Handling Strategy

### Graceful Degradation
- Sample data fallback when file system access fails
- Continue operation when individual files/folders are inaccessible
- User-friendly error messages in status bar

### Exception Management
```csharp
try
{
    // File operation
}
catch (UnauthorizedAccessException)
{
    // Skip inaccessible items
}
catch (Exception ex)
{
    // Log error and show user message
    StatusText = $"Error: {ex.Message}";
}
```

## Performance Considerations

### File System Scanning
- **Depth Limiting**: Maximum 3 levels deep to prevent excessive recursion
- **Result Capping**: Limit to 10,000 files for initial load, 1,000 for search
- **Batch Processing**: Process files in chunks to maintain responsiveness
- **Early Termination**: Stop scanning on user cancellation

### Memory Management
- **Lazy Loading**: Load file details only when needed
- **Collection Virtualization**: UI virtualizes large lists
- **Weak References**: Avoid memory leaks in event handlers
- **Disposal**: Proper disposal of file system resources

### Search Optimization
- **Incremental Search**: Search as user types
- **Debouncing**: Delay search to avoid excessive operations
- **Caching**: Cache recent search results
- **Indexing**: Consider file indexing for large datasets

## Extensibility Points

### Adding New File Types
1. Extend file type lists in `FileService`
2. Add corresponding icons to Resources
3. Update filtering logic

### Custom Commands
1. Create new command in ViewModel
2. Add menu item or button in XAML
3. Implement command handler
4. Update command state management

### Additional Services
1. Create service interface
2. Implement service class
3. Register in dependency injection (if used)
4. Inject into ViewModel

### Platform-Specific Features
1. Use conditional compilation (`#if WINDOWS`)
2. Implement platform-specific services
3. Use dependency injection for platform services

## Security Considerations

### File System Access
- Respect file system permissions
- Handle unauthorized access gracefully
- Validate file paths to prevent directory traversal
- Sanitize user input for file operations

### Data Protection
- Bookmark data stored in user's app data folder
- No sensitive data in plain text
- Proper file permissions on data files

## Testing Strategy

### Unit Testing
- Test ViewModels in isolation
- Mock service dependencies
- Test command logic
- Validate property change notifications

### Integration Testing
- Test service interactions
- File system operation testing
- Search functionality validation

### UI Testing
- Automated UI tests for critical paths
- Manual testing for user experience
- Cross-platform compatibility testing

## Future Enhancements

### Performance Improvements
- File system indexing service
- Background file monitoring
- Incremental updates
- Parallel processing

### Feature Additions
- Advanced search operators
- File content search
- Network drive support
- Plugin architecture

### UI Enhancements
- Customizable themes
- Resizable columns
- Multiple view modes
- Keyboard navigation

This architecture provides a solid foundation for a maintainable, extensible, and performant file search application while following established patterns and best practices.

