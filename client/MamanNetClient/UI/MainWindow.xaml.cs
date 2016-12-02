using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hardcodet.Wpf;
using Hardcodet.Wpf.TaskbarNotification;
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
            (DataContext as Main).ShowPopup += Execute_ShowPopup;
            
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
