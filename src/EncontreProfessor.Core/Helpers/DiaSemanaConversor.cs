using EncontreProfessor.Core.Enums;

namespace EncontreProfessor.Core.Helpers;

public static class DiaSemanaConversor
{
    public static bool TentarConverter(string? texto, out DiaSemanaEnum dia)
    {
        dia = default;

        string textoNormalizado = Normalizador.Normalizar(texto);
        if (textoNormalizado.Length == 0) return false;

        foreach (DiaSemanaEnum candidato in Enum.GetValues<DiaSemanaEnum>())
        {
            if (Normalizador.Normalizar(candidato.ToString()) != textoNormalizado) continue;

            dia = candidato;
            return true;
        }

        return false;
    }

    public static DiaSemanaEnum Converter(string texto)
    {
        if (!TentarConverter(texto, out DiaSemanaEnum dia)) throw new FormatException($"Dia da semana inválido: \"{texto}\".");

        return dia;
    }
}
