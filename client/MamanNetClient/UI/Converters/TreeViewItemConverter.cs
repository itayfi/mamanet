﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MamaNet.UI.Converters
{
    public class TreeViewItemConverter : IValueConverter
    {
        //Todo: delete, for future use only if necessery
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var treeViewItem = value as System.Windows.Controls.TreeViewItem;
            if (treeViewItem != null)
                return treeViewItem.Header;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
