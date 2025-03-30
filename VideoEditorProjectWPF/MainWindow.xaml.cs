using Microsoft.Win32;
using System.IO;
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

namespace VideoEditorProjectWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BackgroundMusicService _musicService = new();
        private readonly MediaElement _backgroundMusic = new()
        {
            LoadedBehavior = MediaState.Manual,
            UnloadedBehavior = MediaState.Stop
        };
        private DispatcherTimer timer;
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
                VideoPlayer.Stop(); // Dừng video hiện tại nếu có
                VideoPlayer.Source = null; // Xóa nguồn cũ trước khi thay thế
                VideoSlider.Value = 0; // Đặt lại thanh trượt về 0

                VideoPlayer.Source = new Uri(openFileDialog.FileName);
            }
        }

        private void VideoPlayer_MediaOpened(object? sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                VideoSlider.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                VideoPlayer.Play(); // Chỉ phát sau khi video đã load xong
                timer.Start();
            }
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                // Cập nhật giới hạn tối đa của Slider theo độ dài video
                VideoSlider.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;

                // Cập nhật giá trị Slider theo thời gian video hiện tại
                VideoSlider.Value = VideoPlayer.Position.TotalSeconds;

                // Cập nhật hiển thị thời gian hiện tại của video
                CurrentTimeText.Text = VideoPlayer.Position.ToString(@"mm\:ss");

                // Cập nhật tổng thời gian video (chỉ cập nhật khi chưa được đặt)
            }
        }

        private void VideoSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan newPosition = TimeSpan.FromSeconds(VideoSlider.Value);
                VideoPlayer.Position = newPosition;

                // Đồng bộ nhạc nền nếu đang phát
                if (MusicPlayer.Source != null && MusicPlayer.Position != newPosition)
                {
                    MusicPlayer.Position = newPosition;
                }
            }
        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Play();
            timer.Start(); // Đảm bảo Timer chạy khi phát video

            if (MusicPlayer.Source != null)
            {
                MusicPlayer.Position = VideoPlayer.Position; // Đồng bộ vị trí nhạc với video
                MusicPlayer.Play();
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Pause();
            MusicPlayer.Pause(); // Tạm dừng nhạc nền khi video tạm dừng
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Stop();
            MusicPlayer.Stop(); // Dừng nhạc nền khi video dừng
        }

        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan newPosition = VideoPlayer.Position + TimeSpan.FromSeconds(5);

                // Giới hạn nếu vượt quá thời lượng video
                if (newPosition > VideoPlayer.NaturalDuration.TimeSpan)
                {
                    newPosition = VideoPlayer.NaturalDuration.TimeSpan;
                }

                // Cập nhật vị trí VideoPlayer & MusicPlayer
                VideoPlayer.Position = newPosition;
                MusicPlayer.Position = newPosition;
            }
        }



        private void SpeedSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpeedSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                double speed = double.Parse(selectedItem.Tag.ToString());

                // Cập nhật tốc độ cho VideoPlayer và MusicPlayer
                VideoPlayer.SpeedRatio = speed;
                MusicPlayer.SpeedRatio = speed;
            }
        }


        private async void LoadMusicList()
        {
            try
            {
                var musicList = await _musicService.GetAllMusicAsync();
                MusicComboBox.Items.Clear(); // Xóa danh sách cũ

                if (musicList.Any())
                {
                    foreach (var music in musicList)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MusicComboBox.Items.Add(new ComboBoxItem
                            {
                                Content = music.Name,
                                Tag = music.FilePath // Lưu đường dẫn vào Tag
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

        private void MusicComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MusicComboBox.SelectedItem is ComboBoxItem selectedItem &&
                selectedItem.Tag is string filePath &&
                !string.IsNullOrWhiteSpace(filePath))
            {
                try
                {
                    if (!File.Exists(filePath))
                    {
                        MessageBox.Show($"Không tìm thấy tệp nhạc: {filePath}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    MusicPlayer.Stop(); // Dừng nhạc trước khi đổi file
                    MusicPlayer.Source = new Uri(filePath, UriKind.Absolute);
                    MusicPlayer.Volume = MusicVolumeSlider.Value; // Đặt volume từ slider
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

    }
}