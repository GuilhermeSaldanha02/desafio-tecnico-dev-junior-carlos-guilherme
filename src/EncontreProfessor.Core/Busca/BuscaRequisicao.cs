namespace EncontreProfessor.Core.Busca;

public class BuscaRequisicao
{
    /// <summary>Idioma da aula. Comparado sem acento e sem diferença de maiúscula.</summary>
    /// <example>Espanhol</example>
    public string? Idioma { get; set; }

    /// <summary>Dias da semana: segunda, terca, quarta, quinta, sexta, sabado ou domingo. O mesmo horário precisa estar livre em todos.</summary>
    /// <example>["segunda"]</example>
    public List<string?>? Dias { get; set; }

    /// <summary>Início da faixa em que o aluno pode ter aula, no formato HH:mm.</summary>
    /// <example>18:00</example>
    public string? InicioFaixa { get; set; }

    /// <summary>Fim da faixa, no formato HH:mm. Precisa ser depois do início.</summary>
    /// <example>20:00</example>
    public string? FimFaixa { get; set; }

    /// <summary>Duração da aula em minutos: 30, 60 ou 90.</summary>
    /// <example>60</example>
    public int? DuracaoMinutos { get; set; }
}
