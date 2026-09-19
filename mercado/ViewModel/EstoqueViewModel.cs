using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using mercado.Service;
using mercado.Model;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows;

namespace mercado.ViewModel
{
    public class EstoqueViewModel : INotifyPropertyChanged
    {
        private readonly ProdutoService _produtoService;
        private ObservableCollection<Produto> _produtosLista = new ObservableCollection<Produto>();
        private Produto _novoProduto = new Produto();

        public ObservableCollection<Produto> ProdutosLista
        {
            get { return _produtosLista; }
            set { _produtosLista = value; onPropertyChanged(nameof(ProdutosLista)); }
        }

        public Produto NovoProduto
        {
            get { return _novoProduto; }
            set { _novoProduto = value; onPropertyChanged(nameof(NovoProduto)); }
        }


        //--- Variaveis de controle da paginação --------------------

        private int _totalItens;
        public int TotalItens
        {
            get { return _totalItens; }
            set { _totalItens = value; onPropertyChanged(nameof(TotalItens)); }
        }

        // Variável para armazenar a página atual
        private int _paginaAtual = 1;
        public int PaginaAtual
        {
            get { return _paginaAtual; }
            set { _paginaAtual = value; onPropertyChanged(nameof(PaginaAtual)); }
        }

        // Variável para armazenar o total de páginas
        private int _totalPaginas = 1;
        public int TotalPaginas
        {
            get { return _totalPaginas; }
            set { _totalPaginas = value; onPropertyChanged(nameof(TotalPaginas)); }
        }

        // Opções para o número de itens por página
        public ObservableCollection<int> OpcoesItensPorPagina { get; set; } = new ObservableCollection<int> { 5, 20, 50, 200, 500, 1000 };

        private int _itensPorPagina = 5;
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

                    CarregarProdutos();
                }
                
            }
        }

        public ICommand PaginaAnteriorCommand { get; set; }
        public ICommand ProximaPaginaCommand { get; set; }

        public ICommand AdicionarProdutoCommand { get; set; }
        public ICommand EditarProdutoCommand { get; set; }
        public ICommand InativarProdutoCommand { get; set; }
        public ICommand ExcluirProdutoCommand { get; set; }

        // ---- Metodos de controle da paginação ---------------------------------
        private void PaginaAnterior(object? obj)
        {
            if (PaginaAtual > 1)
            {
                PaginaAtual--;
                CarregarProdutos();
            }
        }

        private void ProximaPagina(object? obj)
        {
            if (PaginaAtual < TotalPaginas)
            {
                PaginaAtual++;
                CarregarProdutos();
            }
        }

        private void AbrirTelaEdicao(object? obj)
        {
            if (obj is Produto produtoSelecionado)
            {
                var janelaEdicao = new View.EditarProdutoWindow(produtoSelecionado);

                janelaEdicao.ShowDialog();

                CarregarProdutos();
            }
        }

        private void InativarProduto(object? obj)
        {
            if (obj is Produto produtoSelecionado)
            {
                var resposta = MessageBox.Show(
                    $"Tem certeza que deseja inativar o produto: {produtoSelecionado.Nome}?",
                    "Confirmar Inativação",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resposta == MessageBoxResult.Yes)
                {
                    if (produtoSelecionado.Ativo == false)
                    {
                        MessageBox.Show($"O produto {produtoSelecionado.Nome} já está inativo.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    produtoSelecionado.Ativo = false;
                    _produtoService.AtualizarProduto(produtoSelecionado);

                    CarregarProdutos();

                    MessageBox.Show($"Produto {produtoSelecionado.Nome} inativado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            
        }

        private void ExcluirProduto(object? obj)
        {
            if (obj is Produto produtoSelecionado)
            {
                var resposta = MessageBox.Show(
                    $"Tem certeza que deseja excluir o produto: {produtoSelecionado.Nome}?",
                    "Confirmar Exclusão",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resposta == MessageBoxResult.Yes)
                {
                    _produtoService.ExcluirProduto(produtoSelecionado);
                    CarregarProdutos();
                    MessageBox.Show($"Produto {produtoSelecionado.Nome} excluído com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        public EstoqueViewModel()
        {
            _produtoService = new ProdutoService();
            NovoProduto = new Produto();
            ProdutosLista = new ObservableCollection<Produto>();

            AdicionarProdutoCommand = new RelayCommand(_ => SalvarNovoProduto());
            EditarProdutoCommand = new RelayCommand(AbrirTelaEdicao);
            InativarProdutoCommand = new RelayCommand(InativarProduto);
            ExcluirProdutoCommand = new RelayCommand(ExcluirProduto);
            PaginaAnteriorCommand = new RelayCommand(PaginaAnterior);
            ProximaPaginaCommand = new RelayCommand(ProximaPagina);

            CarregarProdutos();
        }

        private void CarregarProdutos()
        {
            var listaDoBanco = _produtoService.ListarTodos();

            TotalItens = listaDoBanco.Count;

            if (TotalItens == 0)
            {
                TotalPaginas = 1;
                ProdutosLista = new ObservableCollection<Produto>();
                return;
            }

            TotalPaginas = (int)Math.Ceiling((double)TotalItens / ItensPorPagina);

            if (PaginaAtual > TotalPaginas)
            {
                PaginaAtual = TotalPaginas;
            }

            var itensPaginados = listaDoBanco
                .OrderBy(p => p.Nome)
                .Skip((PaginaAtual - 1) * ItensPorPagina)
                .Take(ItensPorPagina)
                .ToList();

            ProdutosLista = new ObservableCollection<Produto>(itensPaginados);
        }

        private void SalvarNovoProduto()
        {
            NovoProduto.Custo = Math.Round(NovoProduto.Custo,2);
            NovoProduto.Lucro = Math.Round(NovoProduto.Lucro,2);
            NovoProduto.ValorVenda = Math.Round(NovoProduto.ValorVenda,2);
            NovoProduto.QuantidadeMin = Math.Round(NovoProduto.QuantidadeMin,2);
            NovoProduto.QuantidadeAtual = Math.Round(NovoProduto.QuantidadeAtual,2);

            if (string.IsNullOrWhiteSpace(NovoProduto.Nome) || 
                NovoProduto.Custo < 0 ||
                NovoProduto.Lucro < 0)
            {
                MessageBox.Show(
                    "Por Favor, preencha Nome, Custo e Lucro corretamente.",
                    "Campos Obrigatórios",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            _produtoService.AdicionarProduto(NovoProduto);

            NovoProduto = new Produto();

            MessageBox.Show($"Produto {NovoProduto.Nome} adicionado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            CarregarProdutos();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void onPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}