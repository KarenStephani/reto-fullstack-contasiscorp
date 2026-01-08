# Contasiscorp - Documentación de Arquitectura

## Visión General

Sistema de gestión de comprobantes electrónicos diseñado siguiendo **Clean Architecture** con **Domain-Driven Design (DDD)**. La aplicación permite la emisión, gestión y anulación de facturas y boletas con cálculo automático de IGV según la normativa peruana.

## Arquitectura del Sistema

```
┌──────────────────────────────────────────────────────────┐
│                     Frontend (React)                      │
│                    Puerto 3000 / 5173                     │
│                   TypeScript + Vite                       │
└───────────────────────┬──────────────────────────────────┘
                        │ HTTP/REST
                        ▼
┌──────────────────────────────────────────────────────────┐
│                    API REST (.NET 8)                      │
│                       Puerto 5000                         │
│                   Controllers + Swagger                   │
└───────────────────────┬──────────────────────────────────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
   Domain        Application     Infrastructure
   Layer          Layer              Layer

   Entities      DTOs          DbContext
   Enums         Validators    Repositories
   Exceptions    Mappers       PostgreSQL
```

## Capas de la Aplicación

### 1. Domain Layer (Núcleo)
**Responsabilidad**: Lógica de negocio pura, independiente de tecnologías externas

**Componentes**:

#### Entities
- **Comprobante**: Entidad raíz agregada
  - Propiedades: Id, Tipo, Serie, Numero, FechaEmision, etc.
  - Métodos: `Anular()`, `Validate()`
  - Cálculos: SubTotal, IGV, Total

- **ComprobanteItem**: Entidad de detalle
  - Propiedades: Id, Descripcion, Cantidad, PrecioUnitario
  - Cálculos: Subtotal

#### Enums
- **ComprobanteTipo**: Factura, Boleta
- **ComprobanteEstado**: Emitido, Anulado

#### Exceptions
- **DomainException**: Excepción base
- **ComprobanteAlreadyAnuladoException**: Error de negocio específico

**Reglas de Negocio Implementadas**:
1. Validación de RUC (11 dígitos numéricos)
2. Formato de serie según tipo (F### para facturas, B### para boletas)
3. RUC y razón social obligatorios para facturas
4. Cálculo automático de IGV (18%)
5. No permitir anulación de comprobantes ya anulados

### 2. Application Layer
**Responsabilidad**: Casos de uso, DTOs, validación, transformación de datos

**Componentes**:

#### DTOs
- **CreateComprobanteDto**: Input para crear comprobantes
- **ComprobanteResponseDto**: Output completo con cálculos
- **ComprobanteListDto**: Output resumido para listados
- **PaginatedResponse\<T\>**: Respuesta paginada genérica

#### Validators (FluentValidation)
- **CreateComprobanteDtoValidator**:
  - Valida tipo de comprobante
  - Valida formato de serie
  - Valida RUC (longitud y formato)
  - Valida items (cantidad, precio)
  - Reglas condicionales para facturas vs boletas

- **ComprobanteItemDtoValidator**:
  - Valida descripción
  - Valida cantidad positiva
  - Valida precio no negativo

#### Mappers
- **ComprobanteMapper**:
  - `ToDomain()`: DTO → Entity
  - `ToResponseDto()`: Entity → Response DTO
  - `ToListDto()`: Entity → List DTO

### 3. Infrastructure Layer
**Responsabilidad**: Acceso a datos, implementación de repositorios, tecnologías externas

**Componentes**:

#### Data
- **ApplicationDbContext**: DbContext de EF Core
  - Configuración de entidades
  - Índices únicos
  - Precision de decimales
  - Relaciones cascade

#### Repositories
- **IComprobanteRepository**: Contrato del repositorio
- **ComprobanteRepository**: Implementación
  - `GetByIdAsync()`: Obtener por ID con items
  - `GetPagedAsync()`: Listado paginado con filtros
  - `GetNextNumeroAsync()`: Generar número correlativo
  - `AddAsync()`: Crear comprobante
  - `UpdateAsync()`: Actualizar comprobante

**Estrategias de Query**:
- Eager loading de items con `Include()`
- Filtros dinámicos aplicados con LINQ
- Paginación eficiente con `Skip()` y `Take()`
- Ordenamiento por fecha descendente

### 4. API Layer (Presentación)
**Responsabilidad**: Exposición HTTP, controladores, configuración

**Componentes**:

#### Controllers
- **ComprobantesController**:
  - GET `/api/comprobantes` - Listado paginado con filtros
  - GET `/api/comprobantes/{id}` - Detalle de comprobante
  - POST `/api/comprobantes` - Crear comprobante
  - PUT `/api/comprobantes/{id}/anular` - Anular comprobante

#### Configuration (Program.cs)
- DbContext con PostgreSQL
- Dependency Injection
- Serilog logging
- Swagger/OpenAPI
- CORS configuration
- Migraciones automáticas al inicio

### 5. Frontend Layer (React)
**Responsabilidad**: Interfaz de usuario, experiencia del usuario

**Componentes**:

#### Types
- Interfaces TypeScript que reflejan los DTOs del backend
- Type safety en toda la aplicación

#### Services
- **api.ts**: Cliente HTTP con Axios
- **comprobanteService**: Métodos CRUD
  - `getAll()`: Obtener lista con filtros
  - `getById()`: Obtener detalle
  - `create()`: Crear comprobante
  - `anular()`: Anular comprobante

#### Components
- **App.tsx**: Componente principal con tabs
- **ComprobanteForm.tsx**: Formulario de creación
  - Estado local para form fields
  - Validación en frontend
  - Cálculo en tiempo real de totales
  - Gestión dinámica de items

- **ComprobanteList.tsx**: Listado y filtros
  - Paginación
  - Filtros por tipo, estado, RUC
  - Acción de anulación
  - Formato de fechas y montos

## Flujo de Datos

### Creación de Comprobante

```
1. Usuario → ComprobanteForm
   └─ Ingresa datos y items

2. ComprobanteForm → API (POST /api/comprobantes)
   └─ Envía CreateComprobanteDto

3. Controller → Validator
   └─ FluentValidation verifica reglas

4. Controller → Mapper
   └─ Convierte DTO a Entity

5. Entity → Validate()
   └─ Aplica reglas de dominio

6. Repository → GetNextNumeroAsync()
   └─ Genera número correlativo

7. Repository → AddAsync()
   └─ Persiste en PostgreSQL

8. Controller → Mapper
   └─ Convierte Entity a ResponseDto

9. API → ComprobanteForm
   └─ Devuelve comprobante creado

10. ComprobanteForm → App
    └─ Cambia a tab de listado
```

### Consulta con Filtros

```
1. Usuario → ComprobanteList
   └─ Selecciona filtros

2. ComprobanteList → API (GET /api/comprobantes?filters)
   └─ Envía query parameters

3. Controller → Repository.GetPagedAsync()
   └─ Query con filtros dinámicos

4. Repository → PostgreSQL
   └─ SELECT con WHERE y paginación

5. PostgreSQL → Repository
   └─ Devuelve entities con items

6. Repository → Controller
   └─ (Lista de Comprobante, Total)

7. Controller → Mapper.ToListDto()
   └─ Convierte a DTOs

8. Controller → ComprobanteList
   └─ PaginatedResponse<ComprobanteListDto>

9. ComprobanteList
   └─ Renderiza tabla con datos
```

## Modelo de Datos

### Esquema de Base de Datos

```sql
-- Tabla: comprobantes
CREATE TABLE comprobantes (
    id                      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tipo                    INT NOT NULL,
    serie                   VARCHAR(4) NOT NULL,
    numero                  INT NOT NULL,
    fecha_emision           TIMESTAMP NOT NULL,
    ruc_emisor              VARCHAR(11) NOT NULL,
    razon_social_emisor     VARCHAR(500) NOT NULL,
    ruc_receptor            VARCHAR(11),
    razon_social_receptor   VARCHAR(500),
    estado                  INT NOT NULL DEFAULT 1,
    CONSTRAINT uk_serie_numero UNIQUE (serie, numero)
);

CREATE INDEX ix_comprobantes_fecha_emision ON comprobantes(fecha_emision);
CREATE INDEX ix_comprobantes_estado ON comprobantes(estado);

-- Tabla: comprobante_items
CREATE TABLE comprobante_items (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    comprobante_id      UUID NOT NULL,
    descripcion         VARCHAR(500) NOT NULL,
    cantidad            DECIMAL(18,2) NOT NULL,
    precio_unitario     DECIMAL(18,2) NOT NULL,
    CONSTRAINT fk_items_comprobante
        FOREIGN KEY (comprobante_id)
        REFERENCES comprobantes(id)
        ON DELETE CASCADE
);
```

### Relaciones
- Comprobante (1) → (N) ComprobanteItem
- Eliminación en cascada de items

## Patrones de Diseño

### 1. Clean Architecture
Separación en capas con dependencias unidireccionales:
```
Domain ← Application ← Infrastructure
                     ← API
```

### 2. Repository Pattern
Abstracción de acceso a datos:
```csharp
interface IComprobanteRepository
    → ComprobanteRepository (EF Core)
```

### 3. DTO Pattern
Separación de modelos de dominio y transferencia:
```
Entity (Domain) → Mapper → DTO (API)
```

### 4. Aggregate Pattern (DDD)
Comprobante es el agregado raíz que contiene ComprobanteItems

### 5. Value Objects (Implicit)
RUC, Serie se validan como value objects inmutables

### 6. Domain Events (Preparado)
Estructura preparada para eventos de dominio (ej: ComprobanteEmitido)

## Principios SOLID

### Single Responsibility
- Cada clase tiene una única razón para cambiar
- Controladores: solo HTTP
- Repositorios: solo acceso a datos
- Entidades: solo lógica de negocio

### Open/Closed
- Extensible mediante herencia de excepciones
- Nuevos validadores sin modificar existentes

### Liskov Substitution
- Interfaces de repositorios intercambiables

### Interface Segregation
- IComprobanteRepository con métodos específicos

### Dependency Inversion
- Capas superiores dependen de abstracciones
- Inyección de dependencias en toda la aplicación

## Seguridad

### Validación
1. **Frontend**: Validación inmediata de UX
2. **FluentValidation**: Validación de DTOs
3. **Domain**: Reglas de negocio en entidades

### SQL Injection
- Protección mediante EF Core y consultas parametrizadas

### CORS
- Configurado para desarrollo
- Debe ajustarse para producción

## Testing

### Unit Tests (xUnit)

#### Domain Tests
```csharp
ComprobanteTests:
- Cálculo de subtotal, IGV, total
- Anulación correcta
- Excepciones en anulación duplicada
- Validaciones de RUC
- Validaciones de serie
```

#### Application Tests
```csharp
ValidatorTests:
- Validación de facturas
- Validación de boletas
- Formato de serie
- RUC obligatorio para facturas
- Items requeridos
```

### Coverage
Alta cobertura en:
- Lógica de negocio (Domain)
- Validadores (Application)

## Logging

### Serilog Configuration
```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
```

### Eventos Logueados
- Creación de comprobantes
- Anulaciones
- Errores de validación
- Warnings de dominio

## Performance

### Optimizaciones
1. **Queries eficientes**
   - Eager loading de relaciones
   - Índices en columnas frecuentes
   - Paginación en servidor

2. **Frontend**
   - Vite para builds rápidos
   - Code splitting automático
   - Tree shaking

3. **Caching (Futuro)**
   - Caché de consultas frecuentes
   - Redis para sesiones

## Escalabilidad

### Vertical
- Más CPU/RAM al servidor
- Pool de conexiones de DB optimizado

### Horizontal (Futuro)
- Load balancer para múltiples instancias API
- PostgreSQL read replicas
- CDN para frontend

### Microservicios (Futuro)
Preparado para dividir en:
- Servicio de Comprobantes
- Servicio de Reportes
- Servicio de Integración SUNAT

## DevOps

### Docker
```yaml
services:
  - postgres: Base de datos
  - api: Backend .NET
  - frontend: Frontend React con Nginx
```

### CI/CD (Recomendado)
```
GitHub Actions:
1. Build → Test → Publish
2. Docker Build
3. Deploy to Azure/AWS
```

## Monitoreo (Futuro)

### APM
- Application Insights
- Sentry para errores

### Métricas
- Prometheus
- Grafana dashboards

### Health Checks
```csharp
app.MapHealthChecks("/health");
app.MapHealthChecks("/ready");
```

## Expansiones Futuras

### Corto Plazo
1. Autenticación JWT
2. Roles y permisos
3. Export a PDF
4. Notas de crédito/débito

### Mediano Plazo
1. Firma digital
2. Envío a SUNAT
3. Multi-empresa
4. Reportes avanzados

### Largo Plazo
1. Facturación recurrente
2. Integración con ERP
3. API pública
4. Mobile app

## Referencias

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design - Eric Evans](https://www.domainlanguage.com/ddd/)
- [Entity Framework Core Docs](https://docs.microsoft.com/en-us/ef/core/)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [React Documentation](https://react.dev/)
- [SUNAT - Normativa de Facturación Electrónica](https://cpe.sunat.gob.pe/)
