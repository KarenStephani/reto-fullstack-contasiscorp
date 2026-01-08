#!/bin/bash
# Script para configurar la base de datos local
# Usuario: postgres
# Contraseña: root
# Puerto: 5433

echo "Configurando base de datos local ContasiscorpBD..."

# Verificar conexión
export PGPASSWORD=root
psql -h localhost -p 5433 -U postgres -d postgres -c "SELECT version();"

if [ $? -ne 0 ]; then
    echo "Error: No se puede conectar a PostgreSQL en localhost:5433"
    echo "Asegúrate de que PostgreSQL esté corriendo"
    exit 1
fi

# Crear base de datos si no existe
psql -h localhost -p 5433 -U postgres -d postgres -c "CREATE DATABASE \"ContasiscorpBD\";" 2>/dev/null

# Ejecutar script de tablas
echo "Creando tablas..."
psql -h localhost -p 5433 -U postgres -d ContasiscorpBD -f database_local.sql

if [ $? -eq 0 ]; then
    echo "✓ Base de datos configurada exitosamente"
else
    echo "✗ Error al configurar la base de datos"
    exit 1
fi
