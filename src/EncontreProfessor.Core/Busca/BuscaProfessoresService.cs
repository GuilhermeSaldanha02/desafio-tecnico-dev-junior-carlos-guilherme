using EncontreProfessor.Core.Enums;
using EncontreProfessor.Core.Helpers;
using EncontreProfessor.Core.Interfaces;
using EncontreProfessor.Core.Models;
using EncontreProfessor.Core.Validacao;

namespace EncontreProfessor.Core.Busca;

public class BuscaProfessoresService
{
    private readonly IProfessorRepository _professorRepository;

    public BuscaProfessoresService(IProfessorRepository professorRepository)
    {
        _professorRepository = professorRepository;
    }

    public ResultadoBusca Buscar(BuscaRequisicao? requisicao)
    {
        ResultadoValidacao validacao = ValidadorBusca.Validar(requisicao);
        if (!validacao.EhValida) return ResultadoBusca.ComErros(validacao.Erros);

        CriterioBusca criterio = validacao.Criterio!;
        IReadOnlyList<Intervalo> aulasPossiveis = criterio.AulasPossiveis();

        List<ProfessorDisponivel> disponiveis = new List<ProfessorDisponivel>();
        List<ProfessorNaoElegivel> naoElegiveis = new List<ProfessorNaoElegivel>();

        foreach (Professor professor in _professorRepository.Listar())
        {
            if (!professor.EnsinaIdioma(criterio.Idioma))
            {
                naoElegiveis.Add(new ProfessorNaoElegivel(professor.Id, professor.Nome, MotivosNaoElegivel.NaoEnsinaIdioma));
                continue;
            }

            if (professor.AgendaBloqueada)
            {
                naoElegiveis.Add(new ProfessorNaoElegivel(professor.Id, professor.Nome, MotivosNaoElegivel.AgendaBloqueada));
                continue;
            }

            List<string> horarios = HorariosEmTodosOsDias(professor, criterio.Dias, aulasPossiveis);
            if (horarios.Count == 0)
            {
                naoElegiveis.Add(new ProfessorNaoElegivel(professor.Id, professor.Nome, MotivosNaoElegivel.SemHorarioEmComum));
                continue;
            }

            disponiveis.Add(new ProfessorDisponivel(professor.Id, professor.Nome, professor.AulasNaSemana, horarios));
        }

        BuscaResposta resposta = new BuscaResposta(OrdenarDisponiveis(disponiveis), OrdenarNaoElegiveis(naoElegiveis));
        return ResultadoBusca.ComResposta(resposta);
    }

    private static List<string> HorariosEmTodosOsDias(Professor professor, IReadOnlyList<DiaSemanaEnum> dias, IReadOnlyList<Intervalo> aulasPossiveis)
    {
        return aulasPossiveis
            .Where(aula => dias.All(dia => professor.EstaLivre(dia, aula)))
            .Select(aula => Horario.Formatar(aula.Inicio))
            .ToList();
    }

    private static List<ProfessorDisponivel> OrdenarDisponiveis(List<ProfessorDisponivel> disponiveis)
    {
        return disponiveis
            .OrderBy(professor => professor.AulasNaSemana)
            .ThenBy(professor => Normalizador.Normalizar(professor.Nome), StringComparer.Ordinal)
            .ThenBy(professor => professor.Id)
            .ToList();
    }

    private static List<ProfessorNaoElegivel> OrdenarNaoElegiveis(List<ProfessorNaoElegivel> naoElegiveis)
    {
        return naoElegiveis
            .OrderBy(professor => Normalizador.Normalizar(professor.Nome), StringComparer.Ordinal)
            .ThenBy(professor => professor.Id)
            .ToList();
    }
}
