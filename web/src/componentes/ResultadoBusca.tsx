import { MotivoNaoEnsinaIdioma, type BuscaResposta, type EstadoTela } from '../tipos/busca'
import { ListaNaoElegiveis } from './ListaNaoElegiveis'
import { ListaProfessores } from './ListaProfessores'

type Props = {
  estado: EstadoTela
}

// A API informa o fato; a tela escolhe como explicar (ADR-012)
function mensagemSemProfessores(resposta: BuscaResposta, idioma: string) {
  if (ninguemEnsinaOIdioma(resposta)) return `Nenhum professor ensina ${idioma} no momento.`

  return 'Nenhum professor disponível nesses dias e horários.'
}

// Sem professor por falta de horário, a dica explica a regra do horário fixo (ADR-024)
function dicaSemProfessores(resposta: BuscaResposta, quantidadeDias: number) {
  if (ninguemEnsinaOIdioma(resposta)) return null
  if (quantidadeDias > 1) return 'O mesmo horário precisa estar livre em todos os dias marcados. Tente marcar menos dias ou ampliar a faixa.'

  return 'Tente ampliar a faixa de horário ou escolher outro dia.'
}

function ninguemEnsinaOIdioma(resposta: BuscaResposta) {
  return resposta.naoElegiveis.length > 0 && resposta.naoElegiveis.every((professor) => professor.motivo === MotivoNaoEnsinaIdioma)
}

export function ResultadoBusca({ estado }: Props) {
  if (estado.tipo === 'inicial') {
    return <p className="aviso">Escolha o idioma, os dias e a faixa de horário, e clique em buscar.</p>
  }

  if (estado.tipo === 'carregando') {
    return (
      <p className="aviso aviso-carregando" role="status">
        Buscando professores…
      </p>
    )
  }

  if (estado.tipo === 'falha') {
    return (
      <p className="aviso aviso-falha" role="alert">
        Não foi possível falar com o servidor. Tente de novo.
      </p>
    )
  }

  if (estado.tipo === 'validacao') {
    return (
      <div className="aviso aviso-erro" role="alert">
        <p className="aviso-titulo">Corrija a busca:</p>
        <ul className="erros">
          {estado.erros.map((erro) => (
            <li key={erro}>{erro}</li>
          ))}
        </ul>
      </div>
    )
  }

  const { resposta, idioma, quantidadeDias } = estado
  const dica = dicaSemProfessores(resposta, quantidadeDias)

  return (
    <div className="resultado" role="status">
      {resposta.professores.length === 0 ? (
        <div className="aviso aviso-vazio">
          <p className="aviso-titulo">{mensagemSemProfessores(resposta, idioma)}</p>
          {dica && <p className="aviso-dica">{dica}</p>}
        </div>
      ) : (
        <section aria-labelledby="titulo-disponiveis">
          <h2 id="titulo-disponiveis" className="secao-titulo">
            Professores disponíveis
          </h2>
          <ListaProfessores professores={resposta.professores} />
        </section>
      )}

      {resposta.naoElegiveis.length > 0 && <ListaNaoElegiveis professores={resposta.naoElegiveis} />}
    </div>
  )
}
