import { useState, type FormEvent } from 'react'
import type { BuscaRequisicao } from '../tipos/busca'

const Idiomas = ['Inglês', 'Espanhol', 'Francês']

const DiasDaSemana = [
  { valor: 'segunda', rotulo: 'Segunda' },
  { valor: 'terca', rotulo: 'Terça' },
  { valor: 'quarta', rotulo: 'Quarta' },
  { valor: 'quinta', rotulo: 'Quinta' },
  { valor: 'sexta', rotulo: 'Sexta' },
  { valor: 'sabado', rotulo: 'Sábado' },
  { valor: 'domingo', rotulo: 'Domingo' },
]

const Duracoes = [30, 60, 90]

type Props = {
  carregando: boolean
  onBuscar: (requisicao: BuscaRequisicao) => void
}

// Os controles já evitam a maior parte dos erros, mas o envio não é bloqueado: quem valida é a API (ADR-014)
export function FormularioBusca({ carregando, onBuscar }: Props) {
  const [idioma, setIdioma] = useState('Espanhol')
  const [dias, setDias] = useState<string[]>(['segunda'])
  const [inicioFaixa, setInicioFaixa] = useState('18:00')
  const [fimFaixa, setFimFaixa] = useState('20:00')
  const [duracaoMinutos, setDuracaoMinutos] = useState(60)

  function alternarDia(valor: string) {
    // mantém os dias na ordem da semana, não na ordem do clique
    setDias((atuais) =>
      DiasDaSemana.map((dia) => dia.valor).filter((dia) => (dia === valor ? !atuais.includes(dia) : atuais.includes(dia))),
    )
  }

  function enviar(evento: FormEvent<HTMLFormElement>) {
    evento.preventDefault()
    onBuscar({ idioma, dias, inicioFaixa, fimFaixa, duracaoMinutos })
  }

  return (
    <form className="formulario" onSubmit={enviar} noValidate>
      <label className="campo">
        <span className="campo-rotulo">Idioma</span>
        <select value={idioma} onChange={(evento) => setIdioma(evento.target.value)}>
          {Idiomas.map((opcao) => (
            <option key={opcao} value={opcao}>
              {opcao}
            </option>
          ))}
        </select>
      </label>

      <fieldset className="campo">
        <legend className="campo-rotulo">Dias da semana</legend>
        <div className="dias">
          {DiasDaSemana.map((dia) => (
            <label key={dia.valor} className="dia">
              <input type="checkbox" checked={dias.includes(dia.valor)} onChange={() => alternarDia(dia.valor)} />
              <span>{dia.rotulo}</span>
            </label>
          ))}
        </div>
      </fieldset>

      <div className="faixa">
        <label className="campo">
          <span className="campo-rotulo">Das</span>
          <input type="time" value={inicioFaixa} onChange={(evento) => setInicioFaixa(evento.target.value)} />
        </label>
        <label className="campo">
          <span className="campo-rotulo">Até</span>
          <input type="time" value={fimFaixa} onChange={(evento) => setFimFaixa(evento.target.value)} />
        </label>
      </div>

      <label className="campo">
        <span className="campo-rotulo">Duração da aula</span>
        <select value={duracaoMinutos} onChange={(evento) => setDuracaoMinutos(Number(evento.target.value))}>
          {Duracoes.map((duracao) => (
            <option key={duracao} value={duracao}>
              {duracao} minutos
            </option>
          ))}
        </select>
      </label>

      <button type="submit" className="botao" disabled={carregando}>
        {carregando ? 'Buscando…' : 'Buscar professores'}
      </button>
    </form>
  )
}
