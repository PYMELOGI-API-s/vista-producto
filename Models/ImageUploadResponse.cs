namespace BlazorApp.Models;

public class ImageUploadResponse
{
    public bool Success { get; set; }
    public string? ImageUrl { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
}