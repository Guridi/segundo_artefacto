using System.ComponentModel.DataAnnotations;

namespace CatalogoApartamentosWeb.Models;

public class Residencial
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    public string Ubicacion { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Descripcion { get; set; } = string.Empty;
}
