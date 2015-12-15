using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Net;
using Mina.Transport.Serial;

namespace PowerAmpControl.Converter
{
    public class Endpiont2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var endpoint = value as IPEndPoint;
            if (endpoint != null)
            {
                var t = string.Format("{0}:{1}", endpoint.Address, endpoint.Port);
                return t;
            }
            var endpoint2 = value as SerialEndPoint;
            if (endpoint2 != null)
            {
                return string.Format("{0}:{1}", endpoint2.PortName, endpoint2.BaudRate);
            }
            throw new NotImplementedException();
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (string)value;
            var splits = str.Split(':');
            IPAddress ipAddress;
            if (IPAddress.TryParse(splits[0],out ipAddress))
            {
                return new IPEndPoint(ipAddress, int.Parse(splits[1]));
            }
            return new SerialEndPoint(splits[0], int.Parse(splits[1]));
        }
    }
}
