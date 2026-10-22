using PiorEcommerceDoMundo.Services.Interfaces;

namespace PiorEcommerceDoMundo.Services;

public class EstoqueService : IEstoqueService
{
    public List<object> ListarEstoque()
    {
        var db = new AppDbContext();
        var lista = db.Produtos.ToList();
        var resultado = new List<object>();
        foreach (var p in lista)
        {
            resultado.Add(new { produto = p.Nome, quantidade = p.QuantidadeEstoque });
        }
        return resultado;
    }

    public (bool sucesso, Produto produto) Reabastecer(int produtoId, int quantidade)
    {
        if (quantidade <= 0) return (false, null);
        var db = new AppDbContext();
        var produto = db.Produtos.Find(produtoId);
        if (produto == null) return (false, null);

        produto.QuantidadeEstoque = produto.QuantidadeEstoque + quantidade;
        db.SaveChanges();
        Utils.Log("Estoque reabastecido produto " + produtoId);

        return (true, produto);
    }

    public (bool sucesso, string erro, Produto produto) ValidarEBaixarEstoque(int produtoId, int quantidade)
    {
        var db = new AppDbContext();
        var produto = db.Produtos.Find(produtoId);

        if (produto == null)
            return (false, "produto " + produtoId + " nao encontrado", null);

        if (!produto.Ativo)
            return (false, "produto " + produto.Nome + " esta inativo", null);

        if (produto.QuantidadeEstoque < quantidade)
            return (false, "estoque insuficiente para " + produto.Nome, null);

        produto.QuantidadeEstoque = produto.QuantidadeEstoque - quantidade;
        db.SaveChanges();

        return (true, string.Empty, produto);
    }

    public void EstornarEstoque(int produtoId, int quantidade)
    {
        var db = new AppDbContext();
        var produto = db.Produtos.Find(produtoId);
        if (produto != null)
        {
            produto.QuantidadeEstoque = produto.QuantidadeEstoque + quantidade;
            db.SaveChanges();
        }
    }
}