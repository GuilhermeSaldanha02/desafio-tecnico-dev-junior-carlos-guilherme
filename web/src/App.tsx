import { useState } from 'react'
import { buscarProfessores } from './api/buscarProfessores'
import { FormularioBusca } from './componentes/FormularioBusca'
import { ResultadoBusca } from './componentes/ResultadoBusca'
import type { BuscaRequisicao, EstadoTela } from './tipos/busca'

function App() {
  const [estado, setEstado] = useState<EstadoTela>({ tipo: 'inicial' })

  async function buscar(requisicao: BuscaRequisicao) {
    setEstado({ tipo: 'carregando' })

    const resultado = await buscarProfessores(requisicao)
    if (resultado.tipo !== 'sucesso') {
      setEstado(resultado)
      return
    }

    setEstado({ tipo: 'sucesso', resposta: resultado.resposta, idioma: requisicao.idioma })
  }

  return (
    <div className="pagina">
      <header className="cabecalho">
        <h1 className="titulo">Encontre um Professor</h1>
        <p className="subtitulo">Professores com o mesmo horário livre em todos os dias que você escolher.</p>
      </header>

      <main className="conteudo">
        <section className="painel" aria-label="Busca">
          <FormularioBusca carregando={estado.tipo === 'carregando'} onBuscar={buscar} />
        </section>
        <section className="painel painel-resultado" aria-label="Resultado" aria-live="polite">
          <ResultadoBusca estado={estado} />
        </section>
      </main>
    </div>
  )
}

export default App
