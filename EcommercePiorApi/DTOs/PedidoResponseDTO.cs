
using PiorEcommerceDoMundo.Enums;

namespace PiorEcommerceDoMundo.DTOs;

public class PedidoResponseDTO
{
    public int Id { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteEmail { get; set; } = string.Empty;
    public DateTime DataPedido { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string FormaPagamento { get; set; } = string.Empty;
    public string CodigoRastreio { get; set; } = string.Empty;
    public List<ItemPedidoResponseDTO> Itens { get; set; } = new();

    public static PedidoResponseDTO FromEntity(Pedido pedido)
    {
        return new PedidoResponseDTO
        {
            Id = pedido.Id,
            ClienteNome = pedido.ClienteNome,
            ClienteEmail = pedido.ClienteEmail,
            DataPedido = pedido.DataPedido,
            Status = pedido.Status.ToString(),
            ValorTotal = pedido.ValorTotal,
            FormaPagamento = pedido.FormaPagamento,
            CodigoRastreio = pedido.CodigoRastreio,

            Itens = pedido.Itens.Select(item => new ItemPedidoResponseDTO
            {
                ProdutoId = item.ProdutoId,
                NomeProduto = item.Produto?.Nome ?? string.Empty,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario,
                Subtotal = item.PrecoUnitario * item.Quantidade
            }).ToList()
        };
    }
}
