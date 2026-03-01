namespace Application.DTOs.Response
{
    public record PagedResponse<T>(
        IEnumerable<T> Items,
        int CurrentPage,
        int PageSize,
        int TotalCount,
        int TotalPages,
        bool HasNext,
        bool HasPrevious
    );
}
