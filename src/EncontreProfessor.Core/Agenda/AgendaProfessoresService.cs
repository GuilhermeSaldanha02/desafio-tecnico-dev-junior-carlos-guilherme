using EncontreProfessor.Core.Enums;
using EncontreProfessor.Core.Helpers;
using EncontreProfessor.Core.Interfaces;
using EncontreProfessor.Core.Models;

namespace EncontreProfessor.Core.Agenda;

// Fora do enunciado (ADR-025): ajuda quem testa a entender por que um professor aparece ou fica de fora.
// As aulas marcadas aparecem só como horário ocupado, sem dizer de quem é a aula.
public class AgendaProfessoresService
{
    private readonly IProfessorRepository _professorRepository;

    public AgendaProfessoresService(IProfessorRepository professorRepository)
    {
        _professorRepository = professorRepository;
    }

    public IReadOnlyList<AgendaProfessor> Listar()
    {
        return _professorRepository.Listar()
            .OrderBy(professor => Normalizador.Normalizar(professor.Nome), StringComparer.Ordinal)
            .ThenBy(professor => professor.Id)
            .Select(MontarAgenda)
            .ToList();
    }

    private static AgendaProfessor MontarAgenda(Professor professor)
    {
        List<AgendaDia> dias = new List<AgendaDia>();

        foreach (DiaSemanaEnum dia in Enum.GetValues<DiaSemanaEnum>())
        {
            List<PeriodoAgenda> disponivel = professor.Disponibilidades
                .Where(disponibilidade => disponibilidade.Dia == dia)
                .Select(disponibilidade => Formatar(disponibilidade.Periodo))
                .ToList();

            List<PeriodoAgenda> ocupado = professor.AulasMarcadas
                .Where(aula => aula.Dia == dia)
                .Select(aula => Formatar(aula.Periodo))
                .ToList();

            if (disponivel.Count == 0 && ocupado.Count == 0) continue;

            dias.Add(new AgendaDia(dia.ToString().ToLowerInvariant(), disponivel, ocupado));
        }

        return new AgendaProfessor(professor.Id, professor.Nome, professor.Idiomas, professor.AgendaBloqueada, professor.AulasNaSemana, dias);
    }

    private static PeriodoAgenda Formatar(Intervalo periodo) => new PeriodoAgenda(Horario.Formatar(periodo.Inicio), Horario.Formatar(periodo.Fim));
}
