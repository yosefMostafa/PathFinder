using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ProjectFinder.Models;
using ProjectFinder.Services;
using ProjectFinder.Commands;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using ProjectFinder.Services.Windows;
using ProjectFinder.Services.windows;
using System.Threading.Tasks;
using backendLogic.src.searchEngine.models;
namespace ProjectFinder
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly FileService _fileService;
        private bool _isGettingData = false;
        private readonly FileOperationService _fileOperationService;
        private readonly BookmarkService _bookmarkService;

        private string _searchText = string.Empty;
        private string _selectedFilter = "All";
        private string _statusText = "Ready";
        private bool _isRegexEnabled;
        private bool _isAudioFilterEnabled;
        private bool _matchCase;
        private bool _matchWholeWord;
        private bool _isFullScreenTogglerVisible = false; // Assuming you want to control visibility of fullscreen toggler

        public MainPageViewModel()
        {
            _fileService = new FileService();
            _fileOperationService = new FileOperationService();
            _bookmarkService = new BookmarkService();
            Files = new ObservableCollection<FileItem>();
            SelectedFiles = new ObservableCollection<object>();

#if WINDOWS
            var toggleItem = new ToolbarItem
            {
                Text = "Toggle Fullscreen",
                Command = FullScreenToggler
            };
#endif

            InitializeCommands();
            LoadInitialFiles();
        }


        public ObservableCollection<FileItem> Files { get; }
        public ObservableCollection<object> SelectedFiles { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                PerformSearch();
            }
        }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged();
                UpdateFilterStatus();
                PerformSearch();
            }
        }

        public string StatusText
        {
            get => _statusText;
            set
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }
        public bool IsFullScreenTogglerVisible
        {
            get => _isFullScreenTogglerVisible;
            set
            {
                _isFullScreenTogglerVisible = value;
                OnPropertyChanged();
            }
        }
        public bool IsRegexEnabled
        {
            get => _isRegexEnabled;
            set
            {
                _isRegexEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool IsGettingData
        {
            get => _isGettingData;
            private set
            {
                _isGettingData = value;
                Console.WriteLine($"IsGettingData set to: {_isGettingData}");
                OnPropertyChanged();
            }
        }

        public bool IsAudioFilterEnabled
        {
            get => _isAudioFilterEnabled;
            set
            {
                _isAudioFilterEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool MatchCase
        {
            get => _matchCase;
            set
            {
                _matchCase = value;
                OnPropertyChanged();
                PerformSearch();
            }
        }

        public bool MatchWholeWord
        {
            get => _matchWholeWord;
            set
            {
                _matchWholeWord = value;
                OnPropertyChanged();
                PerformSearch();
            }
        }

        // Commands
        public ICommand NewCommand { get; private set; } = null!;
        public ICommand OpenCommand { get; private set; } = null!;
        public ICommand ExitCommand { get; private set; } = null!;
        public ICommand CopyCommand { get; private set; } = null!;
        public ICommand CutCommand { get; private set; } = null!;
        public ICommand PasteCommand { get; private set; } = null!;
        public ICommand DeleteCommand { get; private set; } = null!;
        public ICommand SelectAllCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;
        public ICommand DetailsViewCommand { get; private set; } = null!;
        public ICommand ListViewCommand { get; private set; } = null!;
        public ICommand MatchCaseCommand { get; private set; } = null!;
        public ICommand MatchWholeWordCommand { get; private set; } = null!;
        public ICommand UseRegexCommand { get; private set; } = null!;
        public ICommand AddBookmarkCommand { get; private set; } = null!;
        public ICommand OrganizeBookmarksCommand { get; private set; } = null!;
        public ICommand OptionsCommand { get; private set; } = null!;
        public ICommand AboutCommand { get; private set; } = null!;
        public ICommand OpenFileCommand { get; private set; } = null!;
        public ICommand RenameCommand { get; private set; } = null!;
        public ICommand RightClickCommand { get; private set; } = null!;
        public ICommand FullScreenToggler { get; private set; } = null!;
        public ICommand ChangeProjectCommand { get; private set; } = null!;


        private void InitializeCommands()
        {
            NewCommand = new RelayCommand(OnNew);
            OpenCommand = new RelayCommand(OnOpen, () => SelectedFiles.Any());
            ExitCommand = new RelayCommand(OnExit);
            CopyCommand = new RelayCommand(OnCopy, () => SelectedFiles.Any());
            CutCommand = new RelayCommand(OnCut, () => SelectedFiles.Any());
            PasteCommand = new RelayCommand(OnPaste, () => _fileOperationService.HasClipboardContent);
            DeleteCommand = new RelayCommand(OnDelete, () => SelectedFiles.Any());
            SelectAllCommand = new RelayCommand(OnSelectAll);
            RefreshCommand = new RelayCommand(OnRefresh);
            DetailsViewCommand = new RelayCommand(OnDetailsView);
            ListViewCommand = new RelayCommand(OnListView);
            MatchCaseCommand = new RelayCommand(OnMatchCase);
            MatchWholeWordCommand = new RelayCommand(OnMatchWholeWord);
            UseRegexCommand = new RelayCommand(OnUseRegex);
            AddBookmarkCommand = new RelayCommand(OnAddBookmark);
            OrganizeBookmarksCommand = new RelayCommand(OnOrganizeBookmarks);
            OptionsCommand = new RelayCommand(OnOptions);
            AboutCommand = new RelayCommand(OnAbout);
            OpenFileCommand = new RelayCommand<FileItem>(OnOpenFile);
            RenameCommand = new RelayCommand(OnRename, () => SelectedFiles.Count == 1);
            FullScreenToggler = new RelayCommand(ToggleFullScreen);
            ChangeProjectCommand = new RelayCommand<string>(param =>
                { OnChangeProject(param!); }
            );
            RightClickCommand = new RelayCommand<(FileItem, double, double)>(tuple =>
            {
                var (item, x, y) = tuple;
                OnRightClick(item, x, y);
            });

            // Subscribe to selection changes to update command states
            SelectedFiles.CollectionChanged += (s, e) => UpdateCommandStates();
        }
        private void ToggleFullScreen()
        {
#if WINDOWS
            WindowsServicesHandler.ToggleFullScreenAsync();
#endif
        }
        private void UpdateCommandStates()
        {
            ((RelayCommand)OpenCommand).RaiseCanExecuteChanged();
            ((RelayCommand)CopyCommand).RaiseCanExecuteChanged();
            ((RelayCommand)CutCommand).RaiseCanExecuteChanged();
            ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RenameCommand).RaiseCanExecuteChanged();
        }
        private void OnChangeProject(string projectName)
        {
            switch (projectName)
            {
                case "Node":
                    _fileService.changeEngineSearch(ProjectTypeEnum.Node);
                    LoadInitialFiles();
                    break;
                case "Flutter":
                    _fileService.changeEngineSearch(ProjectTypeEnum.FlutterProjects);
                    LoadInitialFiles();
                    break;
                case "PDF":
                    _fileService.changeEngineSearch(ProjectTypeEnum.PDF);
                    LoadInitialFiles();
                    break;
                // Add more cases as needed
                default:
                    Console.WriteLine($"Unknown project type: {projectName}");
                    return;
            }
            // Logic to change project, e.g., open a dialog to select a new project

        }
        private void OnRightClick(FileItem file, double x, double y)
        {
            Console.WriteLine($"Right-clicked on file: {file.FullPath} at ({x}, {y})");
            WindowsServicesHandler.CloseMenuDelegate();
            WindowsServicesHandler.ShowMenuDelegate(file.ParentPath, x, y);

        }
        private async void LoadInitialFiles()
        {

            try
            {
                IsGettingData = true;
                StatusText = "Loading files...";

                Console.WriteLine(IsGettingData);
                Files.Clear();
                List<FileItem> files = await _fileService.GetFilesAsync();


                Console.WriteLine($"Loaded {files.Count} files from service.");
                foreach (var file in files)
                {
                    Files.Add(file);
                }
                IsGettingData = false;
                Console.WriteLine(IsGettingData);

                StatusText = $"{Files.Count:N0} objects";
                Console.WriteLine($"Loaded {Files.Count} files.");

            }
            catch (Exception ex)
            {
                StatusText = $"Error loading files: {ex.Message}";
            }
        }

        private async void PerformSearch()
        {

            // if (string.IsNullOrWhiteSpace(SearchText))
            // {
            //     LoadInitialFiles();
            //     return;
            // }

            // try
            // {
            //     StatusText = "Searching...";
            //     var searchResults = await _fileService.SearchFilesAsync(SearchText, SelectedFilter, IsRegexEnabled);

            //     Files.Clear();
            //     foreach (var file in searchResults)
            //     {
            //         Files.Add(file);
            //     }

            //     StatusText = $"{Files.Count:N0} objects";
            // }
            // catch (Exception ex)
            // {
            //     StatusText = $"Search error: {ex.Message}";
            // }
        }

        private void UpdateFilterStatus()
        {
            IsAudioFilterEnabled = SelectedFilter == "Audio";
            IsRegexEnabled = SelectedFilter == "REGEX";
        }

        // Command implementations
        private void OnNew() => StatusText = "New folder created";

        private async void OnOpen()
        {
            if (SelectedFiles.FirstOrDefault() is FileItem file)
            {
                var success = await _fileOperationService.OpenFileAsync(file);
                StatusText = success ? $"Opened {file.Name}" : $"Failed to open {file.Name}";
            }
        }

        private void OnExit() => Application.Current?.Quit();

        private async void OnCopy()
        {
            var selectedFileItems = SelectedFiles.OfType<FileItem>().ToList();
            if (selectedFileItems.Any())
            {
                var success = await _fileOperationService.CopyFilesAsync(selectedFileItems);
                StatusText = success ? $"Copied {selectedFileItems.Count} items" : "Copy failed";
            }
        }

        private async void OnCut()
        {
            var selectedFileItems = SelectedFiles.OfType<FileItem>().ToList();
            if (selectedFileItems.Any())
            {
                var success = await _fileOperationService.CutFilesAsync(selectedFileItems);
                StatusText = success ? $"Cut {selectedFileItems.Count} items" : "Cut failed";
            }
        }

        private async void OnPaste()
        {
            // For simplicity, paste to the first selected directory or current directory
            var targetPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var success = await _fileOperationService.PasteFilesAsync(targetPath);
            StatusText = success ? "Paste completed" : "Paste failed";
        }

        private async void OnDelete()
        {
            var selectedFileItems = SelectedFiles.OfType<FileItem>().ToList();
            if (selectedFileItems.Any())
            {
                var success = await _fileOperationService.DeleteFilesAsync(selectedFileItems);
                StatusText = success ? $"Deleted {selectedFileItems.Count} items" : "Delete failed";
                if (success)
                {
                    foreach (var item in selectedFileItems)
                    {
                        Files.Remove(item);
                    }
                    SelectedFiles.Clear();
                }
            }
        }

        private void OnSelectAll()
        {
            SelectedFiles.Clear();
            foreach (var file in Files)
            {
                SelectedFiles.Add(file);
                file.IsSelected = true;
            }
            StatusText = $"Selected {Files.Count} items";
        }

        private void OnRefresh() => LoadInitialFiles();


        private void OnDetailsView() => StatusText = "Details view selected";
        private void OnListView() => StatusText = "List view selected";

        private void OnMatchCase()
        {
            MatchCase = !MatchCase;
            StatusText = $"Match case {(MatchCase ? "enabled" : "disabled")}";
        }

        private void OnMatchWholeWord()
        {
            MatchWholeWord = !MatchWholeWord;
            StatusText = $"Match whole word {(MatchWholeWord ? "enabled" : "disabled")}";
        }

        private void OnUseRegex()
        {
            IsRegexEnabled = !IsRegexEnabled;
            StatusText = $"Regular expressions {(IsRegexEnabled ? "enabled" : "disabled")}";
            PerformSearch();
        }

        private async void OnAddBookmark()
        {
            var success = await _bookmarkService.AddBookmarkAsync("New Bookmark", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            StatusText = success ? "Bookmark added" : "Failed to add bookmark";
        }

        private void OnOrganizeBookmarks() => StatusText = "Organize bookmarks dialog would open here";
        private void OnOptions() => StatusText = "Options dialog would open here";
        private void OnAbout() => StatusText = "Everything - File Search Tool v1.0";

        private async void OnOpenFile(FileItem? file)
        {
            if (file != null)
            {
                var success = await _fileOperationService.OpenFileAsync(file);
                StatusText = success ? $"Opened {file.Name}" : $"Failed to open {file.Name}";
            }
        }

        private async void OnRename()
        {
            if (SelectedFiles.FirstOrDefault() is FileItem file)
            {
                // In a real application, this would show a rename dialog
                var newName = $"{Path.GetFileNameWithoutExtension(file.Name)}_renamed{Path.GetExtension(file.Name)}";
                var success = await _fileOperationService.RenameFileAsync(file, newName);
                StatusText = success ? $"Renamed to {newName}" : "Rename failed";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

