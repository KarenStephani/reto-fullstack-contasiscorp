/*
  # Fix Receptor Fields to Allow NULL for Boletas

  1. Changes
    - Make ruc_receptor nullable (optional for boletas)
    - Make razon_social_receptor nullable (optional for boletas)
    - Update usuario_creacion to be NOT NULL with default value

  2. Security
    - No changes to RLS policies
*/

-- Make receptor fields nullable for boletas
ALTER TABLE comprobantes 
  ALTER COLUMN ruc_receptor DROP NOT NULL,
  ALTER COLUMN razon_social_receptor DROP NOT NULL;

-- Ensure usuario_creacion has a default
ALTER TABLE comprobantes 
  ALTER COLUMN usuario_creacion SET DEFAULT 'system';

-- Update empty strings to NULL for better data integrity
UPDATE comprobantes 
SET ruc_receptor = NULL 
WHERE ruc_receptor = '';

UPDATE comprobantes 
SET razon_social_receptor = NULL 
WHERE razon_social_receptor = '';
