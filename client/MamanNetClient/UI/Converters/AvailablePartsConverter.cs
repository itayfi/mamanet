using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Networking.Files;

namespace MamaNet.UI.Converters
{
    class AvailablePartsConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var mamaNetFile = value as MamaNetFile;
            if (mamaNetFile != null)
            {
                return mamaNetFile.GetAvailableParts().Length;    
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
