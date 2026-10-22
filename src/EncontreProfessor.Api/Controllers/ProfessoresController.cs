using EncontreProfessor.Api.Erros;
using EncontreProfessor.Core.Busca;
using Microsoft.AspNetCore.Mvc;

namespace EncontreProfessor.Api.Controllers;

[ApiController]
[Route("api/professores")]
public class ProfessoresController : ControllerBase
{
    private readonly BuscaProfessoresService _buscaProfessoresService;

    public ProfessoresController(BuscaProfessoresService buscaProfessoresService)
    {
        _buscaProfessoresService = buscaProfessoresService;
    }

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
