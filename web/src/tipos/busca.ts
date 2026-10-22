// Contrato da API: POST /api/professores/busca

export type BuscaRequisicao = {
  idioma: string
  dias: string[]
  inicioFaixa: string
  fimFaixa: string
  duracaoMinutos: number
}

export type ProfessorDisponivel = {
  id: number
  nome: string
  aulasNaSemana: number
  horarios: string[]
}

export type ProfessorNaoElegivel = {
  id: number
  nome: string
  motivo: string
}

export type BuscaResposta = {
  professores: ProfessorDisponivel[]
  naoElegiveis: ProfessorNaoElegivel[]
}

export const MotivoNaoEnsinaIdioma = 'Não ensina o idioma'

// O que a chamada pode dar: a tela decide o que mostrar a partir disto

export type ResultadoChamada =
  | { tipo: 'sucesso'; resposta: BuscaResposta }
  | { tipo: 'validacao'; erros: string[] }
  | { tipo: 'falha' }

export type EstadoTela =
  | { tipo: 'inicial' }
  | { tipo: 'carregando' }
  | { tipo: 'sucesso'; resposta: BuscaResposta; idioma: string }
  | { tipo: 'validacao'; erros: string[] }
  | { tipo: 'falha' }
