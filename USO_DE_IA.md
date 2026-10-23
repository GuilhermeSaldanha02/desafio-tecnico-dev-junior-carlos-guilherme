# Uso de IA

Os registros abaixo foram feitos durante o desenvolvimento, à medida que as coisas aconteciam, e não reconstruídos no fim.

## Ferramentas

- **Claude Code** (modelo Claude Opus 5.5), no aplicativo desktop: a sessão principal, onde planejei, debati as decisões e a IA escreveu o código.
- **Sessões separadas de QA**, também no Claude Code, com o **Playwright** (automação de navegador): só testavam, sem alterar código, e salvavam um print de cada item. Rodaram sob a minha supervisão: eu disparei cada uma, acompanhei e tomei as decisões que apareceram no meio (por exemplo, liberar as portas da API e da tela para a sessão subir os próprios servidores).
- **Dois agentes de parecer** para a direção visual, um no papel de avaliador técnico e outro no de designer de produto. Eles só opinaram; a escolha foi minha.

## Como usei

- **Leitura e planejamento:** a IA leu o enunciado e os dados, e debatemos a arquitetura e cada decisão que o enunciado não cobre antes de escrever código. Para cada ponto, ela apresentou opções com uma recomendação, e eu escolhi. O resultado está em [docs/DECISIONS.md](docs/DECISIONS.md).
- **Padrão de código:** a IA leu as minhas provas anteriores de POO em C# para seguir o meu estilo de escrita (validar o erro primeiro, caminho feliz no fim), e as regras ficaram em [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).
- **Código:** a IA escreveu o código e explicou onde cada parte fica e por quê. Ela não faz merge na `main`: cada etapa passou pelo meu teste na `staging` e entrou na `main` por um PR que eu revisei e mergeei.
- **Teste:** cada etapa teve dois tipos de QA. O **automatizado**, com o Playwright nas sessões separadas, supervisionado por mim. E o **manual**, feito por mim, no computador e no celular, seguindo o [roteiro de teste](docs/QA.md). Os dois estão registrados em cada PR. Dois dos erros abaixo foram pegos assim.
- **Escopo:** a agenda dos professores (ADR-025) não estava no enunciado. Ela nasceu de uma dúvida minha no teste manual, e eu decidi incluir depois de confirmar que ela não mudava nenhuma regra da busca.

## O que eu garanto que entendo

Revisei cada etapa antes do merge e consigo explicar, sem a IA, onde cada regra do enunciado está no código e qual teste a prova (a tabela "regra → teste" está no PR da etapa 2). As decisões são minhas: a IA recomendou e eu escolhi. Várias direções partiram de mim, e não dela: o fluxo de Git com a `staging` como ambiente de teste, os cards no GitHub Projects, a agenda dos professores, o QA com Playwright em sessões separadas e o roteiro de teste escrito para quem não programa.

## Erros da IA e como percebi

### 1. Fluxo de Git assumido errado

Expliquei que teríamos as branches `main`, `staging` e `feat/*`, e que nunca haveria PR da `staging`. A IA assumiu o fluxo mais comum (`feat` → `staging` → `main`, com a `staging` sendo promovida para a `main`) e respondeu que, sem PR da `staging`, a `main` ficaria vazia.

**Como percebi:** a proposta contradizia o que eu tinha dito. No meu fluxo a `staging` é só ambiente de teste, descartável; o que vai para a `main` é a própria branch `feat`, por PR, depois de aprovada no QA. Expliquei de novo, e a IA corrigiu o fluxo, que está em [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

### 2. Roteiro de QA da tela sem os valores dos campos

A IA escreveu o roteiro de teste da tela com itens como "buscar com os valores iniciais" e "Inglês com faixa 18:00 a 18:30", sem dizer quais dias marcar. Seguindo o roteiro, eu não conseguia reproduzir os resultados esperados, porque o resultado depende dos dias.

**Como percebi:** ao fazer o QA, marquei todos os dias e não apareceu nenhum professor. Primeiro achei que era bug. Não era: a regra do horário fixo exige o mesmo horário livre em todos os dias, e ninguém atende sábado ou domingo. Mas o roteiro não me deixava saber disso. Pedi que cada teste dissesse o valor de cada campo, e a IA reescreveu o roteiro em tabela, acrescentando a agenda dos professores para consulta ([docs/QA.md](docs/QA.md)).

**O que isso mostrou:** o código fazia o que a regra mandava, mas a tela também não explicava a regra. Disso saiu um ajuste na própria tela: textos de ajuda sobre o horário fixo (ADR-024). Nenhum teste automatizado pegaria isso; o teste manual pegou.

### 3. Verificação de celular feita sem os dados na tela

Depois de ajustar o CSS da agenda para telas estreitas, a IA disse que a 320 px não havia rolagem lateral. Mas tinha medido com a API desligada, sem nenhum cartão na tela; ela mesma avisou que os cartões só tinham sido vistos a 375 px.

**Como percebi:** pedi um QA separado com o Playwright, com tudo rodando. A 320 px, com os cartões carregados, a tabela passava até 34 px da borda do cartão. A página não rolava para o lado, então a medida da IA estava certa no que media, mas media a coisa errada. A correção empilha a tabela abaixo de 360 px, e a nova verificação foi feita com os 8 cartões carregados.
