namespace EncontreProfessor.Core.Busca;

public class ResultadoBusca
{
    private ResultadoBusca(IReadOnlyList<string> erros, BuscaResposta? resposta)
    {
        Erros = erros;
        Resposta = resposta;
    }

    public IReadOnlyList<string> Erros { get; }
    public BuscaResposta? Resposta { get; }

    public bool EhSucesso => Erros.Count == 0;

    public static ResultadoBusca ComErros(IReadOnlyList<string> erros) => new ResultadoBusca(erros, null);

    public static ResultadoBusca ComResposta(BuscaResposta resposta) => new ResultadoBusca([], resposta);
}
