using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using Samples;
using ViewModel;
using ViewModel.FilesViewModels;

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
            Application.Current.Exit += OnApplicationExit;
        }

        void OnApplicationExit(object sender, ExitEventArgs e)
        {
            var fileViewModel = Resources["FileViewModel"] as BaseFilesViewModel;
            if (fileViewModel == null) throw new ArgumentNullException("sender");
            fileViewModel.DownloadedFilesViewModel.SavedDownloadedFiles();
        }

        void Execute_ShowPopup(object sender, string e)
        {
            SystemTray.ShowCustomBalloon(new FancyBalloon(e), PopupAnimation.Fade, 5000);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void MainWindow_OnOpening(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = WindowState.Maximized;
        }

        private void MenuItem_OnExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(1);
        }

    }
}
