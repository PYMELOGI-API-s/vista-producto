using System.Net.Http.Json;
using BlazorApp.Models;

namespace BlazorApp.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetProducts();
    Task<Product?> GetProduct(int id);
    Task<Product> CreateProduct(Product product);
    Task<Product> UpdateProduct(int id, Product product);
    Task DeleteProduct(int id);
}

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/productos");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<List<Product>>>(content, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return result?.Data ?? new List<Product>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error de conexión con la API: {ex.Message}");
            throw new Exception("No se pudo conectar con el servidor. Por favor, intente más tarde.", ex);
        }
        catch (System.Text.Json.JsonException ex)
        {
            Console.WriteLine($"Error al procesar la respuesta de la API: {ex.Message}");
            throw new Exception("Error al procesar los datos del servidor.", ex);
        }
    }

    public async Task<Product?> GetProduct(int id)
    {
        var response = await _httpClient.GetFromJsonAsync<ApiResponse<Product>>($"/api/productos/{id}");
        return response?.Data;
    }

    public async Task<Product> CreateProduct(Product product)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/productos", product);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<Product>>();
        return result?.Data ?? throw new Exception("No se pudo crear el producto");
    }

    public async Task<Product> UpdateProduct(int id, Product product)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/productos/{id}", product);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<Product>>();
        return result?.Data ?? throw new Exception("No se pudo actualizar el producto");
    }

    public async Task DeleteProduct(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/productos/{id}");
        response.EnsureSuccessStatusCode();
    }
}