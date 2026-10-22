# Arquitetura

## Visão geral

A solução tem três partes, e o dado anda sempre na mesma direção: **tela → API → regra**.

```
desafio-tecnico-dev-junior-carlos-guilherme/
├─ src/
│  ├─ EncontreProfessor.Core/   regra de negócio pura, sem ASP.NET
│  └─ EncontreProfessor.Api/    camada HTTP: recebe, valida, chama o Core, responde
├─ tests/
│  └─ EncontreProfessor.Tests/  xUnit, testa o Core direto (sem subir a API)
├─ web/                         tela em React + TypeScript (Vite)
├─ docs/                        decisões, design, QA e o enunciado original
└─ professores.json             dados dos professores (carregados em memória)
```

## Responsabilidade de cada projeto

| Projeto | Faz | Não faz |
|---|---|---|
| **Core** | Modelos, validação da busca, cálculo dos horários, ordenação, motivos de quem ficou de fora | Não conhece HTTP, JSON nem arquivo |
| **Api** | Lê o `professores.json`, recebe a requisição, traduz o resultado do Core em 200 ou 400 | Não tem regra de negócio |
| **Tests** | Prova cada regra do enunciado, inclusive as fronteiras | Não sobe a API |
| **web** | Formulário, resultado e todos os estados da tela | Não repete a validação da API (ver ADR-014) |

O Core define a interface `IProfessorRepository`; a Api implementa lendo o arquivo. Assim o Core nunca sabe de onde os professores vêm, e os testes passam professores montados à mão.

## Estrutura interna do Core

```
EncontreProfessor.Core/
├─ Models/       Intervalo, Disponibilidade, AulaMarcada, Professor
├─ Enums/        DiaSemanaEnum
├─ Helpers/      Horario (HH:mm ↔ minutos), Normalizador (sem acento e caixa), DiaSemanaConversor
├─ Validacao/    ValidadorBusca e ResultadoValidacao
├─ Busca/        requisição, critério, resposta, motivos e o BuscaProfessoresService
└─ Interfaces/   IProfessorRepository
```

Os modelos respondem sobre si mesmos: o intervalo sabe se contém ou se sobrepõe a outro, a disponibilidade sabe se uma aula cabe nela, a aula marcada sabe se colide com outra, o professor sabe se ensina um idioma e se está livre num dia e horário. O serviço só orquestra.

Caminho de uma busca:

1. `ValidadorBusca` confere a requisição e, se estiver certa, devolve um `CriterioBusca` já convertido (minutos, dias sem repetição).
2. `CriterioBusca.AulasPossiveis()` gera os inícios candidatos: de 30 em 30 minutos, a partir da primeira hora cheia ou meia hora da faixa, enquanto a aula couber na faixa.
3. Para cada professor, `BuscaProfessoresService` testa os motivos na ordem do enunciado e fica com os inícios livres em **todos** os dias pedidos.
4. O resultado sai ordenado e embrulhado num `ResultadoBusca`: erros ou resposta.

## Estrutura interna da Api

```
EncontreProfessor.Api/
├─ Controllers/  ProfessoresController: POST /api/professores/busca
├─ Dados/        ProfessorRepositoryJson e as classes de leitura do arquivo
├─ Erros/        RespostaErro ({ "erros": [...] }) e RespostaErroLeitura
├─ Program.cs    registro dos serviços, Swagger e tratamento de erro inesperado
└─ EncontreProfessor.Api.http   roteiro de teste com todos os casos
```

## Estrutura interna da tela

```
web/src/
├─ tipos/busca.ts            contrato da API e os estados possíveis da tela
├─ api/buscarProfessores.ts  chama a API e devolve sucesso, validação ou falha
├─ componentes/
│  ├─ FormularioBusca.tsx    idioma, dias, faixa e duração
│  ├─ ResultadoBusca.tsx     escolhe o que mostrar para cada estado
│  ├─ ListaProfessores.tsx   professores disponíveis com os horários
│  └─ ListaNaoElegiveis.tsx  quem ficou de fora e o motivo
└─ App.tsx                   guarda o estado atual e liga formulário e resultado
```

A tela tem um único estado, que é sempre um destes: inicial, carregando, sucesso, erro de validação ou falha. O `buscarProfessores` traduz a resposta HTTP nesses casos, e o `ResultadoBusca` só lê o estado. Em desenvolvimento, o Vite repassa `/api` para `http://localhost:5067`.

## Detalhes da Api

O Controller tem uma linha de decisão: se o `ResultadoBusca` tem erros, 400; senão, 200. Os erros que acontecem **antes** de chegar ao Core (JSON malformado, campo com tipo errado) são traduzidos pelo `RespostaErroLeitura` para o mesmo formato. Um erro inesperado vira 500 com o mesmo formato, sem expor detalhes internos.

## Padrão de código

Segue o estilo das provas de POO do autor, com os ajustes combinados para uma API:

- **Validar o que dá errado primeiro, caminho feliz no fim** (guard clauses).
- **Nunca `else` depois de `return`.** Para classificar, um método pequeno com `if ... return` em sequência.
- **Uma instrução por linha**, chaves em bloco.
- **Service devolve um objeto de resultado** (erros ou resposta). Quem decide entre 200 e 400 é o Controller. Nada de `Console` nem de texto `"Erro: ..."` como retorno.
- **Sem número ou texto solto:** tudo em constante, com nome em **PascalCase** (`DuracoesPermitidas`, `MotivoNaoEnsinaIdioma`).
- Encapsulamento com `private set`, interface para repositório, injeção de dependência pelo construtor, campos `_nome` `private readonly`, `namespace` de arquivo.
- Nomes em português, como no enunciado.

## Fluxo de trabalho (Git)

Branches: `main`, `staging` e uma `feat/*` (ou `chore/*`, `docs/*`) por etapa.

1. A branch de trabalho sai da `main` atualizada.
2. Terminada a etapa, ela é **mergeada direto na `staging`** (sem PR) para o QA testar.
3. Se o QA achar problema, a correção é feita **na branch de trabalho** e mergeada de novo na `staging`.
4. Com o QA aprovado, abre-se o **PR da branch de trabalho para a `main`**, com `Closes #N`.
5. **Nunca há PR da `staging`.** Ela é o ambiente de teste e pode ser apagada e recriada a partir da `main` a qualquer momento.

Commits seguem **Conventional Commits**: `tipo(escopo): descrição`, com o escopo indicando o assunto da branch.

```
feat(busca): cria regra de horário fixo em todos os dias
test(busca): cobre aula que encosta em aula marcada
chore(estrutura): cria solução com Core, Api e Tests
```

As tarefas ficam no [quadro do projeto](https://github.com/users/GuilhermeSaldanha02/projects/1), um card por etapa, com as colunas *A fazer*, *Fazendo*, *Na staging (QA)* e *Feito*.
