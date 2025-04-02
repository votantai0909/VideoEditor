using System;
using System.Collections.Generic;
using System.IO;

namespace VideoEditorProject.Repositories.Repositories.Implement
{
    public class VideoRepository
    {
        public string SaveVideo(string sourcePath, string destinationFolder)
        {
            try
            {
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                string fileName = Path.GetFileName(sourcePath);
                string destinationPath = Path.Combine(destinationFolder, fileName);

                File.Copy(sourcePath, destinationPath, true); // Ghi đè nếu file đã tồn tại
                return destinationPath;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lưu video: " + ex.Message);
            }
        }

        public string LoadVideo(string filePath)
        {
            if (File.Exists(filePath))
                return filePath;
            else
                throw new FileNotFoundException("Không tìm thấy video!");
        }

        public bool DeleteVideo(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa video: " + ex.Message);
            }
        }

        public List<string> GetAllVideos(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
                return new List<string>(Directory.GetFiles(directoryPath, "*.mp4"));
            else
                throw new DirectoryNotFoundException("Thư mục không tồn tại!");
        }
    }
}
