using System.ComponentModel;
using backendLogic.src.searchEngine;

namespace ProjectFinder.Models
{
    public class FileItem : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private string _path = string.Empty;
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

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
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
        public string DateModified {
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

        public string BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                OnPropertyChanged();
            }
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
                Path = result.Path,
                FullPath = result.FullPath,
                Size = result.Size,
                DateModified = result.DateModified?.ToString("M/d/yyyy h:mm tt") ?? string.Empty,
                Icon = result.FileName,
                DateCreated = result.DateCreated?.ToString("M/d/yyyy h:mm tt") ?? string.Empty,
            };
        }
    }
}

