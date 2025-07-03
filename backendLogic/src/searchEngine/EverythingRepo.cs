using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace backendLogic.src.searchEngine
{
    public class EverythingRepo
    {
        private List<EverythingResult> _searchResults = new List<EverythingResult>();
        private readonly EverythingApi _everythingApi;

        public EverythingRepo()
        {
            _everythingApi = new EverythingApi();
        }

        public async Task SetSearch(string searchString)
        {
            await _everythingApi.EnsureIntialized();
            EverythingStatus serachStatus = _everythingApi.search(searchString);
            if ((EverythingStatusCode)serachStatus.EverythingStatusCode != EverythingStatusCode.Success)
            {
                Console.WriteLine($"Search failed: {serachStatus.StatusMessage}");
                return;
            }
            int numResults = EverythingApi.Everything_GetNumResults();
            for (int i = 0; i < numResults; i++)
            {
                IntPtr fileNamePtr = EverythingApi.Everything_GetResultFileName(i);
                IntPtr filePathPtr = EverythingApi.Everything_GetResultPath(i);
                if (fileNamePtr == IntPtr.Zero || filePathPtr == IntPtr.Zero)
                {
                    Console.WriteLine("Null pointer for filename or path.");
                    continue;
                }
                string fileName = fileNamePtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(fileNamePtr) ?? string.Empty : string.Empty;
                if (!fileName.Equals(searchString, StringComparison.OrdinalIgnoreCase))
                    continue;
                _searchResults.Add(GetResult(i));
            }
        }
        public string GetFolderSize(string folderPath)
        {
           Console.WriteLine($"Calculating size for folder: {folderPath}");
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            string everythingSearchString = folderPath + "**";
            EverythingStatus statusCode = _everythingApi.search(everythingSearchString);
            if ((EverythingStatusCode)statusCode.EverythingStatusCode != EverythingStatusCode.Success)
            {
                Console.WriteLine($"Search failed: {statusCode.StatusMessage}");
                return "0 Bytes";
            }
            int numResults = EverythingApi.Everything_GetNumResults();
            long totalSize = 0;
            ulong fileSize = 0;
            for (int i = 0; i < numResults; i++)
            {
                // Only sum sizes of files, not folders themselves
                if (EverythingApi.Everything_IsFileResult(i))
                {
                    if (EverythingApi.Everything_GetResultSize(i, out fileSize))
                    {
                        totalSize += (long)fileSize; // Cast ulong to long
                    }
                    // else: Handle error getting file size (unlikely if IsFile returned true)
                }

                // Optional: You can get the full path for debugging/logging
                // StringBuilder sb = new StringBuilder(260); // Max path length
                // EverythingApi.Everything_GetResultFullPathNameW(i, sb, 260);
                // Console.WriteLine($"  Result {i}: {sb.ToString()} - IsFile: {EverythingApi.Everything_IsFile(i)} - Size: {fileSize}");
            }
            return FormatBytes(totalSize);

        }
        public static string FormatBytes(long bytes)
        {
            string[] Suffix = { "Bytes", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }
            return $"{dblSByte:0.##} {Suffix[i]}";
        }
        public List<EverythingResult> GetSearchResults()
        {
            return new List<EverythingResult>(_searchResults);
        }
        private EverythingResult GetResult(int index)
        {
            string fileName = EverythingApi.Everything_GetResultFileName(index) != IntPtr.Zero ? Marshal.PtrToStringAnsi(EverythingApi.Everything_GetResultFileName(index)) ?? string.Empty : string.Empty;
            string pathName = EverythingApi.Everything_GetResultPath(index) != IntPtr.Zero ? Marshal.PtrToStringAnsi(EverythingApi.Everything_GetResultPath(index)) ?? string.Empty : string.Empty;
            ulong size = 0;
            long dateCreated = 0;
            long dateModified = 0;
            long dateAccessed = 0;
            long dateRun = 0;
            if (!EverythingApi.Everything_GetResultSize(index, out size))
            {
                Console.WriteLine($"Failed to get size for index {index}: {EverythingApi.Everything_GetLastError()}");
            }

            if (!EverythingApi.Everything_GetResultDateCreated(index, out dateCreated))
            {
                Console.WriteLine($"Failed to get date created for index {index}: {EverythingApi.Everything_GetLastError()}");
            }
            if (!EverythingApi.Everything_GetResultDateModified(index, out dateModified))
            {
                Console.WriteLine($"Failed to get date modified for index {index}: {EverythingApi.Everything_GetLastError()}");
            }
            if (!EverythingApi.Everything_GetResultDateAccessed(index, out dateAccessed))
            {
                Console.WriteLine($"Failed to get date accessed for index {index}: {EverythingApi.Everything_GetLastError()}");
            }

            //convert size to kilobytes


            if (!EverythingApi.Everything_GetResultDateRun(index, out dateRun))
            {
                Console.WriteLine($"Failed to get date run for index {index}: {EverythingApi.Everything_GetLastError()}");
            }
            return new EverythingResult
            {
                FileName = fileName,
                Path = pathName,
                Size = size,
                pathSize = "0 Bytes", // Placeholder, can be updated later
                DateCreated = DateTime.FromFileTimeUtc(dateCreated),
                DateModified = DateTime.FromFileTimeUtc(dateModified),
                DateAccessed = DateTime.FromFileTimeUtc(dateAccessed),
                Attributes = (FileAttributes)EverythingApi.Everything_GetResultAttributes(index),
                RunCount = (int)EverythingApi.Everything_GetResultRunCount(index),
                DateRun = DateTime.FromFileTimeUtc(dateRun < 0 ? 0 : dateRun),
            };
        }


    }
}
