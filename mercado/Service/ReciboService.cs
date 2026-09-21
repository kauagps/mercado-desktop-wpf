using mercado.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace mercado.Service
{
    public class ReciboService
    {
        
        public void ImprimirCupomNaTermica(Venda venda, List<ItemVenda> itens)
        {
            PrintDocument pd = new PrintDocument();

            //pd.PrinterSettings.PrinterName = ""

            pd.PrintPage += (sender, e) =>
            {
                if (e.Graphics == null) return;

                Graphics? graficos = e.Graphics;

                Font fonteNormal = new Font("Courier New", 9, System.Drawing.FontStyle.Regular);
                Font fonteNegrito = new Font("Courier New", 9, System.Drawing.FontStyle.Bold);

                int margemEsquerda = 0;
                int alturaAtual = 10;

                // CABEÇALHO
                graficos.DrawString("       SUPERMERCADO Fenix Rocha", fonteNegrito, System.Drawing.Brushes.Black, margemEsquerda, alturaAtual);
                alturaAtual += 20;
                graficos.DrawString("   Recibo de Venda - Sem valor fiscal", fonteNormal, System.Drawing.Brushes.Black, margemEsquerda, alturaAtual);
                alturaAtual += 20;
                graficos.DrawString("Data: " + venda.DataVenda.ToString("dd/MM/yyyy HH:mm"), fonteNormal, System.Drawing.Brushes.Black, margemEsquerda, alturaAtual);
                alturaAtual += 30;

                // TÍTULO DAS COLUNAS (Qtd | Produto | Total)
                graficos.DrawString("QTD PRODUTO               SUBTOTAL", fonteNegrito, System.Drawing.Brushes.Black, margemEsquerda, alturaAtual);
                alturaAtual += 15;
                graficos.DrawString("----------------------------------", fonteNormal, System.Drawing.Brushes.Black, margemEsquerda, alturaAtual);
                alturaAtual += 20;

                // LISTA DE PRODUTOS
                foreach (var item in itens)
                {

                    // Limita o nome do produto a 18 letras para não quebrar a linha do cupom
                    string nome = item.Produto?.Nome ?? "PRODUTO";
                    string nomeProduto = nome.Length > 18
                                         ? nome.Substring(0, 18)
                                         : nome.PadRight(18);

                    // Formata a linha: Qtd (3 espaços), Nome (18 espaços), Subtotal (8 espaços alinhado à direita)
                    string linha = string.Format("{0,-3} {1} {2,8}",
                                                 item.Quantidade.ToString("0"),
                                                 nomeProduto,
                                                 item.Subtotal.ToString("N2"));

                    graficos.DrawString(linha, fonteNormal, System.Drawing.Brushes.Black, margemEsquerda, alturaAtual);
                    alturaAtual += 15;
                }

                alturaAtual += 5;
                graficos.DrawString("----------------------------------", fonteNormal, System.Drawing.Brushes.Black, margemEsquerda, alturaAtual);
                alturaAtual += 20;

                // TOTAL
                graficos.DrawString("TOTAL: R$ " + venda.ValorTotal.ToString("N2"), fonteNegrito, System.Drawing.Brushes.Black, margemEsquerda, alturaAtual);
                alturaAtual += 40; // Espaço em branco no final para a guilhotina cortar certo
            };

            try
            {
                pd.Print();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Erro ao tentar imprimir o cupom: " + ex.Message, "Erro de Impressão", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
