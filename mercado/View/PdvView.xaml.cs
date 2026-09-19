using mercado.Service;
using mercado.ViewModel;
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
        private PdvViewModel _viewModel;


        public PdvView()
        {
            InitializeComponent();
            _viewModel = new PdvViewModel();
            DataContext = _viewModel;

            txtCodigoBarras.Focus();
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
    }
}
