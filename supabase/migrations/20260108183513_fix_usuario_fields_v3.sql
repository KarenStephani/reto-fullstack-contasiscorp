/*
  # Fix usuario fields to allow text values
  
  1. Changes
    - Drop all existing RLS policies from both tables
    - Drop foreign key constraints
    - Change usuario fields from UUID to TEXT
    - Recreate simple policies for development
    
  2. Security
    - Temporarily remove all policies to allow column type changes
    - Add back permissive policies for development
*/

-- Drop all policies from comprobantes table
DROP POLICY IF EXISTS "Usuarios pueden ver sus propios comprobantes" ON comprobantes;
DROP POLICY IF EXISTS "Usuarios pueden crear sus propios comprobantes" ON comprobantes;
DROP POLICY IF EXISTS "Usuarios pueden actualizar sus propios comprobantes" ON comprobantes;
DROP POLICY IF EXISTS "Usuarios pueden eliminar sus propios comprobantes" ON comprobantes;
DROP POLICY IF EXISTS "Permitir SELECT anónimo en desarrollo" ON comprobantes;
DROP POLICY IF EXISTS "Permitir INSERT anónimo en desarrollo" ON comprobantes;
DROP POLICY IF EXISTS "Permitir UPDATE anónimo en desarrollo" ON comprobantes;
DROP POLICY IF EXISTS "Permitir DELETE anónimo en desarrollo" ON comprobantes;

-- Drop all policies from comprobante_items table
DROP POLICY IF EXISTS "Usuarios pueden ver items de sus comprobantes" ON comprobante_items;
DROP POLICY IF EXISTS "Usuarios pueden crear items en sus comprobantes" ON comprobante_items;
DROP POLICY IF EXISTS "Usuarios pueden actualizar items de sus comprobantes" ON comprobante_items;
DROP POLICY IF EXISTS "Usuarios pueden eliminar items de sus comprobantes" ON comprobante_items;
DROP POLICY IF EXISTS "Permitir SELECT anónimo items en desarrollo" ON comprobante_items;
DROP POLICY IF EXISTS "Permitir INSERT anónimo items en desarrollo" ON comprobante_items;
DROP POLICY IF EXISTS "Permitir UPDATE anónimo items en desarrollo" ON comprobante_items;
DROP POLICY IF EXISTS "Permitir DELETE anónimo items en desarrollo" ON comprobante_items;

-- Drop foreign key constraints
ALTER TABLE comprobantes 
  DROP CONSTRAINT IF EXISTS comprobantes_usuario_creacion_fkey,
  DROP CONSTRAINT IF EXISTS comprobantes_usuario_modificacion_fkey;

-- Change column types from UUID to TEXT
ALTER TABLE comprobantes 
  ALTER COLUMN usuario_creacion TYPE TEXT USING usuario_creacion::TEXT,
  ALTER COLUMN usuario_modificacion TYPE TEXT USING usuario_modificacion::TEXT;

-- Create permissive policies for development (allows anonymous access)
CREATE POLICY "Allow all for development"
  ON comprobantes
  FOR ALL
  USING (true)
  WITH CHECK (true);

CREATE POLICY "Allow all items for development"
  ON comprobante_items
  FOR ALL
  USING (true)
  WITH CHECK (true);