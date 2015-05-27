using System;
using Windows.UI.Xaml.Data;

namespace FetchItUniversalAndApi.Common
{
    //Author: Lárus Þór Lee
    /// <summary>
    /// A converter used by LandingPageView to show 2 different images according to wether a value is null or not
    /// </summary>
    class NullToPictureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "../Assets/Button Icons/ExclamationMark.png";
            }
            else
            {
                return "../Assets/Button Icons/Done.png";
            }
        }

        /// <summary>
        /// Why the hell would you want that? Talk to the author
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
