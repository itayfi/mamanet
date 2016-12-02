using System.Windows;
using System.Windows.Controls.Primitives;
using Samples;
using ViewModel;

namespace MamanNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var mainViewModel = DataContext as MainViewModel;
            if (mainViewModel != null) mainViewModel.ShowPopup += Execute_ShowPopup;
        }

        void Execute_ShowPopup(object sender, string e)
        {
            SystemTray.ShowCustomBalloon(new FancyBalloon(e), PopupAnimation.Fade, 5000);
        }

        private void MyWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            WindowState = WindowState.Minimized;
            Hide();
        }

        private void ShowApplication(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = WindowState.Maximized;
        }
    }
}
