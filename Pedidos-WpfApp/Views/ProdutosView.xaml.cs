using System.Windows.Controls;
using Pedidos_WpfApp.ViewModels;

namespace Pedidos_WpfApp.Views
{
    /// <summary>
    /// Interaction logic for ProdutosView.xaml
    /// </summary>
    public partial class ProdutosView : UserControl
    {
        public ProdutosView()
        {
            InitializeComponent();
            DataContext = new ProdutoViewModel();
        }
    }
}
