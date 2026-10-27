using PiorEcommerceDoMundo;
using PiorEcommerceDoMundo.Repositories.Interfaces;
using PiorEcommerceDoMundo.Services.Interfaces;

namespace PiorEcommerceDoMundo.Services;

public class EstoqueService : IEstoqueService
{
    private readonly IProdutoRepository _produtoRepository;

    public EstoqueService(IProdutoRepository produtoRepository) => _produtoRepository = produtoRepository;

    public async Task<object> ListarEstoqueAsync()
    {
        var produtos = await _produtoRepository.ListarAsync();
        return produtos.Select(p => new { p.Id, p.Nome, p.QuantidadeEstoque }).ToList();
    }

    public async Task<(bool sucesso, Produto? produto)> ReabastecerAsync(int produtoId, int quantidade)
    {
        if (produtoId <= 0 || quantidade <= 0) return (false, null);

        var produto = await _produtoRepository.ObterPorIdAsync(produtoId);
        if (produto is null) return (false, null);

        produto.QuantidadeEstoque += quantidade;
        _produtoRepository.Atualizar(produto);
        await _produtoRepository.SalvarAlteracoesAsync();
        return (true, produto);
    }

    public async Task EstornarEstoqueAsync(List<ItemPedido> itens)
    {
        if (itens is null || itens.Count == 0) return;

        foreach (var item in itens)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
            if (produto is null) continue;

            produto.QuantidadeEstoque += item.Quantidade;
            _produtoRepository.Atualizar(produto);
        }

        await _produtoRepository.SalvarAlteracoesAsync();
    }
}
