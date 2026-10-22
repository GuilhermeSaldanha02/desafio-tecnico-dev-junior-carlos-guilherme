using EncontreProfessor.Core.Helpers;
using EncontreProfessor.Core.Models;

namespace EncontreProfessor.Tests.Models;

public class IntervaloTests
{
    private static Intervalo Criar(string inicio, string fim) => new Intervalo(Horario.Converter(inicio), Horario.Converter(fim));

    [Theory]
    [InlineData("18:00", "19:00", false)] // termina exatamente quando a aula marcada começa
    [InlineData("20:00", "21:00", false)] // começa exatamente quando a aula marcada termina
    [InlineData("18:30", "19:30", true)]  // invade o começo
    [InlineData("19:30", "20:30", true)]  // invade o fim
    [InlineData("19:15", "19:45", true)]  // fica dentro
    [InlineData("18:00", "21:00", true)]  // engloba
    public void SeSobrepoe_ComparadoComAula19As20_RespeitaQueEncostarNaoEColisao(string inicio, string fim, bool esperado)
    {
        Intervalo aulaMarcada = Criar("19:00", "20:00");

        Assert.Equal(esperado, aulaMarcada.SeSobrepoe(Criar(inicio, fim)));
    }

    [Theory]
    [InlineData("18:00", "20:00", true)]  // ocupa o bloco inteiro
    [InlineData("19:00", "20:00", true)]  // termina junto com o bloco
    [InlineData("19:30", "20:30", false)] // passa do fim do bloco
    [InlineData("17:30", "18:30", false)] // começa antes do bloco
    public void Contem_BlocoDas18As20_SoAceitaAulaInteiraDentro(string inicio, string fim, bool esperado)
    {
        Intervalo bloco = Criar("18:00", "20:00");

        Assert.Equal(esperado, bloco.Contem(Criar(inicio, fim)));
    }
}
