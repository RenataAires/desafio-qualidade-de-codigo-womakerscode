using PiorEcommerceDoMundo.Services.Interfaces;

namespace PiorEcommerceDoMundo.Services;

public class DescontoService : IDescontoService
{
    private const decimal ValorMinimoParaDesconto = 500m;
    private const decimal TaxaDescontoValorAlto = 0.10m; // 10%
    private const decimal TaxaDescontoClienteVip = 0.20m; // 20%

    public decimal AplicarDesconto(decimal total, string clienteEmail)
    {
        if (total > ValorMinimoParaDesconto)
        {
            return total - (total * TaxaDescontoValorAlto);
        }
        else if (!string.IsNullOrEmpty(clienteEmail) && clienteEmail.Contains("vip"))
        {
            return total - (total * TaxaDescontoClienteVip);
        }

        return total;
    }
}