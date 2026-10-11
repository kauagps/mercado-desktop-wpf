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
                    Desconto = item.Desconto,
                    Acrescimo = item.Acrescimo
                });
            }
            dgCarrinho.ItemsSource = _carrinho;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Validação em tempo real ao terminar edição de célula
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var item = (ItemVenda)e.Row.DataContext;
                var column = e.Column as DataGridBoundColumn;
                if (column == null) return;

                var bindingPath = (column.Binding as Binding)?.Path.Path;
                if (string.IsNullOrEmpty(bindingPath)) return;

                // Obter o valor digitado no TextBox de edição
                var textBox = e.EditingElement as TextBox;
                if (textBox == null) return;

                string texto = textBox.Text.Trim().Replace(".", ",");
                if (!decimal.TryParse(texto, out decimal valorDigitado))
                {
                    MessageBox.Show("Valor inválido. Digite um número válido.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Cancel = true;
                    return;
                }

                // Validações por coluna
                switch (bindingPath)
                {
                    case "Quantidade":
                        if (valorDigitado <= 0)
                        {
                            MessageBox.Show("A quantidade deve ser maior que zero.", "Quantidade Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                            e.Cancel = true;
                            return;
                        }
                        // Validar se produto não fracionado aceita apenas inteiros
                        if (item.Produto != null && !item.Produto.Fracionado && (valorDigitado % 1) != 0)
                        {
                            MessageBox.Show($"O produto '{item.Produto.Nome}' só aceita quantidade inteira.", "Quantidade Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                            e.Cancel = true;
                            return;
                        }
                        break;

                    case "Desconto":
                        if (valorDigitado < 0)
                        {
                            MessageBox.Show("O desconto não pode ser negativo.", "Desconto Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                            e.Cancel = true;
                            return;
                        }
                        if (valorDigitado > item.ValorUnitario)
                        {
                            MessageBox.Show($"O desconto não pode ser maior que o valor unitário (R$ {item.ValorUnitario:N2}).", "Desconto Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                            e.Cancel = true;
                            return;
                        }
                        break;

                    case "Acrescimo":
                        if (valorDigitado < 0)
                        {
                            MessageBox.Show("O acréscimo não pode ser negativo.", "Acréscimo Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                            e.Cancel = true;
                            return;
                        }
                        break;
                }
            }
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

                // Validação: acréscimo não negativo
                if (item.Acrescimo < 0)
                {
                    MessageBox.Show($"O acréscimo do produto '{item.Produto?.Nome}' não pode ser negativo.", "Acréscimo Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (item.Subtotal < 0)
                {
                    MessageBox.Show($"O subtotal do produto '{item.Produto?.Nome}' ficou negativo. Verifique o desconto/acréscimo aplicado.", "Subtotal Negativo", MessageBoxButton.OK, MessageBoxImage.Warning);
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
