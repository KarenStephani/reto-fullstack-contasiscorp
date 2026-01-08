# Configuración de Base de Datos Local PostgreSQL

## Datos de Conexión

**Base de Datos:** `ContasiscorpBD`
**Usuario:** `postgres`
**Password:** `root`
**Host:** `localhost`
**Puerto:** `5433`


## Archivos Actualizados

### 1. `.env` (Frontend)
```env
# Base de Datos Local PostgreSQL
DB_HOST=localhost
DB_PORT=5432
DB_NAME=ContasiscorpBD
DB_USER=postgres
DB_PASSWORD=root

# API Backend
VITE_API_URL=http://localhost:5000
```

### 2. `docker-compose.yml`
- Password de PostgreSQL cambiado a `root`
- Variable de entorno del API actualizada con el nuevo password

### 3. `src/Api/appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ContasiscorpBD;Username=postgres;Password=root"
  }
}
```

### 4. `src/Api/appsettings.Development.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ContasiscorpBD;Username=postgres;Password=root"
  }
}
```

### 5. Microservicios Actualizados

Todos los siguientes servicios han sido actualizados con la cadena de conexión correcta:

- **AuthService** (`src/AuthService/appsettings.json`)
- **ProductService** (`src/ProductService/appsettings.json`)
- **UserService** (`src/UserService/appsettings.json`)

## Instrucciones de Uso

### Para Docker Compose

1. Levantar los servicios:
   ```bash
   docker-compose up --build
   ```

2. La base de datos PostgreSQL estará disponible en:
   - Host: `localhost`
   - Puerto: `5432`
   - Database: `ContasiscorpBD`
   - User: `postgres`
   - Password: `root`

### Para Desarrollo Local

**IMPORTANTE:** Debes iniciar primero el backend y luego el frontend.

1. **Crear la base de datos:**
   ```bash
   createdb -U postgres ContasiscorpBD
   ```

2. **Ejecutar las migraciones de Entity Framework:**
   ```bash
   cd src/Api
   dotnet ef database update --project ../Infrastructure
   ```

3. **Iniciar el backend (API):**
   ```bash
   cd src/Api
   dotnet run
   ```

   La API estará disponible en: `http://localhost:5000`
   Swagger estará disponible en: `http://localhost:5000/swagger`

4. **En otra terminal, iniciar el frontend:**
   ```bash
   cd frontend
   npm install
   npm run dev
   ```

   El frontend estará disponible en: `http://localhost:5173`

**NOTA:** El frontend necesita que el backend esté corriendo en `http://localhost:5000` para funcionar correctamente.

## Cadena de Conexión

### Para uso en Docker (servicios en contenedores):
```
Host=postgres;Database=ContasiscorpBD;Username=postgres;Password=root
```

### Para uso local (desarrollo):
```
Host=localhost;Database=ContasiscorpBD;Username=postgres;Password=root
```

## Notas Importantes

- El password ha sido cambiado de `postgres` a `root` en todos los archivos de configuración
- La base de datos se llama `ContasiscorpBD` (con mayúsculas)
- Todos los microservicios apuntan a la misma base de datos local
- Las migraciones de Supabase están disponibles pero no se utilizan en este modo

## Verificación de Conexión

Para verificar que la conexión funciona correctamente:

```bash
psql -U postgres -d ContasiscorpBD -h localhost
# Ingresar password: root
```

## Seguridad

⚠️ **ADVERTENCIA:** Este password (`root`) es para desarrollo local únicamente.
**NUNCA** uses este password en producción.
