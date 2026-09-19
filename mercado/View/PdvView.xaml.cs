using mercado.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace mercado.View
{
    /// <summary>
    /// Interação lógica para PdvView.xam
    /// </summary>
    public partial class PdvView : UserControl
    {
        private PdvViewModel _viewModel;
        
        
        public PdvView()
        {
            InitializeComponent();
            _viewModel = new PdvViewModel();
            DataContext = _viewModel;

            txtCodigoBarras.Focus();
        }

        private void txtCodigoBarras_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.BiparProduto(txtCodigoBarras.Text);
                txtCodigoBarras.Clear();
                txtCodigoBarras.Focus();
            }
        }
    }
}
