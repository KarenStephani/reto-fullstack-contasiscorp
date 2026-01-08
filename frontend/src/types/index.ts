export interface ComprobanteItem {
  codigoProducto?: string
  descripcion: string
  cantidad: number
  precioUnitario: number
  unidadMedida?: string
  subtotal?: number
}

export interface CreateComprobanteDto {
  tipo: number
  serie: string
  numero: string
  fechaEmision: string
  rucEmisor: string
  razonSocialEmisor: string
  rucReceptor?: string
  razonSocialReceptor?: string
  moneda?: string
  observaciones?: string
  items: ComprobanteItem[]
}

export interface ComprobanteResponse {
  id: string
  tipo: string | number
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
  estado: string | number
  items: ComprobanteItem[]
}

export interface ComprobanteListItem {
  id: string
  tipo: string | number
  serie: string
  numero: number
  fechaEmision: string
  rucReceptor: string
  total: number
  estado: string | number
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
  serie?: string
}
