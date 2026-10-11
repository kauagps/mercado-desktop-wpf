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
using System.Windows.Shapes;


namespace mercado.View
{
    
    public partial class AjusteValorWindow : Window
    {
        
        public decimal ValorDigitado { get; private set; }

        public bool EhPorcentual { get; private set; }

        public AjusteValorWindow(string mensagem)
        {
            InitializeComponent();

            lblMensagem.Text = mensagem;

            txtValorReal.Focus();
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            ConfirmarValor();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ConfirmarValor()
        {
            if (valorReal.IsChecked == true)
            {
                string texto = txtValorReal.Text.Replace(".", ",");
                if (decimal.TryParse(texto, out decimal valorConvertido) && valorConvertido >= 0)
                {
                    ValorDigitado = valorConvertido;
                    EhPorcentual = false;
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Por favor, digite um valor numérico válido.", "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtValorReal.SelectAll();
                }
            }
            else // porcentagem
            {
                string texto = txtValorPorcent.Text.Replace(".", ",");
                if (decimal.TryParse(texto, out decimal valorConvertido) && valorConvertido >= 0 && valorConvertido <= 100)
                {
                    ValorDigitado = valorConvertido; // ex: 10 = 10%
                    EhPorcentual = true;
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Por favor, digite um percentual válido (0 a 100).", "Percentual Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtValorPorcent.SelectAll();
                }
            }
        }

        private void txtValor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConfirmarValor();
            }
            else if(e.Key == Key.Escape)
            {
                this.DialogResult = false;
            }
        }

        private void trocarInput(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb == null) return;

            bool isValorReal = rb == valorReal;

            if (txtValorReal != null)
            {
                txtValorReal.IsEnabled = isValorReal;
            }
            if(txtValorPorcent != null)
            {
                txtValorPorcent.IsEnabled = !isValorReal;
            }

            if (isValorReal && txtValorReal != null)
            {
                txtValorReal.Focus();

            }
            else if(!isValorReal && txtValorPorcent != null)
            {
                txtValorPorcent.Focus();
            }
        }

        public void ForcarModoValorReal()
        {
            valorReal.IsChecked = true;
            valorPorcent.IsEnabled = false;
            trocarInput(valorReal, new RoutedEventArgs());
        }
    }
}
