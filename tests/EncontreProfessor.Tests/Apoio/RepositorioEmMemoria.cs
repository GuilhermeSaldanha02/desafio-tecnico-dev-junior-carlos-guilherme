using EncontreProfessor.Core.Interfaces;
using EncontreProfessor.Core.Models;

namespace EncontreProfessor.Tests.Apoio;

public class RepositorioEmMemoria : IProfessorRepository
{
    private readonly List<Professor> _professores;

    public RepositorioEmMemoria(params Professor[] professores)
    {
        _professores = professores.ToList();
    }

    public IReadOnlyList<Professor> Listar() => _professores;
}
