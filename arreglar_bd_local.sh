#!/bin/bash

echo "===================================="
echo "Arreglando Base de Datos Local"
echo "===================================="
echo ""

# Verificar si psql está disponible
if ! command -v psql &> /dev/null; then
    echo "ERROR: psql no está disponible"
    echo ""
    echo "Instala PostgreSQL client o agrega psql al PATH"
    echo ""
    exit 1
fi

# Ejecutar script de corrección
echo "Ejecutando script de corrección..."
echo ""

PGPASSWORD=root psql -h localhost -p 5433 -U postgres -d ContasiscorpBD -f fix_usuario_columns.sql

if [ $? -eq 0 ]; then
    echo ""
    echo "===================================="
    echo "✓ Base de datos corregida exitosamente"
    echo "===================================="
    echo ""
    echo "Próximos pasos:"
    echo "1. Reinicia el backend ejecutando: ./restart_backend.sh"
    echo "2. Prueba crear un comprobante"
    echo ""
else
    echo ""
    echo "===================================="
    echo "ERROR: No se pudo aplicar el script"
    echo "===================================="
    echo ""
    echo "Verifica:"
    echo "- Que PostgreSQL esté corriendo"
    echo "- Que la base de datos ContasiscorpBD exista"
    echo "- Que la contraseña sea correcta (actualmente: root)"
    echo ""
    exit 1
fi
