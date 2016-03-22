using AmeCreateProject.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AmeCreateProject.Views
{
    [ValueConversion(typeof(string), typeof(EnumMediaViewStatus))]
    public sealed class MediaStatusToImageConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public string NoneValue { get; set; } = "";
        public string EmptyValue { get; set; } = "";
        public string InProgressValue { get; set; } = "/Resources/wait.gif";
        public string CompletedValue { get; set; } = "/Resources/done.png";
        public string FailedValue { get; set; } = "/Resources/error.png";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch(value as EnumMediaViewStatus?)
            {
                case EnumMediaViewStatus.InProgress:
                    return InProgressValue;
                case EnumMediaViewStatus.Completed:
                    return CompletedValue;
                case EnumMediaViewStatus.Failed:
                    return FailedValue;
                case EnumMediaViewStatus.Empty:
                    return EmptyValue;
                default:
                    return NoneValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
