using EncontreProfessor.Api.Erros;
using EncontreProfessor.Core.Busca;
using Microsoft.AspNetCore.Mvc;

namespace EncontreProfessor.Api.Controllers;

[ApiController]
[Route("api/professores")]
[Produces("application/json")]
public class ProfessoresController : ControllerBase
{
    private readonly BuscaProfessoresService _buscaProfessoresService;

    public ProfessoresController(BuscaProfessoresService buscaProfessoresService)
    {
        _buscaProfessoresService = buscaProfessoresService;
    }

    /// <summary>Busca os professores que podem dar aula no mesmo horário em todos os dias pedidos.</summary>
    /// <response code="200">Professores disponíveis com os horários possíveis, e os que ficaram de fora com o motivo.</response>
    /// <response code="400">Requisição inválida: a lista "erros" traz todos os problemas encontrados.</response>
    [HttpPost("busca")]
    [ProducesResponseType(typeof(BuscaResposta), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaErro), StatusCodes.Status400BadRequest)]
    public IActionResult Buscar([FromBody] BuscaRequisicao? requisicao)
    {
        ResultadoBusca resultado = _buscaProfessoresService.Buscar(requisicao);
        if (!resultado.EhSucesso) return BadRequest(new RespostaErro(resultado.Erros));

        return Ok(resultado.Resposta);
    }
}
