using EncontreProfessor.Core.Models;

namespace EncontreProfessor.Core.Interfaces;

public interface IProfessorRepository
{
    IReadOnlyList<Professor> Listar();
}
