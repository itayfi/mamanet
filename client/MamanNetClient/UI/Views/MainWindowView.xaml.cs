using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using Common.Utilities;
using Samples;
using ViewModels.Files;

namespace UI.Views
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
