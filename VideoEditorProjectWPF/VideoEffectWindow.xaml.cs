using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using VideoEditorProject.Services.Services;
using Path = System.IO.Path;

namespace VideoEditorProjectWPF
{
    /// <summary>
    /// Interaction logic for VideoEffectWindow.xaml
    /// </summary>
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

            Loaded += (s, e) =>
            {
                StartTimeSlider.ValueChanged += StartTimeSlider_ValueChanged;
                EndTimeSlider.ValueChanged += EndTimeSlider_ValueChanged;
                StartTimeSlider.PreviewMouseUp += StartTimeSlider_PreviewMouseUp; // Ensure video plays again after dragging
                EndTimeSlider.PreviewMouseUp += EndTimeSlider_PreviewMouseUp; // Ensure video plays again after dragging
            };

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
                    MessageBox.Show($"Lỗi khi sao chép video: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            InitializeTimer();
        }

        private void StartTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (StartTimeText != null)
            {
                // Cập nhật thời gian hiển thị trên TextBlock
                StartTimeText.Text = TimeSpan.FromSeconds(StartTimeSlider.Value).ToString(@"mm\:ss");
            }

            // Kiểm tra xem video có đang được phát hay không
            bool isVideoPlaying = mediaPlayer.CanPause && mediaPlayer.Position < mediaPlayer.NaturalDuration.TimeSpan;

            if (mediaPlayer != null && StartTimeSlider.IsMouseCaptured)
            {
                TimeSpan newPosision = TimeSpan.FromSeconds(StartTimeSlider.Value);
                // Cập nhật vị trí video theo thời gian bắt đầu mới mà không dừng video
                mediaPlayer.Position = newPosision;


            }
        }

        private void EndTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (EndTimeText != null)
            {
                EndTimeText.Text = TimeSpan.FromSeconds(EndTimeSlider.Value).ToString(@"mm\:ss");
            }
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _timer.Tick += (s, e) =>
            {
                if (mediaPlayer != null && mediaPlayer.NaturalDuration.HasTimeSpan)
                {
                    double totalSeconds = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;

                    // Cập nhật giá trị tối đa của slider
                    StartTimeSlider.Maximum = totalSeconds;
                    EndTimeSlider.Maximum = totalSeconds;

                    // Cập nhật slider theo video nếu người dùng chưa kéo slider
                    if (!StartTimeSlider.IsMouseCaptured)
                    {
                        // Đảm bảo giá trị slider đồng bộ với vị trí video
                        StartTimeSlider.Value = mediaPlayer.Position.TotalSeconds;
                    }

                    // Cập nhật TextBlock thời gian
                    if (StartTimeText != null)
                        StartTimeText.Text = TimeSpan.FromSeconds(StartTimeSlider.Value).ToString(@"mm\:ss");

                    if (EndTimeText != null)
                        EndTimeText.Text = TimeSpan.FromSeconds(EndTimeSlider.Value).ToString(@"mm\:ss");
                }
            };
            _timer.Start();
        }

        private void StartTimeSlider_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Ensure that after dragging, the video continues playing
            if (mediaPlayer != null)
            {
                mediaPlayer.Play();
            }
        }

        private void EndTimeSlider_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Ensure that after dragging, the video continues playing
            if (mediaPlayer != null)
            {
                mediaPlayer.Play();
            }
        }

        private void PlayVideo(string videoPath)
        {
            mediaPlayer.Source = new Uri(videoPath);
            mediaPlayer.Play();
        }

        private async void ApplyEffect(string effectType, double startTime, double endTime)
        {
            mediaPlayer.Pause();

            string uniqueIdentifier = Guid.NewGuid().ToString();
            string newTempPath = Path.Combine(Path.GetDirectoryName(_tempVideoPath), $"temp_{effectType}_{uniqueIdentifier}.mp4");

            Window loadingWindow = CreateLoadingWindow();
            loadingWindow.Show();

            try
            {
                await Task.Run(() =>
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
                                _videoService.CutVideo(_tempVideoPath, newTempPath, TimeSpan.FromSeconds(startTime), TimeSpan.FromSeconds(endTime));
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() => MessageBox.Show($"Có lỗi trong quá trình áp dụng hiệu ứng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error));
                    }
                });

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
        }
        // Cắt video
        private void BtnCut_Click(object sender, RoutedEventArgs e)
        {
            double startTime = StartTimeSlider.Value;
            double endTime = EndTimeSlider.Value;
            ApplyEffect("cut", startTime, endTime);
        }

        // Trắng đen (Gray)
        private void BtnGray_Click(object sender, RoutedEventArgs e)
        {

            ApplyEffect("gray", 0, 0);
        }

        // Tua ngược (Reverse)
        private void BtnReverse_Click(object sender, RoutedEventArgs e)
        {

            ApplyEffect("reverse", 0, 0);
        }

        // Slow Motion
        private void BtnSlowMotion_Click(object sender, RoutedEventArgs e)
        {

            ApplyEffect("slowmotion", 0, 0);
        }


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

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.Source != null)
            {
                mediaPlayer.Stop(); 
                mediaPlayer.Position = TimeSpan.Zero; 
                mediaPlayer.Play(); 
            }
        }
    }
}
