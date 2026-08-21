using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace mercado.Model
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Custo { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Lucro { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorVenda { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantidadeMin { get; set; }

        public bool Fracionado { get; set; }
        public bool Ativo { get; set; } = true;

        

    }
}
