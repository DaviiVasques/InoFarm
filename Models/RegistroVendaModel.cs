
namespace InoFarm.Models;

 public class RegistroVendaModels
{
    public int Id { get; set; }
    public string Responsavel { get; set; }
    public string Acao { get; set; } // Fornecimento, Venda, Devolução
    public DateTime Data { get; set; }
    public string Detalhes { get; set; }
    public string FormaPagamento { get; set; } // Crédito, Débito
    public decimal Valor { get; set; }
}

