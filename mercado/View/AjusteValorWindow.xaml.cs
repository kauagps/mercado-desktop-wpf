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

        public AjusteValorWindow(string mensagem)
        {
            InitializeComponent();

            lblMensagem.Text = mensagem;

            txtValor.Focus();
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
            string texto = txtValor.Text.Replace(".", ",");

            if (decimal.TryParse(texto, out decimal valorConvertido))
            {
                ValorDigitado = valorConvertido;

                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Por favor, digite um valor numérico válido.", "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtValor.SelectAll();
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
    }
}
