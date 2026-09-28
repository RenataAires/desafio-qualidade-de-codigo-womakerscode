using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PiorEcommerceDoMundo;

// um unico controller cuidando de produtos, estoque, pedidos, pagamento e logistica.
// nada de camadas, nada de injecao de dependencia, nada de SOLID. de proposito.
[ApiController]
public class EcommerceController : ControllerBase
{
    // ==================== PRODUTOS (CRUD) ====================

    [HttpGet("api/produtos")]
    public IActionResult ListarProdutos()
    {
        var db = new AppDbContext();
        var lista = db.Produtos.ToList();
        return Ok(lista);
    }

    [HttpGet("api/produtos/{id}")]
    public IActionResult BuscarProduto(int id)
    {
        var db = new AppDbContext();
        var produto = db.Produtos.Find(id);
        if (produto == null) return NotFound();
        return Ok(produto);
    }

    [HttpPost("api/produtos")]
    public IActionResult CriarProduto([FromBody] Produto produto)
    {
        if (produto.Nome == null || produto.Nome == "")
        {
            return BadRequest("nome invalido");
        }
        if (produto.Preco <= 0)
        {
            return BadRequest("preco invalido");
        }

        var db = new AppDbContext();
        db.Produtos.Add(produto);
        db.SaveChanges();
        Utils.Log("Produto criado: " + produto.Nome);
        return Ok(produto);
    }

    [HttpPut("api/produtos/{id}")]
    public IActionResult AtualizarProduto(int id, [FromBody] Produto produto)
    {
        var db = new AppDbContext();
        var existente = db.Produtos.Find(id);
        if (existente == null) return NotFound();

        existente.Nome = produto.Nome;
        existente.Descricao = produto.Descricao;
        existente.Preco = produto.Preco;
        existente.QuantidadeEstoque = produto.QuantidadeEstoque;
        existente.Ativo = produto.Ativo;
        db.SaveChanges();
        return Ok(existente);
    }

    [HttpDelete("api/produtos/{id}")]
    public IActionResult DeletarProduto(int id)
    {
        var db = new AppDbContext();
        var existente = db.Produtos.Find(id);
        if (existente == null) return NotFound();

        db.Produtos.Remove(existente);
        db.SaveChanges();
        return Ok();
    }

    // ==================== ESTOQUE ====================

    [HttpGet("api/estoque")]
    public IActionResult ListarEstoque()
    {
        // logica praticamente igual a ListarProdutos, copiada e colada aqui
        var db = new AppDbContext();
        var lista = db.Produtos.ToList();
        var resultado = new List<object>();
        foreach (var p in lista)
        {
            resultado.Add(new { produto = p.Nome, quantidade = p.QuantidadeEstoque });
        }
        return Ok(resultado);
    }

    [HttpPost("api/estoque/reabastecer/{produtoId}/{quantidade}")]
    public IActionResult ReabastecerEstoque(int produtoId, int quantidade)
    {
        var db = new AppDbContext();
        var produto = db.Produtos.Find(produtoId);
        if (produto == null) return NotFound();

        produto.QuantidadeEstoque = produto.QuantidadeEstoque + quantidade;
        db.SaveChanges();
        Utils.Log("Estoque reabastecido produto " + produtoId);
        return Ok(produto);
    }

    // ==================== PEDIDOS (o fluxo inteiro numa unica funcao gigante) ====================

    [HttpPost("api/pedidos")]
    public IActionResult CriarPedido([FromBody] PedidoRequest request)
    {
        if (request.ClienteNome == null || request.ClienteNome == "")
        {
            return BadRequest("informe o nome do cliente");
        }
        if (!Utils.ValidarEmail(request.ClienteEmail))
        {
            return BadRequest("email invalido");
        }
        if (request.Itens == null || request.Itens.Count == 0)
        {
            return BadRequest("pedido sem itens");
        }

        var db = new AppDbContext();

        var pedido = new Pedido();
        pedido.ClienteNome = request.ClienteNome;
        pedido.ClienteEmail = request.ClienteEmail;
        pedido.DataPedido = DateTime.Now;
        pedido.FormaPagamento = request.FormaPagamento;
        pedido.Status = "Pendente";
        pedido.Itens = new List<ItemPedido>();

        decimal total = 0;

        foreach (var itemReq in request.Itens)
        {
            var produto = db.Produtos.Find(itemReq.ProdutoId);
            if (produto == null)
            {
                return BadRequest("produto " + itemReq.ProdutoId + " nao encontrado");
            }
            if (!produto.Ativo)
            {
                return BadRequest("produto " + produto.Nome + " esta inativo");
            }
            if (produto.QuantidadeEstoque < itemReq.Quantidade)
            {
                return BadRequest("estoque insuficiente para " + produto.Nome);
            }

            // da baixa no estoque direto aqui dentro do loop, sem transacao com o pagamento
            produto.QuantidadeEstoque = produto.QuantidadeEstoque - itemReq.Quantidade;

            var item = new ItemPedido();
            item.ProdutoId = produto.Id;
            item.Quantidade = itemReq.Quantidade;
            item.PrecoUnitario = produto.Preco;
            pedido.Itens.Add(item);

            total = total + (produto.Preco * itemReq.Quantidade);
        }

        // regra de desconto arbitraria, com numeros magicos, direto no meio do fluxo do pedido
        if (total > 500)
        {
            total = total - (total * 0.10m);
        }
        else if (request.ClienteEmail.Contains("vip"))
        {
            total = total - (total * 0.20m);
        }

        pedido.ValorTotal = total;

        db.Pedidos.Add(pedido);
        db.SaveChanges(); // ja salva o pedido e o estoque debitado antes mesmo de saber se o pagamento passa

        bool pagamentoOk = PagamentoMock.ProcessarPagamento(total, request.FormaPagamento);

        if (!pagamentoOk)
        {
            pedido.Status = "Falhou";
            db.SaveChanges();
            Utils.Log("Pagamento falhou pedido " + pedido.Id);
            // estoque ja foi debitado la em cima e NUNCA eh estornado se o pagamento falhar (bug conhecido)
            return BadRequest("pagamento recusado");
        }

        pedido.Status = "Pago";
        db.SaveChanges();

        var codigoRastreio = LogisticaMock.EnviarAsync(pedido).Result; // chamada assincrona usada de forma bloqueante
        pedido.CodigoRastreio = codigoRastreio;
        pedido.Status = "Enviado";
        db.SaveChanges();

        Utils.Log("Pedido " + pedido.Id + " criado com sucesso");

        return Ok(pedido);
    }

    [HttpGet("api/pedidos")]
    public IActionResult ListarPedidos()
    {
        var db = new AppDbContext();
        var lista = db.Pedidos.Include(p => p.Itens).ToList();
        return Ok(lista);
    }

    [HttpGet("api/pedidos/{id}")]
    public IActionResult BuscarPedido(int id)
    {
        var db = new AppDbContext();
        var pedido = db.Pedidos.Include(p => p.Itens).FirstOrDefault(p => p.Id == id);
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
            pedido.Status = "Falhou";
            db.SaveChanges();
            return BadRequest("pagamento recusado novamente");
        }

        pedido.Status = "Pago";
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
        pedido.Status = "Enviado";
        db.SaveChanges();
        return Ok(pedido);
    }
}
