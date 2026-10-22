using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EncontreProfessor.Api.Erros;

// Erros que acontecem antes da regra de negócio: JSON malformado ou campo com tipo errado.
// O ASP.NET responderia em inglês e em outro formato; aqui sai no mesmo { "erros": [...] } da validação.
public static class RespostaErroLeitura
{
    public const string ErroCorpoInvalido = "O corpo da requisição não é um JSON válido.";
    public const string ErroCampoInvalido = "O campo \"{0}\" tem um valor de tipo inválido.";

    private const string PrefixoCampoJson = "$.";

    public static IActionResult Criar(ActionContext contexto)
    {
        List<string> errosDeCampo = new List<string>();

        foreach (KeyValuePair<string, ModelStateEntry> entrada in contexto.ModelState)
        {
            if (entrada.Value.Errors.Count == 0) continue;
            if (!entrada.Key.StartsWith(PrefixoCampoJson)) continue;

            errosDeCampo.Add(string.Format(ErroCampoInvalido, entrada.Key.Substring(PrefixoCampoJson.Length)));
        }

        if (errosDeCampo.Count == 0) return new BadRequestObjectResult(new RespostaErro([ErroCorpoInvalido]));

        return new BadRequestObjectResult(new RespostaErro(errosDeCampo));
    }
}
