using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Enums;
using PiorEcommerceDoMundo.Services.Interfaces;

namespace PiorEcommerceDoMundo;

[ApiController]
public class EcommerceController : ControllerBase
{
    private readonly IProdutoService _produtoService;
    private readonly IDescontoService _descontoService;
    private readonly IEstoqueService _estoqueService;
    private readonly IPedidoService _pedidoService;
    private readonly AppDbContext _db; 

    public EcommerceController(
        IProdutoService produtoService, 
        IDescontoService descontoService, 
        IEstoqueService estoqueService, 
        IPedidoService pedidoService,
        AppDbContext db) 
    {
        _produtoService = produtoService;
        _descontoService = descontoService;
        _estoqueService = estoqueService; 
        _pedidoService = pedidoService;   
        _db = db; 
    }

    // ==================== PRODUTOS (CRUD) ====================
    
    [HttpGet("api/produtos")]
    public async Task<IActionResult> ListarProdutos()
    {
        var lista = await _produtoService.ListarTodosAsync();
        return Ok(lista);
    }

    [HttpGet("api/produtos/{id}")]
    public async Task<IActionResult> BuscarProduto(int id)
    {
        var (sucesso, produto) = await _produtoService.BuscarPorIdAsync(id);

        if (!sucesso) return NotFound();
        return Ok(produto);
    }

    [HttpPost("api/produtos")]
    public async Task<IActionResult> CriarProduto([FromBody] Produto produto)
    {
        var (sucesso, erro, produtoCriado) = await _produtoService.CriarAsync(produto);

        if (!sucesso) return BadRequest(erro);
        return Ok(produtoCriado);
    }

    [HttpPut("api/produtos/{id}")]
    public async Task<IActionResult> AtualizarProduto(int id, [FromBody] Produto produto)
    {
        var (sucesso, atualizado) = await _produtoService.AtualizarAsync(id, produto);

        if (!sucesso) return NotFound();
        return Ok(atualizado);
    }

    [HttpDelete("api/produtos/{id}")]
    public async Task<IActionResult> DeletarProduto(int id)
    {
        bool deletado = await _produtoService.DeletarAsync(id);

        if (!deletado) return NotFound();
        return Ok();
    }

    // ==================== ESTOQUE ====================

    [HttpGet("api/estoque")]
    public async Task<IActionResult> ListarEstoque()
    {
        var resultado = await _estoqueService.ListarEstoqueAsync();
        return Ok(resultado);
    }

    [HttpPost("api/estoque/reabastecer/{produtoId}/{quantidade}")]
    public async Task<IActionResult> ReabastecerEstoque(int produtoId, int quantidade)
    {
        var (sucesso, produto) = await _estoqueService.ReabastecerAsync(produtoId, quantidade);
        if (!sucesso) return NotFound();

        return Ok(produto);
    }

    // ==================== PEDIDOS ====================

    [HttpPost("api/pedidos")]
    public async Task<IActionResult> CriarPedido([FromBody] PedidoRequest request)
    {
        var (sucesso, erro, pedido) = await _pedidoService.CriarAsync(request);

        if (!sucesso) return BadRequest(erro);
        
        return Ok(pedido);
    }

    [HttpGet("api/pedidos")]
    public async Task<IActionResult> ListarPedidos()
    {
        var lista = await _pedidoService.ListarTodosAsync();
        return Ok(lista);
    }

    [HttpGet("api/pedidos/{id}")]
    public async Task<IActionResult> BuscarPedido(int id)
    {
        var pedido = await _pedidoService.BuscarPorIdAsync(id);
        
        if (pedido == null) return NotFound();
        return Ok(pedido);
    }

    // ==================== PAGAMENTO ====================

    [HttpPost("api/pagamentos/reprocessar/{pedidoId}")]
    public IActionResult ReprocessarPagamento(int pedidoId)
    {
        var pedido = _db.Pedidos.Include(p => p.Itens).FirstOrDefault(p => p.Id == pedidoId);
        if (pedido == null) return NotFound();

        bool ok = PagamentoMock.ProcessarPagamento(pedido.ValorTotal, pedido.FormaPagamento);
        if (!ok)
        {
            pedido.Status = StatusPedido.Falhou.ToString();
            _db.SaveChanges();
            return BadRequest("pagamento recusado novamente");
        }

        pedido.Status = StatusPedido.Pago.ToString();
        _db.SaveChanges();
        return Ok(pedido);
    }

    // ==================== LOGISTICA ====================

    [HttpPost("api/logistica/reenviar/{pedidoId}")]
    public IActionResult ReenviarLogistica(int pedidoId)
    {
        var pedido = _db.Pedidos.FirstOrDefault(p => p.Id == pedidoId);
        if (pedido == null) return NotFound();

        var codigo = LogisticaMock.EnviarAsync(pedido).Result;
        pedido.CodigoRastreio = codigo;
        pedido.Status = StatusPedido.Enviado.ToString();
        _db.SaveChanges();
        return Ok(pedido);
    }
}