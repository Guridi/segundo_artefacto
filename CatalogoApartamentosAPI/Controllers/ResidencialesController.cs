using CatalogoApartamentosAPI.Interfaces;
using CatalogoApartamentosAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoApartamentosAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResidencialesController : ControllerBase
{
    private readonly IResidencialService _residencialService;
    private readonly ILogger<ResidencialesController> _logger;
    private readonly IConfiguration _configuration;

    public ResidencialesController(
        IResidencialService residencialService,
        ILogger<ResidencialesController> logger,
        IConfiguration configuration)
    {
        _residencialService = residencialService;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetResidenciales()
    {
        _logger.LogInformation(
            "{Mensaje}",
            _configuration["MiConfiguracion:Mensaje"]);

        return Ok(_residencialService.GetResidenciales());
    }

    [HttpGet("{id:int}")]
    public IActionResult GetResidencialById(int id)
    {
        var residencial = _residencialService.GetResidencialById(id);

        if (residencial == null)
        {
            return NotFound("El residencial no existe.");
        }

        return Ok(residencial);
    }

    [HttpPost]
    public IActionResult CrearResidencial(Residencial residencial)
    {
        residencial.Id = _residencialService.CrearResidencial(residencial);

        _logger.LogInformation("Residencial creado.");

        return CreatedAtAction(
            nameof(GetResidencialById),
            new { id = residencial.Id },
            residencial);
    }

    [HttpPut("{id:int}")]
    public IActionResult ActualizarResidencial(
        int id,
        Residencial residencial)
    {
        bool actualizado =
            _residencialService.ActualizarResidencial(id, residencial);

        if (!actualizado)
        {
            return NotFound("El residencial no existe.");
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult EliminarResidencial(int id)
    {
        if (_residencialService.TieneApartamentos(id))
        {
            _logger.LogWarning(
                "No se puede eliminar el residencial {Id} porque tiene apartamentos.",
                id);

            return Conflict("No se puede eliminar un residencial que tiene apartamentos.");
        }

        bool eliminado =
            _residencialService.EliminarResidencial(id);

        if (!eliminado)
        {
            return NotFound("El residencial no existe.");
        }

        return NoContent();
    }
}