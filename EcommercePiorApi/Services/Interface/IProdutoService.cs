namespace PiorEcommerceDoMundo.Services.Interfaces;


public interface IProdutoService
{
    List<Produto> ListarTodos();
    (bool sucesso, Produto produto) BuscarPorId(int id);
    (bool sucesso, string erro, Produto produto) Criar(Produto produto);
    (bool sucesso, Produto produto) Atualizar(int id, Produto produto);
    bool Deletar(int id);
}