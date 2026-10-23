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
| *(antes da primeira busca)* | "Escolha o idioma, os dias e a faixa de horário, e clique em buscar." e a explicação de que os horários mostrados ficam livres em todos os dias marcados |
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

**Referência:** o site da King of Languages ([kingoflanguages.com.br](https://kingoflanguages.com.br/)), olhado em tela real em 23/10/2026. É a escola para a qual o desafio foi feito. Os valores ficam só nas variáveis do topo de `web/src/index.css`; aqui ficam as regras (ADR-026).

**O que foi tirado do site:**
- azul como cor principal e vermelho como cor de ação (o botão "Buscar professores");
- manchete em duas cores ("Encontre" em vermelho, "um Professor" em azul), como "Domine o inglês de forma rápida e eficaz";
- Poppins nos títulos e Inter no texto; cantos de 10 px nos controles.

**O que não foi copiado, e por quê:**
- logo e leão: são a marca deles; usar faria a tela parecer oficial da empresa;
- faixa azul de topo e cartões vermelhos chapados: funcionam num site de venda, mas numa ferramenta de busca carregam a tela, e o vermelho em bloco se confunde com erro.

**Regras:**
- **Vermelho é ação.** Só o botão principal é vermelho sólido. O erro se distingue pela forma: fundo suave, borda grossa à esquerda e título em negrito.
- **Bloqueio não é erro.** "Agenda bloqueada" é uma etiqueta cinza neutra.
- **Só controle tem moldura.** No resultado, professores são separados por linha. Os horários são etiquetas de canto pequeno, sem cursor de clique, para não parecerem botões. Na agenda os cartões ficam, porque cada um contém uma tabela.
- **O que é clicável parece clicável.** "Ver agenda dos professores" é azul, com seta e sublinhado ao passar o mouse.
- **O estado inicial ensina a regra do horário fixo**, antes da primeira busca.
