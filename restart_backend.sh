#!/bin/bash

echo "Reiniciando backend de Contasiscorp..."

# Buscar y detener el proceso de la API si está corriendo
echo "Deteniendo proceso existente..."
pkill -f "Contasiscorp.Api" || echo "No hay proceso existente"

# Esperar un momento
sleep 2

# Navegar al directorio de la API
cd "$(dirname "$0")/src/Api"

# Limpiar y reconstruir
echo "Limpiando proyecto..."
dotnet clean

echo "Reconstruyendo proyecto..."
dotnet build

# Iniciar la API
echo "Iniciando API..."
dotnet run

