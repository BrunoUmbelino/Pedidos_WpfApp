using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Pedidos_WpfApp.Models;
using Pedidos_WpfApp.Services;

namespace Pedidos_WpfApp.ViewModels
{
    public class ProdutoViewModel : BaseViewModel
    {
        private readonly ProdutoService _produtoService;
        private Produto _produtoSelecionado;
        private string _filtroNome;
        private string _filtroCodigo;
        private decimal? _filtroValorInicial;
        private decimal? _filtroValorFinal;
        private bool _modoEdicao = false;

        public ProdutoViewModel()
        {
            _produtoService = new ProdutoService();
            Produtos = new ObservableCollection<Produto>(_produtoService.ObterTodosOsProdutos());

            NovoProdutoCommand = new RelayCommand(NovoProduto);
            IncluirCommand = new RelayCommand(IncluirProduto);
            EditarCommand = new RelayCommand(EditarProduto, PodeEditarOuExcluir);
            SalvarCommand = new RelayCommand(SalvarProduto, PodeSalvar);
            ExcluirCommand = new RelayCommand(ExcluirProduto, PodeEditarOuExcluir);

            PesquisarCommand = new RelayCommand(PesquisarProdutos);
            LimparFiltrosCommand = new RelayCommand(LimparFiltros);

            NovoProduto();
        }

        public ObservableCollection<Produto> Produtos { get; private set; }

        public Produto ProdutoSelecionado
        {
            get => _produtoSelecionado;
            set
            {
                if (SetProperty(ref _produtoSelecionado, value))
                {
                    _modoEdicao = false;
                    CommandManager.InvalidateRequerySuggested();
                    OnPropertyChanged(nameof(CamposEditaveis));
                }
            }
        }

        public string FiltroNome
        {
            get => _filtroNome;
            set => SetProperty(ref _filtroNome, value);
        }

        public string FiltroCodigo
        {
            get => _filtroCodigo;
            set => SetProperty(ref _filtroCodigo, value);
        }

        public decimal? FiltroValorInicial
        {
            get => _filtroValorInicial;
            set => SetProperty(ref _filtroValorInicial, value);
        }

        public decimal? FiltroValorFinal
        {
            get => _filtroValorFinal;
            set => SetProperty(ref _filtroValorFinal, value);
        }

        public bool ModoEdicao
        {
            get => _modoEdicao;
            set
            {
                if (SetProperty(ref _modoEdicao, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                    OnPropertyChanged(nameof(CamposEditaveis));
                }
            }
        }

        public bool CamposEditaveis
        {
            get
            {
                if (ProdutoSelecionado == null)
                    return true;

                return ProdutoSelecionado.Id == 0 || ModoEdicao;
            }
        }

        public ICommand IncluirCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand SalvarCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand PesquisarCommand { get; }
        public ICommand LimparFiltrosCommand { get; }
        public ICommand NovoProdutoCommand { get; }

        private void IncluirProduto()
        {
            if (!ValidarProduto())
                return;

            _produtoService.AdicionarProduto(ProdutoSelecionado);
            Produtos.Add(new Produto
            {
                Id = ProdutoSelecionado.Id,
                Nome = ProdutoSelecionado.Nome,
                Codigo = ProdutoSelecionado.Codigo,
                Valor = ProdutoSelecionado.Valor
            });

            System.Windows.MessageBox.Show("Produto salvo com sucesso!", "Sucesso",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            NovoProduto();
            PesquisarProdutos();
        }

        private void EditarProduto()
        {
            if (ProdutoSelecionado.Id > 0)
            {
                ModoEdicao = true;
            }
        }

        private void SalvarProduto()
        {
            if (!ValidarProduto() || ProdutoSelecionado.Id == 0)
                return;

            _produtoService.EditarProduto(ProdutoSelecionado);

            var produtosVisualizacao = Produtos.FirstOrDefault(p => p.Id == ProdutoSelecionado.Id);
            if (produtosVisualizacao != null)
            {
                var index = Produtos.IndexOf(produtosVisualizacao);
                Produtos[index] = new Produto
                {
                    Id = ProdutoSelecionado.Id,
                    Nome = ProdutoSelecionado.Nome,
                    Codigo = ProdutoSelecionado.Codigo,
                    Valor = ProdutoSelecionado.Valor
                };
            }

            ModoEdicao = false;
            NovoProduto();
        }

        private void ExcluirProduto()
        {
            if (ProdutoSelecionado?.Id > 0)
            {
                _produtoService.DeletarProdutoPorId(ProdutoSelecionado.Id);
                var produto = Produtos.FirstOrDefault(p => p.Id == ProdutoSelecionado.Id);
                if (produto != null)
                {
                    Produtos.Remove(produto);
                }
                NovoProduto();
                PesquisarProdutos();
            }
        }

        private void PesquisarProdutos()
        {
            decimal? valorInicial = FiltroValorInicial;
            decimal? valorFinal = FiltroValorFinal;

            var resultados = _produtoService.PesquisarProdutos(FiltroNome, FiltroCodigo, valorInicial, valorFinal);
            Produtos.Clear();
            foreach (var produto in resultados)
            {
                Produtos.Add(produto);
            }
        }

        private void LimparFiltros()
        {
            FiltroNome = string.Empty;
            FiltroCodigo = string.Empty;
            FiltroValorInicial = null;
            FiltroValorFinal = null;
            PesquisarProdutos();
        }

        private void NovoProduto()
        {
            ProdutoSelecionado = new Produto();
            _modoEdicao = false;
            CommandManager.InvalidateRequerySuggested();
            OnPropertyChanged(nameof(CamposEditaveis));
        }

        private bool PodeEditarOuExcluir()
        {
            return ProdutoSelecionado?.Id > 0 && !ModoEdicao;
        }

        private bool PodeSalvar()
        {
            return ModoEdicao &&
                    ProdutoSelecionado != null &&
                   !string.IsNullOrWhiteSpace(ProdutoSelecionado.Nome) &&
                   !string.IsNullOrWhiteSpace(ProdutoSelecionado.Codigo) &&
                   ProdutoSelecionado.Valor > 0;
        }

        private bool ValidarProduto()
        {
            if (string.IsNullOrWhiteSpace(ProdutoSelecionado.Nome))
            {
                System.Windows.MessageBox.Show("Nome é obrigatório!", "Validação",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(ProdutoSelecionado.Codigo))
            {
                System.Windows.MessageBox.Show("Código é obrigatório!", "Validação",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return false;
            }

            if (ProdutoSelecionado.Valor <= 0)
            {
                System.Windows.MessageBox.Show("Valor deve ser maior que zero!", "Validação",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
