import { useState } from 'react'
import { comprobanteService } from '../services/api'
import type { CreateComprobanteDto, ComprobanteItem } from '../types'
import { TipoComprobante } from '../constants/enums'

interface Props {
  onSuccess: () => void
}

export default function ComprobanteForm({ onSuccess }: Props) {
  const [tipo, setTipo] = useState<'Factura' | 'Boleta'>('Factura')
  const [serie, setSerie] = useState('F001')
  const [numero, setNumero] = useState('00000001')
  const [rucEmisor, setRucEmisor] = useState('')
  const [razonSocialEmisor, setRazonSocialEmisor] = useState('')
  const [rucReceptor, setRucReceptor] = useState('')
  const [razonSocialReceptor, setRazonSocialReceptor] = useState('')
  const [items, setItems] = useState<ComprobanteItem[]>([
    { codigoProducto: 'PROD001', descripcion: '', cantidad: 1, precioUnitario: 0, unidadMedida: 'NIU' }
  ])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState(false)

  const handleTipoChange = (newTipo: 'Factura' | 'Boleta') => {
    setTipo(newTipo)
    setSerie(newTipo === 'Factura' ? 'F001' : 'B001')
  }

  const addItem = () => {
    setItems([...items, { codigoProducto: 'PROD001', descripcion: '', cantidad: 1, precioUnitario: 0, unidadMedida: 'NIU' }])
  }

  const removeItem = (index: number) => {
    if (items.length > 1) {
      setItems(items.filter((_, i) => i !== index))
    }
  }

  const updateItem = (index: number, field: keyof ComprobanteItem, value: string | number) => {
    const newItems = [...items]
    newItems[index] = { ...newItems[index], [field]: value }
    setItems(newItems)
  }

  const calculateSubtotal = () => {
    return items.reduce((sum, item) => sum + (item.cantidad * item.precioUnitario), 0)
  }

  const calculateIGV = () => {
    return calculateSubtotal() * 0.18
  }

  const calculateTotal = () => {
    return calculateSubtotal() + calculateIGV()
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError(null)
    setSuccess(false)
    setLoading(true)

    const data: CreateComprobanteDto = {
      tipo: tipo === 'Factura' ? TipoComprobante.Factura : TipoComprobante.Boleta,
      serie,
      numero,
      fechaEmision: new Date().toISOString(),
      rucEmisor,
      razonSocialEmisor,
      moneda: 'PEN',
      items: items.map(item => ({
        ...item,
        codigoProducto: item.codigoProducto || 'PROD001',
        unidadMedida: item.unidadMedida || 'NIU'
      }))
    }

    if (tipo === 'Factura') {
      data.rucReceptor = rucReceptor
      data.razonSocialReceptor = razonSocialReceptor
    }

    try {
      await comprobanteService.create(data)
      setSuccess(true)
      setTimeout(() => onSuccess(), 1500)
    } catch (err: any) {
      setError(err.response?.data?.message || err.response?.data?.errors?.[0]?.message || 'Error al crear el comprobante')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="card">
      <h2>Nuevo Comprobante</h2>

      {error && <div className="error">{error}</div>}
      {success && <div className="success">Comprobante creado exitosamente</div>}

      <form onSubmit={handleSubmit}>
        <div className="form-grid">
          <div className="form-group">
            <label>Tipo de Comprobante</label>
            <select
              value={tipo}
              onChange={(e) => handleTipoChange(e.target.value as 'Factura' | 'Boleta')}
              required
            >
              <option value="Factura">Factura</option>
              <option value="Boleta">Boleta</option>
            </select>
          </div>

          <div className="form-group">
            <label>Serie</label>
            <input
              type="text"
              value={serie}
              onChange={(e) => setSerie(e.target.value)}
              placeholder="F001"
              required
            />
          </div>

          <div className="form-group">
            <label>Número</label>
            <input
              type="text"
              value={numero}
              onChange={(e) => setNumero(e.target.value)}
              placeholder="00000001"
              required
            />
          </div>
        </div>

        <div className="form-grid">
          <div className="form-group">
            <label>RUC Emisor</label>
            <input
              type="text"
              value={rucEmisor}
              onChange={(e) => setRucEmisor(e.target.value)}
              placeholder="20123456789"
              maxLength={11}
              required
            />
          </div>

          <div className="form-group">
            <label>Razón Social Emisor</label>
            <input
              type="text"
              value={razonSocialEmisor}
              onChange={(e) => setRazonSocialEmisor(e.target.value)}
              placeholder="Empresa SAC"
              required
            />
          </div>
        </div>

        {tipo === 'Factura' && (
          <div className="form-grid">
            <div className="form-group">
              <label>RUC Receptor</label>
              <input
                type="text"
                value={rucReceptor}
                onChange={(e) => setRucReceptor(e.target.value)}
                placeholder="20987654321"
                maxLength={11}
                required={tipo === 'Factura'}
              />
            </div>

            <div className="form-group">
              <label>Razón Social Receptor</label>
              <input
                type="text"
                value={razonSocialReceptor}
                onChange={(e) => setRazonSocialReceptor(e.target.value)}
                placeholder="Cliente SAC"
                required={tipo === 'Factura'}
              />
            </div>
          </div>
        )}

        <div className="items-section">
          <h3>Items del Comprobante</h3>

          {items.map((item, index) => (
            <div key={index} className="item-row">
              <div className="form-group">
                <input
                  type="text"
                  value={item.descripcion}
                  onChange={(e) => updateItem(index, 'descripcion', e.target.value)}
                  placeholder="Descripción del producto/servicio"
                  required
                />
              </div>

              <div className="form-group">
                <input
                  type="number"
                  value={item.cantidad}
                  onChange={(e) => updateItem(index, 'cantidad', parseFloat(e.target.value) || 0)}
                  placeholder="Cantidad"
                  step="0.01"
                  min="0.01"
                  required
                />
              </div>

              <div className="form-group">
                <input
                  type="number"
                  value={item.precioUnitario}
                  onChange={(e) => updateItem(index, 'precioUnitario', parseFloat(e.target.value) || 0)}
                  placeholder="Precio Unit."
                  step="0.01"
                  min="0"
                  required
                />
              </div>

              <div className="form-group">
                <input
                  type="text"
                  value={`S/ ${(item.cantidad * item.precioUnitario).toFixed(2)}`}
                  disabled
                />
              </div>

              <button
                type="button"
                className="button button-danger"
                onClick={() => removeItem(index)}
                disabled={items.length === 1}
              >
                Eliminar
              </button>
            </div>
          ))}

          <button type="button" className="button button-success" onClick={addItem}>
            + Agregar Item
          </button>
        </div>

        <div className="summary">
          <div className="summary-item">
            <label>Subtotal</label>
            <span>S/ {calculateSubtotal().toFixed(2)}</span>
          </div>
          <div className="summary-item">
            <label>IGV (18%)</label>
            <span>S/ {calculateIGV().toFixed(2)}</span>
          </div>
          <div className="summary-item total">
            <label>Total</label>
            <span>S/ {calculateTotal().toFixed(2)}</span>
          </div>
        </div>

        <div className="button-group">
          <button type="submit" className="button button-primary" disabled={loading}>
            {loading ? 'Creando...' : 'Crear Comprobante'}
          </button>
        </div>
      </form>
    </div>
  )
}
