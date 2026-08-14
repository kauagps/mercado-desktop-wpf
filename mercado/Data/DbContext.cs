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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MercadoDB;Trusted_Connection=True;");
        }
    }
}
