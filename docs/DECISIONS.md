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

**Decisão:** comparar nomes com a cultura `pt-BR`. `naoElegiveis` também sai em ordem alfabética.
**Consequência:** "Fábio" fica entre "Elisa" e "Gabriela". Uma comparação ordinal jogaria nomes com acento para depois do Z.

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
