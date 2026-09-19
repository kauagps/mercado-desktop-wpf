using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using mercado.Model;

namespace mercado.ViewModel
{
    public class PagamentoViewModel : INotifyPropertyChanged
    {

        public decimal TotalCompra { get; private set; }

        public ObservableCollection<PagamentoVenda> Pagamentos { get; set; } = new ObservableCollection<PagamentoVenda>();

        public decimal TotalPago => Pagamentos.Sum(p => p.ValorPago);

        public decimal FaltaPagar => TotalCompra > TotalPago ? TotalCompra - TotalPago : 0;

        public decimal Troco => TotalPago > TotalCompra ? TotalPago - TotalCompra : 0;

        public PagamentoViewModel(decimal totalCompra)
        {
            TotalCompra = totalCompra;

            Pagamentos.CollectionChanged += (s, e) => AtualizarTotais();
        }

        public void AdicionarPagamento(string formaPagamento, decimal valorDigitado)
        {
            if (valorDigitado <= 0) return;

            Pagamentos.Add(new PagamentoVenda
            {
                FormaPagamento = formaPagamento,
                ValorPago = valorDigitado
            });
        }

        public void RemoverPagamento(PagamentoVenda pagamento)
        {
            if (pagamento != null)
            {
                Pagamentos.Remove(pagamento);
            }
        }

        private void AtualizarTotais()
        {
            OnPropertyChanged(nameof(TotalPago));
            OnPropertyChanged(nameof(FaltaPagar));
            OnPropertyChanged(nameof(Troco));
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
