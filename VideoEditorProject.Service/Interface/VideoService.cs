using System;
using System.Diagnostics;
using System.IO;
using VideoEditorProject.Repositories.Repositories.Implement;

namespace VideoEditorProject.Services.Services
{
    public class VideoService
    {
        private readonly VideoRepository _videoRepository;

        // Constructor không cần nhận vào VideoRepository nữa
        public VideoService()
        {
            _videoRepository = new VideoRepository(); // Khởi tạo VideoRepository trong constructor
        }

        public void CutVideo(string inputPath, string outputPath, TimeSpan start, TimeSpan duration)
        {
            // Sử dụng phương thức của VideoRepository để lưu video
            string videoPath = _videoRepository.SaveVideo(inputPath, outputPath);
            var args = $"-i \"{videoPath}\" -ss {start} -t {duration} -c copy \"{outputPath}\"";
            RunFFmpeg(args);
        }

        public void MergeVideos(string[] inputPaths, string outputPath)
        {
            string listFile = "videos.txt";
            File.WriteAllLines(listFile, inputPaths);

            var args = $"-f concat -safe 0 -i \"{listFile}\" -c copy \"{outputPath}\"";
            RunFFmpeg(args);
        }

        public void ApplyEffect(string inputPath, string outputPath, string effect)
        {
            string args = effect switch
            {
                "gray" => $"-i \"{inputPath}\" -vf format=gray \"{outputPath}\"",
                "blur" => $"-i \"{inputPath}\" -vf \"boxblur=5\" \"{outputPath}\"",
                "reverse" => $"-i \"{inputPath}\" -vf reverse -af areverse \"{outputPath}\"",
                "slow" => $"-i \"{inputPath}\" -filter:v \"setpts=2.0*PTS\" \"{outputPath}\"",
                _ => throw new Exception("Hiệu ứng không hợp lệ!")
            };

            RunFFmpeg(args);
        }

        public void ReverseVideo(string inputPath, string outputPath)
        {
            string args = $"-i \"{inputPath}\" -vf reverse -af areverse \"{outputPath}\"";
            RunFFmpeg(args);
        }

        public void SlowMotion(string inputPath, string outputPath, double speedFactor)
        {
            string args = $"-i \"{inputPath}\" -filter:v \"setpts={speedFactor}*PTS\" \"{outputPath}\"";
            RunFFmpeg(args);
        }

        public void RunFFmpeg(string arguments)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();

                string error = process.StandardError.ReadToEnd();
                Console.WriteLine("FFmpeg Error: " + error);

                process.WaitForExit();
                Console.WriteLine("FFmpeg Done!");
            }
        }
    }
}
