using System.Text.Json.Serialization;

namespace BlazorApp.Models;

public class Product
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nombre")]
    public string? Nombre { get; set; }

    [JsonPropertyName("descripcion")]
    public string? Descripcion { get; set; }

    [JsonPropertyName("codigoBarras")]
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El código de barras es requerido")]
    [System.ComponentModel.DataAnnotations.RegularExpression(@"^\d{10,15}$", ErrorMessage = "El código de barras debe contener entre 10 y 15 dígitos")]
    public string? CodigoBarras { get; set; }

    [JsonPropertyName("precio")]
    public decimal Precio { get; set; }

    [JsonPropertyName("stock")]
    public int Stock { get; set; }

    [JsonPropertyName("categoria")]
    public string? Categoria { get; set; }

    [JsonPropertyName("imagen")]
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "La imagen es requerida")]
    [System.ComponentModel.DataAnnotations.RegularExpression(@"^(/images?/|images?/).*\.(jpg|jpeg|png|gif|webp)$", ErrorMessage = "La ruta de la imagen debe comenzar con /images/ o images/ y terminar en .jpg, .jpeg, .png, .gif o .webp")]
    public string? Imagen { get; set; }

    [JsonPropertyName("fechaCreacion")]
    public DateTime FechaCreacion { get; set; }

    [JsonPropertyName("fechaActualizacion")]
    public DateTime FechaActualizacion { get; set; }
}
