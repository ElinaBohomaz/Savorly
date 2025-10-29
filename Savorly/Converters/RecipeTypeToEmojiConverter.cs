using System;
using System.Globalization;
using System.Windows.Data;
using Savorly.Models;

namespace Savorly.Converters
{
    public class RecipeTypeToEmojiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RecipeType recipeType)
            {
                return recipeType == RecipeType.Food ? "🍳" : "🥤";
            }
            return "🍳";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}