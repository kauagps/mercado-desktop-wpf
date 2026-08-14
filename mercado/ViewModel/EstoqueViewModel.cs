using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using mercado.Service;
using mercado.Model;
using System.Windows.Input;
using System.ComponentModel;

namespace mercado.ViewModel
{
    public class EstoqueViewModel : INotifyPropertyChanged
    {
        private readonly ProdutoService _produtoService;
        private ObservableCollection<Produto>? _produtosLista;
        private Produto? _novoProduto;


        public ObservableCollection<Produto> ProdutosLista
        {
            get { return _produtosLista!; }
            set { _produtosLista = value; onPropertyChanged(nameof(ProdutosLista)); }
        }

        public Produto NovoProduto
        {
            get { return _novoProduto!; }
            set { _novoProduto = value; onPropertyChanged(nameof(NovoProduto)); }
        }

        public ICommand AdicionarProdutoCommand { get; set; }

        public EstoqueViewModel()
        {
            _produtoService = new ProdutoService();
            NovoProduto = new Produto();

            AdicionarProdutoCommand = new RelayCommand(SalvarNovoProduto);

            CarregarProdutos();
        }

        private void CarregarProdutos()
        {
            var listaDoBanco = _produtoService.ListarTodos();

            ProdutosLista = new ObservableCollection<Produto>(listaDoBanco);
        }

        private void SalvarNovoProduto()
        {
            _produtoService.AdicionarProduto(NovoProduto);

            ProdutosLista.Add(NovoProduto);

            NovoProduto = new Produto();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void onPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
