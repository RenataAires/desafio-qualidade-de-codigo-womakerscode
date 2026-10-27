using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Repositories.Interfaces;

namespace PiorEcommerceDoMundo.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _db;

    public ProdutoRepository(AppDbContext db) => _db = db;

    public Task<List<Produto>> ListarAsync() => _db.Produtos.AsNoTracking().ToListAsync();

    public Task<Produto?> ObterPorIdAsync(int id) => _db.Produtos.FirstOrDefaultAsync(p => p.Id == id);

    public async Task AdicionarAsync(Produto produto) => await _db.Produtos.AddAsync(produto);

    public void Atualizar(Produto produto) => _db.Produtos.Update(produto);

    public void Remover(Produto produto) => _db.Produtos.Remove(produto);

    public Task<int> SalvarAlteracoesAsync() => _db.SaveChangesAsync();
}
