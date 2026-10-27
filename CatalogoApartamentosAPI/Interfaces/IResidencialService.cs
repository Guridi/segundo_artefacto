using CatalogoApartamentosAPI.Models;

namespace CatalogoApartamentosAPI.Interfaces;

public interface IResidencialService
{
    IEnumerable<Residencial> GetResidenciales();
    Residencial? GetResidencialById(int id);
    int CrearResidencial(Residencial residencial);
    bool ActualizarResidencial(int id, Residencial residencial);
    bool EliminarResidencial(int id);
    bool TieneApartamentos(int id);
}
