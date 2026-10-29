using System;
using System.Collections.Generic;
using System.Linq;
using mercado.Data;
using mercado.Model;
using BCrypt.Net;

namespace mercado.Service
{
    public class UsuarioService
    {
        public List<Usuario> ListarTodos()
        {
            using var context = new MercadoContext();
            return context.Usuarios.OrderBy(u => u.Nome).ToList();
        }

        public Usuario? BuscarPorNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return null;

            using var context = new MercadoContext();
            return context.Usuarios.FirstOrDefault(u => u.Nome == nome);
        }

        public Usuario? BuscarPorId(int id)
        {
            using var context = new MercadoContext();
            return context.Usuarios.Find(id);
        }

        public bool NomeJaExiste(string nome, int idUsuarioAtual = 0)
        {
            using var context = new MercadoContext();
            return NomeJaExiste(context, nome, idUsuarioAtual);
        }

        private static bool NomeJaExiste(MercadoContext context, string nome, int idUsuarioAtual = 0)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return false;

            return context.Usuarios.Any(u => u.Nome == nome && u.Id != idUsuarioAtual);
        }

        public void AdicionarUsuario(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Nome))
                throw new ArgumentException("Nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(usuario.SenhaHash))
                throw new ArgumentException("Senha é obrigatória.");

            using var context = new MercadoContext();

            if (NomeJaExiste(context, usuario.Nome))
                throw new InvalidOperationException("Já existe um usuário com este nome.");

            // Hash da senha se ainda não estiver hasheada
            string senhaHash = usuario.SenhaHash;
            if (!EhHash(senhaHash))
            {
                ValidarPoliticaSenha(senhaHash);
                senhaHash = BCrypt.Net.BCrypt.HashPassword(senhaHash);
            }

            usuario.SenhaHash = senhaHash;
            usuario.DataCriacao = DateTime.Now;

            context.Usuarios.Add(usuario);
            context.SaveChanges();
        }

        public void AtualizarUsuario(Usuario usuario, int? usuarioLogadoId = null)
        {
            using var context = new MercadoContext();

            var usuarioExistente = context.Usuarios.Find(usuario.Id);
            if (usuarioExistente == null)
                throw new InvalidOperationException("Usuário não encontrado.");

            if (string.IsNullOrWhiteSpace(usuario.Nome))
                throw new ArgumentException("Nome é obrigatório.");

            if (NomeJaExiste(context, usuario.Nome, usuario.Id))
                throw new InvalidOperationException("Já existe outro usuário com este nome.");

            // Impede que o usuário logado inative ou rebaixe a si mesmo
            ValidarAlteracaoDoProprioUsuario(usuarioExistente, usuario, usuarioLogadoId);

            // BR-05: não permitir inativar ou rebaixar o último administrador ativo
            ValidarPeloMenosUmAdminAtivo(context, usuarioExistente, usuario.Tipo, usuario.Ativo);

            // Se senha foi alterada (não está vazia e não é o hash atual), hashear nova senha.
            // O objeto recebido não é mutado: o hash é calculado localmente.
            string senhaHash = usuarioExistente.SenhaHash;
            if (!string.IsNullOrWhiteSpace(usuario.SenhaHash) && usuario.SenhaHash != usuarioExistente.SenhaHash)
            {
                senhaHash = usuario.SenhaHash;
                if (!EhHash(senhaHash))
                {
                    ValidarPoliticaSenha(senhaHash);
                    senhaHash = BCrypt.Net.BCrypt.HashPassword(senhaHash);
                }
            }

            usuarioExistente.Nome = usuario.Nome;
            usuarioExistente.Tipo = usuario.Tipo;
            usuarioExistente.SenhaHash = senhaHash;
            usuarioExistente.Ativo = usuario.Ativo;

            context.SaveChanges();
        }

        public void ExcluirUsuario(Usuario usuario, int? usuarioLogadoId = null)
        {
            using var context = new MercadoContext();

            var usuarioExistente = context.Usuarios.Find(usuario.Id);
            if (usuarioExistente == null)
                throw new InvalidOperationException("Usuário não encontrado.");

            if (usuarioLogadoId.HasValue && usuarioExistente.Id == usuarioLogadoId.Value)
                throw new InvalidOperationException("Você não pode excluir o próprio usuário logado.");

            // BR-05: não permitir excluir o último administrador ativo
            ValidarPeloMenosUmAdminAtivo(context, usuarioExistente, usuarioExistente.Tipo, false);

            context.Usuarios.Remove(usuarioExistente);
            context.SaveChanges();
        }

        private static void ValidarAlteracaoDoProprioUsuario(Usuario usuarioExistente, Usuario usuario, int? usuarioLogadoId)
        {
            if (!usuarioLogadoId.HasValue || usuarioExistente.Id != usuarioLogadoId.Value)
                return;

            if (!usuario.Ativo)
                throw new InvalidOperationException("Você não pode inativar o próprio usuário logado.");

            if (usuarioExistente.Tipo == TipoUsuario.Administrador && usuario.Tipo != TipoUsuario.Administrador)
                throw new InvalidOperationException("Você não pode remover o próprio acesso de administrador.");
        }

        /// <summary>
        /// BR-05: garante que sempre exista pelo menos um administrador ativo.
        /// A validação considera a situação atual no banco e o novo estado desejado.
        /// </summary>
        private static void ValidarPeloMenosUmAdminAtivo(MercadoContext context, Usuario usuarioExistente, TipoUsuario novoTipo, bool novoAtivo)
        {
            // A restrição só se aplica quando o usuário era administrador ativo
            // e deixará de ser (inativação, rebaixamento ou exclusão).
            if (usuarioExistente.Tipo != TipoUsuario.Administrador || !usuarioExistente.Ativo)
                return;

            bool continuaraAdminAtivo = novoTipo == TipoUsuario.Administrador && novoAtivo;
            if (continuaraAdminAtivo)
                return;

            bool existeOutroAdminAtivo = context.Usuarios.Any(u =>
                u.Id != usuarioExistente.Id &&
                u.Tipo == TipoUsuario.Administrador &&
                u.Ativo);

            if (!existeOutroAdminAtivo)
            {
                throw new InvalidOperationException(
                    "Não é possível inativar, rebaixar ou excluir o último administrador ativo do sistema.");
            }
        }

        /// <summary>
        /// Altera a senha do usuário e remove a exigência de troca no próximo login.
        /// </summary>
        public void AlterarSenha(int usuarioId, string novaSenha)
        {
            if (string.IsNullOrWhiteSpace(novaSenha))
                throw new ArgumentException("A nova senha é obrigatória.");

            ValidarPoliticaSenha(novaSenha);

            using var context = new MercadoContext();

            var usuario = context.Usuarios.Find(usuarioId);
            if (usuario == null)
                throw new InvalidOperationException("Usuário não encontrado.");

            if (BCrypt.Net.BCrypt.Verify(novaSenha, usuario.SenhaHash))
                throw new InvalidOperationException("A nova senha deve ser diferente da senha atual.");

            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);
            usuario.DeveTrocarSenha = false;
            context.SaveChanges();
        }

        /// <summary>
        /// Política mínima de senha: 8 caracteres, com pelo menos uma letra e um número.
        /// </summary>
        public static void ValidarPoliticaSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha) || senha.Length < 8)
                throw new ArgumentException("A senha deve ter pelo menos 8 caracteres.");

            if (!senha.Any(char.IsLetter) || !senha.Any(char.IsDigit))
                throw new ArgumentException("A senha deve conter pelo menos uma letra e um número.");
        }

        private static bool EhHash(string valor)
        {
            return valor.StartsWith("$2a$") || valor.StartsWith("$2b$") || valor.StartsWith("$2y$");
        }

        public Usuario? Autenticar(string nome, string senha)
        {
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(senha))
                return null;

            using var context = new MercadoContext();
            var usuario = context.Usuarios.FirstOrDefault(u => u.Nome == nome && u.Ativo);
            if (usuario == null)
                return null;

            bool senhaValida = BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash);
            return senhaValida ? usuario : null;
        }

        public int ContarAdminsAtivos()
        {
            using var context = new MercadoContext();
            return context.Usuarios.Count(u => u.Tipo == TipoUsuario.Administrador && u.Ativo);
        }

        public bool ExisteOutroAdminAtivo(int usuarioId)
        {
            using var context = new MercadoContext();
            return context.Usuarios.Any(u => u.Tipo == TipoUsuario.Administrador && u.Ativo && u.Id != usuarioId);
        }

        public bool PodeExcluir(int usuarioId)
        {
            using var context = new MercadoContext();
            var usuario = context.Usuarios.Find(usuarioId);
            if (usuario == null)
                return false;

            if (usuario.Tipo != TipoUsuario.Administrador || !usuario.Ativo)
                return true;

            return context.Usuarios.Count(u => u.Tipo == TipoUsuario.Administrador && u.Ativo) > 1;
        }
    }
}
