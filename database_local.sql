/*
  Sistema de Comprobantes Electrónicos - Contasiscorp
  Script para PostgreSQL Local

  NOTA: Este script está adaptado para PostgreSQL vanilla sin Supabase.
  Incluye una tabla básica de usuarios para las relaciones.
*/

-- Crear la base de datos ContasiscorpBD
CREATE DATABASE "ContasiscorpBD"
  WITH
  OWNER = postgres
  ENCODING = 'UTF8'
  LC_COLLATE = 'en_US.utf8'
  LC_CTYPE = 'en_US.utf8'
  TABLESPACE = pg_default
  CONNECTION LIMIT = -1;

-- Conectarse a la base de datos (ejecutar manualmente: \c ContasiscorpBD)
-- O ejecutar este script después de conectarse a ContasiscorpBD

-- Crear extensión para UUIDs
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Crear tabla de usuarios (básica para relaciones)
CREATE TABLE IF NOT EXISTS usuarios (
  id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  email text UNIQUE NOT NULL,
  password_hash text NOT NULL,
  nombre text NOT NULL,
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz
);

-- Crear tabla de comprobantes
CREATE TABLE IF NOT EXISTS comprobantes (
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
  usuario_creacion uuid NOT NULL REFERENCES usuarios(id),
  usuario_modificacion uuid REFERENCES usuarios(id),
  UNIQUE(serie, numero)
);

-- Crear tabla de items de comprobante
CREATE TABLE IF NOT EXISTS comprobante_items (
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
CREATE INDEX IF NOT EXISTS idx_comprobantes_usuario_creacion ON comprobantes(usuario_creacion);
CREATE INDEX IF NOT EXISTS idx_comprobantes_serie_numero ON comprobantes(serie, numero);
CREATE INDEX IF NOT EXISTS idx_comprobantes_fecha_emision ON comprobantes(fecha_emision);
CREATE INDEX IF NOT EXISTS idx_comprobantes_tipo_estado ON comprobantes(tipo, estado);
CREATE INDEX IF NOT EXISTS idx_comprobante_items_comprobante_id ON comprobante_items(comprobante_id);

-- Comentarios descriptivos
COMMENT ON TABLE comprobantes IS 'Tabla principal de comprobantes electrónicos';
COMMENT ON TABLE comprobante_items IS 'Detalle de items/líneas de cada comprobante';
COMMENT ON TABLE usuarios IS 'Tabla de usuarios del sistema';

COMMENT ON COLUMN comprobantes.tipo IS '1=Factura, 2=Boleta, 3=NotaCredito, 4=NotaDebito, 5=Recibo, 6=GuiaRemision';
COMMENT ON COLUMN comprobantes.estado IS '1=Borrador, 2=Vigente/Emitido, 3=Anulado, 4=Rechazado';
COMMENT ON COLUMN comprobantes.moneda IS 'Código ISO de moneda (PEN=Soles, USD=Dólares)';
COMMENT ON COLUMN comprobante_items.unidad_medida IS 'NIU=Unidad, ZZ=Servicio, etc (Catálogo SUNAT)';

-- Datos de ejemplo (opcional - descomentar si deseas datos de prueba)
/*
INSERT INTO usuarios (email, password_hash, nombre) VALUES
  ('admin@contasiscorp.com', '$2a$10$dummy_hash', 'Administrador'),
  ('usuario@contasiscorp.com', '$2a$10$dummy_hash', 'Usuario Demo');
*/
