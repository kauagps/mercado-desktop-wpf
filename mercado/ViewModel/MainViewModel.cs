using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using mercado.Model;
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
        public UsuariosViewModel UsuariosVM { get; set; }

        public ICommand NavEstoqueCommand { get; set; }
        public ICommand NavPdvCommand { get; set; }
        public ICommand NavUsuariosCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        // Usuário logado e permissões
        public Usuario UsuarioLogado { get; private set; }
        public bool EhAdmin => UsuarioLogado?.Tipo == TipoUsuario.Administrador;

        public MainViewModel(Usuario usuarioLogado)
        {
            UsuarioLogado = usuarioLogado;

            EstoqueVM = new EstoqueViewModel();
            PdvVM = new PdvViewModel();
            UsuariosVM = new UsuariosViewModel(usuarioLogado.Id);

            NavEstoqueCommand = new RelayCommand(_ => NavegarParaEstoque());
            NavPdvCommand = new RelayCommand(_ => NavegarParaPdv());
            NavUsuariosCommand = new RelayCommand(_ => NavegarParaUsuarios(), _ => EhAdmin);
            LogoutCommand = new RelayCommand(_ => Logout());

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

        public void NavegarParaUsuarios()
        {
            if (!EhAdmin)
            {
                MessageBox.Show("Acesso restrito a administradores.", "Acesso Negado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CurrentViewModel == UsuariosVM) return;

            if (!PodeMudarDeTela()) return;
            CurrentViewModel = UsuariosVM;
        }

        private void Logout()
        {
            var resposta = MessageBox.Show(
                "Deseja realmente sair do sistema?",
                "Confirmar Logout",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resposta != MessageBoxResult.Yes)
                return;

            // Reaproveita a validação de venda em andamento no PDV
            if (!PodeMudarDeTela())
                return;

            // Abre o login ANTES de fechar a janela atual, para que o
            // ShutdownMode.OnLastWindowClose não encerre a aplicação.
            var loginWindow = new View.LoginWindow();
            Application.Current.MainWindow = loginWindow;
            loginWindow.Show();

            var mainWindow = Application.Current.Windows
                .OfType<MainWindow>()
                .FirstOrDefault();

            mainWindow?.Close();
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