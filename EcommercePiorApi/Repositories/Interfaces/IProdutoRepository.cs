#nullable enable

using PiorEcommerceDoMundo;

namespace PiorEcommerceDoMundo.Repositories.Interfaces;

public interface IProdutoRepository
{
    Task<List<Produto>> ListarAsync();

    Task<Produto?> ObterPorIdAsync(int id);

    Task AdicionarAsync(Produto produto);

    void Atualizar(Produto produto);

    void Remover(Produto produto);

    Task<int> SalvarAlteracoesAsync();
    
}