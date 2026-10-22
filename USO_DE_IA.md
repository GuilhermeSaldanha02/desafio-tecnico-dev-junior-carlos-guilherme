# Uso de IA

*Documento em construção: os registros são feitos durante o desenvolvimento, não no fim.*

## Ferramentas

- **Claude Code** (modelo Claude Opus 5.5), no aplicativo desktop.

## Como usei

- **Leitura e planejamento:** a IA leu o enunciado e os dados, e debatemos a arquitetura e cada decisão que o enunciado não cobre antes de escrever código. Para cada ponto, ela apresentou opções com uma recomendação, e eu escolhi. O resultado está em [docs/DECISIONS.md](docs/DECISIONS.md).
- **Padrão de código:** a IA leu as minhas provas anteriores de POO em C# para seguir o meu estilo de escrita (validar o erro primeiro, caminho feliz no fim), e as regras ficaram em [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).
- **Código:** a IA escreve o código e explica onde cada parte fica e por quê. Eu reviso, testo na `staging` e faço o merge na `main`. Ela não faz merge na `main`.

## Erros da IA e como percebi

### 1. Fluxo de Git assumido errado

Expliquei que teríamos as branches `main`, `staging` e `feat/*`, e que nunca haveria PR da `staging`. A IA assumiu o fluxo mais comum (`feat` → `staging` → `main`, com a `staging` sendo promovida para a `main`) e respondeu que, sem PR da `staging`, a `main` ficaria vazia.

**Como percebi:** a proposta contradizia o que eu tinha dito. No meu fluxo a `staging` é só ambiente de teste, descartável; o que vai para a `main` é a própria branch `feat`, por PR, depois de aprovada no QA. Expliquei de novo, e a IA corrigiu o fluxo, que está em [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

### 2. Roteiro de QA da tela sem os valores dos campos

A IA escreveu o roteiro de teste da tela com itens como "buscar com os valores iniciais" e "Inglês com faixa 18:00 a 18:30", sem dizer quais dias marcar. Seguindo o roteiro, eu não conseguia reproduzir os resultados esperados, porque o resultado depende dos dias.

**Como percebi:** ao fazer o QA, marquei todos os dias e não apareceu nenhum professor. Primeiro achei que era bug. Não era: a regra do horário fixo exige o mesmo horário livre em todos os dias, e ninguém atende sábado ou domingo. Mas o roteiro não me deixava saber disso. Pedi que cada teste dissesse o valor de cada campo, e a IA reescreveu o roteiro em tabela, acrescentando a agenda dos professores para consulta ([docs/QA.md](docs/QA.md)).

**O que isso mostrou:** o código fazia o que a regra mandava, mas a tela também não explicava a regra. Disso saiu um ajuste na própria tela: textos de ajuda sobre o horário fixo (ADR-024). Nenhum teste automatizado pegaria isso; o teste manual pegou.
