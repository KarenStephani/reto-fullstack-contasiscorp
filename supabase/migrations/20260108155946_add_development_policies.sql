/*
  # Políticas de desarrollo para testing sin autenticación

  1. Cambios
    - Agregar políticas para permitir acceso anónimo en desarrollo
    - Hacer opcional el campo usuario_creacion para desarrollo
    - Permitir operaciones CRUD sin autenticación

  2. Notas
    - IMPORTANTE: Estas políticas deben ser removidas en producción
    - Solo para facilitar desarrollo y testing del frontend
*/

-- Hacer opcional el campo usuario_creacion temporalmente
ALTER TABLE comprobantes ALTER COLUMN usuario_creacion DROP NOT NULL;

-- Agregar políticas para acceso anónimo (desarrollo)
CREATE POLICY "Permitir SELECT anónimo en desarrollo"
  ON comprobantes FOR SELECT
  TO anon
  USING (true);

CREATE POLICY "Permitir INSERT anónimo en desarrollo"
  ON comprobantes FOR INSERT
  TO anon
  WITH CHECK (true);

CREATE POLICY "Permitir UPDATE anónimo en desarrollo"
  ON comprobantes FOR UPDATE
  TO anon
  USING (true)
  WITH CHECK (true);

CREATE POLICY "Permitir DELETE anónimo en desarrollo"
  ON comprobantes FOR DELETE
  TO anon
  USING (true);

-- Políticas para items de comprobante (desarrollo)
CREATE POLICY "Permitir SELECT anónimo items en desarrollo"
  ON comprobante_items FOR SELECT
  TO anon
  USING (true);

CREATE POLICY "Permitir INSERT anónimo items en desarrollo"
  ON comprobante_items FOR INSERT
  TO anon
  WITH CHECK (true);

CREATE POLICY "Permitir UPDATE anónimo items en desarrollo"
  ON comprobante_items FOR UPDATE
  TO anon
  USING (true)
  WITH CHECK (true);

CREATE POLICY "Permitir DELETE anónimo items en desarrollo"
  ON comprobante_items FOR DELETE
  TO anon
  USING (true);