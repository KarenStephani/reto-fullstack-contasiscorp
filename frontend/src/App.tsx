import { useState } from 'react'
import ComprobanteForm from './components/ComprobanteForm'
import ComprobanteList from './components/ComprobanteList'

function App() {
  const [activeTab, setActiveTab] = useState<'list' | 'create'>('list')
  const [refreshTrigger, setRefreshTrigger] = useState(0)

  const handleComprobanteCreated = () => {
    setRefreshTrigger(prev => prev + 1)
    setActiveTab('list')
  }

  return (
    <>
      <div className="header">
        <div className="container">
          <h1>Contasiscorp</h1>
          <p>Sistema de Gestión de Comprobantes Electrónicos</p>
        </div>
      </div>

      <div className="container">
        <div className="tabs">
          <button
            className={`tab ${activeTab === 'list' ? 'active' : ''}`}
            onClick={() => setActiveTab('list')}
          >
            Lista de Comprobantes
          </button>
          <button
            className={`tab ${activeTab === 'create' ? 'active' : ''}`}
            onClick={() => setActiveTab('create')}
          >
            Nuevo Comprobante
          </button>
        </div>

        {activeTab === 'list' && (
          <ComprobanteList key={refreshTrigger} />
        )}

        {activeTab === 'create' && (
          <ComprobanteForm onSuccess={handleComprobanteCreated} />
        )}
      </div>
    </>
  )
}

export default App
