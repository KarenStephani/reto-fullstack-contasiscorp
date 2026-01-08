/*
  # Sistema de Comprobantes Electrónicos - Contasiscorp

  ## Nuevas Tablas

  ### Tabla `comprobantes`
  - `id` (uuid, primary key) - Identificador único del comprobante
  - `serie` (text) - Serie del comprobante (ej: F001, B001)
  - `numero` (text) - Número correlativo del comprobante
  - `tipo` (int) - Tipo de comprobante (1=Factura, 2=Boleta, 3=NotaCredito, 4=NotaDebito, 5=Recibo, 6=GuiaRemision)
  - `estado` (int) - Estado del comprobante (1=Borrador, 2=Vigente/Emitido, 3=Anulado, 4=Rechazado)
  - `fecha_emision` (timestamptz) - Fecha de emisión del comprobante
  - `ruc_emisor` (text) - RUC de la empresa emisora (11 dígitos)
  - `razon_social_emisor` (text) - Razón social del emisor
  - `ruc_receptor` (text) - RUC del receptor (11 dígitos)
  - `razon_social_receptor` (text) - Razón social del receptor
  - `monto_total` (decimal) - Monto total del comprobante (incluye IGV)
  - `monto_igv` (decimal) - Monto del IGV (18%)
  - `monto_subtotal` (decimal) - Monto subtotal (sin IGV)
  - `moneda` (text) - Código de moneda (default: PEN)
  - `observaciones` (text, nullable) - Observaciones adicionales
  - `fecha_creacion` (timestamptz) - Fecha de creación del registro
  - `fecha_modificacion` (timestamptz, nullable) - Fecha de última modificación
  - `usuario_creacion` (uuid) - Usuario que creó el registro
  - `usuario_modificacion` (uuid, nullable) - Usuario que modificó el registro

  ### Tabla `comprobante_items`
  - `id` (uuid, primary key) - Identificador único del item
  - `comprobante_id` (uuid, foreign key) - Referencia al comprobante
  - `codigo_producto` (text) - Código del producto/servicio
  - `descripcion` (text) - Descripción del producto/servicio
  - `cantidad` (decimal) - Cantidad del producto/servicio
  - `precio_unitario` (decimal) - Precio unitario del producto/servicio
  - `sub_total` (decimal) - Subtotal del item (cantidad × precio_unitario)
  - `unidad_medida` (text) - Unidad de medida (default: NIU - Unidad)

  ## Índices
  - Índice en `comprobantes(usuario_creacion)` para filtrar por usuario
  - Índice en `comprobantes(serie, numero)` para búsquedas rápidas
  - Índice en `comprobantes(fecha_emision)` para filtros por fecha
  - Índice en `comprobantes(tipo, estado)` para reportes
  - Índice en `comprobante_items(comprobante_id)` para joins eficientes

  ## Seguridad (RLS)
  - RLS habilitado en ambas tablas
  - Los usuarios autenticados pueden ver solo sus propios comprobantes
  - Los usuarios autenticados pueden crear comprobantes asociados a su usuario
  - Los usuarios autenticados pueden actualizar solo sus propios comprobantes
  - Los usuarios autenticados pueden eliminar solo sus propios comprobantes
*/

-- Crear tabla de comprobantes
CREATE TABLE IF NOT EXISTS comprobantes (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
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
  usuario_creacion uuid NOT NULL REFERENCES auth.users(id),
  usuario_modificacion uuid REFERENCES auth.users(id),
  UNIQUE(serie, numero)
);

-- Crear tabla de items de comprobante
CREATE TABLE IF NOT EXISTS comprobante_items (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
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

-- Habilitar Row Level Security
ALTER TABLE comprobantes ENABLE ROW LEVEL SECURITY;
ALTER TABLE comprobante_items ENABLE ROW LEVEL SECURITY;

-- Políticas de seguridad para comprobantes
CREATE POLICY "Usuarios pueden ver sus propios comprobantes"
  ON comprobantes FOR SELECT
  TO authenticated
  USING (auth.uid() = usuario_creacion);

CREATE POLICY "Usuarios pueden crear sus propios comprobantes"
  ON comprobantes FOR INSERT
  TO authenticated
  WITH CHECK (auth.uid() = usuario_creacion);

CREATE POLICY "Usuarios pueden actualizar sus propios comprobantes"
  ON comprobantes FOR UPDATE
  TO authenticated
  USING (auth.uid() = usuario_creacion)
  WITH CHECK (auth.uid() = usuario_creacion);

CREATE POLICY "Usuarios pueden eliminar sus propios comprobantes"
  ON comprobantes FOR DELETE
  TO authenticated
  USING (auth.uid() = usuario_creacion);

-- Políticas de seguridad para items de comprobante
CREATE POLICY "Usuarios pueden ver items de sus comprobantes"
  ON comprobante_items FOR SELECT
  TO authenticated
  USING (
    EXISTS (
      SELECT 1 FROM comprobantes
      WHERE comprobantes.id = comprobante_items.comprobante_id
      AND comprobantes.usuario_creacion = auth.uid()
    )
  );

CREATE POLICY "Usuarios pueden crear items en sus comprobantes"
  ON comprobante_items FOR INSERT
  TO authenticated
  WITH CHECK (
    EXISTS (
      SELECT 1 FROM comprobantes
      WHERE comprobantes.id = comprobante_items.comprobante_id
      AND comprobantes.usuario_creacion = auth.uid()
    )
  );

CREATE POLICY "Usuarios pueden actualizar items de sus comprobantes"
  ON comprobante_items FOR UPDATE
  TO authenticated
  USING (
    EXISTS (
      SELECT 1 FROM comprobantes
      WHERE comprobantes.id = comprobante_items.comprobante_id
      AND comprobantes.usuario_creacion = auth.uid()
    )
  )
  WITH CHECK (
    EXISTS (
      SELECT 1 FROM comprobantes
      WHERE comprobantes.id = comprobante_items.comprobante_id
      AND comprobantes.usuario_creacion = auth.uid()
    )
  );

CREATE POLICY "Usuarios pueden eliminar items de sus comprobantes"
  ON comprobante_items FOR DELETE
  TO authenticated
  USING (
    EXISTS (
      SELECT 1 FROM comprobantes
      WHERE comprobantes.id = comprobante_items.comprobante_id
      AND comprobantes.usuario_creacion = auth.uid()
    )
  );