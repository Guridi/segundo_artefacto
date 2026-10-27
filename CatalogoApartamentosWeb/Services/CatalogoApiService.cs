using System.Net;
using System.Net.Http.Json;
using CatalogoApartamentosWeb.Interfaces;
using CatalogoApartamentosWeb.Models;

namespace CatalogoApartamentosWeb.Services;

public class CatalogoApiService : ICatalogoApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CatalogoApiService> _logger;
    private readonly string _rutaResidenciales;
    private readonly string _rutaApartamentos;

    public CatalogoApiService(
        HttpClient httpClient,
        ILogger<CatalogoApiService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;

        _rutaResidenciales = configuration["CatalogoApi:Endpoints:Residenciales"]
            ?? throw new InvalidOperationException("No se configuró CatalogoApi:Endpoints:Residenciales.");
        _rutaApartamentos = configuration["CatalogoApi:Endpoints:Apartamentos"]
            ?? throw new InvalidOperationException("No se configuró CatalogoApi:Endpoints:Apartamentos.");

        _logger.LogDebug(
            "CatalogoApiService creado. API base: {BaseUrl}",
            _httpClient.BaseAddress);
    }

    public async Task<List<Residencial>> GetResidencialesAsync()
    {
        _logger.LogInformation("Consultando residenciales en {Ruta}.", _rutaResidenciales);

        try
        {
            var residenciales =
                await _httpClient.GetFromJsonAsync<List<Residencial>>(_rutaResidenciales) ?? [];

            _logger.LogInformation("Se recibieron {Cantidad} residenciales.", residenciales.Count);

            return residenciales;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogError(ex, "Error al consultar los residenciales.");
            throw;
        }
    }

    public async Task<Residencial?> GetResidencialByIdAsync(int id)
    {
        _logger.LogInformation("Consultando el residencial {Id}.", id);

        try
        {
            using var response = await _httpClient.GetAsync($"{_rutaResidenciales}/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("El residencial {Id} no existe.", id);
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Residencial>();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogError(ex, "Error al consultar el residencial {Id}.", id);
            throw;
        }
    }

    public async Task<List<Apartamento>> GetApartamentosAsync(FiltroApartamentos? filtro = null)
    {
        _logger.LogInformation("Consultando apartamentos en {Ruta}.", _rutaApartamentos);

        try
        {
            IEnumerable<Apartamento> apartamentos =
                await _httpClient.GetFromJsonAsync<List<Apartamento>>(_rutaApartamentos) ?? [];

            // El API no tiene endpoint de filtrado, se filtra al recibir los datos.
            if (filtro?.ResidencialId is int residencialId)
            {
                apartamentos = apartamentos.Where(a => a.ResidencialId == residencialId);
            }

            if (filtro?.SoloDisponibles == true)
            {
                apartamentos = apartamentos.Where(a => a.Disponible);
            }

            var resultado = apartamentos.ToList();

            _logger.LogInformation(
                "Se obtuvieron {Cantidad} apartamentos (residencial: {ResidencialId}, solo disponibles: {SoloDisponibles}).",
                resultado.Count,
                filtro?.ResidencialId?.ToString() ?? "todos",
                filtro?.SoloDisponibles ?? false);

            return resultado;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogError(ex, "Error al consultar los apartamentos.");
            throw;
        }
    }

    public async Task<Apartamento> CrearApartamentoAsync(Apartamento apartamento)
    {
        _logger.LogInformation(
            "Creando el apartamento {Numero} en el residencial {ResidencialId}.",
            apartamento.Numero,
            apartamento.ResidencialId);

        try
        {
            using var response = await _httpClient.PostAsJsonAsync(_rutaApartamentos, apartamento);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                string mensaje = await response.Content.ReadAsStringAsync();

                _logger.LogWarning("El API rechazó el apartamento: {Mensaje}", mensaje);

                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(mensaje) ? "Los datos del apartamento no son válidos." : mensaje);
            }

            response.EnsureSuccessStatusCode();

            var creado = await response.Content.ReadFromJsonAsync<Apartamento>()
                ?? throw new InvalidOperationException("El API no devolvió el apartamento creado.");

            _logger.LogInformation("Apartamento creado con Id {Id}.", creado.Id);

            return creado;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogError(ex, "Error al crear el apartamento {Numero}.", apartamento.Numero);
            throw;
        }
    }
}
