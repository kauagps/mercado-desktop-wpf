using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace mercado.ViewModel
{
    /// <summary>
    /// Converte bool (Ativo) para cor de status (verde/vermelho)
    /// </summary>
    public class BoolToStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool ativo)
            {
                return ativo ? new SolidColorBrush(Color.FromRgb(0x27, 0xAE, 0x60)) : new SolidColorBrush(Color.FromRgb(0xE7, 0x4C, 0x3C));
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converte bool (Ativo) para texto "Ativo"/"Inativo"
    /// </summary>
    public class BoolToStatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool ativo)
            {
                return ativo ? "Ativo" : "Inativo";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converte bool (Ativo) para texto do botão toggle "Inativar"/"Ativar"
    /// </summary>
    public class BoolToToggleTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool ativo)
            {
                return ativo ? "Inativar" : "Ativar";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converte bool (Ativo) para cor de fundo do botão toggle
    /// </summary>
    public class BoolToToggleBgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool ativo)
            {
                return ativo ? new SolidColorBrush(Color.FromRgb(0xF3, 0x9C, 0x12)) : new SolidColorBrush(Color.FromRgb(0x27, 0xAE, 0x60));
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converte bool (Ativo) para cor de texto do botão toggle
    /// </summary>
    public class BoolToToggleFgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool ativo)
            {
                return ativo ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converte bool para Visibility (True = Visible, False = Collapsed)
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }
}