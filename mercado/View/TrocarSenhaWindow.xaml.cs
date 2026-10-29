using System;
using System.Windows;
using System.Windows.Input;
using mercado.Model;
using mercado.Service;

namespace mercado.View
{
    public partial class TrocarSenhaWindow : Window
    {
        private readonly Usuario _usuario;
        private readonly UsuarioService _usuarioService;

        public TrocarSenhaWindow(Usuario usuario)
        {
            InitializeComponent();

            _usuario = usuario;
            _usuarioService = new UsuarioService();
            txtMensagem.Text = $"Olá, {usuario.Nome}. Por segurança, defina uma nova senha antes de continuar.";

            Loaded += (_, _) => txtNovaSenha.Focus();
        }

        private void btnAlterar_Click(object sender, RoutedEventArgs e)
        {
            AlterarSenha();
        }

        private void txtSenha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AlterarSenha();
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }

        private void AlterarSenha()
        {
            string novaSenha = txtNovaSenha.Password;
            string confirmacao = txtConfirmarSenha.Password;

            if (string.IsNullOrWhiteSpace(novaSenha) || string.IsNullOrWhiteSpace(confirmacao))
            {
                MostrarErro("Preencha os dois campos de senha.");
                return;
            }

            if (novaSenha != confirmacao)
            {
                MostrarErro("As senhas não conferem.");
                return;
            }

            try
            {
                _usuarioService.AlterarSenha(_usuario.Id, novaSenha);
                _usuario.DeveTrocarSenha = false;
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void MostrarErro(string mensagem)
        {
            txtErro.Text = mensagem;
            txtErro.Visibility = Visibility.Visible;
            txtNovaSenha.Clear();
            txtConfirmarSenha.Clear();
            txtNovaSenha.Focus();
        }
    }
}
