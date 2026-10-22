# Decisões

Registro das decisões do projeto no formato ADR (*Architecture Decision Record*): o contexto, o que foi decidido e a consequência. Inclui as situações que o enunciado não cobre, como ele pede.

---

## ADR-001: Regra de negócio em projeto separado da API

**Contexto:** o enunciado avalia se a regra de negócio está separada da camada HTTP, e os testes não podem depender de subir a API.
**Decisão:** dois projetos de código (`Core` e `Api`) mais o de testes. Sem camadas extras de Clean Architecture.
**Consequência:** a regra fica em um lugar só e é testada direto. Camadas a mais seriam abstrações sem função num problema deste tamanho.

## ADR-002: Controllers em vez de Minimal API

**Contexto:** as duas formas atendem o endpoint.
**Decisão:** Controllers.
**Consequência:** é o padrão mais comum em APIs .NET corporativas e deixa visível que a camada HTTP é fina: validar, chamar o serviço, responder.

## ADR-003: Validação da requisição no Core, escrita à mão

**Contexto:** a validação precisa ser testável sem subir a API.
**Decisão:** um validador no Core que devolve a lista de erros. Sem bibliotecas como FluentValidation.
**Consequência:** são seis regras; escritas à mão, cada linha é explicável. A Api só converte "tem erros" em 400.

## ADR-004: Horário representado em minutos desde 00:00

**Contexto:** as regras comparam intervalos (cabe no bloco, colide com aula marcada).
**Decisão:** converter `"HH:mm"` em minutos na entrada (`"18:30"` → 1110) e voltar para texto só na saída.
**Consequência:** a colisão vira uma comparação simples: `inicio < fimDaOutra && inicioDaOutra < fim`. Com `<` estrito, encostar não é colisão, como o enunciado exige.

## ADR-005: Idioma comparado sem acento e sem diferença de maiúscula

**Contexto:** quem chama a API direto pode mandar `"ingles"` ou `"INGLÊS"`.
**Decisão:** aparar espaços e comparar sem acento e sem caixa. Isso é **normalização**, não validação: tolera diferença de formato, não corrige erro de digitação.
**Consequência:** `"ingles"` casa com `"Inglês"`; `"Ingês"` (letra faltando) não casa e é tratado como idioma que ninguém ensina. A API não tem lista oficial de idiomas, então não consegue distinguir um erro de digitação de um idioma que a escola não oferece. Corrigir para a palavra mais parecida foi descartado: uma busca que adivinha errado é pior que uma que diz "não achei". A única validação de idioma é a do enunciado: não pode estar vazio.

## ADR-006: Dias normalizados, validados contra a lista e sem repetição

**Contexto:** ao contrário dos idiomas, os dias têm lista fechada no enunciado.
**Decisão:** normalizar como o idioma (`"Terça"` → `terca`); qualquer valor fora dos sete dias é 400 com o dia na mensagem; dia repetido é removido em silêncio.
**Consequência:** `"sabadoo"` dá 400 `Dia inválido: "sabadoo"`. `["segunda", "segunda"]` vira `["segunda"]`, porque repetir não muda o resultado.

## ADR-007: Requisição válida que ninguém atende é 200 com lista vazia

**Contexto:** idioma que ninguém ensina (`"Alemão"`) ou faixa menor que a duração (18:00–18:30 com 60 min).
**Decisão:** não é erro do cliente. Responde 200 com `professores` vazio e o motivo de cada um em `naoElegiveis`.
**Consequência:** os seis casos de 400 do enunciado são todos de formato; estes não são, e criar novos 400 seria inventar regra.

## ADR-008: Formato da resposta 400

**Contexto:** o enunciado pede "uma mensagem clara", sem definir o formato.
**Decisão:** `{ "erros": ["...", "..."] }`, com **todos** os erros de uma vez, em português. Corpo ausente, JSON malformado ou tipo errado (`"duracaoMinutos": "abc"`) saem no mesmo formato.
**Consequência:** o usuário corrige tudo de uma vez, e o front lê sempre o mesmo formato. Por padrão o ASP.NET responderia em inglês e em outro formato; isso é configurado na Api.

## ADR-009: Formato de horário estrito

**Decisão:** só `HH:mm` com dois dígitos, de `00:00` a `23:59`. `"9:00"`, `"24:00"` e `"18:60"` são 400.
**Consequência:** é a leitura literal de "fora do formato `HH:mm`", e os dados garantem que nada atravessa a meia-noite.

## ADR-010: `aulasNaSemana` conta a semana inteira

**Decisão:** total de aulas marcadas do professor, não só nos dias pedidos.
**Consequência:** é o que o nome do campo diz e é o critério da ordenação.

## ADR-011: Ordenação alfabética respeitando o português

**Decisão:** ordenar pelo nome **normalizado** (sem acento, em minúsculas, a mesma função usada para idioma e dias), com comparação ordinal. Empate exato no nome desempata pelo `id`. `naoElegiveis` também sai em ordem alfabética.
**Consequência:** "Ágata" vem antes de "Bruno"; uma comparação ordinal do nome original jogaria nomes com acento para depois do Z. Não depender da cultura do sistema (`pt-BR`) faz o resultado ser o mesmo em qualquer máquina, inclusive Linux ou contêiner sem dados de idioma instalados.

## ADR-012: A frase "nenhum professor ensina este idioma" é da tela

**Contexto:** com lista vazia, o usuário precisa entender o porquê.
**Decisão:** a API não ganha campo novo. A tela vê que a lista está vazia e que todos os motivos são "Não ensina o idioma", e então mostra *"Nenhum professor ensina {idioma} no momento"*.
**Consequência:** o contrato da API fica exatamente como o enunciado define, e o texto pode mudar sem mexer na API.

## ADR-013: Faixa do aluno fora de :00/:30

**Contexto:** o aluno pode pedir a faixa a partir de `18:15`.
**Decisão:** a requisição é válida; o primeiro início candidato é o próximo `:00` ou `:30` (`18:30`).
**Consequência:** mesma lógica usada para a disponibilidade de professores que começa fora de hora cheia ou meia hora.

## ADR-014: A tela não bloqueia o envio com validação própria

**Contexto:** a tela precisa mostrar os erros de validação devolvidos pela API.
**Decisão:** o formulário usa controles que já evitam erro (select, checkboxes, campo de hora), mas não impede o envio. Quem valida é a API.
**Consequência:** o estado "erro de validação da API" acontece de verdade na tela (por exemplo, sem nenhum dia marcado), e as regras não ficam duplicadas em dois lugares.

## ADR-015: Dados em memória, sem banco

**Decisão:** o `professores.json` é lido uma vez quando a API sobe e fica em memória.
**Consequência:** é o que o enunciado permite e evita dependência de banco ou Docker.

## ADR-016: .NET 8 fixado com `global.json`

**Contexto:** a máquina de desenvolvimento também tem o .NET 10, que seria usado por padrão.
**Decisão:** `global.json` exige um SDK 8.0 e os projetos têm `net8.0` como alvo.
**Consequência:** compila e testa com a mesma versão que o enunciado exige.

## ADR-017: Modelos do Core não conhecem JSON

**Contexto:** os modelos têm construtor e propriedades só de leitura, e o `System.Text.Json` não preenche esse tipo de classe sem atributos.
**Decisão:** nenhum atributo de JSON no Core. A Api lê o arquivo em classes próprias de leitura e converte para os modelos do Core.
**Consequência:** o Core continua sem saber de onde os dados vêm. O teste que carrega o `professores.json` real e confere o exemplo do enunciado fica na etapa da Api, onde existe a leitura do arquivo. Na etapa do Core, o mesmo exemplo é testado com Ana, Bruno e Diego montados à mão, iguais ao arquivo.

## ADR-018: Formato de horário validado por expressão regular

**Contexto:** `"18:00\n"` e dígitos de outros alfabetos passariam em validações mais frouxas.
**Decisão:** a expressão `^([01][0-9]|2[0-3]):[0-5][0-9]\z`, com `[0-9]` em vez de `\d` (que aceita dígitos de outros alfabetos) e `\z` em vez de `$` (que aceita uma quebra de linha no fim).
**Consequência:** o formato é exatamente `HH:mm`, e cada caso estranho tem teste.

## ADR-019: Erros de leitura do JSON no mesmo formato da validação

**Contexto:** JSON malformado ou campo com tipo errado (`"duracaoMinutos": "abc"`) são barrados pelo ASP.NET antes de chegar ao Core, com resposta em inglês e em outro formato.
**Decisão:** substituir a resposta automática (`InvalidModelStateResponseFactory`) por uma que devolve `{ "erros": [...] }` em português: com o nome do campo quando o problema é um campo, ou "O corpo da requisição não é um JSON válido." quando é o corpo inteiro. Corpo vazio chega ao Core como requisição ausente.
**Consequência:** a tela trata um único formato de erro. Observação: um corpo enviado com acentos fora de UTF-8 é lido como texto inválido e cai em "tipo inválido" no campo; navegador, Swagger e Insomnia sempre enviam UTF-8.

## ADR-020: API só em HTTP no ambiente local

**Contexto:** o template redireciona para HTTPS, e a tela vai chamar a API por um proxy do Vite em HTTP.
**Decisão:** remover o redirecionamento; a API roda em `http://localhost:5067`.
**Consequência:** o proxy funciona sem certificado de desenvolvimento. Em produção, o HTTPS seria responsabilidade de quem publica a API.

## ADR-021: Testes referenciam a Api só para ler o arquivo real

**Contexto:** o teste com o `professores.json` real precisa do repositório, que fica na Api (ADR-017).
**Decisão:** o projeto de testes referencia a Api e usa o `ProfessorRepositoryJson` direto, sem subir servidor.
**Consequência:** o enunciado pede testes que não dependam de subir a API, e eles não dependem: criam o repositório e o serviço como objetos comuns.

## ADR-022: Tela sem biblioteca de estado nem de componentes

**Contexto:** a tela tem um formulário e um resultado.
**Decisão:** React com `useState` e `fetch` nativo, CSS próprio com variáveis. O estado da tela é um tipo só, com cinco formas possíveis (inicial, carregando, sucesso, validação, falha), e o TypeScript obriga a tratar cada uma.
**Consequência:** nenhum estado impossível, como "carregando e com erro ao mesmo tempo", pode acontecer. Qualquer resposta que não seja 200 ou 400 com `erros`, inclusive a API fora do ar, vira o estado de falha de comunicação.

## ADR-023: Formulário começa com o exemplo do enunciado

**Decisão:** os valores iniciais são Espanhol, segunda, 18:00 a 20:00 e 60 minutos.
**Consequência:** quem abre a tela pela primeira vez vê um resultado com um clique, e ele bate com o exemplo do enunciado.

## ADR-024: A tela explica a regra do horário fixo

**Contexto:** no QA, marcar os sete dias deu "nenhum professor disponível", e pareceu erro. Não era: a regra 3 exige o mesmo início livre em todos os dias, e ninguém atende sábado ou domingo. O código estava certo; a tela não explicava a regra.
**Decisão:** uma linha de ajuda abaixo de "Dias da semana" e uma dica junto da mensagem de lista vazia. Com mais de um dia marcado, a dica fala da regra e sugere marcar menos dias; com um dia só, sugere ampliar a faixa ou trocar o dia.
**Consequência:** a dúvida é resolvida na própria tela, sem mudar a API nem a regra. É o tipo de problema que um teste automatizado não pega e o QA manual pegou.

## ADR-025: Agenda dos professores, fora do enunciado

**Contexto:** no QA, quem testa a tela não conhece o `professores.json` e não consegue saber por que um professor aparece ou fica de fora. O primeiro usuário real da tela é quem avalia o desafio.
**Decisão:** `GET /api/professores` devolve a agenda de cada professor (idiomas, períodos livres e horários ocupados por dia, agenda bloqueada), e a tela mostra isso num painel recolhível "Ver agenda dos professores". O serviço fica em `Core/Agenda`, separado da busca, e só lê os dados: não filtra nem calcula horário.
- A Carla aparece com "Agenda bloqueada: não recebe novos alunos". A regra 2 diz que ela não aparece **no resultado da busca**, e ela continua sem aparecer lá; escondê-la da agenda deixaria sem explicação o motivo dela em "Ficaram de fora".
- As aulas marcadas aparecem só como horário ocupado, sem dizer de quem é a aula. Num sistema real, o horário de outros alunos não deveria ser exposto.

**Consequência:** nenhum arquivo da busca nem dos testes da busca mudou, e os 68 testes anteriores continuam passando sem alteração. O contrato do `POST /api/professores/busca` é o do enunciado.
