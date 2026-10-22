# Roteiro de QA

O QA roda na branch `staging`, depois que a branch da etapa foi mergeada nela. Problema encontrado vira comentário no card da etapa e é corrigido na branch da etapa, nunca na `staging`.

Em toda etapa: sem erro de ortografia no que foi escrito (código, mensagens e documentação).

## Etapa 1: estrutura (#1)

- [ ] `dotnet build` sem erros e sem avisos
- [ ] `dotnet test` roda (ainda sem testes)
- [ ] `dotnet --version` dentro da pasta mostra 8.0.x
- [ ] Pastas iguais ao desenho do [ARCHITECTURE.md](ARCHITECTURE.md)
- [ ] Documentação lida e sem erro de ortografia

## Etapa 2: regras no Core (#2)

- [ ] `dotnet test` verde
- [ ] Cada regra do enunciado tem pelo menos um teste (tabela regra → teste no PR)
- [ ] Fronteiras cobertas: aula que encosta, bloco quebrado, bloco que começa 18:15, empate na ordenação, prioridade dos motivos
- [ ] Exemplo do enunciado reproduzido com Ana, Bruno e Diego montados à mão (o teste com o arquivo real fica na etapa 3, ver ADR-017)

## Etapa 3: API (#3)

Suba a API com `dotnet run --project src/EncontreProfessor.Api` e teste pelo Swagger (`http://localhost:5067/swagger`), pelo Insomnia (importando `http://localhost:5067/swagger/v1/swagger.json`) ou pelo arquivo `src/EncontreProfessor.Api/EncontreProfessor.Api.http`, que tem os 12 casos abaixo numerados:

- [ ] Teste automatizado carregando o `professores.json` real reproduz o exemplo do enunciado
- [ ] Exemplo do enunciado: Ana com `18:00`; Diego com `18:00`, `18:30`, `19:00`
- [ ] `"idioma": "ingles"` encontra os professores de Inglês, e a ordem alfabética sai certa com acento
- [ ] Francês: 200, lista vazia, todos com "Não ensina o idioma"
- [ ] Cada um dos seis casos de 400, com mensagem em português
- [ ] Corpo vazio, JSON malformado e tipo errado: 400 no mesmo formato

## Etapa 4: tela (#4)

- [ ] Os seis estados do [DESIGN.md](DESIGN.md), um por um
- [ ] Falha de comunicação: desligar a API e buscar
- [ ] Erro de validação: buscar sem marcar nenhum dia
- [ ] Lista de quem ficou de fora aparece com o motivo

## Etapa 5: visual (#5)

- [ ] A tela continua passando pelos seis estados
- [ ] Nada quebra em tela de celular

## Etapa 6: entrega (#6)

- [ ] Roteiro completo das etapas 2 a 5 com tudo junto na `staging`
- [ ] README seguido do zero, numa pasta limpa, e funciona
