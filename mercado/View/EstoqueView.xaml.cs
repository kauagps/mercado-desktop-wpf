using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace mercado.View
{
    /// <summary>
    /// Interação lógica para EstoqueView.xam
    /// </summary>
    public partial class EstoqueView : UserControl
    {
        public EstoqueView()
        {
            InitializeComponent();
        }

        private void chkFracionado_Click(object sender, RoutedEventArgs e)
        {
            txtQuantidadeMin.Text = "0";
        }

        private void CalcularPrecoIdeal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtCusto == null || txtLucro == null || tbPrecoIdeal == null) return;

            string textoCusto = txtCusto.Text.Replace('.', ',');
            string textoLucro = txtLucro.Text.Replace('.', ',');

            if (double.TryParse(textoCusto, out double custo) && double.TryParse(textoLucro, out double lucro))
            {
                double precoIdeal = custo + (custo * (lucro / 100));
                tbPrecoIdeal.Text = $"Valor recomendado: (R$) {precoIdeal:N2}";

            }
            else
            {
                tbPrecoIdeal.Text = "Valor recomendado: (R$) 0,00";

            }
        }
    }
}
