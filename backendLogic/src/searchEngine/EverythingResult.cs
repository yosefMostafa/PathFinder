
using backendLogic.src.searchEngine;

namespace backendLogic.src.searchEngine
{

    public struct EverythingResult
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string pathSize { get; set; } // This is a string representation of the path size, not the actual size
        public ulong Size { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateAccessed { get; set; }
        public FileAttributes Attributes { get; set; }
        public int RunCount { get; set; }
        public DateTime? DateRun { get; set; }

        public string FullPath => System.IO.Path.Combine(Path ?? "", FileName ?? "");
        public string toString()
        {
            return $"{FileName} ({Path}) - Size: {Size} bytes, Created: {DateCreated}, Modified: {DateModified}, Accessed: {DateAccessed}, Attributes: {Attributes}, Run Count: {RunCount}, Last Run: {DateRun}";
        }
        public void Print()
        {
            //print each property of the struct
            Console.WriteLine($"FileName: {FileName}");
            Console.WriteLine($"Path: {Path}");
            Console.WriteLine($"Size: {Size}");
            Console.WriteLine($"pathSize: {pathSize}");
            Console.WriteLine($"DateCreated: {DateCreated}");
            Console.WriteLine($"DateModified: {DateModified}");
            Console.WriteLine($"DateAccessed: {DateAccessed}");
            Console.WriteLine($"Attributes: {Attributes}");
            Console.WriteLine($"RunCount: {RunCount}");
            Console.WriteLine($"DateRun: {DateRun}");
            Console.WriteLine($"FullPath: {FullPath}");
            Console.WriteLine("-------------------");


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
