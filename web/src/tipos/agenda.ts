// Contrato da API: GET /api/professores (fora do enunciado, ADR-025)

export type PeriodoAgenda = {
  inicio: string
  fim: string
}

export type AgendaDia = {
  dia: string
  disponivel: PeriodoAgenda[]
  ocupado: PeriodoAgenda[]
}

export type AgendaProfessor = {
  id: number
  nome: string
  idiomas: string[]
  agendaBloqueada: boolean
  aulasNaSemana: number
  dias: AgendaDia[]
}

export type EstadoAgenda =
  | { tipo: 'fechada' }
  | { tipo: 'carregando' }
  | { tipo: 'sucesso'; agenda: AgendaProfessor[] }
  | { tipo: 'falha' }
