using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mercado.Model
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string CodigoBarras { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Custo { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Lucro { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorVenda { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantidadeAtual { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantidadeMin { get; set; }

        public bool Fracionado { get; set; }


        public bool Ativo { get; set; } = true;

        

    }
}
