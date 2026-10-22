import type { ProfessorNaoElegivel } from '../tipos/busca'

type Props = {
  professores: ProfessorNaoElegivel[]
}

export function ListaNaoElegiveis({ professores }: Props) {
  return (
    <section className="nao-elegiveis" aria-labelledby="titulo-nao-elegiveis">
      <h2 id="titulo-nao-elegiveis" className="secao-titulo">
        Ficaram de fora
      </h2>
      <ul className="nao-elegiveis-lista">
        {professores.map((professor) => (
          <li key={professor.id} className="nao-elegivel">
            <span className="nao-elegivel-nome">{professor.nome}</span>
            <span className="nao-elegivel-motivo">{professor.motivo}</span>
          </li>
        ))}
      </ul>
    </section>
  )
}
