using PiorEcommerceDoMundo.Enums;
using PiorEcommerceDoMundo.Services.Interfaces;
using PiorEcommerceDoMundo.Repositories.Interfaces;
using PiorEcommerceDoMundo.Repositories;

namespace PiorEcommerceDoMundo.Services;

public class PedidoService : IPedidoService
{
    private readonly IDescontoService _descontoService;
    private readonly IEstoqueService _estoqueService;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProdutoRepository _produtoRepository; // Precisa pra validar o estoque

    public PedidoService(
        IDescontoService descontoService, 
        IEstoqueService estoqueService, 
        IPedidoRepository pedidoRepository,
        IProdutoRepository produtoRepository)
    {
        _descontoService = descontoService;
        _estoqueService = estoqueService;
        _pedidoRepository = pedidoRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<(bool sucesso, string erro, Pedido pedido)> CriarAsync(PedidoRequest request)
    {
        if (string.IsNullOrEmpty(request.ClienteNome)) return (false, "informe o nome do cliente", null!);
        if (!Utils.ValidarEmail(request.ClienteEmail)) return (false, "email invalido", null!);
        if (request.Itens == null || request.Itens.Count == 0) return (false, "pedido sem itens", null!);

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
            var produto = await _produtoRepository.ObterPorIdAsync(itemReq.ProdutoId);
            if (produto == null) return (false, "produto " + itemReq.ProdutoId + " nao encontrado", null!);
            if (!produto.Ativo) return (false, "produto " + produto.Nome + " esta inativo", null!);
            if (produto.QuantidadeEstoque < itemReq.Quantidade) return (false, "estoque insuficiente para " + produto.Nome, null!);

            produto.QuantidadeEstoque -= itemReq.Quantidade;
            _produtoRepository.Atualizar(produto);

            var item = new ItemPedido
            {
                ProdutoId = produto.Id,
                Quantidade = itemReq.Quantidade,
                PrecoUnitario = produto.Preco
            };
            pedido.Itens.Add(item);

            total += (produto.Preco * itemReq.Quantidade);
        }

        total = _descontoService.AplicarDesconto(total, request.ClienteEmail);
        pedido.ValorTotal = total;

        await _pedidoRepository.AdicionarAsync(pedido);
        await _pedidoRepository.SalvarAlteracoesAsync(); 

        bool pagamentoOk = PagamentoMock.ProcessarPagamento(total, request.FormaPagamento);

        if (!pagamentoOk)
        {
            pedido.Status = StatusPedido.Falhou.ToString();
            _pedidoRepository.Atualizar(pedido);
            await _pedidoRepository.SalvarAlteracoesAsync();
            
            // Estorno assíncrono!
            await _estoqueService.EstornarEstoqueAsync(pedido.Itens);
            
            return (false, "pagamento recusado", null!);
        }

        pedido.Status = StatusPedido.Pago.ToString();
        _pedidoRepository.Atualizar(pedido);
        await _pedidoRepository.SalvarAlteracoesAsync();

        var codigoRastreio = LogisticaMock.EnviarAsync(pedido).Result;
        pedido.CodigoRastreio = codigoRastreio;
        pedido.Status = StatusPedido.Enviado.ToString();
        _pedidoRepository.Atualizar(pedido);
        await _pedidoRepository.SalvarAlteracoesAsync();

        return (true, "", pedido);
    }

    public async Task<List<Pedido>> ListarTodosAsync()
    {
        return await _pedidoRepository.ListarAsync();
    }

    public async Task<Pedido> BuscarPorIdAsync(int id)
    {
        return await _pedidoRepository.ObterPorIdAsync(id);
    }
}