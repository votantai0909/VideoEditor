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
    }
}
