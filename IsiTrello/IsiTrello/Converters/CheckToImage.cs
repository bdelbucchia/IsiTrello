using System;
using System.Globalization;
using Xamarin.Forms;

namespace IsiTrello.Infrastructures
{
    class CheckToImage : IValueConverter
    {
        public string checkImg = "checked.png";
        public string uncheckImg = "unchecked.png";

        public object Convert(object value, Type targeType, object parameter, CultureInfo culture)
        {
            if (value is bool && value != null)
            {
                var checkState = (bool)value;
                if (checkState)
                    return checkImg;
                else
                    return uncheckImg;
            }
            else
            {
                throw new Exception("Invalid argument, value must be bool");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
