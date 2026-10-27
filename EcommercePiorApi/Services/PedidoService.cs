using PiorEcommerceDoMundo;
using PiorEcommerceDoMundo.Enums;
using PiorEcommerceDoMundo.Repositories.Interfaces;
using PiorEcommerceDoMundo.Services.Interfaces;
using PiorEcommerceDoMundo.DTOs;

namespace PiorEcommerceDoMundo.Services;

public class PedidoService : IPedidoService
{
    private readonly IDescontoService _descontoService;
    private readonly IEstoqueService _estoqueService;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProdutoRepository _produtoRepository;

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

    public async Task<(bool sucesso, string erro, Pedido? pedido)> CriarAsync(PedidoRequest request)
    {
        if (request is null) return (false, "Os dados do pedido são obrigatórios.", null);
        if (string.IsNullOrWhiteSpace(request.ClienteNome)) return (false, "Informe o nome do cliente.", null);
        if (!Utils.ValidarEmail(request.ClienteEmail)) return (false, "E-mail inválido.", null);
        if (request.Itens is null || request.Itens.Count == 0) return (false, "O pedido precisa ter pelo menos um item.", null);
        if (request.Itens.Any(i => i.ProdutoId <= 0 || i.Quantidade <= 0))
            return (false, "Cada item precisa ter um produto válido e quantidade maior que zero.", null);

        var pedido = new Pedido
        {
            ClienteNome = request.ClienteNome.Trim(),
            ClienteEmail = request.ClienteEmail.Trim(),
            DataPedido = DateTime.UtcNow,
            FormaPagamento = request.FormaPagamento?.Trim() ?? string.Empty,
            Status = StatusPedido.Pendente,
            Itens = new List<ItemPedido>()
        };

        decimal total = 0;
        var produtosDoPedido = new Dictionary<int, Produto>();

        // Primeiro valida todos os itens e calcula o total. Não altera estoque antes do pagamento.
        foreach (var itemRequest in request.Itens)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(itemRequest.ProdutoId);
            if (produto is null) return (false, $"Produto {itemRequest.ProdutoId} não encontrado.", null);
            if (!produto.Ativo) return (false, $"O produto {produto.Nome} está inativo.", null);

            var quantidadeJaSolicitada = pedido.Itens
                .Where(i => i.ProdutoId == produto.Id)
                .Sum(i => i.Quantidade);
            if (produto.QuantidadeEstoque < quantidadeJaSolicitada + itemRequest.Quantidade)
                return (false, $"Estoque insuficiente para {produto.Nome}.", null);

            produtosDoPedido[produto.Id] = produto;
            pedido.Itens.Add(new ItemPedido
            {
                ProdutoId = produto.Id,
                Quantidade = itemRequest.Quantidade,
                PrecoUnitario = produto.Preco
            });
            total += produto.Preco * itemRequest.Quantidade;
        }

        pedido.ValorTotal = _descontoService.AplicarDesconto(total, pedido.ClienteEmail);

        // Mantém o registro de pedidos recusados, mas ainda não baixa o estoque.
        await _pedidoRepository.AdicionarAsync(pedido);
        await _pedidoRepository.SalvarAlteracoesAsync();

        var pagamentoAprovado = await PagamentoMock.ProcessarPagamentoAsync(pedido.ValorTotal, pedido.FormaPagamento);
        if (!pagamentoAprovado)
        {
            pedido.Status = StatusPedido.Falhou;
            _pedidoRepository.Atualizar(pedido);
            await _pedidoRepository.SalvarAlteracoesAsync();
            return (false, "Pagamento recusado.", null);
        }

        foreach (var item in pedido.Itens)
        {
            var produto = produtosDoPedido[item.ProdutoId];
            produto.QuantidadeEstoque -= item.Quantidade;
            _produtoRepository.Atualizar(produto);
        }

        pedido.Status = StatusPedido.Pago;
        _pedidoRepository.Atualizar(pedido);
        await _pedidoRepository.SalvarAlteracoesAsync();

        // A chamada logística é aguardada sem bloquear a thread com .Result.
        pedido.CodigoRastreio = await LogisticaMock.EnviarAsync(pedido);
        pedido.Status = StatusPedido.Enviado;
        _pedidoRepository.Atualizar(pedido);
        await _pedidoRepository.SalvarAlteracoesAsync();

        return (true, string.Empty, pedido);
    }

    public Task<List<Pedido>> ListarTodosAsync() => _pedidoRepository.ListarAsync();

    public Task<Pedido?> BuscarPorIdAsync(int id)
    {
        if (id <= 0) return Task.FromResult<Pedido?>(null);
        return _pedidoRepository.ObterPorIdAsync(id);
    }
}
