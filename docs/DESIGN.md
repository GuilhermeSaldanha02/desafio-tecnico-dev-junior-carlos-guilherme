# Design da tela

## Formulário

| Campo | Controle | Valores |
|---|---|---|
| Idioma | select | Inglês, Espanhol, Francês |
| Dias da semana | checkboxes, com a ajuda "O horário da aula será o mesmo em todos os dias marcados." | segunda a domingo |
| Faixa de horário | dois campos de hora | início e fim |
| Duração | select | 30, 60 ou 90 minutos |

Os controles evitam a maior parte dos erros, mas o envio não é bloqueado: quem valida é a API (ver ADR-014 em [DECISIONS.md](DECISIONS.md)).

## Estados

A tela lê a resposta da API e escolhe o que mostrar. A API informa o fato; a tela escolhe como explicar.

| A API devolve | A tela mostra |
|---|---|
| *(aguardando resposta)* | estado de carregando; botão de buscar desabilitado |
| 200 com professores | cada professor com os horários possíveis e as aulas na semana |
| 200 vazio, todos com "Não ensina o idioma" | "Nenhum professor ensina {idioma} no momento" |
| 200 vazio, com outros motivos | "Nenhum professor disponível nesses dias e horários", com uma dica: com mais de um dia marcado, "O mesmo horário precisa estar livre em todos os dias marcados. Tente marcar menos dias ou ampliar a faixa."; com um dia, "Tente ampliar a faixa de horário ou escolher outro dia." |
| 400 com `{ "erros": [...] }` | a lista de erros devolvida pela API |
| sem resposta (API fora do ar) | "Não foi possível falar com o servidor. Tente de novo." |

Em todo resultado 200, abaixo da lista principal, aparece quem ficou de fora e o motivo.

## Agenda dos professores (fora do enunciado)

Painel recolhível "Ver agenda dos professores", abaixo da busca. Só chama a API na primeira vez que é aberto (ADR-025).

| Situação | O painel mostra |
|---|---|
| fechado | só o título |
| aberto, aguardando | "Carregando agenda…" |
| aberto, com resposta | um cartão por professor: idiomas, e por dia os períodos livres e os horários ocupados; a Carla com "Agenda bloqueada: não recebe novos alunos" |
| aberto, sem resposta | "Não foi possível carregar a agenda. Feche e abra de novo para tentar outra vez." |

## Visual

*A definir na etapa `feat/visual`: paleta e tipografia de inspiração, com a fonte da pesquisa.*
