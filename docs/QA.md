# Roteiro de teste

Este roteiro foi escrito para que **qualquer pessoa, mesmo sem saber programar**, consiga testar o projeto inteiro e saber se deu certo. Cada teste diz o que fazer, o valor de cada campo e o que deve aparecer.

O registro dos testes feitos em cada etapa do desenvolvimento está na descrição de cada PR, separando o teste automatizado (Playwright, supervisionado pelo Guilherme) do teste manual.

## Antes de começar

1. Siga o [README](../README.md) até o **passo 5**. No fim, você terá **dois terminais abertos**: um com a API ligada e outro com a tela ligada.
2. Abra o navegador (Chrome ou Edge) em **http://localhost:5173**.
3. Marque cada caixinha `[ ]` deste roteiro conforme o teste passar. Se algo sair diferente do esperado, anote o número do teste e o que apareceu.

**Para voltar a tela ao começo** antes de cada teste: aperte **F5** no navegador. O formulário volta a Espanhol, segunda-feira, 18:00 às 20:00, 60 minutos.

---

## Parte A: testes automáticos (2 minutos)

Num terceiro terminal, dentro da pasta do projeto, digite:

```bash
dotnet test
```

- [ ] O terminal termina com **0 com falha e 74 aprovados** (`Aprovado!  – Com falha: 0, Aprovado: 74` ou, em inglês, `Passed!  - Failed: 0, Passed: 74`).

---

## Parte B: a tela (15 minutos)

### Como preencher o formulário

| Campo | Como mudar |
|---|---|
| **Idioma** | clique na caixa e escolha Inglês, Espanhol ou Francês |
| **Dias da semana** | clique no quadradinho de cada dia para marcar ou desmarcar |
| **Das / Até** | clique na hora e digite, por exemplo, `18:00` |
| **Duração da aula** | clique na caixa e escolha 30, 60 ou 90 minutos |

Depois de preencher, clique no botão vermelho **Buscar professores**. O resultado aparece à direita (no celular, embaixo). **"Ficaram de fora"** é a lista abaixo do resultado, com cada professor e o motivo.

### Testes

Comece cada teste com **F5**. Campos não citados ficam como vieram.

| # | Idioma | Dias marcados | Das | Até | Duração | O que deve aparecer |
|---|---|---|---|---|---|---|
| 1 | *(não mexa em nada e não clique em buscar)* | | | | | "Escolha o idioma, os dias e a faixa de horário, e clique em buscar." e, abaixo, a explicação de que os horários mostrados ficam livres em todos os dias marcados |
| 2 | Espanhol | só segunda | 18:00 | 20:00 | 60 | **Ana**: `18:00`. **Diego**: `18:00` `18:30` `19:00`. Em "Ficaram de fora", os outros 6 com "Não ensina o idioma" |
| 3 | Inglês | segunda **e** quarta | 18:00 | 22:00 | 60 | **Ana**: `18:00` `20:00` `20:30` `21:00`. **Fábio**: `18:30` `19:00`. **Bruno**: `18:00` `20:00`. **Gabriela**: `18:30` `19:00` `19:30`. Em "Ficaram de fora": Carla (Agenda bloqueada), Diego (Não ensina o idioma), Elisa e Heitor (Sem horário em comum nos dias pedidos) |
| 4 | Francês | só segunda | 18:00 | 20:00 | 60 | "Nenhum professor ensina Francês no momento." e os 8 professores em "Ficaram de fora" |
| 5 | Inglês | só segunda | 18:00 | **18:30** | 60 | "Nenhum professor disponível nesses dias e horários." e, abaixo, "Tente ampliar a faixa de horário ou escolher outro dia." |
| 6 | Inglês | **todos os 7 dias** | 18:00 | 20:00 | 60 | "Nenhum professor disponível nesses dias e horários." e, abaixo, "O mesmo horário precisa estar livre em todos os dias marcados. Tente marcar menos dias ou ampliar a faixa." |
| 7 | Inglês | **nenhum** (desmarque a segunda) | 18:00 | 20:00 | 60 | Caixa com borda vermelha à esquerda: "Corrija a busca:" e "Informe pelo menos um dia da semana." |

- [ ] Teste 1
- [ ] Teste 2
- [ ] Teste 3
- [ ] Teste 4
- [ ] Teste 5
- [ ] Teste 6
- [ ] Teste 7

**Por que o teste 6 não acha ninguém:** a aula precisa ser no **mesmo horário em todos os dias marcados**, e nenhum professor atende sábado nem domingo. A tabela "Agenda dos professores", no fim deste arquivo, mostra isso.

### Teste 8: API desligada

1. No **terminal da API** (terminal 1), aperte **Ctrl + C** e espere uns 5 segundos.
2. Volte ao navegador e clique em **Buscar professores**.
3. - [ ] Aparece, com borda vermelha à esquerda: "Não foi possível falar com o servidor. Tente de novo."
4. Ligue a API de novo: no terminal 1, digite `dotnet run --project src/EncontreProfessor.Api` e espere `Now listening on: http://localhost:5067`.

### Teste 9: agenda dos professores (fora do enunciado)

1. Role a página até o fim e clique em **"Ver agenda dos professores"** (texto azul com uma setinha).
2. - [ ] Aparecem **8 cartões**, um por professor, de Ana a Heitor.
3. - [ ] O cartão da **Carla** tem a etiqueta cinza "Agenda bloqueada: não recebe novos alunos".
4. - [ ] No cartão do **Bruno**, na linha da quarta, aparecem dois períodos livres: `18:00–19:00` e `20:00–22:00`.
5. Compare os cartões com a tabela "Agenda dos professores" no fim deste arquivo: devem ser iguais.

### Teste 10: celular

1. No navegador, aperte **F12**. Abre uma área de ferramentas.
2. Aperte **Ctrl + Shift + M** (ou clique no ícone de celular no topo dessa área).
3. No topo da página aparece uma barra com a largura da tela. Escolha uma largura de **375** e depois de **320**.
4. Faça o **teste 3** e abra a **agenda** (teste 9) em cada largura.
5. - [ ] O formulário fica em cima e o resultado embaixo.
6. - [ ] Nada sai pela lateral: não aparece barra de rolagem para os lados, e nenhum texto ou tabela passa da borda do seu quadro.
7. - [ ] Em "Ficaram de fora", o motivo de cada professor fica embaixo do nome, todos alinhados à esquerda.
8. Para voltar ao normal, aperte **F12** de novo.

### Teste 11: aparência

- [ ] O título tem "Encontre" em vermelho e "um Professor" em azul.
- [ ] A aba do navegador mostra um ícone próprio (relógio azul com ponteiro vermelho).
- [ ] O botão "Buscar professores" é vermelho e escurece quando o mouse passa por cima.
- [ ] Os horários do resultado aparecem em etiquetas azuis; os professores do resultado são separados por uma linha.

---

## Parte C: a API sem a tela, pelo Swagger (10 minutos)

O Swagger é uma página que já vem com a API e permite testar sem a tela.

1. Com a API ligada (terminal 1), abra no navegador: **http://localhost:5067/swagger**
2. Clique na faixa **POST /api/professores/busca** para abrir.
3. Clique em **Try it out** (à direita).
4. A caixa **Request body** já vem com o exemplo do enunciado. Clique em **Execute**.
5. - [ ] Em **Server response**, o **Code** é `200`, e o texto mostra Ana com `"18:00"` e Diego com `"18:00"`, `"18:30"`, `"19:00"`.

Agora troque o conteúdo da caixa **Request body**: apague tudo, cole cada texto abaixo e clique em **Execute** de novo.

**C1: todos os erros de uma vez.** Deve dar `400` e 5 mensagens em português.

```json
{ "idioma": "", "dias": ["sabadoo"], "inicioFaixa": "9:00", "fimFaixa": "24:00", "duracaoMinutos": 45 }
```

- [ ] `400`, com: "O idioma é obrigatório.", "Dia inválido: \"sabadoo\"…", os dois erros de formato HH:mm e "A duração deve ser de 30, 60 ou 90 minutos."

**C2: fim da faixa antes do início.**

```json
{ "idioma": "Inglês", "dias": ["segunda"], "inicioFaixa": "20:00", "fimFaixa": "18:00", "duracaoMinutos": 60 }
```

- [ ] `400`, com "O fim da faixa deve ser depois do início."

**C3: idioma sem acento e dia com acento.**

```json
{ "idioma": "INGLES", "dias": ["Terça", "terca"], "inicioFaixa": "18:00", "fimFaixa": "20:00", "duracaoMinutos": 60 }
```

- [ ] `200`, com só o **Bruno**, no horário `"19:00"`.

**C4: número escrito como texto.**

```json
{ "idioma": "Inglês", "dias": ["segunda"], "inicioFaixa": "18:00", "fimFaixa": "20:00", "duracaoMinutos": "abc" }
```

- [ ] `400`, com "O campo \"duracaoMinutos\" tem um valor de tipo inválido."

**C5: agenda dos professores.** Feche a faixa do POST, abra a faixa **GET /api/professores**, clique em **Try it out** e em **Execute**.

- [ ] `200`, com os 8 professores em ordem alfabética.

Quem preferir, os mesmos casos (e mais alguns, 13 no total) estão prontos no arquivo `src/EncontreProfessor.Api/EncontreProfessor.Api.http`, que pode ser aberto no Visual Studio, no Rider, no VS Code (com a extensão REST Client) ou importado no Insomnia.

---

## Ao terminar

Desligue a API e a tela: em cada terminal, **Ctrl + C**.

---

## Agenda dos professores (referência para conferir resultados)

Copiada do `professores.json`. Use para entender por que um professor aparece ou fica de fora.

| Professor | Idiomas | Dias e horários disponíveis | Aulas já marcadas |
|---|---|---|---|
| Ana | Inglês, Espanhol | segunda 18:00–22:00; quarta 18:00–22:00 | segunda 19:00 (60 min) |
| Bruno | Inglês | segunda 18:00–21:00; terça 18:00–20:00; quarta 18:00–19:00 e 20:00–22:00; quinta 18:00–20:00 | terça 18:00 (60 min); quinta 18:00 (60 min) |
| Carla | Inglês | segunda e quarta 08:00–22:00 | nenhuma (**agenda bloqueada**) |
| Diego | Espanhol | segunda 18:00–22:00; terça 08:00–12:00; quarta 18:00–22:00 | terça 08:30 (30 min) |
| Elisa | Inglês | segunda 18:00–22:00 | nenhuma |
| Fábio | Inglês | segunda 18:15–20:00; quarta 18:15–20:00; sexta 09:00–12:00 | sexta 10:00 (60 min) |
| Gabriela | Inglês | segunda 17:00–23:00; quarta 17:00–23:00 | segunda 18:00 (30 min); quarta 20:30 (60 min) |
| Heitor | Inglês | segunda 18:00–21:00; quarta 18:00–21:00 | segunda 19:00 e 20:00 (60 min); quarta 18:00 e 19:00 (60 min) |

Ninguém ensina Francês, e ninguém atende sábado ou domingo.
