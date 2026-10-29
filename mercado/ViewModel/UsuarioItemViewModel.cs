using mercado.Model;

namespace mercado.ViewModel
{
    /// <summary>
    /// Item exibido na lista de usuários, com as permissões de ação
    /// calculadas para o usuário logado.
    /// </summary>
    public class UsuarioItemViewModel
    {
        public Usuario Usuario { get; }

        public int Id => Usuario.Id;
        public string Nome => Usuario.Nome;
        public TipoUsuario Tipo => Usuario.Tipo;
        public bool Ativo => Usuario.Ativo;

        public bool PodeInativar { get; }
        public bool PodeExcluir { get; }
        public string ToolTipInativar { get; }
        public string ToolTipExcluir { get; }

        public UsuarioItemViewModel(Usuario usuario, int usuarioLogadoId, int totalAdminsAtivos)
        {
            Usuario = usuario;

            bool ehProprioUsuario = usuario.Id == usuarioLogadoId;
            bool ehAdminAtivo = usuario.Tipo == TipoUsuario.Administrador && usuario.Ativo;
            bool ehUltimoAdminAtivo = ehAdminAtivo && totalAdminsAtivos <= 1;

            PodeInativar = !ehProprioUsuario && !ehUltimoAdminAtivo;
            PodeExcluir = !ehProprioUsuario && !ehUltimoAdminAtivo;

            if (ehProprioUsuario)
            {
                ToolTipInativar = "Você não pode inativar o próprio usuário logado.";
                ToolTipExcluir = "Você não pode excluir o próprio usuário logado.";
            }
            else if (ehUltimoAdminAtivo)
            {
                ToolTipInativar = "Não é possível inativar o último administrador ativo.";
                ToolTipExcluir = "Não é possível excluir o último administrador ativo.";
            }
            else
            {
                ToolTipInativar = usuario.Ativo ? "Inativar usuário" : "Ativar usuário";
                ToolTipExcluir = "Excluir usuário";
            }
        }
    }
}
