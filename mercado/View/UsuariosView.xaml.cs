using mercado.Service;
using System.Windows;
using System.Windows.Controls;
using mercado.Model;

namespace mercado.View
{
    public partial class UsuariosView : UserControl
    {
        public UsuariosView()
        {
            InitializeComponent();
        }

        private void btnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            // Transferir senhas do PasswordBox para o ViewModel
            if (DataContext is mercado.ViewModel.UsuariosViewModel vm)
            {
                vm.NovoUsuario.SenhaHash = txtNovaSenha.Password;
            }
        }
    }
}