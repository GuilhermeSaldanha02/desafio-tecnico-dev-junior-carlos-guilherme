using EncontreProfessor.Core.Enums;

namespace EncontreProfessor.Core.Models;

public class Disponibilidade
{
    public Disponibilidade(DiaSemanaEnum dia, Intervalo periodo)
    {
        Dia = dia;
        Periodo = periodo;
    }

    public DiaSemanaEnum Dia { get; }
    public Intervalo Periodo { get; }

    public bool Comporta(DiaSemanaEnum dia, Intervalo aula) => Dia == dia && Periodo.Contem(aula);
}
