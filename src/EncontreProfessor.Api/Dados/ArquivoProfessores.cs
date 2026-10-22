namespace EncontreProfessor.Api.Dados;

// Formato do professores.json. Serve só para leitura do arquivo; quem tem regra são os modelos do Core.
public record ArquivoProfessores(List<ProfessorArquivo> Professores);

public record ProfessorArquivo(
    int Id,
    string Nome,
    List<string> Idiomas,
    bool AgendaBloqueada,
    List<DisponibilidadeArquivo> Disponibilidade,
    List<AulaMarcadaArquivo> AulasMarcadas);

public record DisponibilidadeArquivo(string Dia, string Inicio, string Fim);

public record AulaMarcadaArquivo(string Dia, string Inicio, int DuracaoMinutos);
