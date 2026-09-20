using System;
using System.Collections.Generic;
using System.Text;
using mercado.Model;
using mercado.Data;

namespace mercado.Service
{
    public class VendaService
    {
        private readonly MercadoContext _context;

        public VendaService()
        {
            _context = new MercadoContext();
        }

        public void SalvarVendaCompleta(Venda venda)
        {
            _context.Vendas.Add(venda);

            foreach (var item in venda.Itens)
            {
                var produtoNoBanco = _context.Produtos.Find(item.Produto);

                if (produtoNoBanco != null)
                {
                    produtoNoBanco.QuantidadeAtual -= item.Quantidade;
                }
            }


            _context.SaveChanges();
        }
    }
}
