import { MotivoNaoEnsinaIdioma, type BuscaResposta, type EstadoTela } from '../tipos/busca'
import { ListaNaoElegiveis } from './ListaNaoElegiveis'
import { ListaProfessores } from './ListaProfessores'

type Props = {
  estado: EstadoTela
}

// A API informa o fato; a tela escolhe como explicar (ADR-012)
function mensagemSemProfessores(resposta: BuscaResposta, idioma: string) {
  const ninguemEnsina =
    resposta.naoElegiveis.length > 0 && resposta.naoElegiveis.every((professor) => professor.motivo === MotivoNaoEnsinaIdioma)
  if (ninguemEnsina) return `Nenhum professor ensina ${idioma} no momento.`

  return 'Nenhum professor disponível nesses dias e horários.'
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

  const { resposta, idioma } = estado

  return (
    <div className="resultado" role="status">
      {resposta.professores.length === 0 ? (
        <p className="aviso aviso-vazio">{mensagemSemProfessores(resposta, idioma)}</p>
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
