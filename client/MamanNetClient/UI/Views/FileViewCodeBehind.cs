using System;
using System.Windows;
using System.Windows.Controls;
using MamaNet.UI.Upload;
using Microsoft.Win32;

namespace MamaNet.UI.Views
{
    public partial class FileViewCodeBehind
    {
        public FileViewCodeBehind()
        {
            InitializeComponent();
        }

        private void AddNewFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "MamaNet Metadata File|*.mamanet"
            };
            var result = openFileDialog.ShowDialog();
            var button = (sender) as Button;
            if (button != null) button.Tag = openFileDialog.FileName;
        }
    }
}
