namespace Otlob.ApiErrors;

public static class SearchApiError
{
    public static readonly ApiError EmptyQuery = 
        new(Code: "SearchErrors.EmptyQuery", "The search query cannot be empty.", StatusCodes.Status400BadRequest);

    public static readonly ApiError CategoryNotFound = 
        new(Code: "SearchErrors.CategoryNotFound", "No category found with the specified name.", StatusCodes.Status404NotFound);
}
