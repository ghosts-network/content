using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Content.Reactions;
using StackExchange.Redis;

namespace GhostNetwork.Content.Redis;

public class RedisReactionStorage : IReactionStorage
{
    private readonly IDatabase db;

    public RedisReactionStorage(IDatabase db)
    {
        this.db = db;
    }

    public async Task<IDictionary<string, int>> GetStats(string key)
    {
        var result = await db.SortedSetRangeByScoreWithScoresAsync(key, order: Order.Descending);

        return result?.ToDictionary(x => (string) x.Element, x => (int) x.Score)
               ?? new Dictionary<string, int>();
    }

    public async Task<Reaction?> GetReactionByAuthorAsync(string key, string author)
    {
        var result = await db.HashGetAsync($"u:{key}", author);
        return result.HasValue ? new Reaction(key, (string) result) : null;
    }

    public async Task<IEnumerable<Reaction>> GetReactionsByAuthorAsync(string author, IReadOnlyCollection<string> keys)
    {
        var result = new List<Reaction>(keys.Count);
        foreach (var key in keys)
        {
            var reaction = await GetReactionByAuthorAsync(key, author);
            result.Add(new Reaction(key, reaction?.Type));
        }

        return result;
    }

    public async Task<IDictionary<string, Dictionary<string, int>>> GetGroupedReactionsAsync(IEnumerable<string> keys)
    {
        var dict = new Dictionary<string, Dictionary<string, int>>(keys.Count());
        foreach (var key in keys)
        {
            dict[key] = (Dictionary<string, int>) await GetStats(key);
        }

        return dict;
    }

    public async Task UpsertAsync(string key, string author, string type)
    {
        var oldType = await db.HashGetAsync("u:" + key, author);
        if (oldType.HasValue && (string) oldType == type)
        {
            return;
        }

        if (await db.HashSetAsync("u:" + key, author, type))
        {
            await db.SortedSetIncrementAsync(key, type, 1);
        }
        else
        {
            await db.SortedSetDecrementAsync(key, (string) oldType, 1);
            await db.SortedSetIncrementAsync(key, type, 1);
        }
    }

    public async Task DeleteByAuthorAsync(string key, string author)
    {
        var value = await db.HashGetAsync("u:" + key, author);
        if (value.HasValue)
        {
            await db.HashDeleteAsync("u:" + key, author);
            await db.SortedSetDecrementAsync(key, value, 1);
        }
    }

    public async Task DeleteAsync(string key)
    {
        await db.KeyDeleteAsync(key);
        await db.KeyDeleteAsync($"u:{key}");
    }
}
