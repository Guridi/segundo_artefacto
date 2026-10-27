using CatalogoApartamentosWeb.Models;

namespace CatalogoApartamentosWeb.Interfaces;

public interface ICatalogoApiService
{
    Task<List<Residencial>> GetResidencialesAsync();
    Task<Residencial?> GetResidencialByIdAsync(int id);
    Task<List<Apartamento>> GetApartamentosAsync(FiltroApartamentos? filtro = null);
    Task<Apartamento> CrearApartamentoAsync(Apartamento apartamento);
}
