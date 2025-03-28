using System;
using System.Collections.Generic;
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
using VideoEditorProject.Repositories.Entity;
using VideoEditorProject.Services.Services;

namespace VideoEditorProjectWPF
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private AccountService _service = new();
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordTextBox.Text.Trim();

            Account acc = _service.Authentication(email, password);

            if (acc == null)
            {
                MessageBox.Show("Invalid email or password", "Wrong credentials", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MainWindow main = new MainWindow();
            this.Hide();
            main.Show();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to quit this app", "Quit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;

            Application.Current.Shutdown();
        }
    }
}
