@echo off
echo ====================================
echo Arreglando Base de Datos Local
echo ====================================
echo.

REM Verificar si psql está disponible
where psql >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: psql no está en el PATH
    echo.
    echo Por favor, agrega PostgreSQL al PATH o ejecuta este comando manualmente:
    echo.
    echo "C:\Program Files\PostgreSQL\16\bin\psql.exe" -h localhost -p 5433 -U postgres -d ContasiscorpBD -f fix_usuario_columns.sql
    echo.
    pause
    exit /b 1
)

echo Ejecutando script de corrección...
echo.
psql -h localhost -p 5433 -U postgres -d ContasiscorpBD -f fix_usuario_columns.sql

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ====================================
    echo ✓ Base de datos corregida exitosamente
    echo ====================================
    echo.
    echo Próximos pasos:
    echo 1. Reinicia el backend ejecutando: restart_backend.bat
    echo 2. Prueba crear un comprobante
    echo.
) else (
    echo.
    echo ====================================
    echo ERROR: No se pudo aplicar el script
    echo ====================================
    echo.
    echo Verifica:
    echo - Que PostgreSQL esté corriendo
    echo - Que la base de datos ContasiscorpBD exista
    echo - Que la contraseña sea correcta
    echo.
)

pause
