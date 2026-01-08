# Configuración de Base de Datos Local

## Configuración Actual

El proyecto está configurado para conectarse a tu PostgreSQL local con:

- **Host:** localhost
- **Puerto:** 5433
- **Base de datos:** ContasiscorpBD
- **Usuario:** postgres
- **Contraseña:** root

## Pasos para Configurar la Base de Datos

### Opción 1: Usando el script (Linux/Mac)

```bash
# Desde la raíz del proyecto
./setup_local_db.sh
```

### Opción 2: Manual (Windows/Linux/Mac)

#### 1. Crear la base de datos

**En Windows (PowerShell):**
```powershell
$env:PGPASSWORD="root"
psql -h localhost -p 5433 -U postgres -d postgres -c "CREATE DATABASE \"ContasiscorpBD\";"
```

**En Linux/Mac (Terminal):**
```bash
export PGPASSWORD=root
psql -h localhost -p 5433 -U postgres -d postgres -c "CREATE DATABASE \"ContasiscorpBD\";"
```

#### 2. Ejecutar el script de tablas

**En Windows (PowerShell):**
```powershell
$env:PGPASSWORD="root"
psql -h localhost -p 5433 -U postgres -d ContasiscorpBD -f database_local.sql
```

**En Linux/Mac (Terminal):**
```bash
export PGPASSWORD=root
psql -h localhost -p 5433 -U postgres -d ContasiscorpBD -f database_local.sql
```

### Opción 3: Usando pgAdmin o DBeaver

1. Conecta a tu servidor PostgreSQL (localhost:5433)
2. Crea una nueva base de datos llamada `ContasiscorpBD`
3. Abre el archivo `database_local.sql`
4. Ejecuta el script en la base de datos `ContasiscorpBD`

## Verificar la Configuración

Puedes verificar que las tablas se crearon correctamente:

```sql
-- Conectarse a ContasiscorpBD
\c ContasiscorpBD

-- Listar tablas
\dt

-- Deberías ver:
-- comprobantes
-- comprobante_items
```

## Ejecutar el Backend

Una vez configurada la base de datos:

```bash
cd src/Api
dotnet run
```

El backend se conectará automáticamente a tu base de datos local.

## Estructura de Tablas Creadas

### Tabla: comprobantes
- Almacena los comprobantes electrónicos (facturas, boletas, etc.)
- Campos principales: serie, numero, tipo, estado, montos, RUC emisor/receptor

### Tabla: comprobante_items
- Almacena el detalle de cada comprobante
- Campos: producto, cantidad, precio, subtotal
- Relación con comprobantes (CASCADE DELETE)

## Troubleshooting

### Error: "psql: command not found"
Asegúrate de que PostgreSQL está instalado y agregado al PATH del sistema.

### Error: "Connection refused"
Verifica que PostgreSQL esté corriendo en el puerto 5433:
```bash
# Windows
netstat -an | findstr 5433

# Linux/Mac
netstat -an | grep 5433
```

### Error: "password authentication failed"
Verifica que la contraseña de tu usuario postgres sea "root" o actualiza el archivo `src/Api/appsettings.json` con la contraseña correcta.

## Arreglar Columnas de Usuario (UUID → TEXT)

Si obtienes el error:
```
la columna «usuario_creacion» es de tipo uuid pero la expresión es de tipo text
```

Esto significa que las columnas de usuario tienen el tipo incorrecto.

### Solución Rápida

**Opción 1: Script automático (Windows)**
```cmd
arreglar_bd_local.bat
```

**Opción 2: Script automático (Linux/Mac/Git Bash)**
```bash
./arreglar_bd_local.sh
```

**Opción 3: Manual con psql**
```bash
# Windows PowerShell
$env:PGPASSWORD="root"
psql -h localhost -p 5433 -U postgres -d ContasiscorpBD -f fix_usuario_columns.sql

# Linux/Mac/Git Bash
PGPASSWORD=root psql -h localhost -p 5433 -U postgres -d ContasiscorpBD -f fix_usuario_columns.sql
```

**Opción 4: Desde pgAdmin**
1. Abre pgAdmin 4
2. Conéctate a PostgreSQL (localhost:5433)
3. Selecciona la base de datos `ContasiscorpBD`
4. Abre Query Tool
5. Carga y ejecuta el archivo `fix_usuario_columns.sql`

Después de ejecutar el script, reinicia el backend:
```bash
cd src/Api
dotnet clean
dotnet build
dotnet run
```
