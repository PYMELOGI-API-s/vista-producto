using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
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

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<IEnumerable<Product>> GetProducts()
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/productos");
            request.Headers.Add("Accept", "application/json");
            
            Console.WriteLine($"Realizando solicitud GET a: {_httpClient.BaseAddress}/api/productos");
            var response = await _httpClient.SendAsync(request);
            
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status Code: {(int)response.StatusCode} ({response.StatusCode})");
            Console.WriteLine($"Headers: {string.Join(", ", response.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"))}");
            Console.WriteLine($"Respuesta recibida: {content}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"La API respondió con status code: {response.StatusCode}");
            }

            // Intenta deserializar primero como una lista directa
            try
            {
                var productList = JsonSerializer.Deserialize<List<Product>>(content, _jsonOptions);
                if (productList != null)
                {
                    Console.WriteLine($"Deserializado exitosamente como List<Product>. Productos encontrados: {productList.Count}");
                    return productList;
                }
            }
            catch (JsonException)
            {
                Console.WriteLine("No se pudo deserializar como List<Product>, intentando como ApiResponse<List<Product>>");
            }

            // Si falla, intenta deserializar como ApiResponse
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<Product>>>(content, _jsonOptions);
            if (apiResponse?.Data != null)
            {
                Console.WriteLine($"Deserializado exitosamente como ApiResponse. Productos encontrados: {apiResponse.Data.Count}");
                return apiResponse.Data;
            }

            Console.WriteLine("No se pudieron obtener productos de la respuesta");
            return new List<Product>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener productos: {ex.GetType().Name} - {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            throw new Exception("No se pudieron obtener los productos. Por favor, intente más tarde.", ex);
        }
    }

    public async Task<Product?> GetProduct(int id)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/productos/{id}");
            request.Headers.Add("Accept", "application/json");
            
            Console.WriteLine($"Realizando solicitud GET a: {_httpClient.BaseAddress}/api/productos/{id}");
            var response = await _httpClient.SendAsync(request);
            
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status Code: {(int)response.StatusCode} ({response.StatusCode})");
            Console.WriteLine($"Headers: {string.Join(", ", response.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"))}");
            Console.WriteLine($"Respuesta recibida: {content}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"La API respondió con status code: {response.StatusCode}");
            }

            // Intenta deserializar directamente como Product
            try
            {
                var product = JsonSerializer.Deserialize<Product>(content, _jsonOptions);
                if (product != null)
                {
                    Console.WriteLine("Deserializado exitosamente como Product");
                    return product;
                }
            }
            catch (JsonException)
            {
                Console.WriteLine("No se pudo deserializar como Product, intentando como ApiResponse<Product>");
            }

            // Si falla, intenta deserializar como ApiResponse
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<Product>>(content, _jsonOptions);
            if (apiResponse?.Data != null)
            {
                Console.WriteLine("Deserializado exitosamente como ApiResponse<Product>");
                return apiResponse.Data;
            }

            Console.WriteLine("No se pudo obtener el producto de la respuesta");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener producto {id}: {ex.GetType().Name} - {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            throw new Exception($"No se pudo obtener el producto {id}. Por favor, intente más tarde.", ex);
        }
    }

    public async Task<Product> CreateProduct(Product product)
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(product);
            Console.WriteLine($"Realizando solicitud POST a /productos con datos: {json}");
            
            var response = await _httpClient.PostAsJsonAsync("/productos", product);
            Console.WriteLine($"Status Code: {response.StatusCode}");
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response (Create): {content}");
            
            response.EnsureSuccessStatusCode();
            
            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<Product>>(content, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            return apiResponse?.Data ?? throw new Exception("No se pudo crear el producto: la respuesta no contiene datos");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error al crear producto - HTTP: {ex.Message}");
            throw new Exception($"Error al crear el producto: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear producto: {ex.Message}");
            throw new Exception($"Error inesperado al crear el producto: {ex.Message}", ex);
        }
    }

    public async Task<Product> UpdateProduct(int id, Product product)
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(product);
            Console.WriteLine($"Realizando solicitud PUT a /productos/{id} con datos: {json}");
            
            var response = await _httpClient.PutAsJsonAsync($"/productos/{id}", product);
            Console.WriteLine($"Status Code: {response.StatusCode}");
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response (Update): {content}");
            
            response.EnsureSuccessStatusCode();
            
            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<Product>>(content, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            return apiResponse?.Data ?? throw new Exception("No se pudo actualizar el producto: la respuesta no contiene datos");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error al actualizar producto - HTTP: {ex.Message}");
            throw new Exception($"Error al actualizar el producto: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al actualizar producto: {ex.Message}");
            throw new Exception($"Error inesperado al actualizar el producto: {ex.Message}", ex);
        }
    }

    public async Task DeleteProduct(int id)
    {
        try
        {
            Console.WriteLine($"Realizando solicitud DELETE a /productos/{id}");
            var response = await _httpClient.DeleteAsync($"/productos/{id}");
            Console.WriteLine($"Status Code: {response.StatusCode}");
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response (Delete): {content}");
            
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error al eliminar producto - HTTP: {ex.Message}");
            throw new Exception($"Error al eliminar el producto: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar producto: {ex.Message}");
            throw new Exception($"Error inesperado al eliminar el producto: {ex.Message}", ex);
        }
    }
}