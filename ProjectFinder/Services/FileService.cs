using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using ProjectFinder.Models;

using backendLogic.src.searchEngine;
using System.Runtime.CompilerServices;
using backendLogic.src.searchEngine.models;
using System.Diagnostics;

namespace ProjectFinder.Services
{
    public class FileService
    {
        private readonly Engine _engine = new Engine(ProjectTypeEnum.PDF);
        private readonly List<string> _audioExtensions = new() { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a" };
        private readonly List<string> _videoExtensions = new() { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm" };
        private readonly List<string> _imageExtensions = new() { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".svg" };
        private readonly List<string> _documentExtensions = new() { ".pdf", ".doc", ".docx", ".txt", ".rtf", ".xls", ".xlsx", ".ppt", ".pptx" };

        public async Task<List<FileItem>> GetFilesAsync(string? rootPath = null)
        {
            List<EverythingResult> results;
            try
            {

                Debug.WriteLine("Before Run");

                await Task.Run(() => _engine.Run());
                Debug.WriteLine("After Run");

                results = _engine.GetRunResult();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                // If there's an error, return sample data
                return GetSampleData();
            }
            return results.Select(result => FileItem.FromEverythingResult(result)).ToList();

        }

        public async Task<List<FileItem>> SearchFilesAsync(string searchTerm, string filter = "All", bool useRegex = false)
        {
            return await Task.Run(() =>
            {
                var allFiles = GetFilesAsync().Result;
                var filteredFiles = new List<FileItem>();

                try
                {
                    foreach (var file in allFiles)
                    {
                        bool matchesSearch = false;
                        bool matchesFilter = true;

                        // Apply search filter
                        if (useRegex)
                        {
                            try
                            {
                                var regex = new Regex(searchTerm, RegexOptions.IgnoreCase);
                                matchesSearch = regex.IsMatch(file.Name) || regex.IsMatch(file.Path);
                            }
                            catch
                            {
                                // If regex is invalid, fall back to simple search
                                matchesSearch = file.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                              file.Path.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
                            }
                        }
                        else
                        {
                            matchesSearch = file.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                          file.Path.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
                        }

                        // Apply type filter
                        if (filter != "All")
                        {
                            var extension = System.IO.Path.GetExtension(file.Name).ToLowerInvariant();
                            matchesFilter = filter switch
                            {
                                "Audio" => _audioExtensions.Contains(extension),
                                "Video" => _videoExtensions.Contains(extension),
                                "Images" => _imageExtensions.Contains(extension),
                                "Documents" => _documentExtensions.Contains(extension),
                                _ => true
                            };
                        }

                        if (matchesSearch && matchesFilter)
                        {
                            filteredFiles.Add(file);
                        }
                    }
                }
                catch (Exception)
                {
                    // Return sample search results if there's an error
                    return GetSampleSearchData(searchTerm);
                }

                return filteredFiles.Take(1000).ToList(); // Limit search results
            });
        }

        private void ScanDirectory(string directoryPath, List<FileItem> files, int currentDepth = 0, int maxDepth = 3)
        {
            if (currentDepth > maxDepth) return;

            try
            {
                var directory = new DirectoryInfo(directoryPath);

                // Add directories
                foreach (var dir in directory.GetDirectories().Take(100)) // Limit for performance
                {
                    try
                    {
                        files.Add(CreateFileItem(dir));

                        // Recursively scan subdirectories
                        if (currentDepth < maxDepth)
                        {
                            ScanDirectory(dir.FullName, files, currentDepth + 1, maxDepth);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                // Add files
                foreach (var file in directory.GetFiles().Take(500)) // Limit for performance
                {
                    try
                    {
                        files.Add(CreateFileItem(file));
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            catch
            {
                // Skip directories we can't access
            }
        }

        private FileItem CreateFileItem(FileSystemInfo info)
        {
            var isDirectory = info is DirectoryInfo;
            var size = isDirectory ? "" : FormatFileSize(((FileInfo)info).Length);
            var icon = GetFileIcon(info.Name, isDirectory);

            return new FileItem
            {
                Name = info.Name,
                Path = info.FullName,
                FullPath = info.FullName,
                Size = size,
                DateModified = info.LastWriteTime.ToString("M/d/yyyy h:mm tt"),
                Icon = icon,
                IsDirectory = isDirectory,
                SizeInBytes = isDirectory ? 0 : ((FileInfo)info).Length,
                LastModified = info.LastWriteTime
            };
        }

        private string GetFileIcon(string fileName, bool isDirectory)
        {
            if (isDirectory)
                return "folder.png"; // You would need to add this resource

            var extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".mp3" or ".wav" or ".flac" or ".aac" or ".ogg" or ".wma" or ".m4a" => "audio.png",
                ".mp4" or ".avi" or ".mkv" or ".mov" or ".wmv" or ".flv" or ".webm" => "video.png",
                ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".tiff" or ".svg" => "image.png",
                ".pdf" or ".doc" or ".docx" or ".txt" or ".rtf" => "document.png",
                ".xls" or ".xlsx" => "spreadsheet.png",
                ".ppt" or ".pptx" => "presentation.png",
                ".exe" => "executable.png",
                _ => "file.png"
            };
        }

        private string FormatFileSize(long bytes)
        {
            if (bytes == 0) return "0 B";

            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }

        private List<FileItem> GetSampleData()
        {
            return new List<FileItem>
            {
                new() { Name = "tempo.wav", Path = @"H:\work\Fun\necromancer", Size = "31 kB", DateModified = "7/1/2025 7:15 AM", Icon = "audio.png" },
                new() { Name = "output.wav", Path = @"H:\work\Fun\necromancer", Size = "31 kB", DateModified = "7/1/2025 6:54 AM", Icon = "audio.png" },
                new() { Name = "دوغ جع اصطخت.mp3", Path = @"H:\work\Fun\necromancer", Size = "214 kB", DateModified = "6/30/2025 1:35 PM", Icon = "audio.png" },
                new() { Name = "Sheep laugh final.mp3", Path = @"H:\work\Fun\necromancer", Size = "49 kB", DateModified = "6/30/2025 1:35 PM", Icon = "audio.png" },
                new() { Name = "another-one-dj-khaled.mp3", Path = @"H:\work\Fun\necromancer", Size = "35 kB", DateModified = "6/30/2025 1:35 PM", Icon = "audio.png" },
                new() { Name = "tmpfw9l98z.wav", Path = @"C:\Users\user\AppData\Local\Temp", Size = "257 kB", DateModified = "6/30/2025 1:11 PM", Icon = "audio.png" },
                new() { Name = "tmpzfh3rlok.wav", Path = @"C:\Users\user\AppData\Local\Temp", Size = "203 kB", DateModified = "6/30/2025 1:11 PM", Icon = "audio.png" },
                new() { Name = "tmpfr2nm59f.wav", Path = @"C:\Users\user\AppData\Local\Temp", Size = "157 kB", DateModified = "6/30/2025 1:11 PM", Icon = "audio.png" },
                new() { Name = "tmpduq8ncs.wav", Path = @"C:\Users\user\AppData\Local\Temp", Size = "313 kB", DateModified = "6/30/2025 1:11 PM", Icon = "audio.png" },
                new() { Name = "tmpjh2ctbx.wav", Path = @"C:\Users\user\AppData\Local\Temp", Size = "605 kB", DateModified = "6/30/2025 1:11 PM", Icon = "audio.png" }
            };
        }

        private List<FileItem> GetSampleSearchData(string searchTerm)
        {
            var sampleData = GetSampleData();
            return sampleData.Where(f =>
                f.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                f.Path.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }
    }
}

