# Catálogo de Apartamentos · Sitio Blazor

Segundo artefacto: sitio web en **Blazor Web App (Interactive Server, .NET 10)** que consume el Web API `CatalogoApartamentosAPI` (primer artefacto).

## Cómo ejecutarlo

1. Levantar SQL Server y el API:
   ```bash
   cd ../CatalogoApartamentosAPI
   dotnet run --launch-profile https      # o --launch-profile docker
   ```
   El API queda en `https://localhost:7143`.
2. Levantar el sitio:
   ```bash
   cd ../CatalogoApartamentosWeb
   dotnet run --launch-profile https
   ```
   El sitio queda en `https://localhost:7215`.

> El sitio llama al API por HTTPS, así que el certificado de desarrollo debe ser de confianza (`dotnet dev-certs https --trust`). En Windows con Visual Studio o Rider ya viene configurado.

## Requisitos → dónde se cumplen

| Requisito | Archivo(s) |
|---|---|
| Landing page | `Components/Pages/Home.razor` |
| Al menos dos páginas | `Components/Pages/Residenciales.razor`, `Components/Pages/Apartamentos.razor` |
| Al menos dos componentes por página | Inicio: `HeroBanner`, `TarjetaCaracteristica`, `ResumenCatalogo` · Residenciales: `BuscadorResidencial`, `TarjetaResidencial`, `MensajeEstado` · Apartamentos: `PanelFiltros`, `TablaApartamentos`, `FormularioApartamento`, `MensajeEstado` (todos en `Components/Shared/`) |
| Sintaxis Razor | `@page`, `@inject`, `@code`, `@if`, `@foreach`, `@bind`, `@bind:after`, `@onclick`, `@(...)`, `RenderFragment ChildContent` |
| Comunicación entre componentes | Padre → hijo con `[Parameter]` (por ejemplo `TablaApartamentos`) · Hijo → padre con `EventCallback` (`BuscadorResidencial.OnBuscar`, `PanelFiltros.OnFiltroCambiado`, `FormularioApartamento.OnApartamentoCreado`, `TarjetaResidencial.OnVerApartamentos`) · Ancestro → descendientes con `CascadingValue` (`MainLayout` → `NombreSitio`) · Entre páginas con query string (`[SupplyParameterFromQuery] ResidencialId`) |
| Elementos estáticos | `wwwroot/css/site.css`, `wwwroot/js/site.js` (llamado con `IJSRuntime`), `wwwroot/img/*.svg` |
| Header y footer personalizados | `Components/Layout/MainLayout.razor`, `EncabezadoSitio.razor`, `PieDePagina.razor` |
| Conexión al Web API mostrando información | Las tres páginas muestran datos del API |
| Comunicación con el API desde una clase de servicios | `Interfaces/ICatalogoApiService.cs`, `Services/CatalogoApiService.cs` |
| `Program.cs` configurado | Registro de Razor Components y del `HttpClient` tipado con `BaseUrl` y timeout leídos de la configuración |
| `appsettings.json` configurado | Secciones `Logging`, `CatalogoApi` (BaseUrl, timeout, endpoints) y `Sitio` |
| DI de `ILogger` e `IConfiguration` usada en el servicio | Constructor de `CatalogoApiService`: `IConfiguration` lee los endpoints y `ILogger` registra cada llamada (Information, Warning y Error) |

## Para la presentación

- **Funcionamiento externo:** Inicio → Residenciales (buscar y "Ver apartamentos") → Apartamentos (filtro preseleccionado, crear un apartamento y ver el aviso).
- **Funcionamiento interno:** `Program.cs` → `appsettings.json` → `CatalogoApiService` → logs en la consola del sitio mientras se navega. Si se detiene el API, las páginas muestran un error con el botón "Reintentar" y la consola registra un `fail`.
