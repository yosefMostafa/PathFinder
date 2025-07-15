using System.ComponentModel;
using backendLogic.src.searchEngine;

namespace ProjectFinder.Models
{
    public class FileItem : INotifyPropertyChanged
    {
        private string _filePath = string.Empty;
        private string _name = string.Empty;
        private string _paretSize = string.Empty;
        private string _parentType = string.Empty;
        private string _parentIconPath = string.Empty;
        private string _parentName = string.Empty;
        private string _parentPath = string.Empty;
        private string _size = string.Empty;
        private string _dateCreated = string.Empty;
        private string _dateModified = string.Empty;
        private string _icon = string.Empty;
        private string _backgroundColor = "White";
        private bool _isSelected;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public string ParetSize
        {
            get => _paretSize;
            set
            {
                _paretSize = value;
                OnPropertyChanged();
            }
        }

        public string ParentPath
        {
            get => _parentPath;
            set
            {
                _parentPath = value;
                OnPropertyChanged();
            }
        }
        public string ParentType
        {
            get => _parentType;
            set
            {
                _parentType = value;
                OnPropertyChanged();
            }
        }
        public string ParentIconPath
        {
            get => _parentIconPath;
            set
            {
                _parentIconPath = value;
                OnPropertyChanged();
            }
        }
        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }

        public string Size
        {
            get => _size;
            set
            {
                _size = value;
                OnPropertyChanged();
            }
        }
        public string DateModified
        {
            get => _dateModified;
            set
            {
                _dateModified = value;
                OnPropertyChanged();
            }
        }



        public string DateCreated
        {
            get => _dateCreated;
            set
            {
                _dateCreated = value;
                OnPropertyChanged();
            }
        }

        public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged();
            }
        }
        public string ParentName
        {
            get => _parentName;
            set
            {
                _parentName = value;
                OnPropertyChanged();
            }
        }

        public string BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                OnPropertyChanged();
            }
        }
        public string GetDriveName()
        {
            if (string.IsNullOrEmpty(_parentPath))
                return "Unknown Drive";

            var driveName = _parentPath.Split('\\').FirstOrDefault();
            return driveName ?? "Unknown Drive";
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                BackgroundColor = value ? "#E3F2FD" : "White";
                OnPropertyChanged();
            }
        }

        public string FullPath { get; set; } = string.Empty;
        public bool IsDirectory { get; set; }
        public long SizeInBytes { get; set; }
        public DateTime LastModified { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static FileItem FromEverythingResult(EverythingResult result)
        {
           return new FileItem
            {
                Name = result.FileName,
                ParentPath = result.ParentPath ?? string.Empty,
                FilePath = result.FullPath,
                ParetSize = result.PathSize,
                ParentType = result.FolderType,
                ParentIconPath = result.FolderIcon,
                FullPath = result.FullPath,
                Size = result.Size,
                ParentName = result.ParentPath?.Split("\\").LastOrDefault() ?? string.Empty,
                DateModified = result.DateModified?.ToString("M/d/yyyy h:mm tt") ?? string.Empty,
                Icon = !string.IsNullOrEmpty(result.FolderIcon)
                    ? result.FolderIcon.Split("/").LastOrDefault() ?? string.Empty
                    : string.Empty,
                DateCreated = result.DateCreated?.ToString("M/d/yyyy h:mm tt") ?? string.Empty,
            };
        }
    }
}

