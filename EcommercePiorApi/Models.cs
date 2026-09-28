namespace PiorEcommerceDoMundo;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public decimal Preco { get; set; }
    public int QuantidadeEstoque { get; set; }
    public bool Ativo { get; set; } = true;
}

public class Pedido
{
    public int Id { get; set; }
    public string ClienteNome { get; set; }
    public string ClienteEmail { get; set; }
    public DateTime DataPedido { get; set; }

    // status controlado por string solta, sem enum: "Pendente", "Pago", "Falhou", "Enviado"
    public string Status { get; set; }
    public decimal ValorTotal { get; set; }
    public string FormaPagamento { get; set; }
    public string CodigoRastreio { get; set; }
    public List<ItemPedido> Itens { get; set; } = new();
}

public class ItemPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public Pedido Pedido { get; set; }
    public int ProdutoId { get; set; }
    public Produto Produto { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}

// requests da API misturados no mesmo arquivo dos models, sem pasta de DTOs
public class PedidoRequest
{
    public string ClienteNome { get; set; }
    public string ClienteEmail { get; set; }
    public string FormaPagamento { get; set; }
    public List<ItemRequest> Itens { get; set; }
}

public class ItemRequest
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
}
