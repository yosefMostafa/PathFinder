using ProjectFinder.Models;

namespace ProjectFinder.Services
{
    public class FileOperationService
    {
        private List<FileItem> _clipboard = new();
        private bool _isCutOperation = false;

        public async Task<bool> CopyFilesAsync(IEnumerable<FileItem> files)
        {
            try
            {
                _clipboard = files.ToList();
                _isCutOperation = false;
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CutFilesAsync(IEnumerable<FileItem> files)
        {
            try
            {
                _clipboard = files.ToList();
                _isCutOperation = true;
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> PasteFilesAsync(string destinationPath)
        {
            if (!_clipboard.Any())
                return false;

            try
            {
                foreach (var file in _clipboard)
                {
                    var fileName = Path.GetFileName(file.FullPath);
                    var destinationFile = Path.Combine(destinationPath, fileName);

                    if (file.IsDirectory)
                    {
                        if (_isCutOperation)
                        {
                            Directory.Move(file.FullPath, destinationFile);
                        }
                        else
                        {
                            await CopyDirectoryAsync(file.FullPath, destinationFile);
                        }
                    }
                    else
                    {
                        if (_isCutOperation)
                        {
                            File.Move(file.FullPath, destinationFile);
                        }
                        else
                        {
                            File.Copy(file.FullPath, destinationFile, true);
                        }
                    }
                }

                if (_isCutOperation)
                {
                    _clipboard.Clear();
                    _isCutOperation = false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteFilesAsync(IEnumerable<FileItem> files)
        {
            try
            {
                foreach (var file in files)
                {
                    if (file.IsDirectory)
                    {
                        Directory.Delete(file.FullPath, true);
                    }
                    else
                    {
                        File.Delete(file.FullPath);
                    }
                }
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RenameFileAsync(FileItem file, string newName)
        {
            try
            {
                var directory = Path.GetDirectoryName(file.FullPath);
                var newPath = Path.Combine(directory!, newName);

                if (file.IsDirectory)
                {
                    Directory.Move(file.FullPath, newPath);
                }
                else
                {
                    File.Move(file.FullPath, newPath);
                }

                file.Name = newName;
                file.FullPath = newPath;
                file.Path = newPath;

                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> OpenFileAsync(FileItem file)
        {
            try
            {
                if (file.IsDirectory)
                {
                    // For directories, we would typically navigate to them
                    // This is a placeholder for directory navigation
                    return await Task.FromResult(true);
                }
                else
                {
                    // For files, we would open them with the default application
                    // This is platform-specific and would need proper implementation
                    return await Task.FromResult(true);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool HasClipboardContent => _clipboard.Any();
        public bool IsClipboardCutOperation => _isCutOperation;
        public IReadOnlyList<FileItem> ClipboardContent => _clipboard.AsReadOnly();

        private async Task CopyDirectoryAsync(string sourceDir, string destinationDir)
        {
            await Task.Run(() =>
            {
                Directory.CreateDirectory(destinationDir);

                foreach (var file in Directory.GetFiles(sourceDir))
                {
                    var fileName = Path.GetFileName(file);
                    var destFile = Path.Combine(destinationDir, fileName);
                    File.Copy(file, destFile, true);
                }

                foreach (var dir in Directory.GetDirectories(sourceDir))
                {
                    var dirName = Path.GetFileName(dir);
                    var destDir = Path.Combine(destinationDir, dirName);
                    CopyDirectoryAsync(dir, destDir).Wait();
                }
            });
        }
    }
}

