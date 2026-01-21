using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.Common.Pagination;

public class PaginatedList<T> : List<T>
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize < 10 ? 10 : pageSize;
        CurrentPage = pageNumber <= 0 ? 1 : pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        AddRange(items);
    }

    public PaginatedList<TDTO> Map<TDTO>(Func<T, TDTO> mapper)
    {
        return new([.. this.Select(mapper)], TotalCount, CurrentPage, PageSize);
    }
}
