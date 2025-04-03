using System;
using System.Diagnostics;

namespace VideoEditorProject.Services.Services
{
    public class VideoService
    {
        private static string ffmpegPath = @"C:\Users\ASUS\Downloads\ffmpeg-master-latest-win64-gpl-shared\bin\ffmpeg.exe"; // Đảm bảo đường dẫn FFmpeg chính xác

        // Cắt video từ một đoạn
        public void CutVideo(string inputPath, string outputPath, TimeSpan start, TimeSpan duration)
        {
            string startTime = start.ToString(@"hh\:mm\:ss");
            string durationTime = duration.ToString(@"hh\:mm\:ss");

            // Cấu hình lệnh FFmpeg để cắt video
            string ffmpegArgs = $"-i \"{inputPath}\" -ss {startTime} -t {durationTime} -c:v libx264 -c:a aac -strict experimental \"{outputPath}\"";

            // Gọi FFmpeg qua dòng lệnh
            ExecuteFFmpegCommand(ffmpegArgs);
        }

        // Áp dụng hiệu ứng cho video
        public void ApplyEffect(string inputPath, string outputPath, string effect)
        {
            string filter = effect switch
            {
                "gray" => "hue=s=0", // Hiệu ứng grayscale
                "blur" => "gblur=sigma=5", // Hiệu ứng blur
                _ => throw new ArgumentException("Unsupported effect")
            };

            // Cấu hình lệnh FFmpeg để áp dụng hiệu ứng
            string ffmpegArgs = $"-i \"{inputPath}\" -vf \"{filter}\" -c:v libx264 -c:a aac \"{outputPath}\"";

            // Gọi FFmpeg qua dòng lệnh
            ExecuteFFmpegCommand(ffmpegArgs);
        }

        // Hiệu ứng Slow Motion
        public void SlowMotion(string inputPath, string outputPath, double speedFactor)
        {
            // Cấu hình filter slow-motion
            string filter = $"setpts={speedFactor}*PTS";

            // Cấu hình lệnh FFmpeg để áp dụng slow-motion
            string ffmpegArgs = $"-i \"{inputPath}\" -vf \"{filter}\" -c:v libx264 -c:a aac \"{outputPath}\"";

            // Gọi FFmpeg qua dòng lệnh
            ExecuteFFmpegCommand(ffmpegArgs);
        }

        // Hàm gọi FFmpeg qua dòng lệnh
        private void ExecuteFFmpegCommand(string args)
        {
            Process ffmpegProcess = new Process();
            ffmpegProcess.StartInfo.FileName = ffmpegPath; // Đảm bảo đường dẫn FFmpeg chính xác
            ffmpegProcess.StartInfo.Arguments = args;
            ffmpegProcess.StartInfo.RedirectStandardOutput = true;
            ffmpegProcess.StartInfo.RedirectStandardError = true;
            ffmpegProcess.StartInfo.UseShellExecute = false;
            ffmpegProcess.StartInfo.CreateNoWindow = true;

            // Đọc lỗi từ FFmpeg
            ffmpegProcess.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    Console.WriteLine("FFmpeg Error: " + e.Data);
                }
            };

            // Chạy tiến trình và đợi kết quả
            ffmpegProcess.Start();
            ffmpegProcess.BeginErrorReadLine();
            ffmpegProcess.WaitForExit();

            if (ffmpegProcess.ExitCode == 0)
            {
                Console.WriteLine("Operation successful!");
            }
            else
            {
                Console.WriteLine("Error during operation.");
            }
        }
        // Hàm tạo video reverse (tua ngược video)
        public void ReverseVideo(string inputPath, string outputPath)
        {
            // Cấu hình lệnh FFmpeg để tạo hiệu ứng reverse
            string ffmpegArgs = $"-i \"{inputPath}\" -vf reverse -af areverse -c:v libx264 -c:a aac \"{outputPath}\"";

            // Gọi FFmpeg qua dòng lệnh
            ExecuteFFmpegCommand(ffmpegArgs);
        }

    }
}
