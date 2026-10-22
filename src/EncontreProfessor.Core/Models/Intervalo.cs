namespace EncontreProfessor.Core.Models;

public class Intervalo
{
    public Intervalo(int inicio, int fim)
    {
        Inicio = inicio;
        Fim = fim;
    }

    public int Inicio { get; }
    public int Fim { get; }

    public bool Contem(Intervalo outro) => Inicio <= outro.Inicio && outro.Fim <= Fim;

    public bool SeSobrepoe(Intervalo outro) => Inicio < outro.Fim && outro.Inicio < Fim;
}
