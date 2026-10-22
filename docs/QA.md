# Roteiro de QA

O QA roda na branch `staging`, depois que a branch da etapa foi mergeada nela. Problema encontrado vira comentário no card da etapa e é corrigido na branch da etapa, nunca na `staging`.

Em toda etapa: sem erro de ortografia no que foi escrito (código, mensagens e documentação).

## Agenda dos professores (referência para conferir resultados)

Copiada do `professores.json`. Use para entender por que alguém aparece ou fica de fora.

| Professor | Idiomas | Disponibilidade | Aulas marcadas |
|---|---|---|---|
| Ana | Inglês, Espanhol | segunda 18:00–22:00; quarta 18:00–22:00 | segunda 19:00 (60 min) |
| Bruno | Inglês | segunda 18:00–21:00; terça 18:00–20:00; quarta 18:00–19:00 e 20:00–22:00; quinta 18:00–20:00 | terça 18:00 (60); quinta 18:00 (60) |
| Carla | Inglês | segunda e quarta 08:00–22:00 | nenhuma — **agenda bloqueada** |
| Diego | Espanhol | segunda 18:00–22:00; terça 08:00–12:00; quarta 18:00–22:00 | terça 08:30 (30) |
| Elisa | Inglês | segunda 18:00–22:00 | nenhuma |
| Fábio | Inglês | segunda 18:15–20:00; quarta 18:15–20:00; sexta 09:00–12:00 | sexta 10:00 (60) |
| Gabriela | Inglês | segunda 17:00–23:00; quarta 17:00–23:00 | segunda 18:00 (30); quarta 20:30 (60) |
| Heitor | Inglês | segunda 18:00–21:00; quarta 18:00–21:00 | segunda 19:00 e 20:00 (60); quarta 18:00 e 19:00 (60) |

Ninguém ensina Francês, e ninguém atende sábado ou domingo.

## Etapa 1: estrutura (#1)

- [ ] `dotnet build` sem erros e sem avisos
- [ ] `dotnet test` roda (ainda sem testes)
- [ ] `dotnet --version` dentro da pasta mostra 8.0.x
- [ ] Pastas iguais ao desenho do [ARCHITECTURE.md](ARCHITECTURE.md)
- [ ] Documentação lida e sem erro de ortografia

## Etapa 2: regras no Core (#2)

- [ ] `dotnet test` verde
- [ ] Cada regra do enunciado tem pelo menos um teste (tabela regra → teste no PR)
- [ ] Fronteiras cobertas: aula que encosta, bloco quebrado, bloco que começa 18:15, empate na ordenação, prioridade dos motivos
- [ ] Exemplo do enunciado reproduzido com Ana, Bruno e Diego montados à mão (o teste com o arquivo real fica na etapa 3, ver ADR-017)

## Etapa 3: API (#3)

Suba a API com `dotnet run --project src/EncontreProfessor.Api` e teste pelo Swagger (`http://localhost:5067/swagger`), pelo Insomnia (importando `http://localhost:5067/swagger/v1/swagger.json`) ou pelo arquivo `src/EncontreProfessor.Api/EncontreProfessor.Api.http`, que tem os 12 casos abaixo numerados:

- [ ] Teste automatizado carregando o `professores.json` real reproduz o exemplo do enunciado
- [ ] Exemplo do enunciado: Ana com `18:00`; Diego com `18:00`, `18:30`, `19:00`
- [ ] `"idioma": "ingles"` encontra os professores de Inglês, e a ordem alfabética sai certa com acento
- [ ] Francês: 200, lista vazia, todos com "Não ensina o idioma"
- [ ] Cada um dos seis casos de 400, com mensagem em português
- [ ] Corpo vazio, JSON malformado e tipo errado: 400 no mesmo formato

## Etapa 4: tela (#4)

Suba a API (`dotnet run --project src/EncontreProfessor.Api`) e, em outro terminal, a tela (`cd web`, `npm install` na primeira vez, `npm run dev`). Abra `http://localhost:5173`.

Cada teste parte da página recarregada (F5) e diz o valor de cada campo. "Ficaram de fora" é a seção abaixo do resultado, com o nome à esquerda e o motivo à direita.

| # | Idioma | Dias | Das | Até | Duração | Resultado esperado |
|---|---|---|---|---|---|---|
| 1 | *(abrir a página e não buscar)* | | | | | "Escolha o idioma, os dias e a faixa de horário, e clique em buscar." |
| 2 | Espanhol | segunda | 18:00 | 20:00 | 60 | Ana `18:00`; Diego `18:00 18:30 19:00`; os outros 6 em "Ficaram de fora" com "Não ensina o idioma" |
| 3 | Inglês | segunda e quarta | 18:00 | 22:00 | 60 | Ana `18:00 20:00 20:30 21:00`; Fábio `18:30 19:00`; Bruno `18:00 20:00`; Gabriela `18:30 19:00 19:30` |
| 4 | Francês | segunda | 18:00 | 20:00 | 60 | "Nenhum professor ensina Francês no momento." e os 8 em "Ficaram de fora" |
| 5 | Inglês | segunda | 18:00 | 18:30 | 60 | "Nenhum professor disponível nesses dias e horários." com a dica "Tente ampliar a faixa de horário ou escolher outro dia." |
| 6 | Inglês | os 7 dias | 18:00 | 20:00 | 60 | Ninguém disponível, com a dica "O mesmo horário precisa estar livre em todos os dias marcados…" |
| 7 | Inglês | *nenhum* | 18:00 | 20:00 | 60 | "Corrija a busca: Informe pelo menos um dia da semana." |
| 8 | Espanhol | segunda | 18:00 | 20:00 | 60, **com a API desligada** | "Não foi possível falar com o servidor. Tente de novo." |

- [ ] Durante a busca, o botão mostra "Buscando…" e fica desabilitado (rápido demais para ver a olho; conferido com a resposta atrasada)
- [ ] Abaixo de "Dias da semana" aparece "O horário da aula será o mesmo em todos os dias marcados."
- [ ] Em tela de celular, formulário e resultado ficam um embaixo do outro, sem rolagem para o lado

## Agenda dos professores (#11, fora do enunciado)

Suba a API e a tela como na etapa 4.

- [ ] `dotnet test`: 74 aprovados (os 68 anteriores sem nenhuma alteração, mais 6 da agenda)
- [ ] Swagger ou Insomnia: `GET http://localhost:5067/api/professores` (caso 13 do `.http`) traz os 8 professores em ordem alfabética
- [ ] Na tela, abrir "Ver agenda dos professores": 8 cartões, iguais à tabela de agenda no topo deste arquivo
- [ ] Carla com "Agenda bloqueada: não recebe novos alunos"
- [ ] Bruno, quarta: dois períodos livres, 18:00–19:00 e 20:00–22:00
- [ ] Com a API desligada, recarregar a página e abrir o painel: "Não foi possível carregar a agenda…"
- [ ] A busca continua com os mesmos resultados da etapa 4 (testes 2 e 3 da tabela)
- [ ] No celular (375 px e 320 px), **com a agenda aberta e os cartões carregados**: cartões um embaixo do outro, nenhuma tabela passando da borda do cartão. Abaixo de 360 px, cada dia vira um bloco com "Livre" e "Ocupado" um embaixo do outro

## Etapa 5: visual (#5)

- [ ] A tela continua passando pelos seis estados
- [ ] Nada quebra em tela de celular

## Etapa 6: entrega (#6)

- [ ] Roteiro completo das etapas 2 a 5 com tudo junto na `staging`
- [ ] README seguido do zero, numa pasta limpa, e funciona
