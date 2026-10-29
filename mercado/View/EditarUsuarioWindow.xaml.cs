using mercado.Service;
using System;
using System.Windows;
using System.Windows.Controls;
using mercado.Model;

namespace mercado.View
{
    public partial class EditarUsuarioWindow : Window
    {
        private readonly Usuario _usuarioOriginal;
        private readonly int _usuarioLogadoId;
        private Usuario _usuarioClone;
        private UsuarioService _usuarioService;
        private bool _alteracaoSalva;

        /// <summary>
        /// Indica se a edição foi persistida com sucesso antes da janela fechar.
        /// </summary>
        public bool AlteracaoSalva => _alteracaoSalva;

        public EditarUsuarioWindow(Usuario usuario, int usuarioLogadoId)
        {
            InitializeComponent();

            _usuarioService = new UsuarioService();
            _usuarioOriginal = usuario;
            _usuarioLogadoId = usuarioLogadoId;

            _usuarioClone = new Usuario
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Tipo = usuario.Tipo,
                SenhaHash = usuario.SenhaHash, // Hash original (será mantido se não alterar senha)
                Ativo = usuario.Ativo,
                DeveTrocarSenha = usuario.DeveTrocarSenha,
                DataCriacao = usuario.DataCriacao
            };

            DataContext = _usuarioClone;

            // Na edição do próprio usuário, tipo e status ficam bloqueados.
            if (usuario.Id == usuarioLogadoId)
            {
                cmbTipo.IsEnabled = false;
                chkAtivo.IsEnabled = false;
                borderAvisoProprioUsuario.Visibility = Visibility.Visible;
            }
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            string novaSenha = txtNovaSenha.Password;
            string confirmarSenha = txtConfirmarNovaSenha.Password;

            // Validação de senha se foi preenchida
            if (!string.IsNullOrWhiteSpace(novaSenha) || !string.IsNullOrWhiteSpace(confirmarSenha))
            {
                if (string.IsNullOrWhiteSpace(novaSenha) || string.IsNullOrWhiteSpace(confirmarSenha))
                {
                    MessageBox.Show(
                        "Por favor, preencha ambos os campos de senha ou deixe ambos em branco.",
                        "Senha Incompleta",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (novaSenha != confirmarSenha)
                {
                    MessageBox.Show(
                        "As senhas não coincidem.",
                        "Senhas Diferentes",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Atualizar senha no clone (será hasheada no service)
                _usuarioClone.SenhaHash = novaSenha;
            }

            // Validação de campos obrigatórios
            if (string.IsNullOrWhiteSpace(_usuarioClone.Nome))
            {
                MessageBox.Show(
                    "Por favor, preencha o Nome.",
                    "Campo Obrigatório",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Verificar nome duplicado
            if (_usuarioService.NomeJaExiste(_usuarioClone.Nome, _usuarioClone.Id))
            {
                MessageBox.Show(
                    "Já existe outro usuário com este nome!",
                    "Nome Duplicado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // O usuário logado não pode inativar ou rebaixar a própria conta
            if (_usuarioClone.Id == _usuarioLogadoId)
            {
                if (_usuarioOriginal.Ativo && !_usuarioClone.Ativo)
                {
                    MessageBox.Show(
                        "Você não pode inativar o próprio usuário logado.",
                        "Ação Bloqueada",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (_usuarioOriginal.Tipo == TipoUsuario.Administrador && _usuarioClone.Tipo != TipoUsuario.Administrador)
                {
                    MessageBox.Show(
                        "Você não pode remover o próprio acesso de administrador.",
                        "Ação Bloqueada",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
            }

            // USU004 CA-2: confirmar antes de persistir a alteração
            var confirmacao = MessageBox.Show(
                $"Deseja salvar as alterações do usuário {_usuarioClone.Nome}?",
                "Confirmar Alteração",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacao != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                // Envia o clone para o service. A entidade exibida na lista não é
                // alterada antes da validação, evitando que uma alteração rejeitada
                // permaneça no objeto/contexto da tela.
                _usuarioService.AtualizarUsuario(_usuarioClone, _usuarioLogadoId);

                _alteracaoSalva = true;
                MessageBox.Show("Usuário atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar usuário: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Não pedir confirmação de cancelamento quando a janela foi fechada
            // porque a alteração já foi salva com sucesso.
            if (_alteracaoSalva)
            {
                e.Cancel = false;
                return;
            }

            MessageBoxResult resposta = MessageBox.Show(
                "Tem certeza que deseja cancelar a operação e fechar a tela?",
                "Confirmação de Cancelamento",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resposta == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }
    }
}