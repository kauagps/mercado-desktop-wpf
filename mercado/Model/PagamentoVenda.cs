using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mercado.Model
{
    public class PagamentoVenda
    {
        [Key]
        public int Id { get; set; }

        public int VendaId { get; set; }
        public Venda? Venda { get; set; }
        

        [MaxLength(30)]
        public string FormaPagamento { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorPago { get; set; }
    }
}
