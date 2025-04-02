using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoEditorProject.Services.Interface
{
    public interface IVideoService
    {
        void CutVideo(string inputPath, string outputPath, TimeSpan start, TimeSpan duration);
        void MergeVideos(string[] inputPaths, string outputPath);
        void ApplyEffect(string inputPath, string outputPath, string effect);
        void ReverseVideo(string inputPath, string outputPath);
        void SlowMotion(string inputPath, string outputPath, double speedFactor);
    }

}
