using EncontreProfessor.Api.Dados;
using EncontreProfessor.Api.Erros;
using EncontreProfessor.Core.Busca;
using EncontreProfessor.Core.Interfaces;

const string NomeArquivoProfessores = "professores.json";
const string ErroInterno = "Erro inesperado no servidor. Tente de novo.";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(opcoes => opcoes.InvalidModelStateResponseFactory = RespostaErroLeitura.Criar);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// lido uma vez ao subir a API; se o arquivo faltar, a API não sobe
string caminhoArquivo = Path.Combine(AppContext.BaseDirectory, NomeArquivoProfessores);
builder.Services.AddSingleton<IProfessorRepository>(new ProfessorRepositoryJson(caminhoArquivo));
builder.Services.AddScoped<BuscaProfessoresService>();

WebApplication app = builder.Build();

app.UseExceptionHandler(tratamento => tratamento.Run(async contexto =>
{
    contexto.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await contexto.Response.WriteAsJsonAsync(new RespostaErro([ErroInterno]));
}));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
