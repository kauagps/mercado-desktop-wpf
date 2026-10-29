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
            // PasswordBox não faz binding: transfere as senhas para o ViewModel
            // e só limpa os campos quando o cadastro for concluído com sucesso.
            if (DataContext is mercado.ViewModel.UsuariosViewModel vm)
            {
                vm.NovoUsuario.SenhaHash = txtNovaSenha.Password;
                vm.ConfirmacaoSenha = txtConfirmarSenha.Password;

                if (vm.SalvarNovoUsuario())
                {
                    txtNovaSenha.Clear();
                    txtConfirmarSenha.Clear();
                }
            }
        }
    }
}