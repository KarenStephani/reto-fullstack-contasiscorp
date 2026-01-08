# Contasiscorp - Sistema de Gestión de Comprobantes Electrónicos

Sistema completo de gestión de comprobantes electrónicos (Facturas y Boletas) desarrollado con .NET 8 y React, siguiendo la arquitectura Clean Architecture con DDD (Domain-Driven Design).

## Características Principales

- Emisión de Facturas y Boletas con cálculo automático de IGV
- Validación completa de RUC según normativa peruana
- Anulación de comprobantes
- Búsqueda y filtrado avanzado
- Paginación de resultados
- API REST con documentación Swagger/OpenAPI
- Frontend React moderno y responsivo
- Tests unitarios completos
- Logging estructurado con Serilog
- Contenedorización con Docker

## Arquitectura del Proyecto

El proyecto sigue Clean Architecture con las siguientes capas:

```
/src
├── Domain/              # Entidades, enums, excepciones de dominio
│   ├── Entities/        # Comprobante, ComprobanteItem
│   ├── Enums/           # ComprobanteTipo, ComprobanteEstado
│   └── Exceptions/      # Excepciones de negocio
├── Application/         # DTOs, validadores, mappers
│   ├── DTOs/            # Data Transfer Objects
│   ├── Validators/      # FluentValidation
│   └── Mappers/         # Mapeo entre entidades y DTOs
├── Infrastructure/      # EF Core, repositorios, PostgreSQL
│   ├── Data/            # DbContext, configuraciones
│   └── Repositories/    # Implementación de repositorios
└── Api/                 # Controllers, Swagger, Program.cs
    └── Controllers/     # API REST endpoints

/frontend                # Aplicación React
├── src/
│   ├── components/      # Componentes React
│   ├── services/        # Cliente API
│   └── types/           # Tipos TypeScript
└── ...

/tests
└── Contasiscorp.Tests/ # Tests unitarios con xUnit
    ├── Domain/          # Tests de entidades
    └── Application/     # Tests de validadores
```

## Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker](https://www.docker.com/) y Docker Compose (opcional)
- [PostgreSQL 16](https://www.postgresql.org/) (si no usas Docker)

## Instalación y Configuración

### Opción 1: Con Docker (Recomendado)

1. **Clona el repositorio:**
   ```bash
   git clone https://github.com/tu-usuario/contasiscorp.git
   cd contasiscorp
   ```

2. **Inicia todos los servicios:**
   ```bash
   docker-compose up --build
   ```

3. **Accede a la aplicación:**
   - Frontend: http://localhost:3000
   - API: http://localhost:5000
   - Swagger: http://localhost:5000/swagger

### Opción 2: Sin Docker (Desarrollo Local)

1. **Clona el repositorio:**
   ```bash
   git clone https://github.com/tu-usuario/contasiscorp.git
   cd contasiscorp
   ```

2. **Configura PostgreSQL:**
   ```bash
   # Crea la base de datos
   createdb contasiscorp
   ```

3. **Configura la conexión en appsettings.Development.json:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=contasiscorp;Username=postgres;Password=tu_password"
     }
   }
   ```

4. **Ejecuta las migraciones:**
   ```bash
   dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/Api
   dotnet ef database update --project src/Infrastructure --startup-project src/Api
   ```

5. **Inicia el backend:**
   ```bash
   cd src/Api
   dotnet run
   ```
   La API estará disponible en http://localhost:5000

6. **Inicia el frontend (en otra terminal):**
   ```bash
   cd frontend
   npm install
   npm run dev
   ```
   El frontend estará disponible en http://localhost:5173

## API Endpoints

### Comprobantes

#### GET /api/comprobantes
Obtiene una lista paginada de comprobantes con filtros opcionales.

**Query Parameters:**
- `page` (int): Número de página (default: 1)
- `pageSize` (int): Elementos por página (default: 10)
- `fechaDesde` (DateTime): Fecha desde
- `fechaHasta` (DateTime): Fecha hasta
- `tipo` (string): "Factura" o "Boleta"
- `rucReceptor` (string): RUC del receptor
- `estado` (string): "Emitido" o "Anulado"

**Response:**
```json
{
  "items": [...],
  "total": 100,
  "page": 1,
  "pageSize": 10,
  "totalPages": 10
}
```

#### GET /api/comprobantes/{id}
Obtiene un comprobante específico por ID.

**Response:**
```json
{
  "id": "guid",
  "tipo": "Factura",
  "serie": "F001",
  "numero": 1,
  "fechaEmision": "2024-01-08T10:00:00Z",
  "rucEmisor": "20123456789",
  "razonSocialEmisor": "Empresa SAC",
  "rucReceptor": "20987654321",
  "razonSocialReceptor": "Cliente SAC",
  "subTotal": 100.00,
  "igv": 18.00,
  "total": 118.00,
  "estado": "Emitido",
  "items": [
    {
      "descripcion": "Producto 1",
      "cantidad": 2,
      "precioUnitario": 50.00,
      "subtotal": 100.00
    }
  ]
}
```

#### POST /api/comprobantes
Crea un nuevo comprobante.

**Request Body:**
```json
{
  "tipo": "Factura",
  "serie": "F001",
  "rucEmisor": "20123456789",
  "razonSocialEmisor": "Empresa SAC",
  "rucReceptor": "20987654321",
  "razonSocialReceptor": "Cliente SAC",
  "items": [
    {
      "descripcion": "Producto 1",
      "cantidad": 2,
      "precioUnitario": 50.00
    }
  ]
}
```

**Notas:**
- El número se genera automáticamente
- Para Boletas, `rucReceptor` y `razonSocialReceptor` son opcionales
- El IGV se calcula automáticamente (18%)

#### PUT /api/comprobantes/{id}/anular
Anula un comprobante emitido.

**Response:**
Devuelve el comprobante actualizado con estado "Anulado".

## Reglas de Negocio

### Facturas
- Serie debe tener formato: `F###` (ejemplo: F001)
- Requiere RUC y Razón Social del receptor
- RUC debe tener 11 dígitos numéricos

### Boletas
- Serie debe tener formato: `B###` (ejemplo: B001)
- RUC y Razón Social del receptor son opcionales

### Validaciones
- RUC del emisor es obligatorio (11 dígitos)
- Al menos un item es requerido
- Cantidad debe ser mayor a 0
- Precio unitario no puede ser negativo
- Un comprobante anulado no puede volver a anularse

### Cálculos
- Subtotal = Σ (Cantidad × Precio Unitario)
- IGV = Subtotal × 0.18
- Total = Subtotal + IGV

## Tests

Ejecuta los tests unitarios:

```bash
dotnet test
```

Los tests incluyen:
- Tests de entidades de dominio
- Tests de validadores FluentValidation
- Tests de reglas de negocio
- Tests de cálculos

## Tecnologías Utilizadas

### Backend (.NET 8)
- **ASP.NET Core 8** - Framework web
- **Entity Framework Core 8** - ORM
- **PostgreSQL** - Base de datos
- **FluentValidation** - Validación de DTOs
- **Serilog** - Logging estructurado
- **Swagger/OpenAPI** - Documentación de API
- **xUnit** - Testing framework
- **FluentAssertions** - Assertions para tests

### Frontend (React)
- **React 18** - UI Library
- **TypeScript** - Type safety
- **Vite** - Build tool
- **Axios** - Cliente HTTP

### DevOps
- **Docker** - Contenedorización
- **Docker Compose** - Orquestación
- **Nginx** - Servidor web para frontend

## Estructura de la Base de Datos

### Tabla: comprobantes
| Columna | Tipo | Descripción |
|---------|------|-------------|
| id | uuid | Identificador único (PK) |
| tipo | int | 1=Factura, 2=Boleta |
| serie | varchar(4) | Serie del comprobante |
| numero | int | Número correlativo |
| fecha_emision | timestamp | Fecha de emisión |
| ruc_emisor | varchar(11) | RUC del emisor |
| razon_social_emisor | varchar(500) | Razón social del emisor |
| ruc_receptor | varchar(11) | RUC del receptor (nullable) |
| razon_social_receptor | varchar(500) | Razón social del receptor (nullable) |
| estado | int | 1=Emitido, 2=Anulado |

### Tabla: comprobante_items
| Columna | Tipo | Descripción |
|---------|------|-------------|
| id | uuid | Identificador único (PK) |
| comprobante_id | uuid | Referencia a comprobantes (FK) |
| descripcion | varchar(500) | Descripción del item |
| cantidad | decimal(18,2) | Cantidad |
| precio_unitario | decimal(18,2) | Precio unitario |

### Índices
- `(serie, numero)` - Único
- `fecha_emision` - Para búsquedas
- `estado` - Para filtros

## Mejores Prácticas Implementadas

1. **Clean Architecture**: Separación clara de responsabilidades
2. **Domain-Driven Design**: Lógica de negocio en entidades de dominio
3. **SOLID Principles**: Código mantenible y extensible
4. **Repository Pattern**: Abstracción de acceso a datos
5. **DTO Pattern**: Separación entre modelos de dominio y API
6. **Validation**: FluentValidation para reglas complejas
7. **Error Handling**: Excepciones de dominio personalizadas
8. **Logging**: Serilog para trazabilidad
9. **Testing**: Tests unitarios con alta cobertura
10. **API Documentation**: Swagger/OpenAPI
11. **Type Safety**: TypeScript en frontend

## Expansiones Futuras

- [ ] Autenticación y autorización
- [ ] Firma digital de comprobantes
- [ ] Envío a SUNAT (Perú)
- [ ] Generación de PDF
- [ ] Reportes y estadísticas
- [ ] Notas de crédito y débito
- [ ] Integración con sistemas de facturación
- [ ] Multi-empresa
- [ ] API de webhooks
- [ ] Export a Excel/CSV

## Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.

## Soporte

Para preguntas y soporte, por favor abre un issue en GitHub.

## Autor

Desarrollado por Contasiscorp Team

---

**¡Gracias por usar Contasiscorp!**
