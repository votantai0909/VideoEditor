using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using VideoEditorProject.Services.Services;
using System.Threading.Tasks;
using VideoEditorProject.Repositories.Entity;
using System.Diagnostics;
using Path = System.IO.Path;

namespace VideoEditorProjectWPF
{
    public partial class MainWindow : Window
    {
        private readonly BackgroundMusicService _musicService = new();
        private readonly MediaElement _backgroundMusic = new()
        {
            LoadedBehavior = MediaState.Manual,
            UnloadedBehavior = MediaState.Stop
        };
        private DispatcherTimer timer;
        private Point _dragStartPoint;
        private DateTime _lastClickTime;
        private TextBlock? watermarkText;

        public MainWindow()
        {
            InitializeComponent();
            LoadMusicList();
            VideoPlayer.MediaOpened += VideoPlayer_MediaOpened;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer_Tick;
        }

        private void OpenVideo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Video Files|*.mp4;*.avi;*.wmv;*.mov;*.mp3"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                VideoPlayer.Stop();
                VideoPlayer.Source = null;
                VideoSlider.Value = 0;
                VideoPlayer.Source = new Uri(openFileDialog.FileName);
            }
        }

        private void VideoPlayer_MediaOpened(object? sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                VideoSlider.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                VideoPlayer.Play();
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                VideoSlider.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                VideoSlider.Value = VideoPlayer.Position.TotalSeconds;
                CurrentTimeText.Text = VideoPlayer.Position.ToString(@"mm\:ss");
            }
        }

        private void VideoSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan newPosition = TimeSpan.FromSeconds(VideoSlider.Value);
                VideoPlayer.Position = newPosition;
                if (MusicPlayer.Source != null && MusicPlayer.Position != newPosition)
                {
                    MusicPlayer.Position = newPosition;
                }
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Play();
            timer.Start();
            if (MusicPlayer.Source != null)
            {
                MusicPlayer.Position = VideoPlayer.Position;
                MusicPlayer.Play();
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Pause();
            MusicPlayer.Pause();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Stop();
            MusicPlayer.Stop();
        }

        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan newPosition = VideoPlayer.Position + TimeSpan.FromSeconds(5);
                if (newPosition > VideoPlayer.NaturalDuration.TimeSpan)
                {
                    newPosition = VideoPlayer.NaturalDuration.TimeSpan;
                }
                VideoPlayer.Position = newPosition;
                MusicPlayer.Position = newPosition;
            }
        }

        private void SpeedSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpeedSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                double speed = double.Parse(selectedItem.Tag.ToString());
                VideoPlayer.SpeedRatio = speed;
                MusicPlayer.SpeedRatio = speed;
            }
        }

        private async void LoadMusicList()
        {
            try
            {
                var musicList = await _musicService.GetAllMusicAsync();
                MusicComboBox.Items.Clear();
                if (musicList.Any())
                {
                    foreach (var music in musicList)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MusicComboBox.Items.Add(new ComboBoxItem
                            {
                                Content = music.Name,
                                Tag = music.FilePath
                            });
                        });
                    }
                }
                else
                {
                    MessageBox.Show("Không có nhạc nền trong danh sách!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách nhạc: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddMusic_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.mp3;*.wav;*.aac",
                Title = "Chọn tệp nhạc"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(filePath);

                try
                {
                    // Thêm vào database
                    var newMusic = new BackgroundMusic { Name = fileName, FilePath = filePath };
                    await _musicService.AddMusicAsync(newMusic);

                    MessageBox.Show("Thêm nhạc thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Cập nhật danh sách nhạc
                    LoadMusicList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm nhạc: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void MusicComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MusicComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is string filePath && !string.IsNullOrWhiteSpace(filePath))
            {
                try
                {
                    if (!File.Exists(filePath))
                    {
                        MessageBox.Show($"Không tìm thấy tệp nhạc: {filePath}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    MusicPlayer.Stop();
                    MusicPlayer.Source = new Uri(filePath, UriKind.Absolute);
                    MusicPlayer.Volume = MusicVolumeSlider.Value;
                    MusicPlayer.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi phát nhạc: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MusicVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MusicPlayer != null)
            {
                MusicPlayer.Volume = e.NewValue;
            }
        }

        /*private void AddTextToVideo_Click(object sender, RoutedEventArgs e)
        {
            string text = TextToAdd.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Vui lòng nhập văn bản trước khi chèn.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tạo một TextBlock mới để làm watermark
            TextBlock watermarkText = new TextBlock
            {
                Text = text,
                Foreground = Brushes.Red,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10, 10, 0, 0),
                Visibility = Visibility.Visible
            };

            watermarkText.MouseLeftButtonDown += WatermarkText_MouseLeftButtonDown; // Thêm sự kiện nhấn chuột
            watermarkText.MouseMove += WatermarkText_MouseMove;
            watermarkText.MouseLeftButtonUp += WatermarkText_MouseLeftButtonUp;

            // Thêm TextBlock vào Canvas (container chứa video)
            VideoCanvas.Children.Add(watermarkText);
        }*/

        private void WatermarkText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var watermarkText = (TextBlock)sender;
            var currentClickTime = DateTime.Now;

            // Kiểm tra thời gian giữa các lần nhấn để xác định nếu là đúp chuột
            if ((currentClickTime - _lastClickTime).TotalMilliseconds <= 500)
            {
                // Nếu là nhấn đúp chuột, cho phép chỉnh sửa văn bản
                EditWatermarkText(watermarkText);
            }

            _lastClickTime = currentClickTime;

            _dragStartPoint = e.GetPosition(watermarkText);
            watermarkText.CaptureMouse(); // Capture the mouse events to the watermark
        }

        private void WatermarkText_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var watermarkText = (TextBlock)sender;
                var currentPosition = e.GetPosition(VideoCanvas);

                // Cập nhật vị trí watermark khi kéo chuột
                double left = currentPosition.X - _dragStartPoint.X;
                double top = currentPosition.Y - _dragStartPoint.Y;

                // Đặt lại vị trí mới cho watermark
                Canvas.SetLeft(watermarkText, left);
                Canvas.SetTop(watermarkText, top);
            }
        }

        private void WatermarkText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var watermarkText = (TextBlock)sender;
            watermarkText.ReleaseMouseCapture(); // Release mouse capture khi kết thúc kéo
        }

        private void EditWatermarkText(TextBlock watermarkText)
        {
            // Tạo một TextBox để người dùng chỉnh sửa văn bản
            TextBox editTextBox = new TextBox
            {
                Text = watermarkText.Text,
                FontSize = watermarkText.FontSize,
                FontWeight = watermarkText.FontWeight,
                Foreground = watermarkText.Foreground,
                Width = watermarkText.ActualWidth,
                Height = watermarkText.ActualHeight,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent
            };

            // Cập nhật lại TextBlock khi TextBox mất focus
            editTextBox.LostFocus += (s, args) =>
            {
                watermarkText.Text = editTextBox.Text; // Cập nhật lại văn bản
                VideoCanvas.Children.Remove(editTextBox); // Xóa TextBox
                VideoCanvas.Children.Add(watermarkText); // Thêm TextBlock với văn bản mới
            };

            // Thay thế TextBlock bằng TextBox để chỉnh sửa
            VideoCanvas.Children.Remove(watermarkText);
            VideoCanvas.Children.Add(editTextBox);
            editTextBox.Focus(); // Focus vào TextBox để người dùng có thể nhập
        }


        private void DeleteTextButton_Click(object sender, RoutedEventArgs e)
        {
            // Loop through all the children in the Canvas to find TextBlock elements
            foreach (var child in VideoCanvas.Children)
            {
                if (child is TextBlock watermarkText)
                {
                    VideoCanvas.Children.Remove(watermarkText);
                    break; // Exit the loop after removing the first watermark
                }
            }
        }

        private void OpenEffectWindow_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.Source != null)
            {
                string videoPath = VideoPlayer.Source.LocalPath;

                if (File.Exists(videoPath)) // 🔹 Kiểm tra file trước khi mở hiệu ứng
                {
                    // Khởi tạo VideoService trực tiếp trong đây
                    VideoService videoService = new VideoService();  // Không cần truyền VideoService từ bên ngoài

                    // Truyền videoService trực tiếp vào cửa sổ hiệu ứng
                    VideoEffectWindow effectWindow = new VideoEffectWindow(videoPath);  // Truyền videoPath, không cần truyền videoService

                    if (effectWindow.ShowDialog() == true)
                    {
                        string editedVideoPath = effectWindow.Tag as string;
                        if (!string.IsNullOrEmpty(editedVideoPath) && File.Exists(editedVideoPath))
                        {
                            VideoPlayer.Source = new Uri(editedVideoPath);
                            VideoPlayer.Play();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy video đã chỉnh sửa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("File gốc không tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn video trước!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void AddTextToVideo_Click(object sender, RoutedEventArgs e)
        {
            string text = TextToAdd.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Vui lòng nhập văn bản trước khi chèn.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tạo một TextBlock mới để làm watermark
            watermarkText = new TextBlock
            {
                Text = text,
                Foreground = Brushes.Red,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10, 10, 0, 0),
                Visibility = Visibility.Visible
            };

            watermarkText.MouseLeftButtonDown += WatermarkText_MouseLeftButtonDown; // Thêm sự kiện nhấn chuột
            watermarkText.MouseMove += WatermarkText_MouseMove;
            watermarkText.MouseLeftButtonUp += WatermarkText_MouseLeftButtonUp;

            // Thêm TextBlock vào Canvas (container chứa video)
            VideoCanvas.Children.Add(watermarkText);

            // Câu lệnh để lưu video với watermark
            if (VideoPlayer.Source != null)
            {
                string videoPath = VideoPlayer.Source.LocalPath;
                string outputPath = Path.Combine(Path.GetDirectoryName(videoPath), "output_with_watermark.mp4");

                await SaveVideoWithWatermarkAsync(videoPath, outputPath);

                // Cập nhật lại video sau khi lưu
                VideoPlayer.Source = new Uri(outputPath);
                VideoPlayer.Play();
            }
        }
        private async Task SaveVideoWithWatermarkAsync(string videoPath, string outputPath)
        {
            string ffmpegPath = @"D:\dowload\SE_KI_7\PRN212\FF\ffmpeg-master-latest-win64-gpl-shared\bin\ffmpeg.exe";

            if (!File.Exists(ffmpegPath))
            {
                MessageBox.Show("Không tìm thấy FFmpeg. Vui lòng kiểm tra lại đường dẫn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Câu lệnh FFmpeg để thêm watermark vào video
            string arguments = $"-i \"{videoPath}\" -vf \"drawtext=text='{WatermarkText.Text}':x=10:y=10:fontcolor=white:fontsize=24\" -c:a copy \"{outputPath}\"";

            // Thực thi câu lệnh FFmpeg
            await RunFFmpegCommand(arguments);
        }
        private async Task RunFFmpegCommand(string arguments)
        {
            string ffmpegPath = @"D:\dowload\SE_KI_7\PRN212\FF\ffmpeg-master-latest-win64-gpl-shared\bin\ffmpeg.exe";

            if (!File.Exists(ffmpegPath))
            {
                MessageBox.Show("Không tìm thấy FFmpeg. Vui lòng kiểm tra lại đường dẫn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false, // Quan trọng: Không dùng Shell
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                string output = await process.StandardError.ReadToEndAsync(); // Đọc lỗi nếu có
                await process.WaitForExitAsync();

                MessageBox.Show(output, "FFmpeg Output", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Đảm bảo rằng video đã được tải vào MediaPlayer
            if (VideoPlayer.Source != null)
            {
                string videoPath = VideoPlayer.Source.LocalPath;

                // Kiểm tra xem video có tồn tại không
                if (File.Exists(videoPath))
                {
                    // Gọi hàm lưu video với các hiệu ứng và văn bản đã chỉnh sửa
                    await SaveVideoWithText(videoPath);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy file video.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng mở một video trước khi lưu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task SaveVideoWithText(string videoPath)
        {
            string outputVideoPath = Path.Combine(Path.GetDirectoryName(videoPath), "output_video_with_text.mp4");

            // Xây dựng lệnh FFmpeg để chèn văn bản vào video
            string arguments = $"-i \"{videoPath}\" -vf \"drawtext=text='{TextToAdd.Text}':fontcolor=white:fontsize=24:x=10:y=10\" -codec:a copy \"{outputVideoPath}\"";

            try
            {
                // Thực thi lệnh FFmpeg
                await RunFFmpegCommand1(arguments);
                MessageBox.Show("Video đã được lưu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu video: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task RunFFmpegCommand1(string arguments)
        {
            // Thực thi FFmpeg với các tham số đã cho
            Process ffmpegProcess = new Process();
            ffmpegProcess.StartInfo.FileName = @"D:\dowload\SE_KI_7\PRN212\FF\ffmpeg-master-latest-win64-gpl-shared\bin\ffmpeg.exe"; // Đảm bảo rằng đường dẫn đến FFmpeg đã được thêm vào biến môi trường PATH
            ffmpegProcess.StartInfo.Arguments = arguments;
            ffmpegProcess.StartInfo.RedirectStandardOutput = true;
            ffmpegProcess.StartInfo.RedirectStandardError = true;
            ffmpegProcess.StartInfo.UseShellExecute = false;
            ffmpegProcess.StartInfo.CreateNoWindow = true;

            await Task.Run(() =>
            {
                ffmpegProcess.Start();
                ffmpegProcess.WaitForExit();
            });
        }

    }
}
    