namespace MountAws;

public class PagingHelper
{
    public static IEnumerable<TReturn> Paginate<TReturn>(
        Func<string?, (IEnumerable<TReturn> PageOfResults, string? NextToken)> requestPageAction, int? maxPages = null)
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