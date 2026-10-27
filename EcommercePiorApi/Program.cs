using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using PiorEcommerceDoMundo;
using PiorEcommerceDoMundo.Repositories;
using PiorEcommerceDoMundo.Repositories.Interfaces;
using PiorEcommerceDoMundo.Services;
using PiorEcommerceDoMundo.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("A connection string 'DefaultConnection' não foi configurada.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();

builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IEstoqueService, EstoqueService>();
builder.Services.AddScoped<IDescontoService, DescontoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Temporário enquanto a API ainda retorna entidades; DTOs eliminarão esse ciclo.
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Mantido para preservar o comportamento inicial do desafio.
// Em uma evolução futura, a equipe pode substituir EnsureCreated por migrations.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Produtos.Any())
    {
        db.Produtos.AddRange(
            new Produto { Nome = "Camiseta", Descricao = "Camiseta básica", Preco = 49.90m, QuantidadeEstoque = 100, Ativo = true },
            new Produto { Nome = "Tênis", Descricao = "Tênis esportivo", Preco = 299.90m, QuantidadeEstoque = 50, Ativo = true },
            new Produto { Nome = "Boné", Descricao = "Boné ajustável", Preco = 39.90m, QuantidadeEstoque = 200, Ativo = true },
            new Produto { Nome = "Mochila", Descricao = "Mochila para notebook", Preco = 159.90m, QuantidadeEstoque = 30, Ativo = true }
        );
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
