
using backendLogic.src.searchEngine;

namespace backendLogic.src.searchEngine
{

    public class EverythingResult
    {
        public string FileName { get; set; }
        public string ParentPath { get; set; }
        public string PathSize { get; set; } // This is a string representation of the path size, not the actual size
        public string FolderType { get; set; } // This is a string representation of the folder type, not the actual type
        public string FolderIcon { get; set; } // This is a string representation of the folder icon, not the actual icon
        public string Size { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateAccessed { get; set; }
        public FileAttributes Attributes { get; set; }
        public int RunCount { get; set; }
        public DateTime? DateRun { get; set; }
        public string FullPath => System.IO.Path.Combine(ParentPath ?? "", FileName ?? "");
        public string toString()
        {
            return $"{FileName} ({ParentPath}) - Size: {Size} bytes, Created: {DateCreated}, Modified: {DateModified}, Accessed: {DateAccessed}, Attributes: {Attributes}, Run Count: {RunCount}, Last Run: {DateRun}";
        }
        public EverythingResult()
        {
            FileName = string.Empty;
            ParentPath = string.Empty;
            PathSize = "0 Bytes"; // Default value
            Size = "0 Bytes"; // Default value
            DateCreated = null;
            DateModified = null;
            DateAccessed = null;
            Attributes = FileAttributes.Normal; // Default value
            RunCount = 0; // Default value
            DateRun = null;
            FolderType = string.Empty; // Default value
            FolderIcon = string.Empty; // Default value
        }
        public void Print()
        {
            //print each property of the struct
            Console.WriteLine($"FileName: {FileName}");
            Console.WriteLine($"Path: {ParentPath}");
            Console.WriteLine($"Size: {Size}");
            Console.WriteLine($"pathSize: {PathSize}");
            Console.WriteLine($"DateCreated: {DateCreated}");
            Console.WriteLine($"DateModified: {DateModified}");
            Console.WriteLine($"DateAccessed: {DateAccessed}");
            Console.WriteLine($"Attributes: {Attributes}");
            Console.WriteLine($"RunCount: {RunCount}");
            Console.WriteLine($"FolderType: {FolderType}");
            Console.WriteLine($"FolderIcon: {FolderIcon}");
            Console.WriteLine($"DateRun: {DateRun}");
            Console.WriteLine($"FullPath: {FullPath}");
            Console.WriteLine("-------------------");


        }
        public EverythingResult Clone()
        {
            return new EverythingResult
            {
                FileName = this.FileName,
                ParentPath = this.ParentPath,
                PathSize = this.PathSize,
                Size = this.Size,
                DateCreated = this.DateCreated,
                DateModified = this.DateModified,
                DateAccessed = this.DateAccessed,
                Attributes = this.Attributes,
                RunCount = this.RunCount,
                FolderIcon = this.FolderIcon,
                FolderType = this.FolderType,
                DateRun = this.DateRun
            };
        }
    }
    public struct EverythingStatus
    {
        public string StatusMessage { get; set; }
        public Enum EverythingStatusCode { get; set; }
    }
    public enum EverythingStatusCode
    {
        Success,
        Error,
        NotFound,
        NoResults,
        Unknown
    }
}
