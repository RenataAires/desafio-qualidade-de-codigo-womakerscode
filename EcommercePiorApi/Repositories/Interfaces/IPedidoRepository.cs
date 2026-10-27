using PiorEcommerceDoMundo;
namespace PiorEcommerceDoMundo.Repositories.Interfaces;

public interface IPedidoRepository
{
    Task<List<Pedido>> ListarAsync();

    Task<Pedido?> ObterPorIdAsync(int id);

    Task AdicionarAsync(Pedido pedido);

    void Atualizar(Pedido pedido);

    Task<int> SalvarAlteracoesAsync();
}