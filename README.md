# Hialpesa Purchase Orders API (SI730 – PC2)

RESTful API en C# / .NET 10 / ASP.NET Core + EF Core + MySQL, adaptada desde el proyecto de ejemplo
`catch-up-platform` con cambios mínimos.

- Bounded context: **Sale** (aggregate `PurchaseOrder`)
- Endpoint: `POST /api/v1/purchase-orders`
- Esquema MySQL: `hialpesa` (tabla `purchase_orders`)
- Documentación OpenAPI: `/openapi/v1.json` y Scalar en `/scalar`

## Ejecutar
1. Tener MySQL corriendo y ajustar `ConnectionStrings:DefaultConnection` en `appsettings*.json`
   (usuario/clave). El esquema `hialpesa` y las tablas se crean al iniciar (`EnsureCreated`).
2. `dotnet run --project si730pc2u201621873.API` y abrir `http://localhost:5128/scalar`.
3. Probar con `si730pc2u201621873.API/si730pc2u201621873.API.http`.

Guía de Configuración Backend: Caso Capbase (Covenants)Esta guía detalla paso a paso cómo replicar la estructura del proyecto base CatchUp Platform (.NET + Clean Architecture + DDD + CQRS + Entity Framework Core) y adaptarla al Caso Capbase haciendo los cambios mínimos y ordenados para evitar cualquier conflicto. 

Paso 1: Configuración Inicial y Base (Shared)La carpeta Shared contiene la infraestructura transversal del proyecto (auditoría, repositorios base, patrón Result, convenciones de nombres). Puedes mantenerla casi intacta:appsettings.json: Cambia únicamente el nombre de la base de datos en tu cadena de conexión por la del nuevo proyecto (por ejemplo, capbase-db):{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;user=root;password=tu_password;database=capbase-db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
Carpeta Shared: Mantén intactos IAuditableEntity, AuditableEntityInterceptor, BaseRepository, UnitOfWork, Result y las extensiones de ModelBuilder para el formato snake_case.
📦 Paso 2: Crear el Bounded Context Covenants (Dominio)Reemplaza la carpeta News por Covenants y estructura el dominio siguiendo Domain-Driven Design (DDD):A. Value Objects (Covenants/Domain/Model/ValueObjects/)Crea los objetos de valor que encapsulan las reglas de validación del negocio:CapbaseIdentifier.cs: Envuelve un valor de tipo Guid único para identificar el documento.PartyId.cs: Envuelve un valor de tipo Guid (UUID) que identifica al cliente.LegalityPeriod.cs: Contiene StartDate y EndDate (ambos de tipo DateOnly). Valida que EndDate no sea menor o igual a StartDate e incluye el método booleano IsExpired(DateOnly checkDate).MonetaryAmount.cs: Contiene Value (decimal) y Currency (string). Valida que Value >= 0, que Currency no sea nulo/vacío y permite multiplicar el monto por un factor manteniendo la moneda.B. Enum y Aggregate Root (Covenants/Domain/Model/Aggregates/)CovenantStatus.cs (Enum): Define los estados posibles: Draft (0 - valor por defecto), Active (1) y Expired (2).Covenant.cs (Entidad / Aggregate Root): Implementa IAuditableEntity para registrar automáticamente CreatedAt y UpdatedAt. Contiene las propiedades:Id (int, llave primaria autogenerada)DocumentIdentifier (CapbaseIdentifier)ClientId (PartyId)Period (LegalityPeriod)TotalValue (MonetaryAmount)Status (CovenantStatus)Footnotes (string, opcional)C. Commands y Queries - CQRS (Covenants/Domain/Model/Commands/ y Queries/)CreateCovenantCommand.cs: record que encapsula los datos necesarios para registrar un nuevo convenio.GetCovenantByIdQuery.cs y GetAllCovenantsQuery.cs: Registros (record) para las consultas de lectura.D. Repositorios (Covenants/Domain/Repositories/)ICovenantRepository.cs: Interfaz que hereda de IBaseRepository<Covenant> y añade métodos de búsqueda específicos (por ejemplo, verificar si existe un DocumentIdentifier duplicado).⚙️ Paso 3: Capa de Infraestructura (Covenants/Infrastructure/)CovenantRepository.cs: Implementa ICovenantRepository heredando de BaseRepository<Covenant> y utilizando AppDbContext.AppDbContext.cs (Shared/Infrastructure/Persistence/EFC/Configuration/):Registra la entidad Covenant y configura su llave primaria.Configura las conversiones (HasConversion / OwnsOne) para mapear los Value Objects (CapbaseIdentifier, PartyId, LegalityPeriod, MonetaryAmount) a columnas de MySQL.Añade el índice único (IsUnique()) para DocumentIdentifier.Mantiene builder.UseSnakeCaseNamingConvention(); al final de OnModelCreating.⚡ Paso 4: Capa de Aplicación (Covenants/Application/)CreateCovenantError.cs (Enum): Define los errores controlados de negocio (ej. DuplicateDocumentIdentifier, InvalidPeriod, UnexpectedError).CovenantCommandService.cs: Implementa ICovenantCommandService. Valida que no exista otro convenio con el mismo DocumentIdentifier, crea la entidad, la guarda mediante el repositorio y confirma la transacción con IUnitOfWork.CovenantQueryService.cs: Implementa ICovenantQueryService para resolver las consultas de lectura (Handle).🌐 Paso 5: Capa de Presentación / REST (Covenants/Interfaces/REST/)Resources (Resources/):CreateCovenantResource.cs: DTO con los datos de entrada enviados en el cuerpo (JSON) de la petición POST.CovenantResource.cs: DTO de salida con los campos planos solicitados (id, documentId, clientId, periodStartDate, periodEndDate, monetaryAmountValue, monetaryAmountCurrency, status, footnotes).Transform / Assemblers (Transform/):CreateCovenantCommandFromResourceAssembler.cs: Convierte CreateCovenantResource en CreateCovenantCommand.CovenantResourceFromEntityAssembler.cs: Convierte la entidad Covenant en CovenantResource.ActionResultFromCreateCovenantResultAssembler.cs: Traduce el resultado (Success o Failure) en códigos de estado HTTP adecuados (201 Created, 409 Conflict, 400 Bad Request, 500 Internal Server Error) usando mensajes localizados (IStringLocalizer).Controller (CovenantsController.cs):Configura la ruta con convención Kebab-Case: [Route("api/v1/[controller]")] $\rightarrow$ /api/v1/covenants.Inyecta ICovenantCommandService, ICovenantQueryService e IStringLocalizer<SharedResource>.Expone los endpoints POST y GET.🚀 Paso 6: Configuración Final en Program.csVerifica que el archivo Program.cs conecte todas las piezas en el contenedor de Inyección de Dependencias (DI):// 1. Configuración de la base de datos MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (connectionString != null)
        options.UseMySQL(connectionString);
});

// 2. Registro de dependencias (Scope)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICovenantRepository, CovenantRepository>();
builder.Services.AddScoped<ICovenantCommandService, CovenantCommandService>();
builder.Services.AddScoped<ICovenantQueryService, CovenantQueryService>();

var app = builder.Build();

// 3. Creación automática de base de datos y tablas al iniciar
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// 4. Pipeline HTTP, Localización y Documentación OpenAPI / Scalar
app.UseExceptionHandler();
app.UseRequestLocalization(localizationOptions);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();
app.Run();

