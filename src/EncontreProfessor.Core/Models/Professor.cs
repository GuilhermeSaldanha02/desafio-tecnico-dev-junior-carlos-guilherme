using EncontreProfessor.Core.Enums;
using EncontreProfessor.Core.Helpers;

namespace EncontreProfessor.Core.Models;

public class Professor
{
    public Professor(
        int id,
        string nome,
        IEnumerable<string> idiomas,
        bool agendaBloqueada,
        IEnumerable<Disponibilidade> disponibilidades,
        IEnumerable<AulaMarcada> aulasMarcadas)
    {
        Id = id;
        Nome = nome;
        Idiomas = idiomas.ToList();
        AgendaBloqueada = agendaBloqueada;
        Disponibilidades = disponibilidades.ToList();
        AulasMarcadas = aulasMarcadas.ToList();
    }

    public int Id { get; }
    public string Nome { get; }
    public IReadOnlyList<string> Idiomas { get; }
    public bool AgendaBloqueada { get; }
    public IReadOnlyList<Disponibilidade> Disponibilidades { get; }
    public IReadOnlyList<AulaMarcada> AulasMarcadas { get; }

    public int AulasNaSemana => AulasMarcadas.Count;

    public bool EnsinaIdioma(string idioma)
    {
        string idiomaNormalizado = Normalizador.Normalizar(idioma);

        return Idiomas.Any(idiomaEnsinado => Normalizador.Normalizar(idiomaEnsinado) == idiomaNormalizado);
    }

    public bool EstaLivre(DiaSemanaEnum dia, Intervalo aula)
    {
        bool cabeEmAlgumBloco = Disponibilidades.Any(disponibilidade => disponibilidade.Comporta(dia, aula));
        if (!cabeEmAlgumBloco) return false;

        return !AulasMarcadas.Any(aulaMarcada => aulaMarcada.SeSobrepoe(dia, aula));
    }
}
