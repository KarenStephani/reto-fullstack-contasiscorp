export enum TipoComprobante {
  Factura = 1,
  Boleta = 2,
  NotaCredito = 3,
  NotaDebito = 4,
  Recibo = 5,
  GuiaRemision = 6
}

export enum EstadoComprobante {
  Borrador = 1,
  Emitido = 2,
  Anulado = 3,
  Rechazado = 4
}

export const getTipoComprobanteText = (tipo: number | string): string => {
  if (typeof tipo === 'number') {
    switch (tipo) {
      case TipoComprobante.Factura:
        return 'Factura'
      case TipoComprobante.Boleta:
        return 'Boleta'
      case TipoComprobante.NotaCredito:
        return 'NotaCredito'
      case TipoComprobante.NotaDebito:
        return 'NotaDebito'
      case TipoComprobante.Recibo:
        return 'Recibo'
      case TipoComprobante.GuiaRemision:
        return 'GuiaRemision'
      default:
        return 'Desconocido'
    }
  }
  return tipo || 'Desconocido'
}

export const getEstadoComprobanteText = (estado: number | string): string => {
  if (typeof estado === 'number') {
    switch (estado) {
      case EstadoComprobante.Borrador:
        return 'Borrador'
      case EstadoComprobante.Emitido:
        return 'Emitido'
      case EstadoComprobante.Anulado:
        return 'Anulado'
      case EstadoComprobante.Rechazado:
        return 'Rechazado'
      default:
        return 'Desconocido'
    }
  }
  return estado || 'Desconocido'
}
