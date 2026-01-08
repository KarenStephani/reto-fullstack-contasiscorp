import type {
  CreateComprobanteDto,
  ComprobanteResponse,
  ComprobanteListItem,
  PaginatedResponse,
  ComprobanteFilters
} from '../types'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

export const comprobanteService = {
  async getAll(filters: ComprobanteFilters = {}): Promise<PaginatedResponse<ComprobanteListItem>> {
    const params = new URLSearchParams()
    if (filters.tipo) params.append('tipo', filters.tipo)
    if (filters.estado) params.append('estado', filters.estado)
    if (filters.rucReceptor) params.append('rucReceptor', filters.rucReceptor)
    if (filters.fechaDesde) params.append('fechaInicio', filters.fechaDesde)
    if (filters.fechaHasta) params.append('fechaFin', filters.fechaHasta)
    if (filters.serie) params.append('serie', filters.serie)

    const response = await fetch(`${API_URL}/api/comprobantes?${params}`)
    if (!response.ok) throw new Error('Error al obtener comprobantes')

    const data = await response.json()

    const items: ComprobanteListItem[] = (data || []).map((item: any) => ({
      id: item.id,
      tipo: item.tipo,
      serie: item.serie,
      numero: item.numero,
      fechaEmision: item.fechaEmision,
      rucReceptor: item.rucReceptor,
      total: item.montoTotal,
      estado: item.estado
    }))

    return {
      items: items,
      total: items.length,
      page: filters.page || 1,
      pageSize: filters.pageSize || 10,
      totalPages: Math.ceil(items.length / (filters.pageSize || 10))
    }
  },

  async getById(id: string): Promise<ComprobanteResponse> {
    const response = await fetch(`${API_URL}/api/comprobantes/${id}`)
    if (!response.ok) throw new Error('Error al obtener comprobante')

    const data = await response.json()

    return {
      id: data.id,
      tipo: data.tipo,
      serie: data.serie,
      numero: data.numero,
      fechaEmision: data.fechaEmision,
      rucEmisor: data.rucEmisor,
      razonSocialEmisor: data.razonSocialEmisor,
      rucReceptor: data.rucReceptor,
      razonSocialReceptor: data.razonSocialReceptor,
      subTotal: data.montoSubtotal,
      igv: data.montoIGV,
      total: data.montoTotal,
      estado: data.estado,
      items: data.items.map((item: any) => ({
        descripcion: item.descripcion,
        cantidad: item.cantidad,
        precioUnitario: item.precioUnitario,
        subtotal: item.subTotal
      }))
    }
  },

  async create(data: CreateComprobanteDto): Promise<ComprobanteResponse> {
    const response = await fetch(`${API_URL}/api/comprobantes`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        serie: data.serie,
        numero: data.numero,
        tipo: data.tipo,
        fechaEmision: data.fechaEmision,
        rucEmisor: data.rucEmisor,
        razonSocialEmisor: data.razonSocialEmisor,
        rucReceptor: data.rucReceptor || null,
        razonSocialReceptor: data.razonSocialReceptor || null,
        moneda: data.moneda || 'PEN',
        observaciones: data.observaciones || null,
        items: data.items.map(item => ({
          codigoProducto: item.codigoProducto || 'PROD001',
          descripcion: item.descripcion,
          cantidad: item.cantidad,
          precioUnitario: item.precioUnitario,
          unidadMedida: item.unidadMedida || 'NIU'
        }))
      })
    })

    if (!response.ok) {
      const error = await response.text()
      throw new Error(`Error al crear comprobante: ${error}`)
    }

    const result = await response.json()
    return this.getById(result.id)
  },

  async anular(id: string): Promise<ComprobanteResponse> {
    const response = await fetch(`${API_URL}/api/comprobantes/${id}/anular`, {
      method: 'PUT'
    })

    if (!response.ok) throw new Error('Error al anular comprobante')

    return this.getById(id)
  }
}
