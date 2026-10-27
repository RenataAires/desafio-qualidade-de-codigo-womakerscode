using PiorEcommerceDoMundo;
using PiorEcommerceDoMundo.Repositories.Interfaces;
using PiorEcommerceDoMundo.Services.Interfaces;

namespace PiorEcommerceDoMundo.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoService(IProdutoRepository produtoRepository) => _produtoRepository = produtoRepository;

    public Task<List<Produto>> ListarTodosAsync() => _produtoRepository.ListarAsync();

    public async Task<(bool sucesso, Produto? produto)> BuscarPorIdAsync(int id)
    {
        if (id <= 0) return (false, null);
        var produto = await _produtoRepository.ObterPorIdAsync(id);
        return produto is null ? (false, null) : (true, produto);
    }

    public async Task<(bool sucesso, string erro, Produto? produto)> CriarAsync(Produto produto)
    {
        var erro = ValidarProduto(produto);
        if (erro is not null) return (false, erro, null);

        await _produtoRepository.AdicionarAsync(produto);
        await _produtoRepository.SalvarAlteracoesAsync();
        return (true, string.Empty, produto);
    }

    public async Task<(bool sucesso, Produto? produto)> AtualizarAsync(int id, Produto produtoAtualizado)
    {
        if (id <= 0 || ValidarProduto(produtoAtualizado) is not null) return (false, null);

        var produto = await _produtoRepository.ObterPorIdAsync(id);
        if (produto is null) return (false, null);

        produto.Nome = produtoAtualizado.Nome.Trim();
        produto.Descricao = produtoAtualizado.Descricao?.Trim() ?? string.Empty;
        produto.Preco = produtoAtualizado.Preco;
        produto.QuantidadeEstoque = produtoAtualizado.QuantidadeEstoque;
        produto.Ativo = produtoAtualizado.Ativo;

        _produtoRepository.Atualizar(produto);
        await _produtoRepository.SalvarAlteracoesAsync();
        return (true, produto);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        if (id <= 0) return false;
        var produto = await _produtoRepository.ObterPorIdAsync(id);
        if (produto is null) return false;

        _produtoRepository.Remover(produto);
        await _produtoRepository.SalvarAlteracoesAsync();
        return true;
    }

    private static string? ValidarProduto(Produto? produto)
    {
        if (produto is null) return "Os dados do produto são obrigatórios.";
        if (string.IsNullOrWhiteSpace(produto.Nome)) return "O nome do produto é obrigatório.";
        if (produto.Preco < 0) return "O preço não pode ser negativo.";
        if (produto.QuantidadeEstoque < 0) return "A quantidade em estoque não pode ser negativa.";
        return null;
    }
}
