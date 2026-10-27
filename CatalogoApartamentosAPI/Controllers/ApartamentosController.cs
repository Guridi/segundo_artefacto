using CatalogoApartamentosAPI.Interfaces;
using CatalogoApartamentosAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoApartamentosAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApartamentosController : ControllerBase
{
    private readonly IApartamentoService _apartamentoService;
    private readonly IResidencialService _residencialService;
    private readonly ILogger<ApartamentosController> _logger;

    public ApartamentosController(
        IApartamentoService apartamentoService,
        IResidencialService residencialService,
        ILogger<ApartamentosController> logger)
    {
        _apartamentoService = apartamentoService;
        _residencialService = residencialService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetApartamentos()
    {
        _logger.LogInformation("Consulta del listado de apartamentos.");

        return Ok(_apartamentoService.GetApartamentos());
    }

    [HttpGet("{id:int}")]
    public IActionResult GetApartamentoById(int id)
    {
        var apartamento =
            _apartamentoService.GetApartamentoById(id);

        if (apartamento == null)
        {
            return NotFound("El apartamento no existe.");
        }

        return Ok(apartamento);
    }

    [HttpPost]
    public IActionResult CrearApartamento(Apartamento apartamento)
    {
        var residencial =
            _residencialService.GetResidencialById(
                apartamento.ResidencialId);

        if (residencial == null)
        {
            return BadRequest("El residencial indicado no existe.");
        }

        apartamento.Id =
            _apartamentoService.CrearApartamento(apartamento);

        _logger.LogInformation("Apartamento creado.");

        return CreatedAtAction(
            nameof(GetApartamentoById),
            new { id = apartamento.Id },
            apartamento);
    }

    [HttpPut("{id:int}")]
    public IActionResult ActualizarApartamento(
        int id,
        Apartamento apartamento)
    {
        var residencial =
            _residencialService.GetResidencialById(
                apartamento.ResidencialId);

        if (residencial == null)
        {
            return BadRequest("El residencial indicado no existe.");
        }

        bool actualizado =
            _apartamentoService.ActualizarApartamento(
                id,
                apartamento);

        if (!actualizado)
        {
            return NotFound("El apartamento no existe.");
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult EliminarApartamento(int id)
    {
        bool eliminado =
            _apartamentoService.EliminarApartamento(id);

        if (!eliminado)
        {
            return NotFound("El apartamento no existe.");
        }

        return NoContent();
    }
}