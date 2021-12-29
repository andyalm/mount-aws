namespace MountAws;

public class PagingHelper
{
    public static IEnumerable<TReturn> GetWithPaging<TReturn>(
        Func<string?, PaginatedResponse<TReturn>> requestPageAction, int? maxPages = null)
    {
        string? nextToken = null;
        int totalPages = 0;
        do
        {
            totalPages += 1;
            var response = requestPageAction(nextToken);
            foreach (var result in response.PageOfResults)
            {
                yield return result;
            }

            nextToken = response.NextToken;
        } while (nextToken != null && (maxPages == null || totalPages < maxPages));
    }
}

public record PaginatedResponse<T>
{
    public T[] PageOfResults { get; init; } = Array.Empty<T>();
    public string? NextToken { get; init; }
}