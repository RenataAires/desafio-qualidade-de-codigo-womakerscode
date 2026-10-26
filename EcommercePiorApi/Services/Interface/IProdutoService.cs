namespace PiorEcommerceDoMundo.Services.Interfaces;

public interface IProdutoService
{
    Task<List<Produto>> ListarTodosAsync();
    Task<(bool sucesso, Produto produto)> BuscarPorIdAsync(int id);
    Task<(bool sucesso, string erro, Produto produto)> CriarAsync(Produto produto);
    Task<(bool sucesso, Produto produto)> AtualizarAsync(int id, Produto produtoAtualizado);
    Task<bool> DeletarAsync(int id);
}