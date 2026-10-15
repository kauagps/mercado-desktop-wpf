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

            this.PreviewKeyDown += PagamentoWindow_PreviewKeyDown;
            this.Loaded += PagamentoWindow_Loaded;
        }

        private void PagamentoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtValorPagamento.Focus();
            txtValorPagamento.SelectAll();
        }

        private void PagamentoWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // F1-F4 para selecionar forma de pagamento
            if (e.Key == Key.F1)
            {
                e.Handled = true;
                SelecionarForma(btnDinheiro);
            }
            else if (e.Key == Key.F2)
            {
                e.Handled = true;
                SelecionarForma(btnPix);
            }
            else if (e.Key == Key.F3)
            {
                e.Handled = true;
                SelecionarForma(btnCredito);
            }
            else if (e.Key == Key.F4)
            {
                e.Handled = true;
                SelecionarForma(btnDebito);
            }

            // F5 - Finalizar Venda
            if (e.Key == Key.F5)
            {
                e.Handled = true;
                FinalizarVenda_Click(sender, e);
            }

            // Esc - Cancelar (fecha a janela com confirmação)
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                this.Close();
            }

            // Delete - Excluir Último Pagamento
            if (e.Key == Key.Delete)
            {
                e.Handled = true;
                ExcluirUltimoPagamento();
            }
        }

        private void SelecionarForma(Button btn)
        {
            if (btn?.Content is string texto)
            {
                txtFormaSelecionada.Text = texto.Split('(')[0].Trim();
                txtValorPagamento.Focus();
                txtValorPagamento.SelectAll();
            }
        }

        private void SelecionarFormaPagamento_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                SelecionarForma(btn);
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

        private void btnExcluirUltimoPagamento_Click(object sender, RoutedEventArgs e)
        {
            ExcluirUltimoPagamento();
        }

        private void ExcluirUltimoPagamento()
        {
            if (_viewModel.Pagamentos.Any())
            {
                var ultimoPagamento = _viewModel.Pagamentos.Last();
                _viewModel.RemoverPagamento(ultimoPagamento);

                txtValorPagamento.Text = _viewModel.FaltaPagar.ToString("N2");
                txtValorPagamento.Focus();
                txtValorPagamento.SelectAll();
            }
        }
    }
}
