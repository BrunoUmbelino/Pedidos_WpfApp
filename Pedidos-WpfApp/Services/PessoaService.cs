using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Pedidos_WpfApp.Models;

namespace Pedidos_WpfApp.Services
{
    public class PessoaService
    {
        private readonly string _fileName = "pessoas.json";
        private List<Pessoa> _pessoas;
        private int _nextId = 1;

        public PessoaService()
        {
            _pessoas = DataService<Pessoa>.CarregarDados(_fileName);
            _nextId = _pessoas.Any() ? _pessoas.Max(p => p.Id) + 1 : 1;
        }

        public List<Pessoa> ObterTodasAsPessoas()
        {
            return _pessoas.OrderBy(p => p.Id).ToList();
        }

        public void AdicionarPessoa(Pessoa pessoa)
        {
            pessoa.Id = _nextId++;
            _pessoas.Add(pessoa);
            DataService<Pessoa>.SalvarDados(_pessoas, _fileName);
        }

        public void EditarPessoa(Pessoa pessoa)
        {
            var pessoaExistente = _pessoas.FirstOrDefault(p => p.Id == pessoa.Id);
            if (pessoaExistente != null)
            {
                pessoaExistente.Nome = pessoa.Nome;
                pessoaExistente.CPF = pessoa.CPF;
                pessoaExistente.Endereco = pessoa.Endereco;
                DataService<Pessoa>.SalvarDados(_pessoas, _fileName);
            }
        }

        public void DeletarPessoaPorId(int id)
        {
            var pessoa = _pessoas.FirstOrDefault(p => p.Id == id);
            if (pessoa != null)
            {
                _pessoas.Remove(pessoa);
                DataService<Pessoa>.SalvarDados(_pessoas, _fileName);
            }
        }

        public List<Pessoa> PesquisarPessoas(string nome = null, string cpf = null)
        {
            var query = _pessoas.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(p => p.Nome.ToLower().Contains(nome.ToLower()));

            if (!string.IsNullOrEmpty(cpf))
                query = query.Where(p => p.CPF.Contains(cpf));

            return query.ToList();
        }

    }
}
