using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MamaNet.UI.Converters
{
    public class CommunicationQualityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var lastCommunication = (DateTime) value;
            if (lastCommunication >= DateTime.Now.Subtract(TimeSpan.FromSeconds(10)))
                return "Green";
            return "Red";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
