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
    /// <summary>
    /// Lógica interna para BuscaProdutoWindow.xaml
    /// </summary>
    public partial class BuscaProdutoWindow : Window
    {

        private ProdutoService _produtoService;

        public string CodigoBarrasSelecionado { get; private set; } = string.Empty;
        
        public BuscaProdutoWindow()
        {
            InitializeComponent();
            _produtoService = new ProdutoService();

            this.Loaded += (s, e) =>
            {
                Keyboard.Focus(txtBusca);

                gridResultados.ItemsSource = _produtoService.BuscarPorNome("");
            };
        }

        private void txtBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBusca.Text.Length == 0 || txtBusca.Text.Length >= 2)
            {
                gridResultados.ItemsSource = _produtoService.BuscarPorNome(txtBusca.Text);

            }
        }

        private void txtBusca_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (gridResultados.Items.Count == 0) return;

            if (e.Key == Key.Down)
            {
                e.Handled = true;

                if (gridResultados.SelectedIndex < gridResultados.Items.Count - 1)
                {
                    gridResultados.SelectedIndex++;
                    gridResultados.ScrollIntoView(gridResultados.SelectedItem);
                }
            }
            else if (e.Key == Key.Up)
            {
                e.Handled = true;

                if (gridResultados.SelectedIndex > 0)
                {
                    gridResultados.SelectedIndex--;

                    gridResultados.ScrollIntoView(gridResultados.SelectedItem);
                }
            }
            else if (e.Key == Key.Enter)
            {
                e.Handled = true;

                if (gridResultados.SelectedItem != null)
                {
                    SelecionarProduto();
                }
                else if(gridResultados.Items.Count > 0)
                {
                    gridResultados.SelectedIndex = 0;
                    SelecionarProduto();
                }
            }
        }

        private void gridResultados_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelecionarProduto();
        }

        private void gridResultados_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                SelecionarProduto();
            }
        }

        private void SelecionarProduto()
        {
            if (gridResultados.SelectedItem is Produto produtoEscolhido)
            {
                CodigoBarrasSelecionado = produtoEscolhido.CodigoBarras;
                this.DialogResult = true;
                this.Close();
            }
        }
    }

}
