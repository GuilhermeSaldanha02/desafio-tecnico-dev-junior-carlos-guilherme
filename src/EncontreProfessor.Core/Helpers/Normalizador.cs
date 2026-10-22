using System.Globalization;
using System.Text;

namespace EncontreProfessor.Core.Helpers;

public static class Normalizador
{
    public static string Normalizar(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

        string decomposto = texto.Trim().Normalize(NormalizationForm.FormD);
        StringBuilder semAcento = new StringBuilder(decomposto.Length);

        foreach (char caractere in decomposto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caractere) == UnicodeCategory.NonSpacingMark) continue;

            semAcento.Append(caractere);
        }

        return semAcento.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
    }
}
