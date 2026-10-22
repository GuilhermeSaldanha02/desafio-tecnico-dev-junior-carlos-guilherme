using EncontreProfessor.Core.Busca;
using EncontreProfessor.Core.Enums;
using EncontreProfessor.Core.Validacao;
using EncontreProfessor.Tests.Apoio;

namespace EncontreProfessor.Tests.Validacao;

public class ValidadorBuscaTests
{
    [Fact]
    public void Validar_RequisicaoCorreta_NaoTemErros()
    {
        ResultadoValidacao resultado = ValidadorBusca.Validar(Fabrica.Requisicao());

        Assert.True(resultado.EhValida);
        Assert.NotNull(resultado.Criterio);
    }

    [Fact]
    public void Validar_RequisicaoAusente_DevolveErro()
    {
        ResultadoValidacao resultado = ValidadorBusca.Validar(null);

        Assert.Equal([ValidadorBusca.ErroRequisicaoAusente], resultado.Erros);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validar_IdiomaVazio_DevolveErro(string? idioma)
    {
        ResultadoValidacao resultado = ValidadorBusca.Validar(Fabrica.Requisicao(idioma: idioma));

        Assert.Equal([ValidadorBusca.ErroIdiomaVazio], resultado.Erros);
    }

    [Fact]
    public void Validar_ListaDeDiasVazia_DevolveErro()
    {
        ResultadoValidacao resultado = ValidadorBusca.Validar(Fabrica.Requisicao(dias: []));

        Assert.Equal([ValidadorBusca.ErroSemDias], resultado.Erros);
    }

    [Fact]
    public void Validar_DiasAusentes_DevolveErro()
    {
        BuscaRequisicao requisicao = Fabrica.Requisicao();
        requisicao.Dias = null;

        ResultadoValidacao resultado = ValidadorBusca.Validar(requisicao);

        Assert.Equal([ValidadorBusca.ErroSemDias], resultado.Erros);
    }

    [Theory]
    [InlineData("sabadoo")]
    [InlineData("feriado")]
    [InlineData("")]
    [InlineData(null)]
    public void Validar_DiaInvalido_DevolveErroComODia(string? dia)
    {
        ResultadoValidacao resultado = ValidadorBusca.Validar(Fabrica.Requisicao(dias: ["segunda", dia]));

        Assert.Equal([string.Format(ValidadorBusca.ErroDiaInvalido, dia)], resultado.Erros);
    }

    [Fact]
    public void Validar_DiasComAcentoMaiusculaERepeticao_NormalizaESemRepetir()
    {
        ResultadoValidacao resultado = ValidadorBusca.Validar(Fabrica.Requisicao(dias: ["Terça", "terca", "SEGUNDA"]));

        Assert.True(resultado.EhValida);
        Assert.Equal([DiaSemanaEnum.Terca, DiaSemanaEnum.Segunda], resultado.Criterio!.Dias);
    }

    [Theory]
    [InlineData("9:00")]
    [InlineData("24:00")]
    [InlineData("18:60")]
    [InlineData("1800")]
    [InlineData("18:00\n")]
    [InlineData("")]
    [InlineData(null)]
    public void Validar_InicioForaDoFormato_DevolveErro(string? inicio)
    {
        ResultadoValidacao resultado = ValidadorBusca.Validar(Fabrica.Requisicao(inicioFaixa: inicio));

        Assert.Equal([ValidadorBusca.ErroInicioFaixa], resultado.Erros);
    }

    [Theory]
    [InlineData("22:5")]
    [InlineData("abc")]
    [InlineData(null)]
    public void Validar_FimForaDoFormato_DevolveErro(string? fim)
    {
        ResultadoValidacao resultado = ValidadorBusca.Validar(Fabrica.Requisicao(fimFaixa: fim));

        Assert.Equal([ValidadorBusca.ErroFimFaixa], resultado.Erros);
    }

    [Theory]
    [InlineData("18:00", "18:00")]
    [InlineData("20:00", "18:00")]
    public void Validar_FimIgualOuAntesDoInicio_DevolveErro(string inicio, string fim)
    {
        ResultadoValidacao resultado = ValidadorBusca.Validar(Fabrica.Requisicao(inicioFaixa: inicio, fimFaixa: fim));

        Assert.Equal([ValidadorBusca.ErroFaixaInvertida], resultado.Erros);
    }

    [Fact]
    public void Validar_HorarioInvalido_NaoAcusaTambemFaixaInvertida()
    {
        ResultadoValidacao resultado = ValidadorBusca.Validar(Fabrica.Requisicao(inicioFaixa: "25:00", fimFaixa: "18:00"));

        Assert.Equal([ValidadorBusca.ErroInicioFaixa], resultado.Erros);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(45)]
    [InlineData(120)]
    [InlineData(null)]
    public void Validar_DuracaoForaDe30_60_90_DevolveErro(int? duracao)
    {
        ResultadoValidacao resultado = ValidadorBusca.Validar(Fabrica.Requisicao(duracaoMinutos: duracao));

        Assert.Equal([ValidadorBusca.ErroDuracao], resultado.Erros);
    }

    [Fact]
    public void Validar_VariosCamposErrados_DevolveTodosOsErrosDeUmaVez()
    {
        BuscaRequisicao requisicao = Fabrica.Requisicao(idioma: "", dias: [], inicioFaixa: "9:00", fimFaixa: "x", duracaoMinutos: 45);

        ResultadoValidacao resultado = ValidadorBusca.Validar(requisicao);

        Assert.Equal(
            [
                ValidadorBusca.ErroIdiomaVazio,
                ValidadorBusca.ErroSemDias,
                ValidadorBusca.ErroInicioFaixa,
                ValidadorBusca.ErroFimFaixa,
                ValidadorBusca.ErroDuracao
            ],
            resultado.Erros);
    }
}
