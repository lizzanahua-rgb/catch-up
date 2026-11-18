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

Ver **GUIA_ADAPTACION.md** para recrear cualquier otro caso con cambios mínimos.
