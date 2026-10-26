#nullable enable

using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Repositories.Interfaces;

namespace PiorEcommerceDoMundo.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _db;

    public ProdutoRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Produto>> ListarAsync()
    {
        return await _db.Produtos.ToListAsync();
    }

    public async Task<Produto?> ObterPorIdAsync(int id)
    {
        return await _db.Produtos.FindAsync(id);
    }

    public async Task AdicionarAsync(Produto produto)
    {
        await _db.Produtos.AddAsync(produto);

    }
    public void Atualizar(Produto produto)
    {
    _db.Produtos.Update(produto);
    }
    
    public void Remover(Produto produto)
    {
    _db.Produtos.Remove(produto);
    }

    public async Task<int> SalvarAlteracoesAsync()
{
    return await _db.SaveChangesAsync();
}

}