using EncontreProfessor.Api.Dados;
using EncontreProfessor.Core.Busca;
using EncontreProfessor.Core.Models;
using EncontreProfessor.Tests.Apoio;

namespace EncontreProfessor.Tests.Dados;

public class ProfessorRepositoryJsonTests
{
    private static readonly string CaminhoArquivoReal = Path.Combine(AppContext.BaseDirectory, "professores.json");

    [Fact]
    public void Listar_ArquivoReal_CarregaOsOitoProfessoresComAgendas()
    {
        ProfessorRepositoryJson repositorio = new ProfessorRepositoryJson(CaminhoArquivoReal);

        IReadOnlyList<Professor> professores = repositorio.Listar();

        Assert.Equal(8, professores.Count);
        Professor bruno = professores.Single(professor => professor.Nome == "Bruno");
        Assert.Equal(5, bruno.Disponibilidades.Count);
        Assert.Equal(2, bruno.AulasNaSemana);
        Assert.True(professores.Single(professor => professor.Nome == "Carla").AgendaBloqueada);
    }

    [Fact]
    public void Buscar_ExemploDoEnunciadoComOArquivoReal_DevolveAnaEDiego()
    {
        BuscaProfessoresService service = new BuscaProfessoresService(new ProfessorRepositoryJson(CaminhoArquivoReal));
        BuscaRequisicao requisicao = Fabrica.Requisicao(idioma: "Espanhol", dias: ["segunda"], inicioFaixa: "18:00", fimFaixa: "20:00", duracaoMinutos: 60);

        BuscaResposta resposta = service.Buscar(requisicao).Resposta!;

        Assert.Equal(["Ana", "Diego"], resposta.Professores.Select(professor => professor.Nome));
        Assert.Equal(["18:00"], resposta.Professores[0].Horarios);
        Assert.Equal(["18:00", "18:30", "19:00"], resposta.Professores[1].Horarios);
        Assert.Contains(new ProfessorNaoElegivel(2, "Bruno", MotivosNaoElegivel.NaoEnsinaIdioma), resposta.NaoElegiveis);
    }

    [Fact]
    public void Construtor_ArquivoInexistente_FalhaComMensagemClara()
    {
        FileNotFoundException erro = Assert.Throws<FileNotFoundException>(() => new ProfessorRepositoryJson("nao-existe.json"));

        Assert.Contains("nao-existe.json", erro.Message);
    }
}
