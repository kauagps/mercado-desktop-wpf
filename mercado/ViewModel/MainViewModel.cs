using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using mercado.ViewModel;
using System.Windows.Input;




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

            NavEstoqueCommand = new RelayCommand(NavegarParaEstoque);
            NavPdvCommand = new RelayCommand(NavegarParaPdv);

            CurrentViewModel = EstoqueVM;
        }

        public void NavegarParaEstoque()
        {
            CurrentViewModel = EstoqueVM;
        }

        public void NavegarParaPdv()
        {
            CurrentViewModel = PdvVM;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
