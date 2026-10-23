using EncontreProfessor.Api.Dados;
using EncontreProfessor.Core.Agenda;
using EncontreProfessor.Core.Models;
using EncontreProfessor.Tests.Apoio;

namespace EncontreProfessor.Tests.Agenda;

public class AgendaProfessoresServiceTests
{
    private static IReadOnlyList<AgendaProfessor> Listar(params Professor[] professores) =>
        new AgendaProfessoresService(new RepositorioEmMemoria(professores)).Listar();

    [Fact]
    public void Listar_DiasSaemNaOrdemDaSemanaESoOsQueTemBlocoOuAula()
    {
        Professor fabio = Fabrica.Professor(6, "Fábio", ["Inglês"], ["sexta 09:00-12:00", "segunda 18:15-20:00"]);

        AgendaProfessor agenda = Listar(fabio).Single();

        Assert.Equal(["segunda", "sexta"], agenda.Dias.Select(dia => dia.Dia));
    }

    [Fact]
    public void Listar_BlocoQuebradoNoDia_MostraOsDoisPeriodos()
    {
        Professor bruno = Fabrica.Professor(2, "Bruno", ["Inglês"], ["quarta 18:00-19:00", "quarta 20:00-22:00"]);

        AgendaDia quarta = Listar(bruno).Single().Dias.Single();

        Assert.Equal([new PeriodoAgenda("18:00", "19:00"), new PeriodoAgenda("20:00", "22:00")], quarta.Disponivel);
    }

    [Fact]
    public void Listar_AulaMarcada_ApareceComoPeriodoOcupadoComOFimCalculado()
    {
        Professor ana = Fabrica.Professor(1, "Ana", ["Inglês"], ["segunda 18:00-22:00"], ["segunda 19:00 60"]);

        AgendaDia segunda = Listar(ana).Single().Dias.Single();

        Assert.Equal([new PeriodoAgenda("19:00", "20:00")], segunda.Ocupado);
    }

    [Fact]
    public void Listar_ProfessorComAgendaBloqueada_ApareceMarcado()
    {
        Professor carla = Fabrica.Professor(3, "Carla", ["Inglês"], ["segunda 08:00-22:00"], agendaBloqueada: true);

        AgendaProfessor agenda = Listar(carla).Single();

        Assert.True(agenda.AgendaBloqueada);
    }

    [Fact]
    public void Listar_ProfessoresEmOrdemAlfabeticaRespeitandoAcento()
    {
        Professor carla = Fabrica.Professor(3, "Carla", ["Inglês"], ["segunda 18:00-22:00"]);
        Professor agata = Fabrica.Professor(1, "Ágata", ["Inglês"], ["segunda 18:00-22:00"]);

        IReadOnlyList<AgendaProfessor> agendas = Listar(carla, agata);

        Assert.Equal(["Ágata", "Carla"], agendas.Select(agenda => agenda.Nome));
    }

    [Fact]
    public void Listar_ArquivoReal_TrazOsOitoProfessoresComAsAgendasDoArquivo()
    {
        string caminho = Path.Combine(AppContext.BaseDirectory, "professores.json");
        AgendaProfessoresService service = new AgendaProfessoresService(new ProfessorRepositoryJson(caminho));

        IReadOnlyList<AgendaProfessor> agendas = service.Listar();

        Assert.Equal(8, agendas.Count);
        AgendaProfessor heitor = agendas.Single(agenda => agenda.Nome == "Heitor");
        Assert.Equal(4, heitor.AulasNaSemana);
        Assert.Equal(
            [new PeriodoAgenda("19:00", "20:00"), new PeriodoAgenda("20:00", "21:00")],
            heitor.Dias.Single(dia => dia.Dia == "segunda").Ocupado);
    }
}
