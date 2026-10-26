using PiorEcommerceDoMundo.Services.Interfaces;
using PiorEcommerceDoMundo.Repositories.Interfaces;
using PiorEcommerceDoMundo.Repositories;

namespace PiorEcommerceDoMundo.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<List<Produto>> ListarTodosAsync()
    {
        return await _produtoRepository.ListarAsync();
    }

    public async Task<(bool sucesso, Produto produto)> BuscarPorIdAsync(int id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id);
        if (produto == null) return (false, null!);
        return (true, produto);
    }

    public async Task<(bool sucesso, string erro, Produto produto)> CriarAsync(Produto produto)
    {
        await _produtoRepository.AdicionarAsync(produto);
        await _produtoRepository.SalvarAlteracoesAsync();
        return (true, "", produto);
    }

    public async Task<(bool sucesso, Produto produto)> AtualizarAsync(int id, Produto produtoAtualizado)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id);
        if (produto == null) return (false, null!);

        produto.Nome = produtoAtualizado.Nome;
        produto.Descricao = produtoAtualizado.Descricao;
        produto.Preco = produtoAtualizado.Preco;
        produto.QuantidadeEstoque = produtoAtualizado.QuantidadeEstoque;
        produto.Ativo = produtoAtualizado.Ativo;

        _produtoRepository.Atualizar(produto); 
        await _produtoRepository.SalvarAlteracoesAsync();
        return (true, produto);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id);
        if (produto == null) return false;

        _produtoRepository.Remover(produto); 
        await _produtoRepository.SalvarAlteracoesAsync();
        return true;
    }
}