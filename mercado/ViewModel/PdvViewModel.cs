using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using mercado.Model;
using mercado.Service;

namespace mercado.ViewModel
{
    public class PdvViewModel : INotifyPropertyChanged
    {
        private ProdutoService _produtoService;

        public ObservableCollection<ItemVenda> Carrinho { get; set; } = new ObservableCollection<ItemVenda>();

        private decimal _totalCompra;
        public decimal TotalCompra
        {
            get { return _totalCompra;  }
            set { _totalCompra = value; OnPropertyChanged(nameof(TotalCompra));  }
        }

        private decimal _quantidadeAtual = 1;
        public decimal QuantidadeAtual
        {
            get { return _quantidadeAtual; }
            set { _quantidadeAtual = value; OnPropertyChanged(nameof(QuantidadeAtual)); }
        }

        public PdvViewModel()
        {
            _produtoService = new ProdutoService();
        }

        public void BiparProduto(string codigoBarras)
        {
            if (string.IsNullOrWhiteSpace(codigoBarras)) return;

            var produto = _produtoService.BuscarPorCodigo(codigoBarras);

            if (produto == null)
            {
                MessageBox.Show("Produto não encontrado, ou inativo!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var novoItem = new ItemVenda
            {
                ProdutoId = produto.Id,
                Produto = produto,
                Quantidade = QuantidadeAtual,
                ValorUnitario = produto.ValorVenda,
                Subtotal = produto.ValorVenda * QuantidadeAtual
            };

            Carrinho.Add(novoItem);

            CalcularTotal();

            QuantidadeAtual = 1;
        }

        public void CalcularTotal()
        {
            TotalCompra = Carrinho.Sum(item => item.Subtotal);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
