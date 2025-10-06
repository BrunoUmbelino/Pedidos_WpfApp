using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Pedidos_WpfApp.Models;
using Pedidos_WpfApp.Services;

namespace Pedidos_WpfApp.ViewModels
{
    public class PessoaViewModel : BaseViewModel
    {
        private readonly PessoaService _pessoaService;
        private Pessoa _pessoaSelecionada;
        private string _filtroNome;
        private string _filtroCPF;
        private bool _modoEdicao = false;

        public PessoaViewModel()
        {
            _pessoaService = new PessoaService();
            Pessoas = new ObservableCollection<Pessoa>(_pessoaService.ObterTodasAsPessoas());

            NovaPessoaCommand = new RelayCommand(NovaPessoa);
            IncluirCommand = new RelayCommand(IncluirPessoa);
            EditarCommand = new RelayCommand(EditarPessoa, PodeEditarOuExcluir);
            SalvarCommand = new RelayCommand(SalvarPessoa, PodeSalvar);
            ExcluirCommand = new RelayCommand(ExcluirPessoa, PodeEditarOuExcluir);
            PesquisarCommand = new RelayCommand(PesquisarPessoas);
            LimparFiltrosCommand = new RelayCommand(LimparFiltros);

            NovaPessoa();
        }

        public ObservableCollection<Pessoa> Pessoas { get; private set; }

        public Pessoa PessoaSelecionada
        {
            get => _pessoaSelecionada;
            set
            {
                if (SetProperty(ref _pessoaSelecionada, value))
                {
                    ModoEdicao = false;
                    CommandManager.InvalidateRequerySuggested();
                    OnPropertyChanged(nameof(CamposEditaveis));
                }
            }
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
                if (PessoaSelecionada == null)
                    return true;
                return PessoaSelecionada.Id == 0 || ModoEdicao;
            }
        }

        public string FiltroNome
        {
            get => _filtroNome;
            set => SetProperty(ref _filtroNome, value);
        }

        public string FiltroCPF
        {
            get => _filtroCPF;
            set => SetProperty(ref _filtroCPF, value);
        }

        public ICommand NovaPessoaCommand { get; }
        public ICommand IncluirCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand SalvarCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand PesquisarCommand { get; }
        public ICommand LimparFiltrosCommand { get; }

        private bool PodeEditarOuExcluir()
        {
            return !ModoEdicao && PessoaSelecionada?.Id > 0;
        }

        private bool PodeSalvar()
        {
            return ModoEdicao &&
                   PessoaSelecionada != null &&
                   !string.IsNullOrWhiteSpace(PessoaSelecionada.Nome) &&
                   !string.IsNullOrWhiteSpace(PessoaSelecionada.CPF);
        }

        private void NovaPessoa()
        {
            PessoaSelecionada = new Pessoa();
            ModoEdicao = false;
            CommandManager.InvalidateRequerySuggested();
            OnPropertyChanged(nameof(CamposEditaveis));
        }

        private void IncluirPessoa()
        {
            if (!ValidarPessoa())
                return;

            try
            {
                _pessoaService.AdicionarPessoa(PessoaSelecionada);
                Pessoas.Add(new Pessoa
                {
                    Id = PessoaSelecionada.Id,
                    Nome = PessoaSelecionada.Nome,
                    CPF = PessoaSelecionada.CPF,
                    Endereco = PessoaSelecionada.Endereco
                });

                System.Windows.MessageBox.Show("Pessoa salva com sucesso!", "Sucesso",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

                NovaPessoa();
                PesquisarPessoas();
            }
            catch (System.ArgumentException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Erro de Validação",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }

        private void EditarPessoa()
        {
            if (PessoaSelecionada.Id > 0)
            {
                ModoEdicao = true;
            }
        }

        private void SalvarPessoa()
        {
            if (!ValidarPessoa() || PessoaSelecionada.Id == 0)
                return;

            try
            {
                _pessoaService.EditarPessoa(PessoaSelecionada);

                var pessoaNaLista = Pessoas.FirstOrDefault(p => p.Id == PessoaSelecionada.Id);
                if (pessoaNaLista != null)
                {
                    var index = Pessoas.IndexOf(pessoaNaLista);
                    Pessoas[index] = new Pessoa
                    {
                        Id = PessoaSelecionada.Id,
                        Nome = PessoaSelecionada.Nome,
                        CPF = PessoaSelecionada.CPF,
                        Endereco = PessoaSelecionada.Endereco
                    };
                }

                System.Windows.MessageBox.Show("Pessoa atualizada com sucesso!", "Sucesso",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

                ModoEdicao = false;
                NovaPessoa();
            }
            catch (System.ArgumentException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Erro de Validação",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }

        private void ExcluirPessoa()
        {
            if (PessoaSelecionada?.Id > 0)
            {
                _pessoaService.DeletarPessoaPorId(PessoaSelecionada.Id);
                var pessoa = Pessoas.FirstOrDefault(p => p.Id == PessoaSelecionada.Id);
                if (pessoa != null)
                {
                    Pessoas.Remove(pessoa);
                }
                NovaPessoa();
                PesquisarPessoas();
            }
        }

        private void PesquisarPessoas()
        {
            var resultados = _pessoaService.PesquisarPessoas(FiltroNome, FiltroCPF);
            Pessoas.Clear();
            foreach (var pessoa in resultados)
            {
                Pessoas.Add(pessoa);
            }
        }

        private bool ValidarPessoa()
        {
            if (string.IsNullOrWhiteSpace(PessoaSelecionada.Nome))
            {
                System.Windows.MessageBox.Show("Nome é obrigatório!", "Validação",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(PessoaSelecionada.CPF))
            {
                System.Windows.MessageBox.Show("CPF é obrigatório!", "Validação",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void LimparFiltros()
        {
            FiltroNome = string.Empty;
            FiltroCPF = string.Empty;
            PesquisarPessoas();
        }

    }
}
