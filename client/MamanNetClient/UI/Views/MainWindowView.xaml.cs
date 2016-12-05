using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using Samples;
using ViewModel;
using ViewModel.Files;

namespace MamanNet.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AllFilesViewModel _baseFilesViewModel;

        public MainWindow()
        {
            InitializeComponent();

            Application.Current.Exit += OnApplicationExit;
            _baseFilesViewModel = Resources["AllFileViewModel"] as AllFilesViewModel;
            if (_baseFilesViewModel == null) throw new ArgumentNullException();
            _baseFilesViewModel.DownloadingFilesViewModel.ShowPopup += ShowPopup;
        }

        void OnApplicationExit(object sender, ExitEventArgs e)
        {
            _baseFilesViewModel.SavedDownloadedFiles();
        }

        void ShowPopup(object sender, string e)
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

        private void SystemTray_OnExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(1);
        }

    }
}
