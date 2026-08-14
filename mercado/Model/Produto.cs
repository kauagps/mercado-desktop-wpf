using System;
using System.Collections.Generic;
using System.Text;

namespace mercado.Model
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public decimal Custo { get; set; }
        public decimal Lucro { get; set; }
        public decimal ValorVenda { get; set; }
        public decimal QuantidadeMin { get; set; }

        public bool Fracionado { get; set; }
        public bool Ativo { get; set; } = true;

        

    }
}
