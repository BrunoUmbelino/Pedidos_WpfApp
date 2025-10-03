using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pedidos_WpfApp.Models;
using Newtonsoft.Json;


namespace Pedidos_WpfApp.Services
{
    public class ProdutoService
    {
        private readonly string _produtosFilePath;
        private List<Produto> _produtos;
        private int _nextId = 1;

        public ProdutoService()
        {
            _produtosFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "produtos.json");
            _produtos = new List<Produto>();
            CarregarProdutos();
        }

        public List<Produto> ObterTodosOsProdutos()
        {
            return _produtos.OrderBy(p=>p.Id).ToList();
        }

        public void AdicionarProduto(Produto produto)
        {
            produto.Id = ++_nextId;
            _produtos.Add(produto);
            SalvarProdutos();
        }

        public void EditarProduto(Produto produto)
        {
            var produtoExistente = _produtos.FirstOrDefault(p=>p.Id == produto.Id);
            if (produtoExistente == null) return;

            produtoExistente.Nome = produto.Nome;
            produtoExistente.Codigo = produto.Codigo;
            produtoExistente.Valor = produto.Valor;
            SalvarProdutos();
        }

        public void DeletarProdutoPorId(int id)
        {
            var produtoExistente = _produtos.FirstOrDefault(p=>p.Id == id);
            if (produtoExistente == null) return;

            _produtos.Remove(produtoExistente);
            SalvarProdutos();
        }

        public List<Produto> PesquisarProdutos(string nome = null, string codigo = null, decimal? valorInicial = null, decimal? valorFinal = null)
        {
            var query = _produtos.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(p => p.Nome.Contains(nome));

            if (!string.IsNullOrEmpty(codigo))
                query = query.Where(p=>p.Codigo.Contains(codigo));

            if (valorInicial.HasValue)
                query = query.Where(p=>p.Valor >= valorInicial.Value);

            if (valorFinal.HasValue)
                query = query.Where(p => p.Valor <= valorFinal.Value);

            return query.ToList();
        }

        private void CarregarProdutos()
        {
            try
            {
                if (!File.Exists(_produtosFilePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_produtosFilePath));
                    return;
                }

                var json = File.ReadAllText(_produtosFilePath);
                if (!string.IsNullOrEmpty(json))
                    _produtos = JsonConvert.DeserializeObject<List<Produto>>(json) ?? new List<Produto>();

                _nextId = _produtos.Any() ? _produtos.Max(p => p.Id) + 1 : 1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Falha ao carregar produtos: {ex.Message}");
            }
        }

        private void SalvarProdutos()
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                };

                var json = JsonConvert.SerializeObject( _produtos, settings);
                Directory.CreateDirectory(Path.GetDirectoryName(_produtosFilePath));
                File.WriteAllText(_produtosFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Falha ao salvar produtos: {ex.Message}");
            }
        }
    }
}
