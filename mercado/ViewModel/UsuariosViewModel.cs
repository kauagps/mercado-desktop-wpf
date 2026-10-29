using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;
using mercado.Service;
using mercado.Model;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows;

namespace mercado.ViewModel
{
    public class UsuariosViewModel : INotifyPropertyChanged
    {
        private readonly UsuarioService _usuarioService;
        private readonly int _usuarioLogadoId;
        private ObservableCollection<Usuario> _usuariosLista = new ObservableCollection<Usuario>();
        private Usuario _novoUsuario = new Usuario();

        public ObservableCollection<Usuario> UsuariosLista
        {
            get { return _usuariosLista; }
            set { _usuariosLista = value; onPropertyChanged(nameof(UsuariosLista)); }
        }

        public Usuario NovoUsuario
        {
            get { return _novoUsuario; }
            set { _novoUsuario = value; onPropertyChanged(nameof(NovoUsuario)); }
        }

        /// <summary>
        /// Confirmação da senha digitada no cadastro. Preenchida pelo code-behind,
        /// pois PasswordBox não suporta binding.
        /// </summary>
        public string ConfirmacaoSenha { get; set; } = string.Empty;

        // --- Variaveis Buscar --------------
        private string _textoBusca = string.Empty;
        public string TextoBusca
        {
            get { return _textoBusca; }
            set
            {
                if (_textoBusca != value)
                {
                    _textoBusca = value;
                    PaginaAtual = 1;
                    CarregarUsuarios();
                    onPropertyChanged(nameof(TextoBusca));
                }
            }
        }

        //--- Variaveis de controle da paginação --------------------
        private int _totalItens;
        public int TotalItens
        {
            get { return _totalItens; }
            set { _totalItens = value; onPropertyChanged(nameof(TotalItens)); }
        }

        private int _paginaAtual = 1;
        public int PaginaAtual
        {
            get { return _paginaAtual; }
            set { _paginaAtual = value; onPropertyChanged(nameof(PaginaAtual)); }
        }

        private int _totalPaginas = 1;
        public int TotalPaginas
        {
            get { return _totalPaginas; }
            set { _totalPaginas = value; onPropertyChanged(nameof(TotalPaginas)); }
        }

        public ObservableCollection<int> OpcoesItensPorPagina { get; set; } = new ObservableCollection<int> { 5, 20, 50, 200, 500, 1000 };

        private int _itensPorPagina = 30;
        public int ItensPorPagina
        {
            get { return _itensPorPagina; }
            set
            {
                if (_itensPorPagina != value)
                {
                    _itensPorPagina = value;
                    PaginaAtual = 1;
                    onPropertyChanged(nameof(ItensPorPagina));
                    CarregarUsuarios();
                }
            }
        }

        public ICommand PaginaAnteriorCommand { get; set; }
        public ICommand ProximaPaginaCommand { get; set; }
        public ICommand AdicionarUsuarioCommand { get; set; }
        public ICommand EditarUsuarioCommand { get; set; }
        public ICommand InativarUsuarioCommand { get; set; }
        public ICommand ExcluirUsuarioCommand { get; set; }

        public UsuariosViewModel(int usuarioLogadoId)
        {
            _usuarioService = new UsuarioService();
            _usuarioLogadoId = usuarioLogadoId;
            NovoUsuario = new Usuario();
            UsuariosLista = new ObservableCollection<Usuario>();

            AdicionarUsuarioCommand = new RelayCommand(_ => SalvarNovoUsuario());
            EditarUsuarioCommand = new RelayCommand(AbrirTelaEdicao);
            InativarUsuarioCommand = new RelayCommand(InativarUsuario);
            ExcluirUsuarioCommand = new RelayCommand(ExcluirUsuario);
            PaginaAnteriorCommand = new RelayCommand(PaginaAnterior);
            ProximaPaginaCommand = new RelayCommand(ProximaPagina);

            CarregarUsuarios();
        }

        private void PaginaAnterior(object? obj)
        {
            if (PaginaAtual > 1)
            {
                PaginaAtual--;
                CarregarUsuarios();
            }
        }

        private void ProximaPagina(object? obj)
        {
            if (PaginaAtual < TotalPaginas)
            {
                PaginaAtual++;
                CarregarUsuarios();
            }
        }

        private void AbrirTelaEdicao(object? obj)
        {
            if (obj is Usuario usuarioSelecionado)
            {
                var janelaEdicao = new View.EditarUsuarioWindow(usuarioSelecionado, _usuarioLogadoId);
                janelaEdicao.ShowDialog();
                CarregarUsuarios();
            }
        }

        private void InativarUsuario(object? obj)
        {
            if (obj is Usuario usuarioSelecionado)
            {
                if (usuarioSelecionado.Id == _usuarioLogadoId && usuarioSelecionado.Ativo)
                {
                    MessageBox.Show(
                        "Você não pode inativar o próprio usuário logado.",
                        "Ação Bloqueada",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                var acao = usuarioSelecionado.Ativo ? "inativar" : "ativar";
                var resposta = MessageBox.Show(
                    $"Tem certeza que deseja {acao} o usuário: {usuarioSelecionado.Nome}?",
                    $"Confirmar {acao.FirstCharToUpper() + acao.Substring(1)}",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resposta == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Não altera o objeto exibido na lista antes da validação.
                        var atualizado = new Usuario
                        {
                            Id = usuarioSelecionado.Id,
                            Nome = usuarioSelecionado.Nome,
                            Tipo = usuarioSelecionado.Tipo,
                            SenhaHash = usuarioSelecionado.SenhaHash,
                            Ativo = !usuarioSelecionado.Ativo,
                            DeveTrocarSenha = usuarioSelecionado.DeveTrocarSenha
                        };

                        _usuarioService.AtualizarUsuario(atualizado, _usuarioLogadoId);
                        CarregarUsuarios();
                        MessageBox.Show($"Usuário {atualizado.Nome} {(atualizado.Ativo ? "ativado" : "inativado")} com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Recarrega para descartar a alteração local que foi bloqueada
                        CarregarUsuarios();
                        MessageBox.Show(ex.Message, "Ação Bloqueada", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    catch (Exception ex)
                    {
                        CarregarUsuarios();
                        MessageBox.Show($"Erro ao atualizar usuário: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ExcluirUsuario(object? obj)
        {
            if (obj is Usuario usuarioSelecionado)
            {
                if (usuarioSelecionado.Id == _usuarioLogadoId)
                {
                    MessageBox.Show(
                        "Você não pode excluir o próprio usuário logado.",
                        "Ação Bloqueada",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Verificar se pode excluir (BR-05)
                if (!_usuarioService.PodeExcluir(usuarioSelecionado.Id))
                {
                    MessageBox.Show("Não é possível excluir o último administrador ativo do sistema.", "Ação Bloqueada", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var resposta = MessageBox.Show(
                    $"Tem certeza que deseja excluir o usuário: {usuarioSelecionado.Nome}?",
                    "Confirmar Exclusão",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resposta == MessageBoxResult.Yes)
                {
                    try
                    {
                        _usuarioService.ExcluirUsuario(usuarioSelecionado, _usuarioLogadoId);
                        CarregarUsuarios();
                        MessageBox.Show($"Usuário {usuarioSelecionado.Nome} excluído com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (InvalidOperationException ex)
                    {
                        CarregarUsuarios();
                        MessageBox.Show(ex.Message, "Ação Bloqueada", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    catch (Exception ex)
                    {
                        CarregarUsuarios();
                        MessageBox.Show($"Erro ao excluir usuário: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CarregarUsuarios()
        {
            var listaDoBanco = _usuarioService.ListarTodos();

            if (!string.IsNullOrWhiteSpace(TextoBusca))
            {
                listaDoBanco = listaDoBanco
                    .Where(u => u.Nome.Contains(TextoBusca, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            TotalItens = listaDoBanco.Count;

            if (TotalItens == 0)
            {
                TotalPaginas = 1;
                UsuariosLista = new ObservableCollection<Usuario>();
                return;
            }

            TotalPaginas = (int)Math.Ceiling((double)TotalItens / ItensPorPagina);

            if (PaginaAtual > TotalPaginas)
            {
                PaginaAtual = TotalPaginas;
            }

            var itensPaginados = listaDoBanco
                .OrderBy(u => u.Nome)
                .Skip((PaginaAtual - 1) * ItensPorPagina)
                .Take(ItensPorPagina)
                .ToList();

            UsuariosLista = new ObservableCollection<Usuario>(itensPaginados);
        }

        public bool SalvarNovoUsuario()
        {
            if (string.IsNullOrWhiteSpace(NovoUsuario.Nome) ||
                string.IsNullOrWhiteSpace(NovoUsuario.SenhaHash) ||
                string.IsNullOrWhiteSpace(ConfirmacaoSenha))
            {
                MessageBox.Show(
                    "Por favor, preencha Nome, Senha e Confirmação de Senha.",
                    "Campos Obrigatórios",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            if (NovoUsuario.SenhaHash != ConfirmacaoSenha)
            {
                MessageBox.Show(
                    "As senhas não conferem. Digite a mesma senha nos dois campos.",
                    "Senhas Diferentes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            if (_usuarioService.NomeJaExiste(NovoUsuario.Nome))
            {
                MessageBox.Show(
                    "Já existe um usuário com este nome!",
                    "Nome Duplicado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            try
            {
                _usuarioService.AdicionarUsuario(NovoUsuario);
                NovoUsuario = new Usuario();
                ConfirmacaoSenha = string.Empty;
                MessageBox.Show("Usuário adicionado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                CarregarUsuarios();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar usuário: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void onPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Extension method para capitalizar primeira letra
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}