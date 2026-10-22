using System.Text.RegularExpressions;

namespace EncontreProfessor.Core.Helpers;

public static class Horario
{
    public const int MinutosPorHora = 60;

    private static readonly Regex FormatoHoraMinuto = new Regex(@"^([01][0-9]|2[0-3]):[0-5][0-9]\z");

    public static bool TentarConverter(string? texto, out int minutos)
    {
        minutos = 0;

        if (texto == null) return false;
        if (!FormatoHoraMinuto.IsMatch(texto)) return false;

        int horas = int.Parse(texto.Substring(0, 2));
        int minutosDaHora = int.Parse(texto.Substring(3, 2));

        minutos = horas * MinutosPorHora + minutosDaHora;
        return true;
    }

    public static int Converter(string texto)
    {
        if (!TentarConverter(texto, out int minutos)) throw new FormatException($"Horário fora do formato HH:mm: \"{texto}\".");

        return minutos;
    }

    public static string Formatar(int minutos) => $"{minutos / MinutosPorHora:00}:{minutos % MinutosPorHora:00}";
}
