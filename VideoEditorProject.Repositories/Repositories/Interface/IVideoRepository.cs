using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoEditorProject.Repositories.Repositories.Interface
{
    public interface IVideoRepository
    {
        string SaveVideo(string sourcePath, string destinationFolder);
        string LoadVideo(string filePath);
        bool DeleteVideo(string filePath);
        List<string> GetAllVideos(string directoryPath);

    }
}
