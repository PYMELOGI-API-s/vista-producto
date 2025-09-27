namespace BlazorApp.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; } = default!;
    public PaginationInfo Pagination { get; set; } = default!;
}

public class PaginationInfo
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
}