﻿using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace MamanNet.Views
{
    public partial class FileViewCodeBehind
    {
        public FileViewCodeBehind()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog();
            var button = (sender) as Button;
            if (button != null) button.Tag = openFileDialog.FileName;
        }
    }
}