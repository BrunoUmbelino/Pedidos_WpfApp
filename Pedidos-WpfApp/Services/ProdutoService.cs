using System.Collections.Generic;
using System.Linq;
using Pedidos_WpfApp.Models;


namespace Pedidos_WpfApp.Services
{
    public class ProdutoService
    {
        private readonly string _fileName = "produtos.json";
        private List<Produto> _produtos;
        private int _nextId = 1;

        public ProdutoService()
        {
            _produtos = DataService<Produto>.CarregarDados(_fileName);
            _nextId = _produtos.Any() ? _produtos.Max(p => p.Id) + 1 : 1;
        }

        public List<Produto> ObterTodosOsProdutos()
        {
            return _produtos.OrderBy(p=>p.Id).ToList();
        }

        public void AdicionarProduto(Produto produto)
        {
            produto.Id = ++_nextId;
            _produtos.Add(produto);
            DataService<Produto>.SalvarDados(_produtos, _fileName);
        }

        public void EditarProduto(Produto produto)
        {
            var produtoExistente = _produtos.FirstOrDefault(p=>p.Id == produto.Id);
            if (produtoExistente == null) return;

            produtoExistente.Nome = produto.Nome;
            produtoExistente.Codigo = produto.Codigo;
            produtoExistente.Valor = produto.Valor;
            DataService<Produto>.SalvarDados(_produtos, _fileName);
        }

        public void DeletarProdutoPorId(int id)
        {
            var produtoExistente = _produtos.FirstOrDefault(p=>p.Id == id);
            if (produtoExistente == null) return;

            _produtos.Remove(produtoExistente);
            DataService<Produto>.SalvarDados(_produtos, _fileName);
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
    }
}
