namespace EncontreProfessor.Core.Busca;

public record ProfessorDisponivel(int Id, string Nome, int AulasNaSemana, IReadOnlyList<string> Horarios);
