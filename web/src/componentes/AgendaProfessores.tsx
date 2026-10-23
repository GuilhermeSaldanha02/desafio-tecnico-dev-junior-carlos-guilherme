import { useState, type SyntheticEvent } from 'react'
import { listarAgenda } from '../api/listarAgenda'
import type { AgendaProfessor, EstadoAgenda, PeriodoAgenda } from '../tipos/agenda'

const NomesDosDias: Record<string, string> = {
  segunda: 'Segunda',
  terca: 'Terça',
  quarta: 'Quarta',
  quinta: 'Quinta',
  sexta: 'Sexta',
  sabado: 'Sábado',
  domingo: 'Domingo',
}

// cada período fica inteiro na linha; a quebra só acontece entre um período e outro
function Periodos({ periodos }: { periodos: PeriodoAgenda[] }) {
  if (periodos.length === 0) return <>—</>

  return (
    <>
      {periodos.map((periodo) => (
        <span key={periodo.inicio} className="agenda-periodo">
          {periodo.inicio}–{periodo.fim}
        </span>
      ))}
    </>
  )
}

// Fora do enunciado (ADR-025): ajuda quem testa a entender o resultado da busca
export function AgendaProfessores() {
  const [estado, setEstado] = useState<EstadoAgenda>({ tipo: 'fechada' })

  async function aoAbrir(evento: SyntheticEvent<HTMLDetailsElement>) {
    if (!evento.currentTarget.open) return
    if (estado.tipo === 'sucesso' || estado.tipo === 'carregando') return

    setEstado({ tipo: 'carregando' })

    const agenda = await listarAgenda()
    if (agenda === null) {
      setEstado({ tipo: 'falha' })
      return
    }

    setEstado({ tipo: 'sucesso', agenda })
  }

  return (
    <details className="painel agenda" onToggle={aoAbrir}>
      <summary className="agenda-resumo">Ver agenda dos professores</summary>
      <p className="campo-ajuda">Para entender o resultado da busca: dias, períodos livres e horários já ocupados de cada professor.</p>
      <ConteudoAgenda estado={estado} />
    </details>
  )
}

function ConteudoAgenda({ estado }: { estado: EstadoAgenda }) {
  if (estado.tipo === 'fechada') return null

  if (estado.tipo === 'carregando') {
    return (
      <p className="aviso" role="status">
        Carregando agenda…
      </p>
    )
  }

  if (estado.tipo === 'falha') {
    return (
      <p className="aviso aviso-falha" role="alert">
        Não foi possível carregar a agenda. Feche e abra de novo para tentar outra vez.
      </p>
    )
  }

  return (
    <ul className="agenda-lista">
      {estado.agenda.map((professor) => (
        <CartaoAgenda key={professor.id} professor={professor} />
      ))}
    </ul>
  )
}

function CartaoAgenda({ professor }: { professor: AgendaProfessor }) {
  return (
    <li className="professor">
      <div className="professor-cabecalho">
        <h3 className="professor-nome">{professor.nome}</h3>
        <span className="professor-aulas">{professor.idiomas.join(', ')}</span>
      </div>
      {professor.agendaBloqueada && <p className="agenda-bloqueada">Agenda bloqueada: não recebe novos alunos</p>}
      <table className="agenda-tabela">
        <thead>
          <tr>
            <th scope="col">Dia</th>
            <th scope="col">Livre</th>
            <th scope="col">Ocupado</th>
          </tr>
        </thead>
        <tbody>
          {professor.dias.map((dia) => (
            <tr key={dia.dia}>
              <th scope="row">{NomesDosDias[dia.dia] ?? dia.dia}</th>
              <td data-rotulo="Livre">
                <Periodos periodos={dia.disponivel} />
              </td>
              <td data-rotulo="Ocupado">
                <Periodos periodos={dia.ocupado} />
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </li>
  )
}
