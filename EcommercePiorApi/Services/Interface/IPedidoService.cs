namespace PiorEcommerceDoMundo.Services.Interfaces;

public interface IPedidoService
{
    Task<(bool sucesso, string erro, Pedido pedido)> CriarAsync(PedidoRequest request);
    Task<List<Pedido>> ListarTodosAsync();
    Task<Pedido> BuscarPorIdAsync(int id);
}