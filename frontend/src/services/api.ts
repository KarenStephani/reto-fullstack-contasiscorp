import axios from 'axios'
import type {
  CreateComprobanteDto,
  ComprobanteResponse,
  ComprobanteListItem,
  PaginatedResponse,
  ComprobanteFilters
} from '../types'

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

export const comprobanteService = {
  async getAll(filters: ComprobanteFilters = {}): Promise<PaginatedResponse<ComprobanteListItem>> {
    const params = new URLSearchParams()

    if (filters.page) params.append('page', filters.page.toString())
    if (filters.pageSize) params.append('pageSize', filters.pageSize.toString())
    if (filters.fechaDesde) params.append('fechaDesde', filters.fechaDesde)
    if (filters.fechaHasta) params.append('fechaHasta', filters.fechaHasta)
    if (filters.tipo) params.append('tipo', filters.tipo)
    if (filters.rucReceptor) params.append('rucReceptor', filters.rucReceptor)
    if (filters.estado) params.append('estado', filters.estado)

    const response = await api.get<PaginatedResponse<ComprobanteListItem>>(
      `/comprobantes?${params.toString()}`
    )
    return response.data
  },

  async getById(id: string): Promise<ComprobanteResponse> {
    const response = await api.get<ComprobanteResponse>(`/comprobantes/${id}`)
    return response.data
  },

  async create(data: CreateComprobanteDto): Promise<ComprobanteResponse> {
    const response = await api.post<ComprobanteResponse>('/comprobantes', data)
    return response.data
  },

  async anular(id: string): Promise<ComprobanteResponse> {
    const response = await api.put<ComprobanteResponse>(`/comprobantes/${id}/anular`)
    return response.data
  }
}
