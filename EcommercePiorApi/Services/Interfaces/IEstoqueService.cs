using PiorEcommerceDoMundo;

namespace PiorEcommerceDoMundo.Services.Interfaces;

public interface IEstoqueService
{
    Task<object> ListarEstoqueAsync();
    Task<(bool sucesso, Produto? produto)> ReabastecerAsync(int produtoId, int quantidade);
    Task EstornarEstoqueAsync(List<ItemPedido> itens);
}