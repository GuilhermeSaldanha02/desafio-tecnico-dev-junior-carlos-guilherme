using EncontreProfessor.Core.Enums;

namespace EncontreProfessor.Core.Models;

public class AulaMarcada
{
    public AulaMarcada(DiaSemanaEnum dia, int inicio, int duracaoMinutos)
    {
        Dia = dia;
        Periodo = new Intervalo(inicio, inicio + duracaoMinutos);
    }

    public DiaSemanaEnum Dia { get; }
    public Intervalo Periodo { get; }

    public bool SeSobrepoe(DiaSemanaEnum dia, Intervalo aula) => Dia == dia && Periodo.SeSobrepoe(aula);
}
