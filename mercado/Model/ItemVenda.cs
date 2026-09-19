using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mercado.Model
{
    public class ItemVenda
    {
        [Key]
        public int Id { get; set; }

        public int VendaId { get; set; }
        public Venda? Venda { get; set; }

        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantidade { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal
        {
            get; set;
        }
    }
}
