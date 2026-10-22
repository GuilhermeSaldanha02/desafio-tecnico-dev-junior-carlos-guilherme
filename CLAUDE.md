# Instruções para o agente de IA

Este repositório é um desafio técnico. Antes de qualquer tarefa, leia:

- [docs/ENUNCIADO.md](docs/ENUNCIADO.md): o enunciado original.
- [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md): estrutura, padrão de código e fluxo de Git. **Siga o padrão de código à risca.**
- [docs/DECISIONS.md](docs/DECISIONS.md): decisões já tomadas. Não rediscuta; decisão nova vira um ADR novo, com o OK do autor.
- [docs/DESIGN.md](docs/DESIGN.md) e [docs/QA.md](docs/QA.md): estados da tela e roteiro de teste.

Regras de trabalho:

- Responda e escreva em português do Brasil.
- Regra de negócio só no `EncontreProfessor.Core`; a Api só traduz para HTTP.
- Toda regra nova vem com teste na mesma branch.
- Branch de trabalho sai da `main`; merge direto na `staging` para o QA; PR da branch de trabalho para a `main`. **Nunca PR da `staging`. Nunca faça merge na `main`**: quem faz é o autor.
- Commits em Conventional Commits: `tipo(escopo): descrição`.
- Atualize o `CHANGELOG.md` a cada etapa.
- Quando o autor apontar um erro seu, registre o caso em `USO_DE_IA.md`.
