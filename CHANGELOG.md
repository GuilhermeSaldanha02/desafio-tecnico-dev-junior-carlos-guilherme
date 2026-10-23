# Changelog

Todas as mudanças relevantes do projeto ficam registradas aqui.

O formato segue o [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/), e o projeto usa [Versionamento Semântico](https://semver.org/lang/pt-BR/).

## [1.0.0] - 2026-10-23

Primeira entrega do desafio.

### Adicionado

- Solução .NET 8 com os projetos `EncontreProfessor.Core`, `EncontreProfessor.Api` e `EncontreProfessor.Tests`.
- `global.json` fixando o SDK do .NET 8.
- Documentação em `docs/`: arquitetura, decisões, design da tela, roteiro de QA e o enunciado original.
- Regras da busca no Core: idioma, agenda bloqueada, horário fixo em todos os dias, aula inteira no bloco e na faixa, sem sobrepor aula marcada (encostar é permitido), início só em `:00` ou `:30`, ordenação e motivos de `naoElegiveis`.
- Validação da requisição com os seis casos do enunciado, devolvendo todos os erros de uma vez.
- 65 testes automatizados das regras e da validação, incluindo as fronteiras.
- API `POST /api/professores/busca`, lendo o `professores.json` uma vez ao subir.
- Respostas 400 em português no formato `{ "erros": [...] }`, inclusive para JSON malformado e campo com tipo errado; erro inesperado vira 500 no mesmo formato.
- Swagger em desenvolvimento, com descrição de cada campo e o "Try it out" já preenchido com o exemplo do enunciado; arquivo `.http` com 12 casos de teste.
- Testes com o `professores.json` real, reproduzindo o exemplo do enunciado (total: 68 testes).
- Tela em React + TypeScript (Vite) com formulário de idioma, dias, faixa e duração.
- Resultado com horários e aulas na semana de cada professor, a lista de quem ficou de fora com o motivo, e os estados de carregando, nenhum professor encontrado, erro de validação e falha de comunicação.
- Textos de ajuda que explicam a regra do horário fixo no campo de dias e na lista vazia.
- Visual inspirado no site da King of Languages: azul e vermelho da marca, título em duas cores, Poppins e Inter, ícone próprio; erro, bloqueio e horários com formas distintas; estado inicial explicando o horário fixo.
- Agenda dos professores, fora do enunciado: `GET /api/professores` e painel "Ver agenda dos professores" na tela, só de leitura (total: 74 testes).
- README com o passo a passo para rodar, escrito para quem não programa, as principais decisões e o que ficou faltando.
- Roteiro de teste (`docs/QA.md`) único do produto final, no mesmo padrão: cada teste com o valor de cada campo e o resultado esperado.
- `USO_DE_IA.md` com as ferramentas, como foram usadas, a supervisão do QA e três erros da IA.

### Corrigido

- Tabela da agenda passando da borda do cartão em telas de 320 px (achado no QA com Playwright).
- Lista "Ficaram de fora" com dois alinhamentos em tela estreita (achado no QA com Playwright).

[1.0.0]: https://github.com/GuilhermeSaldanha02/desafio-tecnico-dev-junior-carlos-guilherme/releases/tag/v1.0.0
