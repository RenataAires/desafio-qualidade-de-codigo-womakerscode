namespace PiorEcommerceDoMundo;

// Mock simples e previsível. A Pessoa 5 poderá encapsulá-lo em IPagamentoService.
public static class PagamentoMock
{
    public static Task<bool> ProcessarPagamentoAsync(decimal valor, string formaPagamento)
    {
        Console.WriteLine($"Processando pagamento de {valor:C} via {formaPagamento}.");

        if (valor <= 0 || string.IsNullOrWhiteSpace(formaPagamento))
            return Task.FromResult(false);

        // Cenário previsível para testar recusa: envie "recusado" como forma de pagamento.
        var recusado = formaPagamento.Equals("recusado", StringComparison.OrdinalIgnoreCase)
            || formaPagamento.Equals("falha", StringComparison.OrdinalIgnoreCase);

        return Task.FromResult(!recusado);
    }
}
