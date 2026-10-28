using System.Windows;
using System.Windows.Input;
using mercado.Service;

namespace mercado.View
{
    public partial class LoginWindow : Window
    {
        private readonly UsuarioService _usuarioService;

        public LoginWindow()
        {
            InitializeComponent();
            _usuarioService = new UsuarioService();
            txtUsuario.Focus();
        }

        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            TentarLogin();
        }

        private void txtSenha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TentarLogin();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
            }
        }

        private void TentarLogin()
        {
            string usuario = txtUsuario.Text.Trim();
            string senha = txtSenha.Password;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(senha))
            {
                MostrarErro("Por favor, informe usuário e senha.");
                return;
            }

            var usuarioLogado = _usuarioService.Autenticar(usuario, senha);

            if (usuarioLogado != null)
            {
                // Login bem-sucedido - abrir MainWindow
                var mainWindow = new MainWindow(usuarioLogado);
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MostrarErro("Usuário ou senha inválidos.");
                txtSenha.Clear();
                txtSenha.Focus();
            }
        }

        private void MostrarErro(string mensagem)
        {
            txtErro.Text = mensagem;
            txtErro.Visibility = Visibility.Visible;
        }
    }
}