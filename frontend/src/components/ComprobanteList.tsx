import { useState, useEffect } from 'react'
import { comprobanteService } from '../services/api'
import type { ComprobanteListItem, ComprobanteFilters } from '../types'

export default function ComprobanteList() {
  const [comprobantes, setComprobantes] = useState<ComprobanteListItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [total, setTotal] = useState(0)

  const [filters, setFilters] = useState<ComprobanteFilters>({
    pageSize: 10
  })

  const fetchComprobantes = async () => {
    setLoading(true)
    setError(null)

    try {
      const response = await comprobanteService.getAll({ ...filters, page })
      setComprobantes(response.items)
      setTotalPages(response.totalPages)
      setTotal(response.total)
    } catch (err: any) {
      setError('Error al cargar los comprobantes')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchComprobantes()
  }, [page, filters])

  const handleAnular = async (id: string) => {
    if (!confirm('¿Está seguro de anular este comprobante?')) return

    try {
      await comprobanteService.anular(id)
      fetchComprobantes()
    } catch (err: any) {
      alert(err.response?.data?.message || 'Error al anular el comprobante')
    }
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('es-PE', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    })
  }

  if (loading) {
    return <div className="loading">Cargando comprobantes...</div>
  }

  return (
    <div className="card">
      <h2>Lista de Comprobantes ({total})</h2>

      {error && <div className="error">{error}</div>}

      <div className="filters">
        <div className="form-group">
          <label>Tipo</label>
          <select
            value={filters.tipo || ''}
            onChange={(e) => {
              setFilters({ ...filters, tipo: e.target.value || undefined })
              setPage(1)
            }}
          >
            <option value="">Todos</option>
            <option value="Factura">Factura</option>
            <option value="Boleta">Boleta</option>
          </select>
        </div>

        <div className="form-group">
          <label>Estado</label>
          <select
            value={filters.estado || ''}
            onChange={(e) => {
              setFilters({ ...filters, estado: e.target.value || undefined })
              setPage(1)
            }}
          >
            <option value="">Todos</option>
            <option value="Emitido">Emitido</option>
            <option value="Anulado">Anulado</option>
          </select>
        </div>

        <div className="form-group">
          <label>RUC Receptor</label>
          <input
            type="text"
            value={filters.rucReceptor || ''}
            onChange={(e) => {
              setFilters({ ...filters, rucReceptor: e.target.value || undefined })
              setPage(1)
            }}
            placeholder="20123456789"
          />
        </div>
      </div>

      {comprobantes.length === 0 ? (
        <div className="empty-state">
          <h3>No hay comprobantes</h3>
          <p>Crea tu primer comprobante usando el formulario</p>
        </div>
      ) : (
        <>
          <table className="table">
            <thead>
              <tr>
                <th>Tipo</th>
                <th>Serie-Número</th>
                <th>Fecha Emisión</th>
                <th>RUC Receptor</th>
                <th>Total</th>
                <th>Estado</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {comprobantes.map((comprobante) => (
                <tr key={comprobante.id}>
                  <td>
                    <span className={`badge badge-${comprobante.tipo.toLowerCase()}`}>
                      {comprobante.tipo}
                    </span>
                  </td>
                  <td>{comprobante.serie}-{comprobante.numero}</td>
                  <td>{formatDate(comprobante.fechaEmision)}</td>
                  <td>{comprobante.rucReceptor}</td>
                  <td>S/ {comprobante.total.toFixed(2)}</td>
                  <td>
                    <span className={`badge badge-${comprobante.estado.toLowerCase()}`}>
                      {comprobante.estado}
                    </span>
                  </td>
                  <td>
                    {comprobante.estado === 'Emitido' && (
                      <button
                        className="button button-danger"
                        onClick={() => handleAnular(comprobante.id)}
                      >
                        Anular
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {totalPages > 1 && (
            <div className="pagination">
              <button
                className="button button-secondary"
                onClick={() => setPage(page - 1)}
                disabled={page === 1}
              >
                Anterior
              </button>
              <span>
                Página {page} de {totalPages}
              </span>
              <button
                className="button button-secondary"
                onClick={() => setPage(page + 1)}
                disabled={page === totalPages}
              >
                Siguiente
              </button>
            </div>
          )}
        </>
      )}
    </div>
  )
}
