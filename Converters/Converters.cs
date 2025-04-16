using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Names.Converters
{
        /// <summary>
        /// Converts boolean values to Visibility.Visible or Visibility.Collapsed
        /// </summary>
        public class BooleanToVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value is Visibility && (Visibility)value == Visibility.Visible;
            }
        }

        /// <summary>
        /// Converts boolean values to Visibility.Collapsed or Visibility.Visible (inverted)
        /// </summary>
        public class BooleanToInverseVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (value is bool && (bool)value) ? Visibility.Collapsed : Visibility.Visible;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value is Visibility && (Visibility)value != Visibility.Visible;
            }
        }

        /// <summary>
        /// Formats integers as zero-padded strings
        /// </summary>
        public class IntegerToFormattedStringConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is int index)
                {
                    // Add 1 to make it 1-based instead of 0-based
                    return (index + 1).ToString("D3");
                }
                return "000";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Converts delay milliseconds to a formatted timestamp
        /// </summary>
        public class DelayToTimestampConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is int delayMs)
                {
                    // Create TimeSpan from milliseconds
                    TimeSpan time = TimeSpan.FromMilliseconds(delayMs);

                    // For very short durations (less than 1 second)
                    if (time.TotalSeconds < 1)
                    {
                        return string.Format("{0} ms", time.Milliseconds);
                    }
                    // For durations less than 1 minute
                    else if (time.TotalMinutes < 1)
                    {
                        return string.Format("{0}.{1:000} sec", time.Seconds, time.Milliseconds);
                    }
                    // For longer durations
                    else
                    {
                        return string.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                            time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
                    }
                }

                return "0 ms";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Formats command text more descriptively
        /// </summary>
        public class CommandTextConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is string keyName)
                {
                    if (keyName.StartsWith("Mouse"))
                    {
                        // For mouse commands, extract coordinates if present
                        if (keyName.Contains("(") && keyName.Contains(")"))
                        {
                            return keyName;
                        }
                        else
                        {
                            return "Mouse Click";
                        }
                    }
                    else
                    {
                        // For keyboard commands
                        return $"Key Press ({keyName})";
                    }
                }

                return "Unknown Command";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Converts boolean IsPlaying value to the appropriate icon path
        /// </summary>
        public class PlayButtonIconConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is bool isPlaying)
                {
                    // Return the appropriate path geometry string
                    if (isPlaying)
                    {
                        // Pause icon
                        return "M6,5 L10,5 L10,19 L6,19 Z M14,5 L18,5 L18,19 L14,19 Z";
                    }
                    else
                    {
                        // Play icon
                        return "M8 5.14v14l11-7-11-7z";
                    }
                }

                // Default to play icon
                return "M8 5.14v14l11-7-11-7z";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
}
