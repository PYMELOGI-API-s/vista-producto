namespace BlazorApp.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
    public List<string>? Details { get; set; }
    public PaginationInfo? Pagination { get; set; }
    public Dictionary<string, object>? Filters { get; set; }
}

public class PaginationInfo
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
}