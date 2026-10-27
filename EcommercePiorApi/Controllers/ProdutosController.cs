using Microsoft.AspNetCore.Mvc;
using PiorEcommerceDoMundo.DTOs;
using PiorEcommerceDoMundo.Services.Interfaces;

namespace PiorEcommerceDoMundo.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    // GET: api/produtos
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProdutoResponseDTO>>> ListarTodos()
    {
        var produtos = await _produtoService.ListarTodosAsync();

        var resposta = produtos.Select(ProdutoResponseDTO.FromEntity);

        return Ok(resposta);
    }

    // GET: api/produtos/1
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoResponseDTO>> BuscarPorId(int id)
    {
        var (sucesso, produto) =
            await _produtoService.BuscarPorIdAsync(id);

        if (!sucesso || produto is null)
        {
            return NotFound(new
            {
                mensagem = "Produto não encontrado."
            });
        }

        return Ok(ProdutoResponseDTO.FromEntity(produto));
    }

    // POST: api/produtos
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProdutoResponseDTO>> Criar(
        [FromBody] CriarProdutoDTO dto)
    {
        var produto = new Produto
        {
            Nome = dto.Nome.Trim(),
            Descricao = dto.Descricao.Trim(),
            Preco = dto.Preco,
            QuantidadeEstoque = dto.QuantidadeEstoque,
            Ativo = dto.Ativo
        };

        var (sucesso, erro, criado) =
            await _produtoService.CriarAsync(produto);

        if (!sucesso || criado is null)
        {
            return BadRequest(new { mensagem = erro });
        }

        var resposta = ProdutoResponseDTO.FromEntity(criado);

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { id = criado.Id },
            resposta);
    }

    // PUT: api/produtos/1
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoResponseDTO>> Atualizar(
        int id,
        [FromBody] AtualizarProdutoDTO dto)
    {
        var produtoAtualizado = new Produto
        {
            Nome = dto.Nome.Trim(),
            Descricao = dto.Descricao.Trim(),
            Preco = dto.Preco,
            QuantidadeEstoque = dto.QuantidadeEstoque,
            Ativo = dto.Ativo
        };

        var (sucesso, produto) =
            await _produtoService.AtualizarAsync(id, produtoAtualizado);

        if (!sucesso || produto is null)
        {
            return NotFound(new
            {
                mensagem = "Produto não encontrado ou dados inválidos."
            });
        }

        return Ok(ProdutoResponseDTO.FromEntity(produto));
    }

    // DELETE: api/produtos/1
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(int id)
    {
        var removido = await _produtoService.DeletarAsync(id);

        if (!removido)
        {
            return NotFound(new
            {
                mensagem = "Produto não encontrado."
            });
        }

        return NoContent();
    }
}
