using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using mercado.Data;
using mercado.Model;


namespace mercado.Service
{
    public class ProdutoService
    {
        private readonly MercadoContext _context;

        public ProdutoService()
        {
            _context = new MercadoContext();
        }

        public List<Produto> ListarTodos()
        {
            return _context.Produtos.ToList();
        }

        public void AdicionarProduto(Produto novoProduto)
        {
            _context.Produtos.Add(novoProduto);
            _context.SaveChanges();
        }
    }
}
