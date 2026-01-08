-- Conectarse a la base de datos ContasiscorpBD primero
-- \c ContasiscorpBD

-- Crear extensión para UUIDs
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Eliminar tablas si existen (para desarrollo)
DROP TABLE IF EXISTS comprobante_items CASCADE;
DROP TABLE IF EXISTS comprobantes CASCADE;
DROP TABLE IF EXISTS usuarios CASCADE;

-- Crear tabla de usuarios (básica para relaciones)
CREATE TABLE usuarios (
  id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  email text UNIQUE NOT NULL,
  password_hash text NOT NULL,
  nombre text NOT NULL,
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz
);

-- Crear tabla de comprobantes (usuario_creacion es opcional)
CREATE TABLE comprobantes (
  id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  serie text NOT NULL,
  numero text NOT NULL,
  tipo int NOT NULL CHECK (tipo BETWEEN 1 AND 6),
  estado int NOT NULL DEFAULT 1 CHECK (estado BETWEEN 1 AND 4),
  fecha_emision timestamptz NOT NULL DEFAULT now(),
  ruc_emisor text NOT NULL CHECK (length(ruc_emisor) = 11),
  razon_social_emisor text NOT NULL,
  ruc_receptor text NOT NULL CHECK (length(ruc_receptor) = 11),
  razon_social_receptor text NOT NULL,
  monto_total decimal(18,2) NOT NULL DEFAULT 0,
  monto_igv decimal(18,2) NOT NULL DEFAULT 0,
  monto_subtotal decimal(18,2) NOT NULL DEFAULT 0,
  moneda text NOT NULL DEFAULT 'PEN',
  observaciones text,
  fecha_creacion timestamptz NOT NULL DEFAULT now(),
  fecha_modificacion timestamptz,
  usuario_creacion uuid REFERENCES usuarios(id),
  usuario_modificacion uuid REFERENCES usuarios(id),
  UNIQUE(serie, numero)
);

-- Crear tabla de items de comprobante
CREATE TABLE comprobante_items (
  id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  comprobante_id uuid NOT NULL REFERENCES comprobantes(id) ON DELETE CASCADE,
  codigo_producto text NOT NULL,
  descripcion text NOT NULL,
  cantidad decimal(18,4) NOT NULL CHECK (cantidad > 0),
  precio_unitario decimal(18,2) NOT NULL CHECK (precio_unitario >= 0),
  sub_total decimal(18,2) NOT NULL DEFAULT 0,
  unidad_medida text NOT NULL DEFAULT 'NIU'
);

-- Crear índices para optimizar consultas
CREATE INDEX idx_comprobantes_usuario_creacion ON comprobantes(usuario_creacion);
CREATE INDEX idx_comprobantes_serie_numero ON comprobantes(serie, numero);
CREATE INDEX idx_comprobantes_fecha_emision ON comprobantes(fecha_emision);
CREATE INDEX idx_comprobantes_tipo_estado ON comprobantes(tipo, estado);
CREATE INDEX idx_comprobante_items_comprobante_id ON comprobante_items(comprobante_id);

-- Comentarios descriptivos
COMMENT ON TABLE comprobantes IS 'Tabla principal de comprobantes electrónicos';
COMMENT ON TABLE comprobante_items IS 'Detalle de items/líneas de cada comprobante';
COMMENT ON TABLE usuarios IS 'Tabla de usuarios del sistema';

COMMENT ON COLUMN comprobantes.tipo IS '1=Factura, 2=Boleta, 3=NotaCredito, 4=NotaDebito, 5=Recibo, 6=GuiaRemision';
COMMENT ON COLUMN comprobantes.estado IS '1=Borrador, 2=Vigente/Emitido, 3=Anulado, 4=Rechazado';
COMMENT ON COLUMN comprobantes.moneda IS 'Código ISO de moneda (PEN=Soles, USD=Dólares)';
COMMENT ON COLUMN comprobante_items.unidad_medida IS 'NIU=Unidad, ZZ=Servicio, etc (Catálogo SUNAT)';
