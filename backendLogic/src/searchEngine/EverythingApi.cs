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
        public static extern bool Everything_IsFileResult( int nIndex);

        [DllImport("Everything64.dll")]
        public static extern int Everything_GetNumResults();

        [DllImport("Everything64.dll")]
        public static extern IntPtr Everything_GetResultFileName(int nIndex);

        [DllImport("Everything64.dll")]
        public static extern IntPtr Everything_GetResultPath(int nIndex);

        [DllImport("Everything64.dll")]
        public static extern int Everything_GetLastError();
        [DllImport("Everything64.dll")]
        public static extern bool Everything_GetResultSize(int index,out ulong size);
        [DllImport("Everything64.dll")]
        public static extern bool Everything_GetResultDateCreated(int index,out long dateCreated);
        [DllImport("Everything64.dll")]
        public static extern bool Everything_GetResultDateModified(int index,out long dateModified);
        [DllImport("Everything64.dll")]
        public static extern bool Everything_GetResultDateAccessed(int index,out long dateAccessed);
        [DllImport("Everything64.dll")]
        public static extern uint Everything_GetResultAttributes(int index);
        [DllImport("Everything64.dll")]
        public static extern uint Everything_GetResultRunCount(int index);
        [DllImport("Everything64.dll")]
        public static extern bool Everything_GetResultDateRun(int index,out long dateRun);

        public async Task EnsureIntialized()
        {
            bool ready = false;
            int maxAttempts = 100;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {


                try
                {
                    Everything_SetSearch("xxx");
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

        public EverythingStatus search(string searchString)
        {
            Everything_SetSearch(searchString); // 🔍 search term
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

                return everything_error_handler();

            }

            return new EverythingStatus
            {
                StatusMessage = "Search completed successfully.",
                EverythingStatusCode = EverythingStatusCode.Success
            };
        }
        private EverythingStatus everything_error_handler()
        {
            int errorCode = Everything_GetLastError();
            Console.WriteLine($"Error querying Everything. Error code: {errorCode}");
            switch (errorCode)
            {
                case 1:
                    return new EverythingStatus
                    {
                        StatusMessage = "CreateProcess failed. Ensure Everything is installed and the path is correct.",
                        EverythingStatusCode = EverythingStatusCode.Error
                    };

                case 2:
                    return new EverythingStatus
                    {
                        StatusMessage = "IPC failed — check if Everything is running.",
                        EverythingStatusCode = EverythingStatusCode.Error
                    };

                case 3:
                    return new EverythingStatus
                    {
                        StatusMessage = "Invalid index.",
                        EverythingStatusCode = EverythingStatusCode.Error
                    };

                case 4:
                    return new EverythingStatus
                    {
                        StatusMessage = "Invalid request flags.",
                        EverythingStatusCode = EverythingStatusCode.Error
                    };
                case 5:
                    return new EverythingStatus
                    {
                        StatusMessage = "Invalid sort type.",
                        EverythingStatusCode = EverythingStatusCode.Error
                    };
                default:
                    return new EverythingStatus
                    {
                        StatusMessage = $"Unknown error occurred. Error code: {errorCode}",
                        EverythingStatusCode = EverythingStatusCode.Error
                    };

            }
        }
    }

}
