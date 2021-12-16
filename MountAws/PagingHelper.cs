namespace MountAws;

public class PagingHelper
{
    public static IEnumerable<TReturn> GetWithPaging<TReturn>(
        Func<string?, PaginatedResponse<TReturn>> requestPageAction)
    {
        string? nextToken = null;
        do
        {
            var response = requestPageAction(nextToken);
            foreach (var result in response.PageOfResults)
            {
                yield return result;
            }

            nextToken = response.NextToken;
        } while (nextToken != null);
    }
}

public record PaginatedResponse<T>
{
    public T[] PageOfResults { get; init; } = Array.Empty<T>();
    public string? NextToken { get; init; }
}