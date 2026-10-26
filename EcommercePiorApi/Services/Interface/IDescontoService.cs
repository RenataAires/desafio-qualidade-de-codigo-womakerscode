namespace PiorEcommerceDoMundo.Services.Interfaces;

public interface IDescontoService
{
    decimal AplicarDesconto(decimal total, string clienteEmail);
}