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
using mercado.Model;
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
            string valorDigitadoTexto = txtValorPagamento.Text.Replace(".", ",");

            if (decimal.TryParse(valorDigitadoTexto, out decimal valorDigitado))
            {

                if (valorDigitado <= 0)
                {
                    MessageBox.Show("Digite um valor maior que zero", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtValorPagamento.Focus();
                    txtValorPagamento.SelectAll();
                    return;

                }
                _viewModel.AdicionarPagamento(txtFormaSelecionada.Text, valorDigitado);

                txtValorPagamento.Text = _viewModel.FaltaPagar.ToString("N2");
                txtValorPagamento.Focus();
                txtValorPagamento.SelectAll();
            }
            else
            {
                MessageBox.Show("Digite um valor válido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                txtValorPagamento.Focus();
                txtValorPagamento.SelectAll();
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

        private void RemoverPagamento_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button botaoClicado)
            { 
                if (botaoClicado.DataContext is PagamentoVenda pagamentoParaRemover)
                {
                    _viewModel.RemoverPagamento(pagamentoParaRemover);

                    txtValorPagamento.Text = _viewModel.FaltaPagar.ToString("N2");
                    txtValorPagamento.Focus();
                    txtValorPagamento.SelectAll();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (VendaConcluida == true) return;

            MessageBoxResult resposta = MessageBox.Show(
                "Tem certeza que deseja cancelar a operação e fechar a tela?",
                "Confirmação de Cancelamento",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resposta == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }
    }
}
