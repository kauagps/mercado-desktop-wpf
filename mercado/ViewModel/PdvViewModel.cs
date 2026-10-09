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

        private decimal _desconto;
        public decimal Desconto
        {
            get { return _desconto; }
            set
            {
                _desconto = value;
                OnPropertyChanged(nameof(Desconto));
                CalcularTotal();
            }
        }

        private decimal _acrescimo;
        public decimal Acrescimo
        {
            get { return _acrescimo; }
            set
            {
                _acrescimo = value;
                OnPropertyChanged(nameof(Acrescimo));
                CalcularTotal();
            }
        }

        public decimal DescontoValorDigitado { get; set; } = 0;
        public bool DescontoEhPorcentual { get; set; } = false;

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
                Desconto = this.Desconto,
                Acrescimo = this.Acrescimo,
                DataVenda = System.DateTime.Now,
                Pagamentos = pagamentosRealizados,
                Itens = Carrinho.Select(c => new ItemVenda
                {
                    ProdutoId = c.ProdutoId,
                    Quantidade = c.Quantidade,
                    ValorUnitario = c.ValorUnitario,
                    Subtotal = c.Subtotal,
                    Desconto = c.Desconto
                }).ToList()
            };

            foreach (var item in novaVenda.Itens)
            {
                item.Produto = null!;
            }

            _vendaService.SalvarVendaCompleta(novaVenda);

            MessageBoxResult respostaImpressao = MessageBox.Show(
                "Venda concluída com sucesso!\n\nDeseja imprimir o comprovante para o cliente?",
                "Imprimir Cupom",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if(respostaImpressao == MessageBoxResult.Yes)
            {
                var reciboService = new ReciboService();

                reciboService.ImprimirCupomNaTermica(novaVenda, Carrinho.ToList());
            }

            CancelarVenda();
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
            decimal somaItens = Carrinho.Sum(item => item.Subtotal);

            decimal descontoGlobal = _desconto;
            if (DescontoEhPorcentual)
            {
                descontoGlobal = somaItens * (DescontoValorDigitado / 100m);
            }

            decimal totalComAjustes = somaItens + Acrescimo - descontoGlobal;
            _desconto = descontoGlobal;
            OnPropertyChanged(nameof(Desconto));

            TotalCompra = totalComAjustes < 0 ? 0 : totalComAjustes;
        }

        public void AplicarDescontoAoItem(ItemVenda item, decimal valor, bool ehPorcentual)
        {
            if (item == null) return;

            decimal desconto = 0;
            if (ehPorcentual)
            {
                desconto = item.Subtotal * (valor / 100);
            }
            else
            {
                desconto = valor;
            }

            // Limitar o desconto entre 0 e o subtotal do item
            if (desconto < 0) desconto = 0;
            if (desconto > item.Subtotal) desconto = item.Subtotal;

            item.Desconto = desconto;
            int idx = Carrinho.IndexOf(item);
            if (idx >= 0)
            {
                Carrinho.RemoveAt(idx);
                Carrinho.Insert(idx, item);
            }
            CalcularTotal();
        }

        public void CancelarVenda()
        {
            Carrinho.Clear();

            Desconto = 0;

            Acrescimo = 0;

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
