import type { ProfessorDisponivel } from '../tipos/busca'

type Props = {
  professores: ProfessorDisponivel[]
}

function textoAulasNaSemana(quantidade: number) {
  if (quantidade === 1) return '1 aula na semana'

  return `${quantidade} aulas na semana`
}

export function ListaProfessores({ professores }: Props) {
  return (
    <ul className="professores">
      {professores.map((professor) => (
        <li key={professor.id} className="professor">
          <div className="professor-cabecalho">
            <h3 className="professor-nome">{professor.nome}</h3>
            <span className="professor-aulas">{textoAulasNaSemana(professor.aulasNaSemana)}</span>
          </div>
          <p className="horarios-rotulo">Horários possíveis</p>
          <ul className="horarios">
            {professor.horarios.map((horario) => (
              <li key={horario} className="horario">
                {horario}
              </li>
            ))}
          </ul>
        </li>
      ))}
    </ul>
  )
}
