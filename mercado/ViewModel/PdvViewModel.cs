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

        private VendaService _vendaService;

        private ProdutoService _produtoService;



        public ObservableCollection<ItemVenda> Carrinho { get; set; } = new ObservableCollection<ItemVenda>();

        private decimal _totalCompra;

        public PdvViewModel()
        {
            _produtoService = new ProdutoService();
            _vendaService = new VendaService();

        }


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

        public void ConcluirVenda(System.Collections.Generic.List<PagamentoVenda> pagamentosRealizados)
        {

            var novaVenda = new Venda
            {
                ValorTotal = TotalCompra,
                DataVenda = System.DateTime.Now,
                Pagamentos = pagamentosRealizados,
                Itens = Carrinho.Select(c => new ItemVenda
                {
                    ProdutoId = c.ProdutoId,
                    Quantidade = c.Quantidade,
                    ValorUnitario = c.ValorUnitario,
                    Subtotal = c.Subtotal
                }).ToList()
            };

            foreach (var item in novaVenda.Itens)
            {
                item.Produto = null;
            }

            _vendaService.SalvarVendaCompleta(novaVenda);

            var reciboService = new ReciboService();
            reciboService.GerarCupomTxt(novaVenda, Carrinho.ToList());

            Carrinho.Clear();
            QuantidadeAtual = 1;
            CalcularTotal();
            MessageBox.Show("Venda concluída com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
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

            if (!produto.Fracionado && QuantidadeAtual % 1 != 0)
            {
                MessageBox.Show($"O produto '{produto.Nome}' é vendido apenas por unidade inteira.\nNão é possível vender {QuantidadeAtual}.",
                        "Quantidade Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);

                QuantidadeAtual = 1; // Reseta o multiplicador por segurança
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

        public void CancelarVenda()
        {
            Carrinho.Clear();

            QuantidadeAtual = 1;

            CalcularTotal();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
