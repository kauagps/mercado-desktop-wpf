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
        private readonly MercadoContext _context;

        public UsuarioService()
        {
            _context = new MercadoContext();
        }

        public List<Usuario> ListarTodos()
        {
            return _context.Usuarios.OrderBy(u => u.Nome).ToList();
        }

        public Usuario? BuscarPorNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return null;

            return _context.Usuarios.FirstOrDefault(u => u.Nome == nome);
        }

        public Usuario? BuscarPorId(int id)
        {
            return _context.Usuarios.Find(id);
        }

        public bool NomeJaExiste(string nome, int idUsuarioAtual = 0)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return false;

            return _context.Usuarios.Any(u => u.Nome == nome && u.Id != idUsuarioAtual);
        }

        public void AdicionarUsuario(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Nome))
                throw new ArgumentException("Nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(usuario.SenhaHash))
                throw new ArgumentException("Senha é obrigatória.");

            if (NomeJaExiste(usuario.Nome))
                throw new InvalidOperationException("Já existe um usuário com este nome.");

            // Hash da senha se ainda não estiver hasheada
            if (!usuario.SenhaHash.StartsWith("$2a$") && !usuario.SenhaHash.StartsWith("$2b$") && !usuario.SenhaHash.StartsWith("$2y$"))
            {
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.SenhaHash);
            }

            usuario.DataCriacao = DateTime.Now;
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        public void AtualizarUsuario(Usuario usuario)
        {
            var usuarioExistente = _context.Usuarios.Find(usuario.Id);
            if (usuarioExistente == null)
                throw new InvalidOperationException("Usuário não encontrado.");

            if (string.IsNullOrWhiteSpace(usuario.Nome))
                throw new ArgumentException("Nome é obrigatório.");

            if (NomeJaExiste(usuario.Nome, usuario.Id))
                throw new InvalidOperationException("Já existe outro usuário com este nome.");

            // BR-05: não permitir inativar ou rebaixar o último administrador ativo
            ValidarPeloMenosUmAdminAtivo(usuarioExistente, usuario.Tipo, usuario.Ativo);

            // Se senha foi alterada (não está vazia e não é o hash atual), hashear nova senha
            if (!string.IsNullOrWhiteSpace(usuario.SenhaHash) && usuario.SenhaHash != usuarioExistente.SenhaHash)
            {
                if (!usuario.SenhaHash.StartsWith("$2a$") && !usuario.SenhaHash.StartsWith("$2b$") && !usuario.SenhaHash.StartsWith("$2y$"))
                {
                    usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.SenhaHash);
                }
            }
            else
            {
                // Manter hash original se senha não foi alterada
                usuario.SenhaHash = usuarioExistente.SenhaHash;
            }

            usuarioExistente.Nome = usuario.Nome;
            usuarioExistente.Tipo = usuario.Tipo;
            usuarioExistente.SenhaHash = usuario.SenhaHash;
            usuarioExistente.Ativo = usuario.Ativo;

            _context.Usuarios.Update(usuarioExistente);
            _context.SaveChanges();
        }

        public void ExcluirUsuario(Usuario usuario)
        {
            var usuarioExistente = _context.Usuarios.Find(usuario.Id);
            if (usuarioExistente == null)
                throw new InvalidOperationException("Usuário não encontrado.");

            // BR-05: não permitir excluir o último administrador ativo
            ValidarPeloMenosUmAdminAtivo(usuarioExistente, usuarioExistente.Tipo, false);

            _context.Usuarios.Remove(usuarioExistente);
            _context.SaveChanges();
        }

        /// <summary>
        /// BR-05: garante que sempre exista pelo menos um administrador ativo.
        /// A validação considera a situação atual no banco e o novo estado desejado.
        /// </summary>
        private void ValidarPeloMenosUmAdminAtivo(Usuario usuarioExistente, TipoUsuario novoTipo, bool novoAtivo)
        {
            // A restrição só se aplica quando o usuário era administrador ativo
            // e deixará de ser (inativação, rebaixamento ou exclusão).
            if (usuarioExistente.Tipo != TipoUsuario.Administrador || !usuarioExistente.Ativo)
                return;

            bool continuaraAdminAtivo = novoTipo == TipoUsuario.Administrador && novoAtivo;
            if (continuaraAdminAtivo)
                return;

            bool existeOutroAdminAtivo = _context.Usuarios.Any(u =>
                u.Id != usuarioExistente.Id &&
                u.Tipo == TipoUsuario.Administrador &&
                u.Ativo);

            if (!existeOutroAdminAtivo)
            {
                throw new InvalidOperationException(
                    "Não é possível inativar, rebaixar ou excluir o último administrador ativo do sistema.");
            }
        }

        public Usuario? Autenticar(string nome, string senha)
        {
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(senha))
                return null;

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Nome == nome && u.Ativo);
            if (usuario == null)
                return null;

            bool senhaValida = BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash);
            return senhaValida ? usuario : null;
        }

        public int ContarAdminsAtivos()
        {
            return _context.Usuarios.Count(u => u.Tipo == TipoUsuario.Administrador && u.Ativo);
        }

        public bool ExisteOutroAdminAtivo(int usuarioId)
        {
            return _context.Usuarios.Any(u => u.Tipo == TipoUsuario.Administrador && u.Ativo && u.Id != usuarioId);
        }

        public bool PodeExcluir(Usuario usuario)
        {
            if (usuario.Tipo != TipoUsuario.Administrador || !usuario.Ativo)
                return true;

            return ContarAdminsAtivos() > 1;
        }
    }
}