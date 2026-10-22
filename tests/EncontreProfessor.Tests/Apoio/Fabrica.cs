using EncontreProfessor.Core.Busca;
using EncontreProfessor.Core.Helpers;
using EncontreProfessor.Core.Models;

namespace EncontreProfessor.Tests.Apoio;

public static class Fabrica
{
    // disponibilidade no formato "segunda 18:00-22:00"; aula marcada no formato "segunda 19:00 60"
    public static Professor Professor(
        int id,
        string nome,
        string[] idiomas,
        string[] disponibilidade,
        string[]? aulasMarcadas = null,
        bool agendaBloqueada = false)
    {
        List<Disponibilidade> blocos = disponibilidade.Select(CriarDisponibilidade).ToList();
        List<AulaMarcada> aulas = (aulasMarcadas ?? []).Select(CriarAulaMarcada).ToList();

        return new Professor(id, nome, idiomas, agendaBloqueada, blocos, aulas);
    }

    public static BuscaRequisicao Requisicao(
        string? idioma = "Inglês",
        string?[]? dias = null,
        string? inicioFaixa = "18:00",
        string? fimFaixa = "22:00",
        int? duracaoMinutos = 60)
    {
        return new BuscaRequisicao
        {
            Idioma = idioma,
            Dias = (dias ?? ["segunda"]).ToList(),
            InicioFaixa = inicioFaixa,
            FimFaixa = fimFaixa,
            DuracaoMinutos = duracaoMinutos
        };
    }

    private static Disponibilidade CriarDisponibilidade(string texto)
    {
        string[] partes = texto.Split(' ');
        string[] horarios = partes[1].Split('-');

        Intervalo periodo = new Intervalo(Horario.Converter(horarios[0]), Horario.Converter(horarios[1]));
        return new Disponibilidade(DiaSemanaConversor.Converter(partes[0]), periodo);
    }

    private static AulaMarcada CriarAulaMarcada(string texto)
    {
        string[] partes = texto.Split(' ');

        return new AulaMarcada(DiaSemanaConversor.Converter(partes[0]), Horario.Converter(partes[1]), int.Parse(partes[2]));
    }
}
