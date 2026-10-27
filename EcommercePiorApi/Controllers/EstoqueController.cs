
using Microsoft.AspNetCore.Mvc;
using PiorEcommerceDoMundo.DTOs;
using PiorEcommerceDoMundo.Services.Interfaces;

namespace PiorEcommerceDoMundo.Controllers;

[ApiController]
[Route("api/estoque")]
public class EstoqueController : ControllerBase
{
    private readonly IEstoqueService _estoqueService;

    public EstoqueController(IEstoqueService estoqueService)
    {
        _estoqueService = estoqueService;
    }

    // GET: api/estoque
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarEstoque()
    {
        var estoque = await _estoqueService.ListarEstoqueAsync();

        return Ok(estoque);
    }

    // POST: api/estoque/reabastecer
    [HttpPost("reabastecer")]
    [ProducesResponseType(typeof(ProdutoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProdutoResponseDTO>> Reabastecer(
        [FromBody] ReabastecerEstoqueDTO dto)
    {
        var (sucesso, produto) =
            await _estoqueService.ReabastecerAsync(
                dto.ProdutoId,
                dto.Quantidade);

        if (!sucesso || produto is null)
        {
            return BadRequest(new
            {
                mensagem = "Produto não encontrado ou quantidade inválida."
            });
        }

        return Ok(ProdutoResponseDTO.FromEntity(produto));
    }
}
