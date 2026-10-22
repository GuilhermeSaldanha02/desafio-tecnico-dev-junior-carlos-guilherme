using EncontreProfessor.Core.Enums;
using EncontreProfessor.Core.Models;

namespace EncontreProfessor.Core.Busca;

public class CriterioBusca
{
    private const int MinutosEntreInicios = 30;

    public CriterioBusca(string idioma, IReadOnlyList<DiaSemanaEnum> dias, Intervalo faixa, int duracaoMinutos)
    {
        Idioma = idioma;
        Dias = dias;
        Faixa = faixa;
        DuracaoMinutos = duracaoMinutos;
    }

    public string Idioma { get; }
    public IReadOnlyList<DiaSemanaEnum> Dias { get; }
    public Intervalo Faixa { get; }
    public int DuracaoMinutos { get; }

    public IReadOnlyList<Intervalo> AulasPossiveis()
    {
        List<Intervalo> aulas = new List<Intervalo>();

        for (int inicio = ArredondarParaProximoInicio(Faixa.Inicio); inicio + DuracaoMinutos <= Faixa.Fim; inicio += MinutosEntreInicios)
        {
            aulas.Add(new Intervalo(inicio, inicio + DuracaoMinutos));
        }

        return aulas;
    }

    private static int ArredondarParaProximoInicio(int minutos)
    {
        int resto = minutos % MinutosEntreInicios;
        if (resto == 0) return minutos;

        return minutos + MinutosEntreInicios - resto;
    }
}
