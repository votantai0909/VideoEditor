using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using VideoEditorProject.Services.Interface;
using VideoEditorProject.Services.Services;
using Microsoft.Win32;

namespace VideoEditorProjectWPF
{
    public partial class VideoEffectWindow : Window
    {
        private string _originalVideoPath;
        private string _tempVideoPath;
        private readonly VideoService _videoService;
        private List<string> _effects = new List<string>();
        private DispatcherTimer _timer;

        public VideoEffectWindow(string videoPath)
        {
            InitializeComponent();

            var videoService = new VideoService();
            _videoService = videoService;
            _originalVideoPath = videoPath;

            // Tạo thư mục tạm nếu chưa tồn tại
            string tempDirectory = Path.Combine(Environment.CurrentDirectory, "TempVideos");
            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }

            // Cập nhật lại _tempVideoPath khi chuyển qua video mới
            string uniqueIdentifier = Guid.NewGuid().ToString();
            _tempVideoPath = Path.Combine(tempDirectory, $"temp_video_{uniqueIdentifier}.mp4");

            if (File.Exists(_originalVideoPath))
            {
                try
                {
                    File.Copy(_originalVideoPath, _tempVideoPath, true); // Sao chép video gốc vào video tạm mới
                    PlayVideo(_tempVideoPath);  // Phát video tạm
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi sao chép video: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Video gốc không tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            InitializeTimer();
        }



        private void InitializeTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _timer.Tick += (s, e) =>
            {
                if (mediaPlayer.NaturalDuration.HasTimeSpan)
                    VideoSlider.Value = mediaPlayer.Position.TotalSeconds;
            };
            _timer.Start();
        }

        private void PlayVideo(string videoPath)
    {
        mediaPlayer.Source = new Uri(videoPath);
        mediaPlayer.Play();
    }

        private void ApplyEffect(string effectType)
        {
            mediaPlayer.Pause();

            string uniqueIdentifier = Guid.NewGuid().ToString();
            string newTempPath = Path.Combine(Path.GetDirectoryName(_tempVideoPath), $"temp_{effectType}_{uniqueIdentifier}.mp4");

            Window loadingWindow = CreateLoadingWindow();
            loadingWindow.Show();

            try
            {
                // Sử dụng Task.Run để thực hiện tác vụ trong nền
                Task.Run(() =>
                {
                    try
                    {
                        switch (effectType)
                        {
                            case "gray":
                                _videoService.ApplyEffect(_tempVideoPath, newTempPath, "gray");
                                break;
                            case "reverse":
                                _videoService.ReverseVideo(_tempVideoPath, newTempPath);
                                break;
                            case "slowmotion":
                                _videoService.SlowMotion(_tempVideoPath, newTempPath, 2.0);
                                break;
                            case "cut":
                                _videoService.CutVideo(_tempVideoPath, newTempPath, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20));
                                break;
                        }

                        // Đóng cửa sổ loading trong luồng UI
                        loadingWindow.Dispatcher.Invoke(() => loadingWindow.Close());

                        // Kiểm tra nếu video tạm mới tồn tại
                        if (File.Exists(newTempPath))
                        {
                            _tempVideoPath = newTempPath;
                            _effects.Add(effectType);
                            Dispatcher.Invoke(() => UpdateVideoPlayer()); // Gọi UpdateVideoPlayer trong luồng UI
                        }
                        else
                        {
                            Dispatcher.Invoke(() => MessageBox.Show("Lỗi áp dụng hiệu ứng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Đảm bảo thông báo lỗi trong luồng UI
                        loadingWindow.Dispatcher.Invoke(() => loadingWindow.Close());
                        Dispatcher.Invoke(() => MessageBox.Show($"Có lỗi trong quá trình áp dụng hiệu ứng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error));
                    }
                });
            }
            catch (Exception ex)
            {
                loadingWindow.Dispatcher.Invoke(() => loadingWindow.Close());
                MessageBox.Show($"Có lỗi trong quá trình áp dụng hiệu ứng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void BtnGray_Click(object sender, RoutedEventArgs e) => ApplyEffect("gray");
        private void BtnReverse_Click(object sender, RoutedEventArgs e) => ApplyEffect("reverse");
        private void BtnSlowMotion_Click(object sender, RoutedEventArgs e) => ApplyEffect("slowmotion");
        private void BtnCut_Click(object sender, RoutedEventArgs e) => ApplyEffect("cut");

        private void UpdateVideoPlayer()
        {
            // Dừng và đóng mediaPlayer trước khi cập nhật nguồn mới
            mediaPlayer.Stop();
            mediaPlayer.Close();  // Đóng mediaPlayer để giải phóng tài nguyên

            // Cập nhật video với hiệu ứng mới
            mediaPlayer.Source = new Uri(_tempVideoPath);

            // Phát lại video mới sau khi cập nhật nguồn
            mediaPlayer.Play();
        }



        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            if (_effects.Count == 0)
            {
                this.Tag = _originalVideoPath;
                this.DialogResult = true;
                this.Close();
                return;
            }

            MessageBoxResult result = MessageBox.Show("Bạn có muốn lưu chỉnh sửa không?", "Xác nhận", MessageBoxButton.YesNoCancel);

            if (result == MessageBoxResult.Yes)
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "MP4 Video|*.mp4",
                    Title = "Chọn nơi lưu video",
                    FileName = "edited_video.mp4"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string savedVideoPath = saveFileDialog.FileName;

                    try
                    {
                        // Dừng video trước khi lưu
                        mediaPlayer.Stop();

                        // Đảm bảo tài nguyên đã được giải phóng
                        mediaPlayer.Close();

                        // Đợi một chút để hệ thống giải phóng tài nguyên (nếu cần)
                        Task.Delay(500).Wait();

                        // Di chuyển tệp video đã chỉnh sửa
                        File.Move(_tempVideoPath, savedVideoPath, true);
                        this.Tag = savedVideoPath;
                        MessageBox.Show("Lưu video thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi lưu video: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Bạn chưa chọn nơi lưu video!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                File.Delete(_tempVideoPath);
                this.Tag = _originalVideoPath;
            }

            this.DialogResult = true;
            this.Close();
        }


        private void VideoSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                // Nếu giá trị của slider thay đổi, cập nhật video
                mediaPlayer.Position = TimeSpan.FromSeconds(VideoSlider.Value);
            }
        }

        private Window CreateLoadingWindow()
        {
            Window loadingWindow = new Window
            {
                Title = "Đang xử lý...",
                Width = 300,
                Height = 100,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Topmost = true,
                ShowInTaskbar = false,
                Content = new System.Windows.Controls.Label
                {
                    Content = "Đang áp dụng hiệu ứng, vui lòng chờ...",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14
                }
            };
            return loadingWindow;
        }
    }
}
