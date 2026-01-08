/*
  Script para corregir el tipo de datos de las columnas de usuario

  PROBLEMA: Las columnas usuario_creacion y usuario_modificacion están como UUID
  SOLUCIÓN: Cambiarlas a TEXT para permitir valores como "system", nombres de usuarios, etc.

  INSTRUCCIONES:
  1. Conéctate a tu base de datos ContasiscorpBD
  2. Ejecuta este script completo

  Usando psql:
    psql -h localhost -p 5433 -U postgres -d ContasiscorpBD -f fix_usuario_columns.sql

  O desde pgAdmin:
    - Conéctate a la base ContasiscorpBD
    - Abre Query Tool
    - Copia y pega este script
    - Ejecuta (F5)
*/

-- Comenzar transacción para poder hacer rollback si algo falla
BEGIN;

-- Verificar el tipo actual de las columnas
DO $$
DECLARE
  v_usuario_creacion_type text;
  v_usuario_modificacion_type text;
BEGIN
  -- Obtener tipo de datos actual
  SELECT data_type INTO v_usuario_creacion_type
  FROM information_schema.columns
  WHERE table_name = 'comprobantes' AND column_name = 'usuario_creacion';

  SELECT data_type INTO v_usuario_modificacion_type
  FROM information_schema.columns
  WHERE table_name = 'comprobantes' AND column_name = 'usuario_modificacion';

  RAISE NOTICE 'Tipo actual de usuario_creacion: %', v_usuario_creacion_type;
  RAISE NOTICE 'Tipo actual de usuario_modificacion: %', v_usuario_modificacion_type;
END $$;

-- Eliminar el índice si existe
DROP INDEX IF EXISTS idx_comprobantes_usuario_creacion;

-- Eliminar columnas antiguas con tipo UUID
ALTER TABLE comprobantes DROP COLUMN IF EXISTS usuario_creacion CASCADE;
ALTER TABLE comprobantes DROP COLUMN IF EXISTS usuario_modificacion CASCADE;

-- Agregar columnas nuevas con tipo TEXT
ALTER TABLE comprobantes ADD COLUMN usuario_creacion text DEFAULT 'system';
ALTER TABLE comprobantes ADD COLUMN usuario_modificacion text;

-- Crear índice para optimizar consultas
CREATE INDEX idx_comprobantes_usuario_creacion ON comprobantes(usuario_creacion);

-- Verificar que el cambio se aplicó correctamente
DO $$
DECLARE
  v_usuario_creacion_type text;
  v_usuario_modificacion_type text;
BEGIN
  SELECT data_type INTO v_usuario_creacion_type
  FROM information_schema.columns
  WHERE table_name = 'comprobantes' AND column_name = 'usuario_creacion';

  SELECT data_type INTO v_usuario_modificacion_type
  FROM information_schema.columns
  WHERE table_name = 'comprobantes' AND column_name = 'usuario_modificacion';

  RAISE NOTICE '----------------------------------------';
  RAISE NOTICE 'Nuevo tipo de usuario_creacion: %', v_usuario_creacion_type;
  RAISE NOTICE 'Nuevo tipo de usuario_modificacion: %', v_usuario_modificacion_type;
  RAISE NOTICE '----------------------------------------';

  IF v_usuario_creacion_type = 'text' AND v_usuario_modificacion_type = 'text' THEN
    RAISE NOTICE '✓ Cambio aplicado correctamente!';
  ELSE
    RAISE EXCEPTION 'ERROR: El cambio no se aplicó correctamente';
  END IF;
END $$;

-- Confirmar cambios
COMMIT;

-- Mostrar mensaje final
\echo ''
\echo '==================================='
\echo '✓ Script ejecutado exitosamente'
\echo '==================================='
\echo ''
\echo 'Las columnas usuario_creacion y usuario_modificacion ahora son de tipo TEXT'
\echo ''
\echo 'Próximos pasos:'
\echo '1. Reinicia tu backend .NET'
\echo '2. Prueba crear un comprobante'
\echo ''
