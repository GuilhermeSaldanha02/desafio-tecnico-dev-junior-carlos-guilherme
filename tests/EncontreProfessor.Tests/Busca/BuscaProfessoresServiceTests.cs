using EncontreProfessor.Core.Busca;
using EncontreProfessor.Core.Models;
using EncontreProfessor.Core.Validacao;
using EncontreProfessor.Tests.Apoio;

namespace EncontreProfessor.Tests.Busca;

public class BuscaProfessoresServiceTests
{
    private static BuscaResposta Buscar(BuscaRequisicao requisicao, params Professor[] professores)
    {
        BuscaProfessoresService service = new BuscaProfessoresService(new RepositorioEmMemoria(professores));
        ResultadoBusca resultado = service.Buscar(requisicao);

        Assert.True(resultado.EhSucesso);
        return resultado.Resposta!;
    }

    private static IReadOnlyList<string> HorariosDe(BuscaResposta resposta, int id) => resposta.Professores.Single(professor => professor.Id == id).Horarios;

    private static string MotivoDe(BuscaResposta resposta, int id) => resposta.NaoElegiveis.Single(professor => professor.Id == id).Motivo;

    // Regra 1: o professor precisa ensinar o idioma

    [Fact]
    public void Buscar_ProfessorQueNaoEnsinaOIdioma_FicaDeForaComMotivo()
    {
        Professor bruno = Fabrica.Professor(2, "Bruno", ["Inglês"], ["segunda 18:00-22:00"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(idioma: "Espanhol"), bruno);

        Assert.Empty(resposta.Professores);
        Assert.Equal(MotivosNaoElegivel.NaoEnsinaIdioma, MotivoDe(resposta, 2));
    }

    [Theory]
    [InlineData("ingles")]
    [InlineData("INGLÊS")]
    [InlineData("  Inglês  ")]
    public void Buscar_IdiomaSemAcentoOuEmMaiusculas_EncontraOProfessor(string idioma)
    {
        Professor elisa = Fabrica.Professor(5, "Elisa", ["Inglês"], ["segunda 18:00-22:00"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(idioma: idioma), elisa);

        Assert.Single(resposta.Professores);
    }

    [Fact]
    public void Buscar_IdiomaQueNinguemEnsina_RespondeListaVaziaENaoErro()
    {
        Professor ana = Fabrica.Professor(1, "Ana", ["Inglês", "Espanhol"], ["segunda 18:00-22:00"]);
        Professor bruno = Fabrica.Professor(2, "Bruno", ["Inglês"], ["segunda 18:00-22:00"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(idioma: "Francês"), ana, bruno);

        Assert.Empty(resposta.Professores);
        Assert.All(resposta.NaoElegiveis, professor => Assert.Equal(MotivosNaoElegivel.NaoEnsinaIdioma, professor.Motivo));
    }

    // Regra 2: agenda bloqueada não aparece

    [Fact]
    public void Buscar_ProfessorComAgendaBloqueada_FicaDeForaComMotivo()
    {
        Professor carla = Fabrica.Professor(3, "Carla", ["Inglês"], ["segunda 08:00-22:00"], agendaBloqueada: true);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(), carla);

        Assert.Empty(resposta.Professores);
        Assert.Equal(MotivosNaoElegivel.AgendaBloqueada, MotivoDe(resposta, 3));
    }

    [Fact]
    public void Buscar_ProfessorBloqueadoQueTambemNaoEnsina_InformaOPrimeiroMotivoDaOrdem()
    {
        Professor carla = Fabrica.Professor(3, "Carla", ["Inglês"], ["segunda 08:00-22:00"], agendaBloqueada: true);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(idioma: "Espanhol"), carla);

        Assert.Equal(MotivosNaoElegivel.NaoEnsinaIdioma, MotivoDe(resposta, 3));
    }

    // Regra 3: horário fixo, o mesmo início livre em todos os dias pedidos

    [Fact]
    public void Buscar_VariosDias_DevolveSoOsHorariosLivresEmTodos()
    {
        Professor ana = Fabrica.Professor(
            1, "Ana", ["Inglês"],
            ["segunda 18:00-22:00", "quarta 18:00-22:00"],
            ["segunda 19:00 60", "quarta 18:00 60"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(dias: ["segunda", "quarta"]), ana);

        // segunda livre: 18:00, 20:00, 20:30, 21:00 | quarta livre: 19:00, 19:30, 20:00, 20:30, 21:00
        Assert.Equal(["20:00", "20:30", "21:00"], HorariosDe(resposta, 1));
    }

    [Fact]
    public void Buscar_HorariosLivresEmCadaDiaMasNenhumEmComum_FicaDeForaComMotivo()
    {
        Professor bruno = Fabrica.Professor(2, "Bruno", ["Inglês"], ["segunda 18:00-19:00", "quarta 20:00-21:00"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(dias: ["segunda", "quarta"]), bruno);

        Assert.Empty(resposta.Professores);
        Assert.Equal(MotivosNaoElegivel.SemHorarioEmComum, MotivoDe(resposta, 2));
    }

    [Fact]
    public void Buscar_DiaPedidoSemNenhumBloco_FicaDeForaComMotivo()
    {
        Professor elisa = Fabrica.Professor(5, "Elisa", ["Inglês"], ["segunda 18:00-22:00"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(dias: ["segunda", "terca"]), elisa);

        Assert.Equal(MotivosNaoElegivel.SemHorarioEmComum, MotivoDe(resposta, 5));
    }

    [Fact]
    public void Buscar_DiasRepetidos_DaOMesmoResultadoQueSemRepetir()
    {
        Professor ana = Fabrica.Professor(1, "Ana", ["Inglês"], ["segunda 18:00-20:00"]);

        BuscaResposta semRepetir = Buscar(Fabrica.Requisicao(dias: ["segunda"]), ana);
        BuscaResposta repetido = Buscar(Fabrica.Requisicao(dias: ["segunda", "Segunda"]), ana);

        Assert.Equal(HorariosDe(semRepetir, 1), HorariosDe(repetido, 1));
    }

    // Regra 4: a aula cabe inteira no bloco e na faixa do aluno

    [Fact]
    public void Buscar_AulaQuePassariaDoFimDoBloco_NaoEOferecida()
    {
        Professor elisa = Fabrica.Professor(5, "Elisa", ["Inglês"], ["segunda 18:00-20:00"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(duracaoMinutos: 90), elisa);

        Assert.Equal(["18:00", "18:30"], HorariosDe(resposta, 5));
    }

    [Fact]
    public void Buscar_BlocoQuebradoNoDia_AulaNaoAtravessaOIntervaloEntreBlocos()
    {
        Professor bruno = Fabrica.Professor(2, "Bruno", ["Inglês"], ["quarta 18:00-19:00", "quarta 20:00-22:00"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(dias: ["quarta"], duracaoMinutos: 90), bruno);

        Assert.Equal(["20:00", "20:30"], HorariosDe(resposta, 2));
    }

    [Fact]
    public void Buscar_AulaQuePassariaDoFimDaFaixa_NaoEOferecida()
    {
        Professor elisa = Fabrica.Professor(5, "Elisa", ["Inglês"], ["segunda 18:00-22:00"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(inicioFaixa: "18:00", fimFaixa: "20:00"), elisa);

        Assert.Equal(["18:00", "18:30", "19:00"], HorariosDe(resposta, 5));
    }

    [Fact]
    public void Buscar_FaixaMenorQueADuracao_RespondeListaVaziaENaoErro()
    {
        Professor elisa = Fabrica.Professor(5, "Elisa", ["Inglês"], ["segunda 18:00-22:00"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(inicioFaixa: "18:00", fimFaixa: "18:30", duracaoMinutos: 60), elisa);

        Assert.Empty(resposta.Professores);
        Assert.Equal(MotivosNaoElegivel.SemHorarioEmComum, MotivoDe(resposta, 5));
    }

    // Regra 5: não sobrepor aula marcada; encostar é permitido

    [Fact]
    public void Buscar_AulaQueEncostaEmAulaMarcada_EPermitida()
    {
        Professor heitor = Fabrica.Professor(8, "Heitor", ["Inglês"], ["segunda 18:00-21:00"], ["segunda 19:00 60"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(fimFaixa: "21:00"), heitor);

        // 18:00 termina às 19:00 e 20:00 começa quando a aula marcada termina
        Assert.Equal(["18:00", "20:00"], HorariosDe(resposta, 8));
    }

    [Fact]
    public void Buscar_AulaQueInvadeParteDeAulaMarcada_NaoEOferecida()
    {
        Professor gabriela = Fabrica.Professor(7, "Gabriela", ["Inglês"], ["segunda 17:00-23:00"], ["segunda 18:00 30"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(inicioFaixa: "17:00", fimFaixa: "20:00"), gabriela);

        // aula marcada 18:00-18:30: 17:30 e 18:00 invadem
        Assert.Equal(["17:00", "18:30", "19:00"], HorariosDe(resposta, 7));
    }

    [Fact]
    public void Buscar_AgendaTodaOcupada_FicaDeForaComMotivo()
    {
        Professor heitor = Fabrica.Professor(8, "Heitor", ["Inglês"], ["segunda 18:00-21:00"], ["segunda 18:00 60", "segunda 19:00 60", "segunda 20:00 60"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(), heitor);

        Assert.Equal(MotivosNaoElegivel.SemHorarioEmComum, MotivoDe(resposta, 8));
    }

    // Regra 6: início só em :00 ou :30

    [Fact]
    public void Buscar_BlocoQueComecaForaDeHoraCheia_PrimeiroInicioEAProximaMeiaHora()
    {
        Professor fabio = Fabrica.Professor(6, "Fábio", ["Inglês"], ["segunda 18:15-20:00"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(), fabio);

        Assert.Equal(["18:30", "19:00"], HorariosDe(resposta, 6));
    }

    [Fact]
    public void Buscar_FaixaQueComecaForaDeHoraCheia_PrimeiroInicioEAProximaMeiaHora()
    {
        Professor elisa = Fabrica.Professor(5, "Elisa", ["Inglês"], ["segunda 18:00-22:00"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(inicioFaixa: "18:15", fimFaixa: "20:00"), elisa);

        Assert.Equal(["18:30", "19:00"], HorariosDe(resposta, 5));
    }

    // Regra 7: ordenação

    [Fact]
    public void Buscar_ProfessoresComMenosAulas_VemPrimeiro()
    {
        Professor ana = Fabrica.Professor(1, "Ana", ["Inglês"], ["segunda 18:00-22:00"], ["segunda 21:00 60"]);
        Professor bruno = Fabrica.Professor(2, "Bruno", ["Inglês"], ["segunda 18:00-22:00"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(fimFaixa: "20:00"), ana, bruno);

        Assert.Equal(["Bruno", "Ana"], resposta.Professores.Select(professor => professor.Nome));
    }

    [Fact]
    public void Buscar_EmpateNoNumeroDeAulas_OrdenaPorNomeRespeitandoAcento()
    {
        Professor carla = Fabrica.Professor(3, "Carla", ["Inglês"], ["segunda 18:00-22:00"]);
        Professor bruno = Fabrica.Professor(2, "Bruno", ["Inglês"], ["segunda 18:00-22:00"]);
        Professor agata = Fabrica.Professor(1, "Ágata", ["Inglês"], ["segunda 18:00-22:00"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(), carla, bruno, agata);

        // numa comparação ordinal "Á" viria depois do "Z"
        Assert.Equal(["Ágata", "Bruno", "Carla"], resposta.Professores.Select(professor => professor.Nome));
    }

    [Fact]
    public void Buscar_NaoElegiveis_VemEmOrdemAlfabetica()
    {
        Professor carla = Fabrica.Professor(3, "Carla", ["Inglês"], ["segunda 18:00-22:00"]);
        Professor agata = Fabrica.Professor(1, "Ágata", ["Inglês"], ["segunda 18:00-22:00"]);

        BuscaResposta resposta = Buscar(Fabrica.Requisicao(idioma: "Espanhol"), carla, agata);

        Assert.Equal(["Ágata", "Carla"], resposta.NaoElegiveis.Select(professor => professor.Nome));
    }

    // Exemplo do enunciado, com Ana, Bruno e Diego montados como no professores.json

    [Fact]
    public void Buscar_ExemploDoEnunciado_DevolveAnaEDiegoComOsHorariosEsperados()
    {
        Professor ana = Fabrica.Professor(1, "Ana", ["Inglês", "Espanhol"], ["segunda 18:00-22:00", "quarta 18:00-22:00"], ["segunda 19:00 60"]);
        Professor bruno = Fabrica.Professor(
            2, "Bruno", ["Inglês"],
            ["segunda 18:00-21:00", "terca 18:00-20:00", "quarta 18:00-19:00", "quarta 20:00-22:00", "quinta 18:00-20:00"],
            ["terca 18:00 60", "quinta 18:00 60"]);
        Professor diego = Fabrica.Professor(4, "Diego", ["Espanhol"], ["segunda 18:00-22:00", "terca 08:00-12:00", "quarta 18:00-22:00"], ["terca 08:30 30"]);

        BuscaRequisicao requisicao = Fabrica.Requisicao(idioma: "Espanhol", dias: ["segunda"], inicioFaixa: "18:00", fimFaixa: "20:00", duracaoMinutos: 60);
        BuscaResposta resposta = Buscar(requisicao, ana, bruno, diego);

        Assert.Equal(["Ana", "Diego"], resposta.Professores.Select(professor => professor.Nome));
        Assert.Equal(["18:00"], HorariosDe(resposta, 1));
        Assert.Equal(["18:00", "18:30", "19:00"], HorariosDe(resposta, 4));
        Assert.All(resposta.Professores, professor => Assert.Equal(1, professor.AulasNaSemana));
        Assert.Equal([new ProfessorNaoElegivel(2, "Bruno", MotivosNaoElegivel.NaoEnsinaIdioma)], resposta.NaoElegiveis);
    }

    // Requisição inválida não chega a buscar

    [Fact]
    public void Buscar_RequisicaoInvalida_DevolveErrosSemResposta()
    {
        BuscaProfessoresService service = new BuscaProfessoresService(new RepositorioEmMemoria());

        ResultadoBusca resultado = service.Buscar(Fabrica.Requisicao(idioma: ""));

        Assert.False(resultado.EhSucesso);
        Assert.Null(resultado.Resposta);
        Assert.Equal([ValidadorBusca.ErroIdiomaVazio], resultado.Erros);
    }
}
