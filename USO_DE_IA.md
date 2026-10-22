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
