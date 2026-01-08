import { supabase } from './supabase'
import type {
  CreateComprobanteDto,
  ComprobanteResponse,
  ComprobanteListItem,
  PaginatedResponse,
  ComprobanteFilters
} from '../types'

const tipoMap: Record<string, number> = {
  'Factura': 1,
  'Boleta': 2,
  'NotaCredito': 3,
  'NotaDebito': 4,
  'Recibo': 5,
  'GuiaRemision': 6
}

const tipoReverseMap: Record<number, string> = {
  1: 'Factura',
  2: 'Boleta',
  3: 'NotaCredito',
  4: 'NotaDebito',
  5: 'Recibo',
  6: 'GuiaRemision'
}

const estadoMap: Record<string, number> = {
  'Borrador': 1,
  'Emitido': 2,
  'Vigente': 2,
  'Anulado': 3,
  'Rechazado': 4
}

const estadoReverseMap: Record<number, string> = {
  1: 'Borrador',
  2: 'Emitido',
  3: 'Anulado',
  4: 'Rechazado'
}

export const comprobanteService = {
  async getAll(filters: ComprobanteFilters = {}): Promise<PaginatedResponse<ComprobanteListItem>> {
    const page = filters.page || 1
    const pageSize = filters.pageSize || 10
    const from = (page - 1) * pageSize
    const to = from + pageSize - 1

    let query = supabase
      .from('comprobantes')
      .select('*', { count: 'exact' })
      .range(from, to)
      .order('fecha_emision', { ascending: false })

    if (filters.tipo) {
      query = query.eq('tipo', tipoMap[filters.tipo])
    }

    if (filters.estado) {
      query = query.eq('estado', estadoMap[filters.estado])
    }

    if (filters.rucReceptor) {
      query = query.eq('ruc_receptor', filters.rucReceptor)
    }

    if (filters.fechaDesde) {
      query = query.gte('fecha_emision', filters.fechaDesde)
    }

    if (filters.fechaHasta) {
      query = query.lte('fecha_emision', filters.fechaHasta)
    }

    const { data, error, count } = await query

    if (error) throw error

    const items: ComprobanteListItem[] = (data || []).map((row: any) => ({
      id: row.id,
      tipo: tipoReverseMap[row.tipo],
      serie: row.serie,
      numero: parseInt(row.numero),
      fechaEmision: row.fecha_emision,
      rucReceptor: row.ruc_receptor,
      total: parseFloat(row.monto_total),
      estado: estadoReverseMap[row.estado]
    }))

    return {
      items,
      total: count || 0,
      page,
      pageSize,
      totalPages: Math.ceil((count || 0) / pageSize)
    }
  },

  async getById(id: string): Promise<ComprobanteResponse> {
    const { data: comprobante, error: compError } = await supabase
      .from('comprobantes')
      .select('*')
      .eq('id', id)
      .single()

    if (compError) throw compError

    const { data: items, error: itemsError } = await supabase
      .from('comprobante_items')
      .select('*')
      .eq('comprobante_id', id)

    if (itemsError) throw itemsError

    return {
      id: comprobante.id,
      tipo: tipoReverseMap[comprobante.tipo],
      serie: comprobante.serie,
      numero: parseInt(comprobante.numero),
      fechaEmision: comprobante.fecha_emision,
      rucEmisor: comprobante.ruc_emisor,
      razonSocialEmisor: comprobante.razon_social_emisor,
      rucReceptor: comprobante.ruc_receptor,
      razonSocialReceptor: comprobante.razon_social_receptor,
      subTotal: parseFloat(comprobante.monto_subtotal),
      igv: parseFloat(comprobante.monto_igv),
      total: parseFloat(comprobante.monto_total),
      estado: estadoReverseMap[comprobante.estado],
      items: items.map((item: any) => ({
        descripcion: item.descripcion,
        cantidad: parseFloat(item.cantidad),
        precioUnitario: parseFloat(item.precio_unitario),
        subtotal: parseFloat(item.sub_total)
      }))
    }
  },

  async create(data: CreateComprobanteDto): Promise<ComprobanteResponse> {
    const subtotal = data.items.reduce((sum, item) => sum + (item.cantidad * item.precioUnitario), 0)
    const igv = subtotal * 0.18
    const total = subtotal + igv

    const { data: maxNumero } = await supabase
      .from('comprobantes')
      .select('numero')
      .eq('serie', data.serie)
      .order('numero', { ascending: false })
      .limit(1)
      .maybeSingle()

    const nuevoNumero = maxNumero ? (parseInt(maxNumero.numero) + 1).toString().padStart(8, '0') : '00000001'

    const { data: comprobante, error: compError } = await supabase
      .from('comprobantes')
      .insert({
        serie: data.serie,
        numero: nuevoNumero,
        tipo: tipoMap[data.tipo],
        estado: 2,
        ruc_emisor: data.rucEmisor,
        razon_social_emisor: data.razonSocialEmisor,
        ruc_receptor: data.rucReceptor || data.rucEmisor,
        razon_social_receptor: data.razonSocialReceptor || data.razonSocialEmisor,
        monto_subtotal: subtotal,
        monto_igv: igv,
        monto_total: total
      })
      .select()
      .single()

    if (compError) throw compError

    const itemsToInsert = data.items.map(item => ({
      comprobante_id: comprobante.id,
      codigo_producto: 'PROD001',
      descripcion: item.descripcion,
      cantidad: item.cantidad,
      precio_unitario: item.precioUnitario,
      sub_total: item.cantidad * item.precioUnitario
    }))

    const { error: itemsError } = await supabase
      .from('comprobante_items')
      .insert(itemsToInsert)

    if (itemsError) throw itemsError

    return this.getById(comprobante.id)
  },

  async anular(id: string): Promise<ComprobanteResponse> {
    const { error } = await supabase
      .from('comprobantes')
      .update({ estado: 3 })
      .eq('id', id)

    if (error) throw error

    return this.getById(id)
  }
}
