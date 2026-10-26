#nullable enable

using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Repositories.Interfaces;

namespace PiorEcommerceDoMundo.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _db;

    public PedidoRepository(AppDbContext db)
    {
        _db = db;
    }
    public async Task<List<Pedido>> ListarAsync()
    {
    return await _db.Pedidos
        .Include(p => p.Itens)
        .ToListAsync();
    }

    public async Task<Pedido?> ObterPorIdAsync(int id)
    {
    return await _db.Pedidos
        .Include(p => p.Itens)
        .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AdicionarAsync(Pedido pedido)
    {
    await _db.Pedidos.AddAsync(pedido);
    }

    public void Atualizar(Pedido pedido)
    {
    _db.Pedidos.Update(pedido);
    }

public async Task<int> SalvarAlteracoesAsync()
    {
    return await _db.SaveChangesAsync();
    }

}