using PiorEcommerceDoMundo.Services.Interfaces;

namespace PiorEcommerceDoMundo.Services;

public class ProdutoService : IProdutoService
{
    public List<Produto> ListarTodos()
    {
        var db = new AppDbContext();
        return db.Produtos.ToList();
    }

    public (bool sucesso, Produto? produto) BuscarPorId(int id)
    {
        var db = new AppDbContext();
        var produto = db.Produtos.Find(id);
        if (produto == null) return (false, null);

        return (true, produto);
    }

    public (bool sucesso, string erro, Produto produto) Criar(Produto produto)
    {
        if (string.IsNullOrEmpty(produto.Nome))
            return (false, "nome invalido", null);

        if (produto.Preco <= 0)
            return (false, "preco invalido", null);

        var db = new AppDbContext();
        db.Produtos.Add(produto);
        db.SaveChanges();
        Utils.Log("Produto criado: " + produto.Nome);

        return (true, string.Empty, produto);
    }

    public (bool sucesso, Produto produto) Atualizar(int id, Produto produto)
    {
        var db = new AppDbContext();
        var existente = db.Produtos.Find(id);
        if (existente == null) return (false, null);

        existente.Nome = produto.Nome;
        existente.Descricao = produto.Descricao;
        existente.Preco = produto.Preco;
        existente.QuantidadeEstoque = produto.QuantidadeEstoque;
        existente.Ativo = produto.Ativo;

        db.SaveChanges();
        return (true, existente);
    }

    public bool Deletar(int id)
    {
        var db = new AppDbContext();
        var existente = db.Produtos.Find(id);
        if (existente == null) return false;

        db.Produtos.Remove(existente);
        db.SaveChanges();
        return true;
    }
}