using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Enums;
using PiorEcommerceDoMundo.Services.Interfaces;

namespace PiorEcommerceDoMundo.Services;

public class PedidoService : IPedidoService
{
    private readonly IDescontoService _descontoService;
    private readonly IEstoqueService _estoqueService;

    public PedidoService(IDescontoService descontoService, IEstoqueService estoqueService)
    {
        _descontoService = descontoService;
        _estoqueService = estoqueService;
    }

    public List<Pedido> ListarTodos()
    {
        var db = new AppDbContext();
        return db.Pedidos.Include(p => p.Itens).ToList();
    }

    public Pedido? BuscarPorId(int id)
    {
        var db = new AppDbContext();
        return db.Pedidos.Include(p => p.Itens).FirstOrDefault(p => p.Id == id);
    }

    public (bool sucesso, string erro, Pedido pedido) Criar(PedidoRequest request)
    {
        if (string.IsNullOrEmpty(request.ClienteNome)) return (false, "informe o nome do cliente", null);
        if (!Utils.ValidarEmail(request.ClienteEmail)) return (false, "email invalido", null);
        if (request.Itens == null || request.Itens.Count == 0) return (false, "pedido sem itens", null);

        var db = new AppDbContext();
        var pedido = new Pedido
        {
            ClienteNome = request.ClienteNome,
            ClienteEmail = request.ClienteEmail,
            DataPedido = DateTime.Now,
            FormaPagamento = request.FormaPagamento,
            Status = StatusPedido.Pendente.ToString(),
            Itens = new List<ItemPedido>()
        };

        decimal total = 0;

        foreach (var itemReq in request.Itens)
        {
            var (estoqueOk, erroEstoque, produto) = _estoqueService.ValidarEBaixarEstoque(itemReq.ProdutoId, itemReq.Quantidade);

            if (!estoqueOk) return (false, erroEstoque, null);

            var item = new ItemPedido
            {
                ProdutoId = produto.Id,
                Quantidade = itemReq.Quantidade,
                PrecoUnitario = produto.Preco
            };
            pedido.Itens.Add(item);
            total += (produto.Preco * itemReq.Quantidade);
        }

        // Calcula o desconto chamando o service
        total = _descontoService.AplicarDesconto(total, request.ClienteEmail);
        pedido.ValorTotal = total;

        db.Pedidos.Add(pedido);
        db.SaveChanges(); 

        bool pagamentoOk = PagamentoMock.ProcessarPagamento(total, request.FormaPagamento);

        if (!pagamentoOk)
        {
            pedido.Status = StatusPedido.Falhou.ToString();
            db.SaveChanges();
            Utils.Log("Pagamento falhou pedido " + pedido.Id);

            // ✅ BUG CORRIGIDO AQUI: Se o pagamento falhou, devolvemos os itens pro estoque!
            foreach(var itemReq in request.Itens)
            {
                _estoqueService.EstornarEstoque(itemReq.ProdutoId, itemReq.Quantidade);
            }

            return (false, "pagamento recusado", null);
        }

        pedido.Status = StatusPedido.Pago.ToString();
        db.SaveChanges();

        var codigoRastreio = LogisticaMock.EnviarAsync(pedido).Result;
        pedido.CodigoRastreio = codigoRastreio;
        pedido.Status = StatusPedido.Enviado.ToString();
        db.SaveChanges();

        Utils.Log("Pedido " + pedido.Id + " criado com sucesso");
        return (true, string.Empty, pedido);
    }
}