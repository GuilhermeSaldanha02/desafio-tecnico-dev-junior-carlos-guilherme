# Changelog

Todas as mudanças relevantes do projeto ficam registradas aqui.

O formato segue o [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/), e o projeto usa [Versionamento Semântico](https://semver.org/lang/pt-BR/).

## [Não lançado]

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
