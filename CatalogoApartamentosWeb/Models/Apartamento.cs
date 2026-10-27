using System.ComponentModel.DataAnnotations;

namespace CatalogoApartamentosWeb.Models;

public class Apartamento
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El número es obligatorio.")]
    [StringLength(20, ErrorMessage = "El número no puede tener más de 20 caracteres.")]
    public string Numero { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Debe tener al menos 1 habitación.")]
    public int Habitaciones { get; set; } = 1;

    [Range(1, int.MaxValue, ErrorMessage = "Debe tener al menos 1 baño.")]
    public int Banos { get; set; } = 1;

    [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "El precio debe ser mayor que cero.")]
    public decimal Precio { get; set; }

    public bool Disponible { get; set; } = true;

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un residencial.")]
    public int ResidencialId { get; set; }
}
