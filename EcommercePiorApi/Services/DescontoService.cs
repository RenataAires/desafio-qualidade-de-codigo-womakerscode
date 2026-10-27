using PiorEcommerceDoMundo.Services.Interfaces;

namespace PiorEcommerceDoMundo.Services;

public class DescontoService : IDescontoService
{
    private const decimal ValorMinimoParaDesconto = 500m;
    private const decimal TaxaDescontoValorAlto = 0.10m;
    private const decimal TaxaDescontoClienteVip = 0.20m;

    public decimal AplicarDesconto(decimal total, string clienteEmail)
    {
        if (total < 0) throw new ArgumentOutOfRangeException(nameof(total), "O total não pode ser negativo.");

        if (total > ValorMinimoParaDesconto)
            return total - total * TaxaDescontoValorAlto;

        if (!string.IsNullOrWhiteSpace(clienteEmail) && clienteEmail.Contains("vip", StringComparison.OrdinalIgnoreCase))
            return total - total * TaxaDescontoClienteVip;

        return total;
    }
}
