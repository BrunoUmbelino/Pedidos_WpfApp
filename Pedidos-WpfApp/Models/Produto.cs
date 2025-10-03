namespace Pedidos_WpfApp.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public decimal Valor { get; set; }

        public Produto() { }

        public Produto(string nome, string codigo, decimal valor) { 
            Nome = nome;
            Codigo = codigo;
            Valor = valor;
        }
    }
}
