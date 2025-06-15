using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using SilkySouls2.Models;

namespace SilkySouls2.Converters
{
    public class SpellTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SpellType spellType)
            {
                switch (spellType)
                {
                    case SpellType.Sorcery:
                        return new SolidColorBrush(Colors.CornflowerBlue);
                    case SpellType.Miracle:
                        return new SolidColorBrush(Colors.Gold);
                    case SpellType.Pyromancy:
                        return new SolidColorBrush(Colors.Red);
                    case SpellType.Hex:
                        return new SolidColorBrush(Colors.DarkViolet);
                    default:
                        return new SolidColorBrush(Colors.Gray);
                }
            }
            return new SolidColorBrush(Colors.Gray);
        }
    
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}