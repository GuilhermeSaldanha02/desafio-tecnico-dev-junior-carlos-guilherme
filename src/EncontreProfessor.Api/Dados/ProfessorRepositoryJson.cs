using System.Text.Json;
using EncontreProfessor.Core.Helpers;
using EncontreProfessor.Core.Interfaces;
using EncontreProfessor.Core.Models;

namespace EncontreProfessor.Api.Dados;

public class ProfessorRepositoryJson : IProfessorRepository
{
    private static readonly JsonSerializerOptions OpcoesLeitura = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    private readonly List<Professor> _professores;

    public ProfessorRepositoryJson(string caminhoArquivo)
    {
        if (!File.Exists(caminhoArquivo)) throw new FileNotFoundException($"Arquivo de professores não encontrado: {caminhoArquivo}");

        string conteudo = File.ReadAllText(caminhoArquivo);
        ArquivoProfessores? arquivo = JsonSerializer.Deserialize<ArquivoProfessores>(conteudo, OpcoesLeitura);
        if (arquivo == null) throw new InvalidDataException($"Arquivo de professores vazio: {caminhoArquivo}");

        _professores = arquivo.Professores.Select(Converter).ToList();
    }

    public IReadOnlyList<Professor> Listar() => _professores;

    private static Professor Converter(ProfessorArquivo professor)
    {
        List<Disponibilidade> disponibilidades = professor.Disponibilidade
            .Select(bloco => new Disponibilidade(
                DiaSemanaConversor.Converter(bloco.Dia),
                new Intervalo(Horario.Converter(bloco.Inicio), Horario.Converter(bloco.Fim))))
            .ToList();

        List<AulaMarcada> aulasMarcadas = professor.AulasMarcadas
            .Select(aula => new AulaMarcada(DiaSemanaConversor.Converter(aula.Dia), Horario.Converter(aula.Inicio), aula.DuracaoMinutos))
            .ToList();

        return new Professor(professor.Id, professor.Nome, professor.Idiomas, professor.AgendaBloqueada, disponibilidades, aulasMarcadas);
    }
}
