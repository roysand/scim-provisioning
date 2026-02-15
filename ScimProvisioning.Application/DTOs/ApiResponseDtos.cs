namespace ScimProvisioning.Application.DTOs;

/// <summary>
/// Generic API response wrapper for successful operations
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The response data
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// Success message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    public ApiResponse(T data, string message = "Operation successful")
    {
        Success = true;
        Data = data;
        Message = message;
    }

    public ApiResponse()
    {
        Success = true;
    }
}

/// <summary>
/// API error response wrapper
/// </summary>
public class ApiErrorResponse
{
    /// <summary>
    /// Indicates the operation failed
    /// </summary>
    public bool Success { get; set; } = false;

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Error code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Additional error details
    /// </summary>
    public Dictionary<string, string[]> Errors { get; set; } = new();

    public ApiErrorResponse(string message, string code = "ERROR")
    {
        Message = message;
        Code = code;
    }

    public ApiErrorResponse()
    {
    }
}

/// <summary>
/// Paginated API response wrapper
/// </summary>
public class ApiPagedResponse<T>
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The paginated data
    /// </summary>
    public List<T> Data { get; set; } = new();

    /// <summary>
    /// Pagination information
    /// </summary>
    public PaginationMetadata Pagination { get; set; } = new();

    /// <summary>
    /// Success message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    public ApiPagedResponse(List<T> data, int totalResults, int startIndex, int itemsPerPage, string message = "Operation successful")
    {
        Success = true;
        Data = data;
        Message = message;
        Pagination = new PaginationMetadata
        {
            TotalResults = totalResults,
            StartIndex = startIndex,
            ItemsPerPage = itemsPerPage
        };
    }

    public ApiPagedResponse()
    {
        Success = true;
    }
}

/// <summary>
/// Pagination metadata
/// </summary>
public class PaginationMetadata
{
    public int TotalResults { get; set; }
    public int StartIndex { get; set; }
    public int ItemsPerPage { get; set; }
}

