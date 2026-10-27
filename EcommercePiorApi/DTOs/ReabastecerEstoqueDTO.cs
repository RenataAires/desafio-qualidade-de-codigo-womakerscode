
using System.ComponentModel.DataAnnotations;

namespace PiorEcommerceDoMundo.DTOs;

public class ReabastecerEstoqueDTO
{
    [Range(1, int.MaxValue,
        ErrorMessage = "O ID do produto deve ser válido.")]
    public int ProdutoId { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int Quantidade { get; set; }
}
