# Encontre um Professor

Solução do desafio técnico de desenvolvimento (C#/.NET 8 e React): uma **API** que encontra os professores que podem dar aula para um novo aluno, no mesmo horário em todos os dias que ele escolher, e uma **tela** para usar essa busca.

- Enunciado original: [docs/ENUNCIADO.md](docs/ENUNCIADO.md)
- Roteiro de teste passo a passo: [docs/QA.md](docs/QA.md)
- Decisões tomadas: [docs/DECISIONS.md](docs/DECISIONS.md)
- Arquitetura e padrão de código: [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)
- Uso de IA no projeto: [USO_DE_IA.md](USO_DE_IA.md)

---

## Como rodar

Os passos abaixo foram escritos para quem nunca rodou um projeto assim. Cada comando deve ser **digitado (ou colado) no terminal e confirmado com Enter**. Onde aparece "o terminal mostra", é o que você deve ver para saber que deu certo.

### Passo 1: instalar o que é preciso (só na primeira vez)

| Programa | Para quê | Onde baixar |
|---|---|---|
| Git | baixar o projeto | https://git-scm.com/downloads |
| .NET SDK **8** | rodar a API e os testes | https://dotnet.microsoft.com/download/dotnet/8.0 (escolha "SDK", não "Runtime") |
| Node.js **20 ou mais novo** | rodar a tela | https://nodejs.org (a versão marcada como "LTS") |

Depois de instalar, **feche e abra o terminal de novo**, para ele reconhecer os programas novos.

**Como abrir o terminal:**
- **Windows:** aperte a tecla Windows, digite `PowerShell` e aperte Enter.
- **macOS:** aperte Cmd + Espaço, digite `Terminal` e aperte Enter.

**Para conferir se instalou certo**, digite cada comando abaixo:

```bash
dotnet --list-sdks
```

O terminal mostra pelo menos uma linha começando com `8.0` (por exemplo, `8.0.426`).

```bash
node --version
```

O terminal mostra `v20` ou um número maior (por exemplo, `v22.11.0`).

### Passo 2: baixar o projeto

```bash
git clone https://github.com/GuilhermeSaldanha02/desafio-tecnico-dev-junior-carlos-guilherme.git
```

```bash
cd desafio-tecnico-dev-junior-carlos-guilherme
```

A partir daqui, **todos os comandos são digitados dentro dessa pasta**.

### Passo 3: rodar os testes automáticos

```bash
dotnet test
```

Na primeira vez demora um pouco (ele baixa as dependências). No fim, o terminal mostra uma linha parecida com esta, com **0 com falha e 74 aprovados**:

```
Aprovado!  – Com falha: 0, Aprovado: 74, Ignorado: 0, Total: 74
```

Se o seu sistema estiver em inglês, a linha aparece como `Passed!  - Failed: 0, Passed: 74`.

### Passo 4: ligar a API (terminal 1)

```bash
dotnet run --project src/EncontreProfessor.Api
```

Espere até o terminal mostrar:

```
Now listening on: http://localhost:5067
```

**Deixe esse terminal aberto.** Enquanto ele estiver aberto, a API está ligada.

Para ver e testar a API sem a tela, abra no navegador: **http://localhost:5067/swagger** (o passo a passo está em [docs/QA.md](docs/QA.md), parte C).

### Passo 5: ligar a tela (terminal 2)

Abra **um segundo terminal** (do mesmo jeito do passo 1) e entre de novo na pasta do projeto:

```bash
cd desafio-tecnico-dev-junior-carlos-guilherme
```

Entre na pasta da tela:

```bash
cd web
```

Instale as dependências da tela. **Só na primeira vez**; leva de 1 a 2 minutos:

```bash
npm install
```

Ligue a tela:

```bash
npm run dev
```

Espere até o terminal mostrar uma linha com:

```
Local:   http://localhost:5173/
```

**Deixe esse terminal aberto também.**

### Passo 6: usar

Abra no navegador: **http://localhost:5173**

O formulário já vem preenchido com o exemplo do enunciado (Espanhol, segunda-feira, das 18:00 às 20:00, aula de 60 minutos). Clique em **Buscar professores**. Devem aparecer:

- **Ana**, com o horário **18:00**;
- **Diego**, com os horários **18:00, 18:30 e 19:00**;
- embaixo, em "Ficaram de fora", os outros 6 professores com o motivo "Não ensina o idioma".

No fim da página, **"Ver agenda dos professores"** mostra os dias e horários de cada professor, para entender por que alguém aparece ou fica de fora.

### Passo 7: desligar

Em **cada um dos dois terminais**, aperte **Ctrl + C**. Se ele perguntar se quer encerrar, digite `S` (ou `Y`) e Enter.

### Se algo der errado

| O que aparece | O que fazer |
|---|---|
| `dotnet` ou `npm` "não é reconhecido como comando" | O programa não foi instalado, ou o terminal foi aberto antes da instalação. Instale (passo 1) e abra um terminal novo. |
| `dotnet --list-sdks` não mostra nenhuma linha com `8.0` | Instale o **SDK** do .NET 8 (passo 1). Ter só o .NET 9 ou 10 não basta: o projeto exige o 8. |
| "address already in use" ou porta 5067 / 5173 em uso | A API ou a tela já estão ligadas em outro terminal. Use esse, ou desligue-o com Ctrl + C. |
| Na tela: "Não foi possível falar com o servidor. Tente de novo." | A API não está ligada. Faça o passo 4 e tente de novo. |
| A tela abre sem as fontes do desenho | Sem internet, a tela usa as fontes do computador. Ela funciona igual. |

---

## Decisões tomadas

O enunciado pede para registrar o que fazer quando as regras não cobrem uma situação. As principais decisões estão abaixo; todas, com contexto e consequência, estão em [docs/DECISIONS.md](docs/DECISIONS.md).

| Situação | Decisão | Onde |
|---|---|---|
| Idioma escrito "ingles" ou "INGLÊS" | Compara sem acento e sem maiúscula: encontra "Inglês". Erro de digitação ("Ingês") não é corrigido. | ADR-005 |
| Dia escrito "Terça" ou repetido | Aceita acento e maiúscula; dia repetido é ignorado; dia fora da lista ("sabadoo") é erro 400. | ADR-006 |
| Idioma que ninguém ensina, ou faixa menor que a aula | Não é erro: responde 200 com a lista vazia e o motivo de cada professor. | ADR-007 |
| Formato do erro 400 | `{ "erros": [...] }` com **todos** os erros de uma vez, em português, inclusive JSON malformado. | ADR-008, 019 |
| Horário "9:00", "24:00" ou "18:60" | Erro 400: o formato é exatamente `HH:mm`, de 00:00 a 23:59. | ADR-009, 018 |
| Faixa começando em 18:15 | Vale: o primeiro horário possível é 18:30. | ADR-013 |
| "Aulas na semana" | Conta a semana inteira, não só os dias pedidos. | ADR-010 |
| Ordem alfabética com acento | "Ágata" vem antes de "Bruno", em qualquer computador. | ADR-011 |
| Mensagem quando ninguém ensina o idioma | A tela escreve "Nenhum professor ensina Francês no momento."; a resposta da API continua igual à do enunciado. | ADR-012 |
| Validação na tela | A tela não bloqueia o envio; quem valida é a API, e a tela mostra os erros dela. | ADR-014 |
| Agenda dos professores | **Fora do enunciado.** Painel só de leitura, criado porque no teste da tela não dava para entender quem ficava de fora sem conhecer os dados. Não muda nenhuma regra da busca. | ADR-025 |

**Arquitetura, em uma frase:** a regra de negócio fica em `src/EncontreProfessor.Core`, sem nada de HTTP; a `src/EncontreProfessor.Api` só recebe a requisição, chama o Core e responde 200 ou 400; os testes em `tests/` testam o Core direto, sem ligar a API. Detalhes em [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

---

## O que ficou faltando

- **Testes automáticos da tela e da camada HTTP.** A regra de negócio tem 74 testes. A API e a tela foram testadas manualmente (Swagger, Insomnia, arquivo `.http`) e por sessões de teste com Playwright, mas esses testes não estão salvos no repositório para rodar de novo sozinhos.
- **Integração contínua.** Não há um GitHub Actions rodando os testes e o build da tela a cada PR.
- **Publicação.** O projeto roda só no computador local; não há um endereço público.
- **Lista oficial de idiomas.** Sem ela, a API não diferencia um erro de digitação de um idioma que a escola não oferece (ADR-005).
- **Banco de dados.** Os professores são lidos do `professores.json` e ficam em memória, como o enunciado permite (ADR-015).

---

## Como o trabalho foi organizado

- **Tarefas:** um card por etapa no [quadro do projeto](https://github.com/users/GuilhermeSaldanha02/projects/1).
- **Branches:** cada etapa numa branch própria; testada na `staging`; entra na `main` por PR revisado e mergeado por mim. Cada PR registra o que foi testado, separando o teste automatizado do manual.
- **Histórico de mudanças:** [CHANGELOG.md](CHANGELOG.md).
