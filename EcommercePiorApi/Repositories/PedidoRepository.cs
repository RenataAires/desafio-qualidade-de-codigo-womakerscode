using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Repositories.Interfaces;

namespace PiorEcommerceDoMundo.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _db;

    public PedidoRepository(AppDbContext db) => _db = db;

    public Task<List<Pedido>> ListarAsync() => _db.Pedidos
        .AsNoTracking()
        .Include(p => p.Itens)
        .ToListAsync();

    public Task<Pedido?> ObterPorIdAsync(int id) => _db.Pedidos
        .Include(p => p.Itens)
        .FirstOrDefaultAsync(p => p.Id == id);

    public async Task AdicionarAsync(Pedido pedido) => await _db.Pedidos.AddAsync(pedido);

    public void Atualizar(Pedido pedido) => _db.Pedidos.Update(pedido);

    public Task<int> SalvarAlteracoesAsync() => _db.SaveChangesAsync();
}
