namespace PiorEcommerceDoMundo.Services.Interfaces;

public interface IEstoqueService
{
    List<object> ListarEstoque();
    (bool sucesso, Produto produto) Reabastecer(int produtoId, int quantidade);
    
    (bool sucesso, string erro, Produto produto) ValidarEBaixarEstoque(int produtoId, int quantidade);
    void EstornarEstoque(int produtoId, int quantidade);
}