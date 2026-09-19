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
using mercado.ViewModel;

namespace mercado.View
{
    /// <summary>
    /// Lógica interna para PagamentoWindow.xaml
    /// </summary>
    public partial class PagamentoWindow : Window
    {

        private PagamentoViewModel _viewModel;

        public bool VendaConcluida { get; private set; } = false;
        public PagamentoWindow(decimal totalCompra)
        {
            InitializeComponent();

            _viewModel = new PagamentoViewModel(totalCompra);
            DataContext = _viewModel;

            txtValorPagamento.Focus();
            txtValorPagamento.SelectAll();
        }

        private void SelecionarFormaPagamento_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                txtFormaSelecionada.Text = btn.Content.ToString();
                txtValorPagamento.Focus();
                txtValorPagamento.SelectAll();
            }
        }

        private void txtValorPagamento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AdicionarPagamento_Click(sender, e);
            }
        }

        private void AdicionarPagamento_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txtValorPagamento.Text, out decimal valorDigitado))
            {
                _viewModel.AdicionarPagamento(txtFormaSelecionada.Text, valorDigitado);

                txtValorPagamento.Text = _viewModel.FaltaPagar.ToString("N2");
                txtValorPagamento.Focus();
                txtValorPagamento.SelectAll();
            }
            else
            {
                MessageBox.Show("Digite um valor válido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FinalizarVenda_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.FaltaPagar > 0)
            {
                MessageBox.Show($"Ainda falta pagar R$ {_viewModel.FaltaPagar:N2}", "Pagamento Incompleto", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            VendaConcluida = true;
            this.Close();
        }
    }
}
