using System;
using System.Collections.Generic;
using System.Text;
using mercado.Model;
using Microsoft.EntityFrameworkCore;

namespace mercado.Data
{
    public class MercadoContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }

        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
        public DbSet<PagamentoVenda> PagamentosVenda { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MercadoDB;Trusted_Connection=True;");
        }
    }
}
