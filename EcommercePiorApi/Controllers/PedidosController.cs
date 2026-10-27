using Microsoft.AspNetCore.Mvc;
using PiorEcommerceDoMundo.DTOs;
using PiorEcommerceDoMundo.Services.Interfaces;

namespace PiorEcommerceDoMundo.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    // GET: api/pedidos
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PedidoResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PedidoResponseDTO>>> ListarTodos()
    {
        var pedidos = await _pedidoService.ListarTodosAsync();

        var resposta = pedidos.Select(PedidoResponseDTO.FromEntity);

        return Ok(resposta);
    }

    // GET: api/pedidos/1
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PedidoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PedidoResponseDTO>> BuscarPorId(int id)
    {
        var pedido = await _pedidoService.BuscarPorIdAsync(id);

        if (pedido is null)
        {
            return NotFound(new
            {
                mensagem = "Pedido não encontrado."
            });
        }

        return Ok(PedidoResponseDTO.FromEntity(pedido));
    }

    // POST: api/pedidos
    [HttpPost]
    [ProducesResponseType(typeof(PedidoResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PedidoResponseDTO>> Criar(
        [FromBody] PedidoRequest dto)
    {
        var (sucesso, erro, pedido) =
            await _pedidoService.CriarAsync(dto);

        if (!sucesso || pedido is null)
        {
            return BadRequest(new { mensagem = erro });
        }

        var resposta = PedidoResponseDTO.FromEntity(pedido);

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { id = pedido.Id },
            resposta);
    }
}
