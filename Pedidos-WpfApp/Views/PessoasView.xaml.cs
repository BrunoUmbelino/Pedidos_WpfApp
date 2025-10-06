using System.Windows.Controls;

namespace Pedidos_WpfApp.Views
{
    /// <summary>
    /// Interaction logic for PessoasView.xaml
    /// </summary>
    public partial class PessoasView : UserControl
    {
        public PessoasView()
        {
            InitializeComponent();
            DataContext = new ViewModels.PessoaViewModel();
        }
    }
}
