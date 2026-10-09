using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using mercado.Model;
using mercado.ViewModel;

namespace mercado.View
{
    public partial class EditarCarrinhoWindow : Window
    {
        private ObservableCollection<ItemVenda> _carrinho;

        public ObservableCollection<ItemVenda> CarrinhoEditado => _carrinho;

        public EditarCarrinhoWindow(ObservableCollection<ItemVenda> carrinhoAtual)
        {
            InitializeComponent();
            // Clonar o carrinho para edição
            _carrinho = new ObservableCollection<ItemVenda>();
            foreach (var item in carrinhoAtual)
            {
                _carrinho.Add(new ItemVenda
                {
                    Id = item.Id,
                    ProdutoId = item.ProdutoId,
                    Produto = item.Produto,
                    Quantidade = item.Quantidade,
                    ValorUnitario = item.ValorUnitario,
                    Subtotal = item.Subtotal,
                    Desconto = item.Desconto
                });
            }
            dgCarrinho.ItemsSource = _carrinho;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Atualiza subtotal automaticamente após edição
            var item = (ItemVenda)e.Row.DataContext;
            // Como Subtotal é calculado automaticamente pelo getter, não precisa atribuir
        }

        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var item = (ItemVenda)btn.Tag;

            int idx = _carrinho.IndexOf(item);
            if (idx < 0) return;

            if (item.Quantidade > 1)
            {
                item.Quantidade -= 1;
                // Recalcular subtotal automaticamente
                var row = dgCarrinho.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (row != null)
                {
                    // Forçar atualização visual do item no DataGrid
                    var itemCopy = item;
                    int index = _carrinho.IndexOf(item);
                    if (index >= 0)
                    {
                        _carrinho.RemoveAt(index);
                        _carrinho.Insert(index, itemCopy);
                    }
                }
            }
            else
            {
                _carrinho.RemoveAt(idx);
            }
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _carrinho)
            {
                // Validação: desconto não negativo e não maior que valor unitário
                if (item.Desconto < 0)
                {
                    MessageBox.Show($"O desconto do produto '{item.Produto?.Nome}' não pode ser negativo.", "Desconto Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (item.Desconto > item.ValorUnitario)
                {
                    MessageBox.Show($"O desconto do produto '{item.Produto?.Nome}' não pode ser maior que o valor unitário.", "Desconto Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (item.Subtotal < 0)
                {
                    MessageBox.Show($"O subtotal do produto '{item.Produto?.Nome}' ficou negativo. Verifique o desconto aplicado.", "Subtotal Negativo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validação: quantidade apenas inteira para produtos não fracionados
                if (item.Produto != null && !item.Produto.Fracionado && (item.Quantidade % 1) != 0)
                {
                    MessageBox.Show($"O produto '{item.Produto.Nome}' só aceita quantidade inteira.", "Quantidade Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validação: aceitar apenas um "." no valor (ex: 23.50)
                string qtdStr = item.Quantidade.ToString();
                int countPontos = qtdStr.Count(c => c == '.');
                if (countPontos > 1)
                {
                    MessageBox.Show($"A quantidade do produto '{item.Produto?.Nome}' contém pontos em excesso. Formato esperado: 23.50", "Formato Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
