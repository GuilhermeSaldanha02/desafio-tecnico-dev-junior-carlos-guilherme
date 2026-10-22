using EncontreProfessor.Core.Busca;
using EncontreProfessor.Core.Enums;
using EncontreProfessor.Core.Helpers;
using EncontreProfessor.Core.Models;

namespace EncontreProfessor.Core.Validacao;

public static class ValidadorBusca
{
    public const string ErroRequisicaoAusente = "O corpo da requisição é obrigatório.";
    public const string ErroIdiomaVazio = "O idioma é obrigatório.";
    public const string ErroSemDias = "Informe pelo menos um dia da semana.";
    public const string ErroDiaInvalido = "Dia inválido: \"{0}\". Use segunda, terca, quarta, quinta, sexta, sabado ou domingo.";
    public const string ErroInicioFaixa = "O início da faixa deve estar no formato HH:mm (ex.: 18:00).";
    public const string ErroFimFaixa = "O fim da faixa deve estar no formato HH:mm (ex.: 20:00).";
    public const string ErroFaixaInvertida = "O fim da faixa deve ser depois do início.";
    public const string ErroDuracao = "A duração deve ser de 30, 60 ou 90 minutos.";

    private static readonly int[] DuracoesPermitidas = [30, 60, 90];

    public static ResultadoValidacao Validar(BuscaRequisicao? requisicao)
    {
        if (requisicao == null) return ResultadoValidacao.Invalida([ErroRequisicaoAusente]);

        List<string> erros = new List<string>();

        if (string.IsNullOrWhiteSpace(requisicao.Idioma)) erros.Add(ErroIdiomaVazio);

        List<DiaSemanaEnum> dias = ValidarDias(requisicao.Dias, erros);

        bool inicioValido = Horario.TentarConverter(requisicao.InicioFaixa, out int inicio);
        if (!inicioValido) erros.Add(ErroInicioFaixa);

        bool fimValido = Horario.TentarConverter(requisicao.FimFaixa, out int fim);
        if (!fimValido) erros.Add(ErroFimFaixa);

        if (inicioValido && fimValido && fim <= inicio) erros.Add(ErroFaixaInvertida);

        bool duracaoValida = requisicao.DuracaoMinutos.HasValue && DuracoesPermitidas.Contains(requisicao.DuracaoMinutos.Value);
        if (!duracaoValida) erros.Add(ErroDuracao);

        if (erros.Count > 0) return ResultadoValidacao.Invalida(erros);

        CriterioBusca criterio = new CriterioBusca(requisicao.Idioma!, dias, new Intervalo(inicio, fim), requisicao.DuracaoMinutos!.Value);
        return ResultadoValidacao.Valida(criterio);
    }

    private static List<DiaSemanaEnum> ValidarDias(List<string?>? textos, List<string> erros)
    {
        List<DiaSemanaEnum> dias = new List<DiaSemanaEnum>();

        if (textos == null || textos.Count == 0)
        {
            erros.Add(ErroSemDias);
            return dias;
        }

        foreach (string? texto in textos)
        {
            if (!DiaSemanaConversor.TentarConverter(texto, out DiaSemanaEnum dia))
            {
                erros.Add(string.Format(ErroDiaInvalido, texto));
                continue;
            }

            if (dias.Contains(dia)) continue;

            dias.Add(dia);
        }

        return dias;
    }
}
