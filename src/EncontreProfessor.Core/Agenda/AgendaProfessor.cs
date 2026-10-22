namespace EncontreProfessor.Core.Agenda;

// Só leitura: mostra a agenda como está, sem aplicar nenhuma regra da busca.
public record AgendaProfessor(
    int Id,
    string Nome,
    IReadOnlyList<string> Idiomas,
    bool AgendaBloqueada,
    int AulasNaSemana,
    IReadOnlyList<AgendaDia> Dias);

public record AgendaDia(string Dia, IReadOnlyList<PeriodoAgenda> Disponivel, IReadOnlyList<PeriodoAgenda> Ocupado);

public record PeriodoAgenda(string Inicio, string Fim);
