using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoEditorProject.Services.Services
{
    public class VideoWatermarkService
    {
        private readonly string _ffmpegPath;

        public VideoWatermarkService()
        {
            // Đặt đường dẫn đến ffmpeg.exe (đảm bảo FFmpeg đã được tải về)
            _ffmpegPath = "ffmpeg";  // Nếu đã thêm vào PATH, chỉ cần "ffmpeg"
        }

        public async Task<string> AddTextWatermarkAsync(string inputVideoPath, string outputVideoPath, string watermarkText, int x, int y)
        {
            if (!File.Exists(inputVideoPath))
                throw new FileNotFoundException("Không tìm thấy file video!", inputVideoPath);

            string ffmpegArgs = $"-i \"{inputVideoPath}\" -vf \"drawtext=text='{watermarkText}':x={x}:y={y}:fontsize=24:fontcolor=white\" -codec:a copy \"{outputVideoPath}\"";

            return await RunFFmpegAsync(ffmpegArgs, outputVideoPath);
        }

        public async Task<string> AddImageWatermarkAsync(string inputVideoPath, string outputVideoPath, string watermarkImagePath, int x, int y)
        {
            if (!File.Exists(inputVideoPath) || !File.Exists(watermarkImagePath))
                throw new FileNotFoundException("Không tìm thấy file!");

            string ffmpegArgs = $"-i \"{inputVideoPath}\" -i \"{watermarkImagePath}\" -filter_complex \"overlay={x}:{y}\" -codec:a copy \"{outputVideoPath}\"";

            return await RunFFmpegAsync(ffmpegArgs, outputVideoPath);
        }

        private async Task<string> RunFFmpegAsync(string args, string outputPath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            if (!File.Exists(outputPath))
                throw new Exception("FFmpeg xử lý lỗi hoặc file output không tồn tại!");

            return outputPath;
        }
    }
}
