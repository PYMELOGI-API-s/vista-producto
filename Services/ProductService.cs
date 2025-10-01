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
    private readonly ILogger<ProductService> _logger;

    public ProductService(HttpClient httpClient, ILogger<ProductService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
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
            using var request = new HttpRequestMessage(HttpMethod.Get, "api/productos");
            request.Headers.Add("Accept", "application/json");
            
            Console.WriteLine($"Realizando solicitud GET a: {_httpClient.BaseAddress}api/productos");
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
            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/productos/{id}");
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
            _logger.LogInformation($"Realizando solicitud POST a api/productos con datos: {json}");
            
            var response = await _httpClient.PostAsJsonAsync("api/productos", product);
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Status Code: {response.StatusCode}");
            _logger.LogInformation($"API Response (Create): {content}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = $"Error al crear producto: {response.StatusCode} - {content}";
                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }
            
            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<Product>>(content, _jsonOptions);
            
            if (apiResponse?.Success != true || apiResponse.Data == null)
            {
                throw new Exception("La API indicó que la operación no fue exitosa");
            }
            
            return apiResponse.Data;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"Error de red al crear producto: {ex.Message}");
            throw new Exception("Error de conexión con el servidor. Por favor, intente más tarde.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear producto: {ex.Message}");
            throw;
        }
    }

    public async Task<Product> UpdateProduct(int id, Product product)
    {
        try
        {
            // Asegurarnos de que el ID coincida
            product.Id = id;
            
            // Asegurarnos de que todos los campos requeridos estén presentes
            if (string.IsNullOrEmpty(product.Nombre))
                throw new Exception("El nombre del producto es requerido");
            if (string.IsNullOrEmpty(product.Descripcion))
                throw new Exception("La descripción del producto es requerida");
            if (string.IsNullOrEmpty(product.CodigoBarras))
                throw new Exception("El código de barras es requerido");
            if (product.Precio <= 0)
                throw new Exception("El precio debe ser mayor a 0");
            if (product.Stock < 0)
                throw new Exception("El stock no puede ser negativo");
            if (string.IsNullOrEmpty(product.Categoria))
                throw new Exception("La categoría es requerida");
            if (string.IsNullOrEmpty(product.Imagen))
                throw new Exception("La imagen es requerida");

            var json = System.Text.Json.JsonSerializer.Serialize(product);
            _logger.LogInformation($"Realizando solicitud PUT a api/productos/{id} con datos: {json}");
            
            var response = await _httpClient.PutAsJsonAsync($"api/productos/{id}", product);
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Status Code: {response.StatusCode}");
            _logger.LogInformation($"API Response (Update): {content}");

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
                    if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Message))
                    {
                        throw new Exception(errorResponse.Message);
                    }
                }
                catch
                {
                    // Si no podemos deserializar la respuesta de error, usamos el mensaje genérico
                    throw new Exception($"Error al actualizar el producto: {response.StatusCode} - {content}");
                }
            }
            
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
            var response = await _httpClient.DeleteAsync($"api/productos/{id}");
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