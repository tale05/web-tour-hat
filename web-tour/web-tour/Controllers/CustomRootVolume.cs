using elFinder.NetCore;

namespace web_tour.Controllers
{
    public class CustomRootVolume : RootVolume
    {
        public List<string> LockedFolders { get; set; } = new List<string>();

        public CustomRootVolume(string rootDirectory, string url, string urlThumb)
            : base(rootDirectory, url, urlThumb)
        {
        }

        public bool IsFolderLocked(string fullPath)
        {
            // Chuẩn hóa đường dẫn
            string normalizedFullPath = Path.GetFullPath(fullPath).TrimEnd(Path.DirectorySeparatorChar).ToLowerInvariant();

            foreach (var lockedFolder in LockedFolders)
            {
                // Tạo đường dẫn đầy đủ đến thư mục bị khóa
                string lockedFolderPath = Path.GetFullPath(Path.Combine(this.RootDirectory, lockedFolder))
                    .TrimEnd(Path.DirectorySeparatorChar).ToLowerInvariant();

                // So sánh đường dẫn
                if (normalizedFullPath.StartsWith(lockedFolderPath))
                {
                    return true; // Thư mục bị khóa
                }
            }

            return false; // Không bị khóa
        }
    }
}
