# Restaurant Management API

API REST del sistema de gestión de restaurante, desarrollada con ASP.NET Core,
Entity Framework Core y SQLite.

## Requisitos

- .NET SDK 10

## Ejecución

```bash
dotnet restore RestaurantManagement.Api/RestaurantManagement.Api.csproj
dotnet run --project RestaurantManagement.Api/RestaurantManagement.Api.csproj
```

La API usa `http://localhost:5000` según `Properties/launchSettings.json`.
Swagger está disponible en `/swagger` cuando el entorno es `Development`.

## Compilación

```bash
dotnet build RestaurantManagement.Api/RestaurantManagement.Api.csproj
```

La base SQLite se crea y migra automáticamente al iniciar la aplicación.

## Configuración

La cadena de conexión y los orígenes permitidos por CORS se definen en
`RestaurantManagement.Api/appsettings.json`. En despliegues reales conviene
sobrescribirlos mediante variables de entorno o configuración del proveedor.
