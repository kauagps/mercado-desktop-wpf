using mercado.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using mercado.Model;

namespace mercado.View
{
    public partial class EditarProdutoWindow : Window
    {
        private Produto _produtoOriginal;
        private Produto _produtoClone;
        private ProdutoService _produtoService;

        public EditarProdutoWindow(Produto produto)
        {
            InitializeComponent();

            _produtoService = new ProdutoService();
            _produtoOriginal = produto;

            _produtoClone = new Produto
            {
                Id = produto.Id,
                CodigoBarras = produto.CodigoBarras,
                Nome = produto.Nome,
                Custo = produto.Custo,
                Lucro = produto.Lucro,
                ValorVenda = produto.ValorVenda,
                QuantidadeMin = produto.QuantidadeMin,
                QuantidadeAtual = produto.QuantidadeAtual,
                Fracionado = produto.Fracionado,
                Ativo = produto.Ativo
            };

            DataContext = _produtoClone;

            double custoInicial = Convert.ToDouble(_produtoClone.Custo);
            double lucroInicial = Convert.ToDouble(_produtoClone.Lucro);
            double precoIdealInicial = custoInicial + (custoInicial * (lucroInicial / 100));

            tbPrecoIdeal.Text = $"Valor recomendado: (R$) {precoIdealInicial:N2}";
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(_produtoClone.Nome) ||
                _produtoClone.Custo < 0 ||
                _produtoClone.Lucro < 0)
            {
                MessageBox.Show(
                    "Por Favor, preencha Nome, Custo e Lucro corretamente.",
                    "Campos Obrigatórios",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            _produtoOriginal.Nome = _produtoClone.Nome;
            _produtoOriginal.CodigoBarras = _produtoClone.CodigoBarras;
            _produtoOriginal.Custo = _produtoClone.Custo;
            _produtoOriginal.Lucro = _produtoClone.Lucro;
            _produtoOriginal.ValorVenda = _produtoClone.ValorVenda;
            _produtoOriginal.QuantidadeMin = _produtoClone.QuantidadeMin;
            _produtoOriginal.QuantidadeAtual = _produtoClone.QuantidadeAtual;
            _produtoOriginal.Fracionado = _produtoClone.Fracionado;
            _produtoOriginal.Ativo = _produtoClone.Ativo;

            _produtoService.AtualizarProduto(_produtoOriginal);

            MessageBox.Show("Produto atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void chkFracionado_Click(object sender, RoutedEventArgs e)
        {
            txtQuantidadeMin.Text = "0";
        }

        private void CalcularPrecoIdeal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtCusto == null || txtLucro == null || tbPrecoIdeal == null) return;

            string textoCusto = txtCusto.Text.Replace('.', ',');
            string textoLucro = txtLucro.Text.Replace('.', ',');

            if (double.TryParse(textoCusto, out double custo) && double.TryParse(textoLucro, out double lucro))
            {
                double precoIdeal = custo + (custo * (lucro / 100));
                tbPrecoIdeal.Text = $"Valor recomendado: (R$) {precoIdeal:N2}";

            }
            else
            {
                tbPrecoIdeal.Text = "Valor recomendado: (R$) 0,00";

            }
        }

        private void txtCodigoBarras_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string codigo = txtCodigoBarras.Text.Trim();
                if (!string.IsNullOrWhiteSpace(codigo))
                {
                    MessageBox.Show($"BIP! Código de barras lido: {codigo}", "Código de Barras", MessageBoxButton.OK, MessageBoxImage.Information);

                    txtCodigoBarras.Focus();
                }
            }
        }
    }
}
