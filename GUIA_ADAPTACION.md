# Guía: adaptar `catch-up-platform` a cualquier caso con cambios mínimos

Esta guía parte del proyecto de ejemplo (CatchUp, bounded context `News`, aggregate `FavoriteSource`)
y del caso resuelto aquí (Hialpesa, bounded context `Sale`, aggregate `PurchaseOrder`).
La idea: **el bounded context `Shared` no se toca** y el contexto de negocio se recrea por
**sustitución 1 a 1** de cada pieza.

---

## 1. Mapa de equivalencias (la tabla más importante)

| Pieza (CatchUp)                                   | En Hialpesa                                          | Qué cambia                                    |
|---------------------------------------------------|------------------------------------------------------|-----------------------------------------------|
| Bounded context `News`                            | `Sale`                                               | Nombre de carpeta y namespace                 |
| Aggregate `FavoriteSource`                        | `PurchaseOrder`                                      | Propiedades                                   |
| `FavoriteSourceAudit` (partial, `IAuditableEntity`)| `PurchaseOrderAudit`                                | Solo el nombre de la clase                    |
| VO `NewsApiKey` (string, máx. 255)                | `Customer` (string, 4–50)                            | Constantes `MinLength`/`MaxLength`            |
| VO `SourceId` (string, máx. 255)                  | `PoNumber` (string, obligatorio)                     | Nombre                                        |
| (no existía)                                      | `Vendor`, `ShipTo` (VO string)                       | Copia de cualquier VO string                  |
| (no existía)                                      | `EFabric` (enum) + `FabricId` (VO int)               | Ver receta 4.3                                |
| `CreateFavoriteSourceCommand`                     | `CreatePurchaseOrderCommand`                         | Parámetros del record                         |
| `IFavoriteSourceRepository`                       | `IPurchaseOrderRepository`                           | Método de búsqueda de duplicados              |
| `FavoriteSourceRepository`                        | `PurchaseOrderRepository`                            | Lambda del `FirstOrDefaultAsync`              |
| `CreateFavoriteSourceError`                       | `CreatePurchaseOrderError`                           | `DuplicatePurchaseOrder`, `UnexpectedError`   |
| `IFavoriteSourceCommandService` + impl.           | `IPurchaseOrderCommandService` + impl.               | Nombres y llamada al repositorio              |
| `CreateFavoriteSourceResource` (request)          | `CreatePurchaseOrderResource`                        | Campos + `[Required]`                         |
| `FavoriteSourceResource` (response)               | `PurchaseOrderResource`                              | Campos (sin auditoría)                        |
| Assemblers (3 + `LocalizedOpenApiAssembler`)      | Los mismos 4                                         | Nombres de campos                             |
| `FavoriteSourcesController`                       | `PurchaseOrdersController`                           | Ruta sale sola: `purchase-orders`             |
| `AppDbContext.OnModelCreating`                    | Mismo método                                         | Bloque `builder.Entity<...>`                  |
| `Program.cs`                                      | Mismo archivo                                        | Usings + 2 líneas de DI                       |
| `Resources/SharedResource*.resx`                  | Mismos 5 archivos                                    | Claves/mensajes                               |
| `appsettings*.json`                               | Mismos 3                                             | `database=hialpesa`                           |

**Regla mental:** `NewsApiKey + SourceId` = "clave compuesta única"; `FavoriteSource` = "entidad".
Todo caso de examen se parece a esto: una entidad, unos VO con reglas, una regla de duplicado.

---

## 2. Orden de trabajo recomendado (100 minutos)

1. **Leer el enunciado y subrayar** (5 min): entidad, atributos y tipos, reglas de negocio, endpoint(s) pedidos,
   nombre del bounded context, nombre del esquema, nombre del proyecto `si730pc2u<código>.API`.
2. **Copiar el repo base** y quitar lo que no es del examen (2 min): `.idea`, `Dockerfile`, `apprunner.yaml`,
   `bin/`, `obj/`, `Migrations/`, `docs/`, CORS, `UseUrls(8080)`.
3. **Renombrar** solución, carpeta, `.csproj` y namespace raíz (5 min). Rider: *Refactor → Rename* sobre el
   proyecto, y *Replace in Files* (Ctrl+Shift+R) de `CatchUpPlatform.API` → `si730pc2u<código>.API`.
4. **Renombrar `News` → bounded context pedido** y `FavoriteSource` → entidad pedida
   (Replace in Files, *match case*: `FavoriteSources`, `FavoriteSource`, `favoriteSource`, `News`).
5. **Value Objects** (10 min): ajustar/crear según el enunciado (receta 4.1–4.3).
6. **Aggregate + Command** (10 min): propiedades y constructor.
7. **Repositorio**: método de búsqueda para la regla de duplicados (receta 4.4).
8. **Command service + Error enum** (10 min).
9. **Resources + Assemblers** (10 min).
10. **Controller** (5 min): solo los endpoints que pide el enunciado.
11. **`AppDbContext`** (10 min): conversiones de VO, índice único.
12. **`Program.cs`, `appsettings`, resx** (10 min).
13. **Compilar y probar** con el archivo `.http` (15 min).
14. **XML docs + `<remarks>` con tu nombre y apellidos**, quitar `bin/obj`, **empaquetar el zip** (10 min).

> Tip: haz primero que compile y responda 201. Los 400/409 después.

---

## 3. Qué se quitó del proyecto base y por qué

| Se quitó                              | Motivo                                                                    |
|---------------------------------------|---------------------------------------------------------------------------|
| `builder.WebHost.UseUrls("...:8080")` | Era para Render. El examen usa `http://localhost:{port}` (launchSettings) |
| CORS (`AddCors`, `UseCors`)           | El enunciado dice "NO forma parte del alcance"                            |
| `Migrations/` + `Database.Migrate()`  | Se usa `Database.EnsureCreated()`: no necesitas `dotnet ef`              |
| Query service, queries, GETs          | El enunciado pide **solo** el POST. Ver receta 4.7 si piden GET           |
| `Dockerfile`, `apprunner.yaml`, `.idea`, `docs/` | Despliegue/ruido, no se evalúa                                 |

Se **conserva** todo lo demás: `Shared`, `Result<T,E>`, interceptor de auditoría, snake_case + plural,
kebab-case + minúsculas, localización, OpenAPI + Scalar, `ProblemDetails`.

---

## 4. Recetas

### 4.1 Value Object string con longitud mínima y máxima (ej. Customer, Vendor)
Copiar `Customer.cs`, cambiar el nombre y las constantes:

```csharp
private const int MinLength = 4;
private const int MaxLength = 50;
...
if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("X cannot be null, empty, or whitespace.");
if (value.Length < MinLength || value.Length > MaxLength) throw new ArgumentException(...);
```

Sin paramName en `ArgumentException` para que el mensaje del body quede limpio.
En `AppDbContext` agrega `.HasMaxLength(50)` para que la columna sea `varchar(50)`.

### 4.2 Value Object string solo obligatorio (ej. PoNumber, ShipTo)
Igual que `SourceId` original (solo máximo 255). Mantén `HasMaxLength(255)`:
**MySQL no permite índices únicos sobre `longtext`**, así que todo campo del índice único debe tener longitud.

### 4.3 Enumeration + Value Object (ej. EFabric / FabricId)
1. `enum EFabric { Algodon = 1, Modal = 2, ... }` — el **nombre del miembro es el "Name"** de la tabla del enunciado.
2. VO `FabricId(int value)` valida con `Enum.IsDefined((EFabric)value)`; si no, `ArgumentException` → **400**.
3. Persistencia: `HasConversion(vo => vo.Value, v => new FabricId(v))` → columna `fabric_id` (int).
4. Si piden **otro campo enum** (ej. `Status`, `Size`), copia `FabricId` y `EFabric` y cambia nombre y miembros.

### 4.4 Regla "no duplicar (A, B)" (409 Conflict)
- Repositorio: `FindByAAndBAsync(A a, B b)` con `FirstOrDefaultAsync(x => x.A == a && x.B == b)`.
- Command service: si existe → `Failure(DuplicateX)`; además se captura el error MySQL 1062 por si hay carrera.
- `AppDbContext`: `HasIndex(x => new { x.A, x.B }).IsUnique()`.
- Assembler de resultado: `DuplicateX => controller.Conflict(localizer["..."].Value)`.

Si la regla es sobre **un solo campo** único: `FindByAAsync(A a)` + `HasIndex(x => x.A).IsUnique()`.

### 4.5 Valor por defecto (ej. fabricId omitido → Algodon)
- Resource: campo **nullable** (`int? FabricId`) sin `[Required]`.
- Command: `FabricId? FabricId`.
- Aggregate: `FabricId = command.FabricId ?? FabricId.Default;` (la regla vive en el dominio).
- VO: `public static FabricId Default => new((int)EFabric.Algodon);`

### 4.6 Campos obligatorios, auditoría y respuesta
- `[Required(ErrorMessage = "RequiredX")]` en el resource de entrada (la clave sale del `.resx`).
- Un `int` obligatorio se declara `int?` + `[Required]`; así "faltante" ≠ `0`.
- **Auditoría** (`createdAt`/`updatedAt`): ya la resuelve `IAuditableEntity` + interceptor.
  Solo copia `XAudit.cs` con el nombre de la entidad. **No** pongas esos campos en el resource de respuesta.

### 4.7 Si el enunciado pide GET (por id, lista, por filtros)
Restaura desde el repo original (cambiando nombres como en la tabla 1):
`GetXByIdQuery`, `IXQueryService`, `XQueryService`, el método `[HttpGet("{id}")]` del controller y la línea
`AddScoped<IXQueryService, XQueryService>()` en `Program.cs`. En el assembler de resultado cambia
`StatusCode(201, resource)` por `CreatedAtAction(nameof(GetXById), new { id = ... }, resource)`.

### 4.8 Si piden PUT/DELETE
`UpdateXCommand` / `DeleteXCommand`, método en el command service (`FindByIdAsync` → `Update`/`Remove` →
`CompleteAsync`), `NotFound` como nuevo valor del enum de error y `[HttpPut("{id}")]`/`[HttpDelete("{id}")]`.

### 4.9 Si hay una relación entre entidades
Nuevo aggregate con su `FooRepository`; el command service valida la existencia del otro (`FindByIdAsync`)
y la FK se mapea con `HasOne/WithMany` en `AppDbContext`. La convención snake_case ya se aplica sola.

### 4.10 Si el caso pide otro bounded context
Crea la carpeta `MiContexto/` con las 4 capas (`Domain`, `Application`, `Infrastructure`, `Interfaces`) y
registra repositorio y servicios en `Program.cs` bajo su propio comentario de bounded context.

---

## 5. Requisitos del enunciado → dónde se cumple

| Requisito (Technical constraints)                       | Dónde                                                              |
|---------------------------------------------------------|--------------------------------------------------------------------|
| C#, .NET 8 / ASP.NET Core                               | `.csproj`. **El proyecto base es .NET 10: cambia `net10.0` a `net8.0` y los paquetes `10.x` a `8.x` si el examen exige .NET 8** (ver nota 7) |
| Proyecto `si730pc2u<código>.API`                        | Carpeta, `.csproj`, `.sln`, namespace raíz                          |
| Bounded context + `Shared` igual al de clase            | `Sale/` y `Shared/` sin tocar                                      |
| ORM con snake_case y tablas en plural                   | `Shared/.../ModelBuilderExtensions` → `purchase_orders`, `po_number`, `fabric_id`, `ship_to`, `created_at` |
| `PackageReference` iguales al ejemplo                   | `.csproj` sin cambios de paquetes (salvo nota 7)                   |
| MySQL, esquema `hialpesa`                               | `appsettings*.json` → `database=hialpesa`                          |
| DDD, layered (domain/application/interfaces/infrastructure), CQRS | Estructura `Sale/*`; comando separado de queries        |
| Nombres en inglés, PascalCase / camelCase privados      | Todo el código                                                     |
| URLs en minúsculas                                      | `LowercaseUrls = true` + `KebabCaseRouteNamingConvention`          |
| Properties en entidades                                 | `PurchaseOrder` usa propiedades con `private set`                  |
| XML docs + `<remarks>` con nombres y apellidos          | Aggregate, controller, command service, repositorio, enum         |
| OpenAPI                                                 | `AddOpenApi` + `LocalizedOpenApiAssembler` + Scalar en `/scalar`   |
| Gestión de excepciones                                  | `try/catch` en controller y command service, `ProblemDetails`      |
| Zip `upc-pre-202402-si730-<sección>-pc2-u<código>.zip`  | Ver sección 7                                                      |

**Códigos HTTP implementados**

| Situación                                              | Status |
|--------------------------------------------------------|--------|
| Creado                                                 | 201 + resource (`id, customer, poNumber, fabricId, vendor, shipTo, quantity`) |
| Falta campo requerido                                  | 400 (ProblemDetails)                                              |
| Customer/Vendor vacío o fuera de 4–50; `fabricId` fuera de 1–8 | 400 (mensaje del VO)                                      |
| Mismo `customer` + `poNumber`                          | 409                                                               |
| Error inesperado / BD caída                            | 500 (ProblemDetails)                                              |

---

## 6. Errores típicos

- **`Unknown database 'hialpesa'`** → usuario sin permiso de `CREATE`. Crea el esquema a mano: `CREATE DATABASE hialpesa;`.
- **Cambiaste el modelo y la tabla no cambia** → `EnsureCreated` no actualiza. Borra el esquema (`DROP DATABASE hialpesa;`) y vuelve a correr.
- **Tablas con nombre `PurchaseOrder`** → falta `builder.UseSnakeCaseNamingConvention();` al final de `OnModelCreating`.
- **Ruta `/api/v1/PurchaseOrders`** → falta `options.Conventions.Add(new KebabCaseRouteNamingConvention())` en `AddControllers`.
- **`Specified key was too long` / `BLOB/TEXT column used in key`** → falta `HasMaxLength` en un campo del índice único.
- **El 400 no sale y da 500** → un VO lanza `ArgumentException` fuera del `try` del controller. El assembler debe invocarse dentro del `try`.
- **El resource devuelve `createdAt`** → quitaste la separación entre entidad y resource; no mapees campos de auditoría.
- **`Color Color` (propiedad y tipo con el mismo nombre)** → es válido en C# (`Customer Customer`), igual que `NewsApiKey NewsApiKey` en el original.

---

## 7. Notas finales

1. **Versión de .NET:** el enunciado del PC2 dice .NET 8; el proyecto base usa .NET 10.
   Si el docente exige .NET 8 en tu práctica: `net8.0`, paquetes `8.x` (`Microsoft.EntityFrameworkCore 8.0.x`,
   `Microsoft.AspNetCore.OpenApi 8.0.x`), y `global.json` a `8.0.x`. Ojo: `AddOpenApi` y el *schema transformer*
   se comportan distinto en .NET 8 (Swashbuckle era lo habitual). **Sigue el `.csproj` del ejemplo visto en clase
   si el enunciado dice "las mismas dependencias".**
2. **Renombrar al código real:** reemplaza `u201621873` por tu código y `NOMBRES Y APELLIDOS DEL ESTUDIANTE`
   por tu nombre (Replace in Files, y renombra carpeta/`.csproj`/`.sln`).
3. **Zip final:** `upc-pre-202402-si730-<sección>-pc2-u<código>.zip`, por ejemplo
   `upc-pre-202402-si730-si91-pc2-u201621873.zip`. Excluye `bin/` y `obj/`. Único formato válido: `.zip`.
4. **Regla de oro:** si el enunciado no lo pide, no lo agregues (GETs, CORS, seguridad, tests).
5. **Validación extra opcional:** el enunciado no exige `quantity > 0`. Si el caso lo pidiera, créalo como VO
   o valídalo en el constructor del aggregate lanzando `ArgumentException` (devuelve 400).
