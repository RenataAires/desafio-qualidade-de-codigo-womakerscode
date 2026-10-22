namespace PiorEcommerceDoMundo.Services.Interfaces;

public interface IPedidoService
{
    List<Pedido> ListarTodos();
    Pedido? BuscarPorId(int id);
    (bool sucesso, string erro, Pedido pedido) Criar(PedidoRequest request);
}