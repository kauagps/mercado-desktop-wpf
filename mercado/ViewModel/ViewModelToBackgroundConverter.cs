using System;
using System.Globalization;
using System.Windows.Data;
using mercado.ViewModel;

namespace mercado.ViewModel
{
    /// <summary>
    /// Conversor que retorna a cor de fundo do botão de navegação baseado no ViewModel atual.
    /// Se o ViewModel atual corresponder ao parâmetro, retorna cor de "selecionado".
    /// </summary>
    public class ViewModelToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return "#34495E"; // Cor padrão (não selecionado)

            var currentVm = value;
            var paramName = parameter.ToString();

            // Verifica qual ViewModel está ativo comparando referências
            // O parâmetro indica qual botão está sendo renderizado ("Estoque" ou "Pdv")
            var mainVm = System.Windows.Application.Current.MainWindow?.DataContext as MainViewModel;
            if (mainVm == null)
                return "#34495E";

            bool isSelected = false;

            if (paramName == "Estoque" && currentVm == mainVm.EstoqueVM)
                isSelected = true;
            else if (paramName == "Pdv" && currentVm == mainVm.PdvVM)
                isSelected = true;

            // Cor quando selecionado (mais clara/brilhante)
            return isSelected ? "#1ABC9C" : "#34495E";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}