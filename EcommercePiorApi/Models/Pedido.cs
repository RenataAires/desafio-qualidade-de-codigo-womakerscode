using PiorEcommerceDoMundo.Enums;
namespace PiorEcommerceDoMundo;

public class Pedido
{
    public int Id { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteEmail { get; set; } = string.Empty;
    public DateTime DataPedido { get; set; }
    public StatusPedido Status { get; set; } = StatusPedido.Pendente;
    public decimal ValorTotal { get; set; }
    public string FormaPagamento { get; set; } = string.Empty;
    public string CodigoRastreio { get; set; } = string.Empty;
    public List<ItemPedido> Itens { get; set; } = new();
}