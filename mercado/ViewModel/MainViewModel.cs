using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using mercado.ViewModel;
using System.Windows.Input;
using System.Windows;




namespace mercado.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentViewModel = null!;
        public object CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public EstoqueViewModel EstoqueVM { get; set; }
        public PdvViewModel PdvVM { get; set; }

        public ICommand NavEstoqueCommand { get; set; }
        public ICommand NavPdvCommand { get; set; }

        public MainViewModel()
        {
            EstoqueVM = new EstoqueViewModel();
            PdvVM = new PdvViewModel();

            NavEstoqueCommand = new RelayCommand(_ => NavegarParaEstoque());
            NavPdvCommand = new RelayCommand(_ => NavegarParaPdv());

            CurrentViewModel = EstoqueVM;
        }

        public void NavegarParaEstoque()
        {
            if (CurrentViewModel == EstoqueVM) return;

            if (!PodeMudarDeTela()) return;
            CurrentViewModel = EstoqueVM;
        }

        public void NavegarParaPdv()
        {
            if (CurrentViewModel == PdvVM) return;

            if (!PodeMudarDeTela()) return;
            CurrentViewModel = PdvVM;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool PodeMudarDeTela()
        {
            if (CurrentViewModel is PdvViewModel pdvViewModel)
            {
                if (pdvViewModel.Carrinho.Count > 0)
                {
                    var resposta = MessageBox.Show(
                        "Existe uma venda em andamento. Se você trocar de tela agora, o carrinho será esvaziado.\n\nDeseja realmente sair do PDV?",
                        "Venda em Andamento",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (resposta == MessageBoxResult.No)
                    {
                        return false;
                    }
                    else
                    {
                        pdvViewModel.CancelarVenda();
                    }
                }
            }

            return true;
        }
    }
}
