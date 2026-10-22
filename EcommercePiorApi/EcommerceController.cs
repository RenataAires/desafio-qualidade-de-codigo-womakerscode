using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Enums;
using PiorEcommerceDoMundo.Services;
using PiorEcommerceDoMundo.Services.Interfaces;

namespace PiorEcommerceDoMundo;

// um unico controller cuidando de produtos, estoque, pedidos, pagamento e logistica.
// nada de camadas, nada de injecao de dependencia, nada de SOLID. de proposito.
[ApiController]
public class EcommerceController : ControllerBase
{private readonly IProdutoService _produtoService;
    private readonly IDescontoService _descontoService;
    private readonly IEstoqueService _estoqueService;
    private readonly IPedidoService _pedidoService;

    
   public EcommerceController(
        IProdutoService produtoService, 
        IDescontoService descontoService, 
        IEstoqueService estoqueService, 
        IPedidoService pedidoService)
    {
        _produtoService = produtoService;
        _descontoService = descontoService;
        _estoqueService = estoqueService; 
        _pedidoService = pedidoService;   
    }
    // ==================== PRODUTOS (CRUD) ====================

    [HttpGet("api/produtos")]
    public IActionResult ListarProdutos()
    {
        var lista = _produtoService.ListarTodos();
        return Ok(lista);
    }

    [HttpGet("api/produtos/{id}")]
    public IActionResult BuscarProduto(int id)
    {
        var (sucesso, produto) = _produtoService.BuscarPorId(id);

        if (!sucesso) return NotFound();
        return Ok(produto);
    }

    [HttpPost("api/produtos")]
    public IActionResult CriarProduto([FromBody] Produto produto)
    {
        var (sucesso, erro, produtoCriado) = _produtoService.Criar(produto);

        if (!sucesso) return BadRequest(erro);
        return Ok(produtoCriado);
    }

    [HttpPut("api/produtos/{id}")]
    public IActionResult AtualizarProduto(int id, [FromBody] Produto produto)
    {
        var (sucesso, atualizado) = _produtoService.Atualizar(id, produto);

        if (!sucesso) return NotFound();
        return Ok(atualizado);
    }

    [HttpDelete("api/produtos/{id}")]
    public IActionResult DeletarProduto(int id)
    {
        bool deletado = _produtoService.Deletar(id);

        if (!deletado) return NotFound();
        return Ok();
    }

    // ==================== ESTOQUE ====================

    [HttpGet("api/estoque")]
    public IActionResult ListarEstoque()
    {
    var resultado = _estoqueService.ListarEstoque();
    return Ok(resultado);
    }

    [HttpPost("api/estoque/reabastecer/{produtoId}/{quantidade}")]
    public IActionResult ReabastecerEstoque(int produtoId, int quantidade)
    {
    var (sucesso, produto) = _estoqueService.Reabastecer(produtoId, quantidade);
    if (!sucesso) return NotFound();

    return Ok(produto);
    }
    // ==================== PEDIDOS (o fluxo inteiro numa unica funcao gigante) ====================

   [HttpPost("api/pedidos")]
    public IActionResult CriarPedido([FromBody] PedidoRequest request)
    {
        // Olha que maravilha! Toda aquela lógica gigante virou 3 linhas:
        var (sucesso, erro, pedido) = _pedidoService.Criar(request);

        if (!sucesso) return BadRequest(erro);
        
        return Ok(pedido);
    }

    [HttpGet("api/pedidos")]
    public IActionResult ListarPedidos()
    {
        return Ok(_pedidoService.ListarTodos());
    }

    [HttpGet("api/pedidos/{id}")]
    public IActionResult BuscarPedido(int id)
    {
        var pedido = _pedidoService.BuscarPorId(id);
        
        if (pedido == null) return NotFound();
        return Ok(pedido);
    }
    // ==================== PAGAMENTO (endpoint separado, mas com logica duplicada do CriarPedido) ====================

    [HttpPost("api/pagamentos/reprocessar/{pedidoId}")]
    public IActionResult ReprocessarPagamento(int pedidoId)
    {
        var db = new AppDbContext();
        var pedido = db.Pedidos.Include(p => p.Itens).FirstOrDefault(p => p.Id == pedidoId);
        if (pedido == null) return NotFound();

        bool ok = PagamentoMock.ProcessarPagamento(pedido.ValorTotal, pedido.FormaPagamento);
        if (!ok)
        {
            pedido. Status = StatusPedido.Falhou.ToString();
            db.SaveChanges();
            return BadRequest("pagamento recusado novamente");
        }

        pedido.Status = StatusPedido.Pago.ToString();
        db.SaveChanges();
        return Ok(pedido);
    }

    // ==================== LOGISTICA (idem, copiado do fluxo de criar pedido) ====================

    [HttpPost("api/logistica/reenviar/{pedidoId}")]
    public IActionResult ReenviarLogistica(int pedidoId)
    {
        var db = new AppDbContext();
        var pedido = db.Pedidos.FirstOrDefault(p => p.Id == pedidoId);
        if (pedido == null) return NotFound();

        var codigo = LogisticaMock.EnviarAsync(pedido).Result;
        pedido.CodigoRastreio = codigo;
        pedido.Status = StatusPedido.Enviado.ToString();
        db.SaveChanges();
        return Ok(pedido);
    }
}
