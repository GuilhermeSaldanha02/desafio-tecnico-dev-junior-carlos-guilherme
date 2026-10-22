import type { AgendaProfessor } from '../tipos/agenda'

const UrlAgenda = '/api/professores'

export async function listarAgenda(): Promise<AgendaProfessor[] | null> {
  try {
    const resposta = await fetch(UrlAgenda)
    if (!resposta.ok) return null

    return (await resposta.json()) as AgendaProfessor[]
  } catch {
    return null
  }
}
