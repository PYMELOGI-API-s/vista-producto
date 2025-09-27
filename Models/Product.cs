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
    public string? CodigoBarras { get; set; }

    [JsonPropertyName("precio")]
    public decimal Precio { get; set; }

    [JsonPropertyName("stock")]
    public int Stock { get; set; }

    [JsonPropertyName("categoria")]
    public string? Categoria { get; set; }

    [JsonPropertyName("imagen")]
    public string? Imagen { get; set; }

    [JsonPropertyName("fechaCreacion")]
    public DateTime FechaCreacion { get; set; }

    [JsonPropertyName("fechaActualizacion")]
    public DateTime FechaActualizacion { get; set; }
}
