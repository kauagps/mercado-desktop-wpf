using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mercado.Model
{
    public class Venda
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime DataVenda { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTotal { get; set; }

        public List<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
        public List<PagamentoVenda> Pagamentos { get; set; } = new List<PagamentoVenda>();
    }
}
