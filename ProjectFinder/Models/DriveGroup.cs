using System;
using System.Collections.ObjectModel;

namespace ProjectFinder.Models;

public class DriveGroup
{
    public string DriveName { get; set; }
    public ObservableCollection<FileItem> Files { get; set; }
    public DriveGroup(string driveName, ObservableCollection<FileItem> files)
    {
        DriveName = driveName;
        Files = files ?? new ObservableCollection<FileItem>();
    }
}
