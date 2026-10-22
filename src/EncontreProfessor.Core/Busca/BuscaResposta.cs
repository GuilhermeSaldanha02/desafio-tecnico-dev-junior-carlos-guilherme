namespace EncontreProfessor.Core.Busca;

public record BuscaResposta(IReadOnlyList<ProfessorDisponivel> Professores, IReadOnlyList<ProfessorNaoElegivel> NaoElegiveis);
