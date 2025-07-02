using System.Runtime.InteropServices;
using backendLogic.src.services.Everything;
namespace backendLogic.src.searchEngine
{
    public class EverythingApi
    {

        const int EVERYTHING_OK = 0;
        const int EVERYTHING_SORT_NAME_ASCENDING = 1;
        const int EVERYTHING_REQUEST_FILE_NAME = 0x00000001;
        const int EVERYTHING_REQUEST_PATH = 0x00000002;
        const int EVERYTHING_REQUEST_FULL_PATH_AND_FILE_NAME = 0x00000004;
        const int EVERYTHING_REQUEST_EXTENSION = 0x00000008;
        const int EVERYTHING_REQUEST_SIZE = 0x00000010;
        const int EVERYTHING_REQUEST_DATE_CREATED = 0x00000020;
        const int EVERYTHING_REQUEST_DATE_MODIFIED = 0x00000040;
        const int EVERYTHING_REQUEST_DATE_ACCESSED = 0x00000080;
        const int EVERYTHING_REQUEST_ATTRIBUTES = 0x00000100;
        const int EVERYTHING_REQUEST_FILE_LIST_FILE_NAME = 0x00000200;
        const int EVERYTHING_REQUEST_RUN_COUNT = 0x00000400;
        const int EVERYTHING_REQUEST_DATE_RUN = 0x00000800;
        const int EVERYTHING_REQUEST_DATE_RECENTLY_CHANGED = 0x00001000;
        const int EVERYTHING_REQUEST_HIGHLIGHTED_FILE_NAME = 0x00002000;
        const int EVERYTHING_REQUEST_HIGHLIGHTED_PATH = 0x00004000;
        const int EVERYTHING_REQUEST_HIGHLIGHTED_FULL_PATH_AND_FILE_NAME = 0x00008000;
        public EverythingApi()
        {
            // Constructor logic if needed
            EverythingService.Run_EverythingProcess();
        }
        [DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
        public static extern void Everything_SetSearch(string lpSearchString);

        [DllImport("Everything64.dll")]
        public static extern void Everything_SetRequestFlags(uint dwRequestFlags);

        [DllImport("Everything64.dll")]
        public static extern void Everything_SetSort(uint dwSortType);

        [DllImport("Everything64.dll")]
        public static extern bool Everything_Query(bool bWait);

        [DllImport("Everything64.dll")]
        public static extern int Everything_GetNumResults();

        [DllImport("Everything64.dll")]
        public static extern IntPtr Everything_GetResultFileName(int nIndex);

        [DllImport("Everything64.dll")]
        public static extern IntPtr Everything_GetResultPath(int nIndex);

        [DllImport("Everything64.dll")]
        public static extern int Everything_GetLastError();
        [DllImport("Everything64.dll")] public static extern long Everything_GetResultSize(int index);
        [DllImport("Everything64.dll")] public static extern long Everything_GetResultDateCreated(int index);
        [DllImport("Everything64.dll")] public static extern long Everything_GetResultDateModified(int index);
        [DllImport("Everything64.dll")] public static extern long Everything_GetResultDateAccessed(int index);
        [DllImport("Everything64.dll")] public static extern uint Everything_GetResultAttributes(int index);
        [DllImport("Everything64.dll")] public static extern uint Everything_GetResultRunCount(int index);
        [DllImport("Everything64.dll")] public static extern long Everything_GetResultDateRun(int index);

        public async Task EnsureIntialized()
        {
            bool ready = false;
            int maxAttempts = 100;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {


                try
                {
                    Everything_SetSearch("xxx");
                    Everything_SetRequestFlags(EVERYTHING_REQUEST_FILE_NAME);
                    Everything_SetSort(EVERYTHING_SORT_NAME_ASCENDING);

                    if (Everything_Query(true))
                    {
                        ready = true;
                        Console.WriteLine("Everything is ready.");
                        break;
                    }
                }
                catch
                {
                    // Ignore and retry
                }
                await Task.Delay(500); // Wait half a second between attempts
            }

            if (!ready)
            {
                Console.WriteLine("❌ Timeout: Everything did not become ready for IPC.");
                return;
            }
        }

        public void Main(string serachableString = "node_modules")
        {
            Everything_SetSearch(serachableString); // 🔍 search term
            Everything_SetRequestFlags(
                EVERYTHING_REQUEST_FILE_NAME |
                EVERYTHING_REQUEST_PATH |
                EVERYTHING_REQUEST_SIZE |
                EVERYTHING_REQUEST_DATE_CREATED |
                EVERYTHING_REQUEST_DATE_MODIFIED |
                EVERYTHING_REQUEST_DATE_ACCESSED |
                EVERYTHING_REQUEST_ATTRIBUTES |
                EVERYTHING_REQUEST_RUN_COUNT |
                EVERYTHING_REQUEST_DATE_RUN
            );
            Everything_SetSort(EVERYTHING_SORT_NAME_ASCENDING);

            if (!Everything_Query(true))
            {

                everything_error_handler();
                return;
            }

            int numResults = Everything_GetNumResults();
            var topLevelProjects = new HashSet<string>();
            for (int i = 0; i < numResults; i++)
            {
                IntPtr fileNamePtr = Everything_GetResultFileName(i);
                IntPtr filePathPtr = Everything_GetResultPath(i);
                if (fileNamePtr == IntPtr.Zero || filePathPtr == IntPtr.Zero)
                {
                    Console.WriteLine("Null pointer for filename or path.");
                    continue;
                }
                string fileName = fileNamePtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(fileNamePtr) ?? string.Empty : string.Empty;
                string filePath = filePathPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(filePathPtr) ?? string.Empty : string.Empty;
                string fullPath = System.IO.Path.Combine(filePath, fileName);
                if (!fileName.Equals(serachableString, StringComparison.OrdinalIgnoreCase))
                    continue;

                // Ignore deeply nested node_modules
                if (fullPath.Split(Path.DirectorySeparatorChar).Count(part => part == serachableString) > 1)
                    continue;
                var parentDirectory = Directory.GetParent(fullPath);
                if (parentDirectory != null)
                {
                    string projectFolder = parentDirectory.FullName;
                    if (!string.IsNullOrEmpty(projectFolder))
                    {
                        topLevelProjects.Add(projectFolder);
                    }
                }
            }
            Console.WriteLine("Top-level Node.js project folders:");
            Console.WriteLine("Number of results: " + topLevelProjects.Count);
            foreach (var folder in topLevelProjects)
            {
                Console.WriteLine(folder);
            }

        }
        private void everything_error_handler()
        {
            int errorCode = Everything_GetLastError();
            Console.WriteLine($"Error querying Everything. Error code: {errorCode}");
            switch (errorCode)
            {
                case 1:
                    Console.WriteLine("CreateProcess failed.");
                    break;
                case 2:
                    Console.WriteLine("IPC failed — check if Everything is running.");
                    break;
                case 3:
                    Console.WriteLine("Invalid index.");
                    break;
                case 4:
                    Console.WriteLine("Invalid call order.");
                    break;
                case 5:
                    Console.WriteLine("Query too large.");
                    break;
                default:
                    Console.WriteLine("Unknown error.");
                    break;
            }
            return;
        }
    }

}
