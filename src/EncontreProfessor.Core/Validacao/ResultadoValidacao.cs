using EncontreProfessor.Core.Busca;

namespace EncontreProfessor.Core.Validacao;

public class ResultadoValidacao
{
    private ResultadoValidacao(IReadOnlyList<string> erros, CriterioBusca? criterio)
    {
        Erros = erros;
        Criterio = criterio;
    }

    public IReadOnlyList<string> Erros { get; }
    public CriterioBusca? Criterio { get; }

    public bool EhValida => Erros.Count == 0;

    public static ResultadoValidacao Invalida(IReadOnlyList<string> erros) => new ResultadoValidacao(erros, null);

    public static ResultadoValidacao Valida(CriterioBusca criterio) => new ResultadoValidacao([], criterio);
}
