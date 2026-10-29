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
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MercadoDB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed: Usuário administrador padrão (senha: admin123)
            // Hash fixo pré-computado para evitar warning de modelo dinâmico
            const string adminHash = "$2a$11$7jk9L7TvgE2tvoVIvZdWLebEiZLLlE2ieDyB1NzqbZ5kFfIOW4gcG";
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nome = "admin",
                    Tipo = TipoUsuario.Administrador,
                    SenhaHash = adminHash,
                    Ativo = true,
                    DeveTrocarSenha = true,
                    DataCriacao = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
