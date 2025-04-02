﻿using Microsoft.Win32;
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
using VideoEditorProject.Services.Interface;
using VideoEditorProject.Services.Services;

namespace VideoEditorProjectWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
        }
        private void OpenVideo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Video Files|*.mp4;*.avi;*.wmv;*.mov"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                VideoPlayer.Source = new Uri(openFileDialog.FileName);
                VideoPlayer.Play();

                // Đợi video load xong, sau đó mới cập nhật Slider
                VideoPlayer.MediaOpened += (s, ev) =>
                {
                    if (VideoPlayer.NaturalDuration.HasTimeSpan)
                    {
                        VideoSlider.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                    }
                };

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
            }
        }

        private void VideoSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                VideoPlayer.Position = TimeSpan.FromSeconds(VideoSlider.Value);
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Play();
            timer.Start(); // Đảm bảo Timer chạy khi phát video
        }

        private void Pause_Click(object sender, RoutedEventArgs e) => VideoPlayer.Pause();
        private void Stop_Click(object sender, RoutedEventArgs e) => VideoPlayer.Stop();

        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan newPosition = VideoPlayer.Position + TimeSpan.FromSeconds(5);
                VideoPlayer.Position = newPosition < VideoPlayer.NaturalDuration.TimeSpan ? newPosition : VideoPlayer.NaturalDuration.TimeSpan;
            }
        }

        private void SpeedSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpeedSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                VideoPlayer.SpeedRatio = double.Parse(selectedItem.Tag.ToString());
            }
        }

        private void OpenEffectWindow_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.Source != null)
            {
                MessageBox.Show($"Video Path: {VideoPlayer.Source?.LocalPath}");

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


    }
}