
using System.ComponentModel.DataAnnotations;

namespace PiorEcommerceDoMundo.DTOs;

public class PedidoRequest
{
    [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
    [StringLength(120, ErrorMessage = "O nome deve ter no máximo 120 caracteres.")]
    public string ClienteNome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail do cliente é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string ClienteEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "A forma de pagamento é obrigatória.")]
    public string FormaPagamento { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe os itens do pedido.")]
    [MinLength(1, ErrorMessage = "O pedido precisa ter pelo menos um item.")]
    public List<ItemRequest> Itens { get; set; } = new();
}
