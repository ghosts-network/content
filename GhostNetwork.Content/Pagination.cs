namespace GhostNetwork.Content;

public record Pagination(string Cursor, int Limit, int Skip)
{
    public static Pagination ByCursor(string cursor, int limit) => new Pagination(cursor, limit, default);

    public static Pagination BySkip(int skip, int limit) => new Pagination(default, limit, skip);
}
