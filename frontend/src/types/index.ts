export interface ComprobanteItem {
  descripcion: string
  cantidad: number
  precioUnitario: number
  subtotal?: number
}

export interface CreateComprobanteDto {
  tipo: string
  serie: string
  rucEmisor: string
  razonSocialEmisor: string
  rucReceptor?: string
  razonSocialReceptor?: string
  items: ComprobanteItem[]
}

export interface ComprobanteResponse {
  id: string
  tipo: string
  serie: string
  numero: number
  fechaEmision: string
  rucEmisor: string
  razonSocialEmisor: string
  rucReceptor?: string
  razonSocialReceptor?: string
  subTotal: number
  igv: number
  total: number
  estado: string
  items: ComprobanteItem[]
}

export interface ComprobanteListItem {
  id: string
  tipo: string
  serie: string
  numero: number
  fechaEmision: string
  rucReceptor: string
  total: number
  estado: string
}

export interface PaginatedResponse<T> {
  items: T[]
  total: number
  page: number
  pageSize: number
  totalPages: number
}

export interface ComprobanteFilters {
  page?: number
  pageSize?: number
  fechaDesde?: string
  fechaHasta?: string
  tipo?: string
  rucReceptor?: string
  estado?: string
}
