namespace PiorEcommerceDoMundo.Services.Interfaces;
using PiorEcommerceDoMundo.DTOs;

public interface IPedidoService
{
    Task<(bool sucesso, string erro, Pedido? pedido)> CriarAsync(PedidoRequest request);
    Task<List<Pedido>> ListarTodosAsync();
    Task<Pedido?> BuscarPorIdAsync(int id);
}