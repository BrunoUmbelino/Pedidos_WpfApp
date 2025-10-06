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
        private readonly string _pessoasFilePath;
        private List<Pessoa> _pessoas;
        private int _nextId = 1;

        public PessoaService()
        {
            _pessoasFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "pessoas.json");
            _pessoas = new List<Pessoa>();
            CarregarPessoas();
        }

        public List<Pessoa> ObterTodasAsPessoas()
        {
            return _pessoas.OrderBy(p => p.Id).ToList();
        }

        public void AdicionarPessoa(Pessoa pessoa)
        {
            pessoa.Id = _nextId++;
            _pessoas.Add(pessoa);
            SalvarPessoas();
        }

        public void EditarPessoa(Pessoa pessoa)
        {
            var pessoaExistente = _pessoas.FirstOrDefault(p => p.Id == pessoa.Id);
            if (pessoaExistente != null)
            {
                pessoaExistente.Nome = pessoa.Nome;
                pessoaExistente.CPF = pessoa.CPF;
                pessoaExistente.Endereco = pessoa.Endereco;
                SalvarPessoas();
            }
        }

        public void DeletarPessoaPorId(int id)
        {
            var pessoa = _pessoas.FirstOrDefault(p => p.Id == id);
            if (pessoa != null)
            {
                _pessoas.Remove(pessoa);
                SalvarPessoas();
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

        private void SalvarPessoas()
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                };

                var json = JsonConvert.SerializeObject(_pessoas, settings);
                Directory.CreateDirectory(Path.GetDirectoryName(_pessoasFilePath));
                File.WriteAllText(_pessoasFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar pessoas: {ex.Message}");
            }
        }

        private void CarregarPessoas()
        {
            try
            {
                if (!File.Exists(_pessoasFilePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_pessoasFilePath));
                    return;
                }

                var json = File.ReadAllText(_pessoasFilePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    _pessoas = JsonConvert.DeserializeObject<List<Pessoa>>(json) ?? new List<Pessoa>();
                }

                _nextId = _pessoas.Any() ? _pessoas.Max(p => p.Id) + 1 : 1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar pessoas: {ex.Message}");
                _pessoas = new List<Pessoa>();
            }
        }
    }
}
