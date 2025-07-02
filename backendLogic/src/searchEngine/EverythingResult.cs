
namespace backendLogic.src.searchEngine
{

    public struct EverythingResult
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateAccessed { get; set; }
        public FileAttributes Attributes { get; set; }
        public int RunCount { get; set; }
        public DateTime? DateRun { get; set; }

        public string FullPath => System.IO.Path.Combine(Path ?? "", FileName ?? "");
    }
}
