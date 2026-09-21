using mercado.Service;
using mercado.ViewModel;
using mercado.View;

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
    /// Interação lógica para PdvView.xam
    /// </summary>
    public partial class PdvView : UserControl
    {
        private PdvViewModel _viewModel => (PdvViewModel)this.DataContext;


        public PdvView()
        {
            InitializeComponent();

            this.Loaded += PdvView_Loaded;

            this.PreviewKeyDown += PdvView_PreviewKeyDown;
        }

        private void PdvView_Loaded(object sender, RoutedEventArgs e)
        {
            txtCodigoBarras.Focus();
            Keyboard.Focus(txtCodigoBarras);
        }

        private void txtCodigoBarras_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.BiparProduto(txtCodigoBarras.Text);
                txtCodigoBarras.Clear();
                txtCodigoBarras.Focus();
            }
        }

        private void FinalizarCompra_Click(object sender, RoutedEventArgs e )
        {
            if (_viewModel.Carrinho.Count == 0)
            {
                MessageBox.Show("O carrinho está vazio! Bipe um produto antes de finalizar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCodigoBarras.Focus();
                return;
            }

            var janelaPagamento = new PagamentoWindow(_viewModel.TotalCompra);
            janelaPagamento.ShowDialog();

            if (janelaPagamento.VendaConcluida)
            {
                if(janelaPagamento.DataContext is PagamentoViewModel pagamentoViewModel)
                {
                    var listaDePagamentos = pagamentoViewModel.Pagamentos.ToList();
                    _viewModel.ConcluirVenda(listaDePagamentos);

                    MessageBox.Show("Venda finalizada e registrada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        
        txtCodigoBarras.Focus();
        }

        private void BuscarProduto_Click(object sender, RoutedEventArgs e)
        {
            var telaBusca = new BuscaProdutoWindow();

            if (telaBusca.ShowDialog() == true)
            {
                string codigoEscolhido = telaBusca.CodigoBarrasSelecionado ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(codigoEscolhido))
                {
                    txtCodigoBarras.Text = codigoEscolhido;

                    _viewModel.BiparProduto(codigoEscolhido);

                    txtCodigoBarras.Clear();
                }

            }

            txtCodigoBarras.Focus();
            Keyboard.Focus(txtCodigoBarras);
        }

        private void PdvView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                e.Handled = true;

                BuscarProduto_Click(sender, e);
            }
        }

        public void CancelarCompra_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Carrinho.Count() > 0)
            {
                var resposta = MessageBox.Show(
                    "Existe uma venda em andamento, deseja realmente cancelar?",
                    "Venda em Andamento",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resposta == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    _viewModel.CancelarVenda();
                }
            }
        }
    }
}
