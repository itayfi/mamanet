using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using Common.LogUtilities;
using DAL;
using MamaNet.UI.Popup;
using MamaNet.UI.Upload;
using Networking.Files;
using Networking.Utilities;
using ViewModels.Files;

namespace MamaNet.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AllFilesViewModel _baseFilesViewModel;

        public MainWindow()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            InitializeComponent();

            Application.Current.Exit += OnApplicationExit;
            _baseFilesViewModel = Resources["AllFileViewModel"] as AllFilesViewModel;
            if (_baseFilesViewModel == null) throw new ArgumentNullException();
            _baseFilesViewModel.DownloadingFilesViewModel.ShowPopup += ShowPopup;
            
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.WriteLogEntry(e.Exception.ToString(),LogSeverity.CriticalError);
        }

        void OnApplicationExit(object sender, ExitEventArgs e)
        {
            _baseFilesViewModel.SavedDownloadedFiles();
        }

        public void ShowPopup(object sender, string e)
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

        private void UploadButtonClick(object sender, RoutedEventArgs e)
        {
            UploadFile uplodaFileDialog = new UploadFile(this);
            uplodaFileDialog.ShowDialog();
        }
    }
}
