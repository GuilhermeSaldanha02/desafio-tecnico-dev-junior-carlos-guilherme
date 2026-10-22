import type { BuscaRequisicao, BuscaResposta, ResultadoChamada } from '../tipos/busca'

const UrlBusca = '/api/professores/busca'
const StatusRequisicaoInvalida = 400

export async function buscarProfessores(requisicao: BuscaRequisicao): Promise<ResultadoChamada> {
  let resposta: Response

  try {
    resposta = await fetch(UrlBusca, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(requisicao),
    })
  } catch {
    return { tipo: 'falha' }
  }

  if (resposta.status === StatusRequisicaoInvalida) return lerErrosDeValidacao(resposta)
  if (!resposta.ok) return { tipo: 'falha' }

  try {
    const corpo = (await resposta.json()) as BuscaResposta
    return { tipo: 'sucesso', resposta: corpo }
  } catch {
    return { tipo: 'falha' }
  }
}

async function lerErrosDeValidacao(resposta: Response): Promise<ResultadoChamada> {
  try {
    const corpo = (await resposta.json()) as { erros?: unknown }
    if (!Array.isArray(corpo.erros)) return { tipo: 'falha' }

    return { tipo: 'validacao', erros: corpo.erros.map(String) }
  } catch {
    return { tipo: 'falha' }
  }
}
