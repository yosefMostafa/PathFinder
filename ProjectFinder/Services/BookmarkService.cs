using System.Text.Json;

namespace ProjectFinder.Services
{
    public class BookmarkService
    {
        private readonly string _bookmarksFilePath;
        private List<Bookmark> _bookmarks = new();

        public BookmarkService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "EverythingApp");
            Directory.CreateDirectory(appFolder);
            _bookmarksFilePath = Path.Combine(appFolder, "bookmarks.json");
            LoadBookmarks();
        }

        public IReadOnlyList<Bookmark> Bookmarks => _bookmarks.AsReadOnly();

        public async Task<bool> AddBookmarkAsync(string name, string path)
        {
            try
            {
                var bookmark = new Bookmark
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Path = path,
                    CreatedDate = DateTime.Now
                };

                _bookmarks.Add(bookmark);
                await SaveBookmarksAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveBookmarkAsync(Guid bookmarkId)
        {
            try
            {
                var bookmark = _bookmarks.FirstOrDefault(b => b.Id == bookmarkId);
                if (bookmark != null)
                {
                    _bookmarks.Remove(bookmark);
                    await SaveBookmarksAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateBookmarkAsync(Guid bookmarkId, string name, string path)
        {
            try
            {
                var bookmark = _bookmarks.FirstOrDefault(b => b.Id == bookmarkId);
                if (bookmark != null)
                {
                    bookmark.Name = name;
                    bookmark.Path = path;
                    await SaveBookmarksAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void LoadBookmarks()
        {
            try
            {
                if (File.Exists(_bookmarksFilePath))
                {
                    var json = File.ReadAllText(_bookmarksFilePath);
                    _bookmarks = JsonSerializer.Deserialize<List<Bookmark>>(json) ?? new List<Bookmark>();
                }
                else
                {
                    // Add some default bookmarks
                    _bookmarks = new List<Bookmark>
                    {
                        new() { Id = Guid.NewGuid(), Name = "Desktop", Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop), CreatedDate = DateTime.Now },
                        new() { Id = Guid.NewGuid(), Name = "Documents", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), CreatedDate = DateTime.Now },
                        new() { Id = Guid.NewGuid(), Name = "Downloads", Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"), CreatedDate = DateTime.Now }
                    };
                }
            }
            catch (Exception)
            {
                _bookmarks = new List<Bookmark>();
            }
        }

        private async Task SaveBookmarksAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_bookmarks, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_bookmarksFilePath, json);
            }
            catch (Exception)
            {
                // Handle save error silently
            }
        }
    }

    public class Bookmark
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}

