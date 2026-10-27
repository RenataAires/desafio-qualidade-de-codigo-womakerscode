
using System.ComponentModel.DataAnnotations;

namespace PiorEcommerceDoMundo.DTOs;

public class AtualizarProdutoDTO
{
    [Required(ErrorMessage = "O nome do produto é obrigatório.")]
    [StringLength(120, ErrorMessage = "O nome deve ter no máximo 120 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999",
        ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Preco { get; set; }

    [Range(0, int.MaxValue,
        ErrorMessage = "O estoque não pode ser negativo.")]
    public int QuantidadeEstoque { get; set; }

    public bool Ativo { get; set; } = true;
}
