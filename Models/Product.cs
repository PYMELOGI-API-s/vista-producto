using System.Text.Json.Serialization;

namespace BlazorApp.Models;

public class Product
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nombre")]
    public string? Name { get; set; }

    [JsonPropertyName("descripcion")]
    public string? Description { get; set; }

    [JsonPropertyName("codigoBarras")]
    public string? Barcode { get; set; }

    [JsonPropertyName("precio")]
    public decimal Price { get; set; }

    [JsonPropertyName("stock")]
    public int Stock { get; set; }

    [JsonPropertyName("categoria")]
    public string? Category { get; set; }

    [JsonPropertyName("imagen")]
    public string? ImageUrl { get; set; }
}
