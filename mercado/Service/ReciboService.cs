using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using mercado.Model;

namespace mercado.Service
{
    public class ReciboService
    {
        public void GerarCupomTxt(Venda vendaSalva, List<ItemVenda> itensDoCarrinho)
        {
            var sb = new StringBuilder();

            // CABEÇALHO
            sb.AppendLine("========================================");
            sb.AppendLine("           MERCADO DO SISTEMA           ");
            sb.AppendLine("           CUPOM NÃO FISCAL             ");
            sb.AppendLine("========================================");
            sb.AppendLine($"Data: {vendaSalva.DataVenda.ToString("dd/MM/yyyy HH:mm:ss")}");
            sb.AppendLine($"Venda Nº: {vendaSalva.Id:D6}"); // Exibe com zeros à esquerda (ex: 000015)
            sb.AppendLine("----------------------------------------");
            sb.AppendLine("QTD   DESCRIÇÃO         V.UN      SUBTOT");
            sb.AppendLine("----------------------------------------");

            //ITENS DA VENDA
            foreach(var item in itensDoCarrinho)
            {
                string nome = item.Produto?.Nome ?? "Item Desconhecido";
                if (nome.Length > 15) nome = nome.Substring(0, 15);
                else nome = nome.PadRight(15);

                sb.AppendLine($"{item.Quantidade,-5} {nome} {item.ValorUnitario,7:N2} {item.Subtotal,8:N2}");
            }

            // TOTAIS
            sb.AppendLine("----------------------------------------");
            sb.AppendLine($"TOTAL DA COMPRA:             R$ {vendaSalva.ValorTotal,8:N2}");
            sb.AppendLine("----------------------------------------");

            //PAGAMENTOS
            sb.AppendLine("PAGAMENTOS: ");
            foreach (var pag in vendaSalva.Pagamentos)
            {
                sb.AppendLine($"{pag.FormaPagamento.PadRight(20)} R$ {pag.ValorPago,8:N2}");
            }

            //RODAPE
            sb.AppendLine("========================================");
            sb.AppendLine("      OBRIGADO PELA PREFERÊNCIA!        ");
            sb.AppendLine("========================================");

            string pastaRecibos = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recibos");
            if (!Directory.Exists(pastaRecibos))
            {
                Directory.CreateDirectory(pastaRecibos);
            }

            string caminhoDoArquivo = Path.Combine(pastaRecibos, $"Cupom_Venda_{vendaSalva.Id:D6}.txt");

            File.WriteAllText(caminhoDoArquivo, sb.ToString());

            Process.Start(new ProcessStartInfo
            {
                FileName = caminhoDoArquivo,
                UseShellExecute = true
            });
        }
    }
}
