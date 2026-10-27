using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Enums;
using PiorEcommerceDoMundo.Services.Interfaces;

namespace PiorEcommerceDoMundo;

// Controller legado mantido temporariamente para preservar os endpoints existentes.
// A Pessoa 4 poderá substituí-lo pelos controllers separados depois de validar os novos.
[ApiController]
public class EcommerceController : ControllerBase
{
    private readonly IProdutoService _produtoService;
    private readonly IEstoqueService _estoqueService;
    private readonly IPedidoService _pedidoService;
    private readonly AppDbContext _db;

    public EcommerceController(IProdutoService produtoService, IEstoqueService estoqueService,
        IPedidoService pedidoService, AppDbContext db)
    {
        _produtoService = produtoService;
        _estoqueService = estoqueService;
        _pedidoService = pedidoService;
        _db = db;
    }

    [HttpGet("api/produtos")]
    public async Task<IActionResult> ListarProdutos() => Ok(await _produtoService.ListarTodosAsync());

    [HttpGet("api/produtos/{id:int}")]
    public async Task<IActionResult> BuscarProduto(int id)
    {
        var (sucesso, produto) = await _produtoService.BuscarPorIdAsync(id);
        return sucesso ? Ok(produto) : NotFound();
    }

    [HttpPost("api/produtos")]
    public async Task<IActionResult> CriarProduto([FromBody] Produto produto)
    {
        var (sucesso, erro, criado) = await _produtoService.CriarAsync(produto);
        return sucesso ? Created($"/api/produtos/{criado!.Id}", criado) : BadRequest(erro);
    }

    [HttpPut("api/produtos/{id:int}")]
    public async Task<IActionResult> AtualizarProduto(int id, [FromBody] Produto produto)
    {
        var (sucesso, atualizado) = await _produtoService.AtualizarAsync(id, produto);
        return sucesso ? Ok(atualizado) : NotFound();
    }

    [HttpDelete("api/produtos/{id:int}")]
    public async Task<IActionResult> DeletarProduto(int id)
        => await _produtoService.DeletarAsync(id) ? NoContent() : NotFound();

    [HttpGet("api/estoque")]
    public async Task<IActionResult> ListarEstoque() => Ok(await _estoqueService.ListarEstoqueAsync());

    [HttpPost("api/estoque/reabastecer/{produtoId:int}/{quantidade:int}")]
    public async Task<IActionResult> ReabastecerEstoque(int produtoId, int quantidade)
    {
        var (sucesso, produto) = await _estoqueService.ReabastecerAsync(produtoId, quantidade);
        return sucesso ? Ok(produto) : BadRequest("Produto inexistente ou quantidade inválida.");
    }

    [HttpPost("api/pedidos")]
    public async Task<IActionResult> CriarPedido([FromBody] PedidoRequest request)
    {
        var (sucesso, erro, pedido) = await _pedidoService.CriarAsync(request);
        return sucesso ? Ok(pedido) : BadRequest(erro);
    }

    [HttpGet("api/pedidos")]
    public async Task<IActionResult> ListarPedidos() => Ok(await _pedidoService.ListarTodosAsync());

    [HttpGet("api/pedidos/{id:int}")]
    public async Task<IActionResult> BuscarPedido(int id)
    {
        var pedido = await _pedidoService.BuscarPorIdAsync(id);
        return pedido is null ? NotFound() : Ok(pedido);
    }

    [HttpPost("api/pagamentos/reprocessar/{pedidoId:int}")]
    public async Task<IActionResult> ReprocessarPagamento(int pedidoId)
    {
        var pedido = await _db.Pedidos.Include(p => p.Itens).FirstOrDefaultAsync(p => p.Id == pedidoId);
        if (pedido is null) return NotFound();

        var ok = await PagamentoMock.ProcessarPagamentoAsync(pedido.ValorTotal, pedido.FormaPagamento);
        if (!ok)
        {
            pedido.Status = StatusPedido.Falhou;
            await _db.SaveChangesAsync();
            return BadRequest("Pagamento recusado novamente.");
        }

        pedido.Status = StatusPedido.Pago;
        await _db.SaveChangesAsync();
        return Ok(pedido);
    }

    [HttpPost("api/logistica/reenviar/{pedidoId:int}")]
    public async Task<IActionResult> ReenviarLogistica(int pedidoId)
    {
        var pedido = await _db.Pedidos.FirstOrDefaultAsync(p => p.Id == pedidoId);
        if (pedido is null) return NotFound();

        pedido.CodigoRastreio = await LogisticaMock.EnviarAsync(pedido);
        pedido.Status = StatusPedido.Enviado;
        await _db.SaveChangesAsync();
        return Ok(pedido);
    }
}
