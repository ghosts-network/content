using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Content.Reactions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace GhostNetwork.Content.Redis;

public class RedisReactionStorage : IReactionStorage
{
    private readonly IDatabase db;
    private readonly ILogger<RedisReactionStorage> logger;

    public RedisReactionStorage(IDatabase db, ILogger<RedisReactionStorage> logger)
    {
        this.db = db;
        this.logger = logger;
    }

    public async Task<IDictionary<string, int>> GetStats(string key)
    {
        var result = await logger.LogExecutionInfo(() =>
            db.SortedSetRangeByScoreWithScoresAsync(key, order: Order.Descending));

        return result?
                   .Where(r => r.Score > 0)
                   .ToDictionary(x => (string) x.Element, x => (int) x.Score)
               ?? new Dictionary<string, int>();
    }

    public async Task<Reaction?> GetReactionByAuthorAsync(string key, string author)
    {
        var result = await logger.LogExecutionInfo(() =>
            db.HashGetAsync($"u:{key}", author));

        return result.HasValue ? new Reaction(key, (string) result) : null;
    }

    public async Task<IEnumerable<Reaction>> GetReactionsByAuthorAsync(string author, IReadOnlyCollection<string> keys)
    {
        var result = new List<Reaction>(keys.Count);
        foreach (var key in keys)
        {
            var reaction = await GetReactionByAuthorAsync(key, author);

            if (reaction != null)
            {
                result.Add(new Reaction(key, reaction.Type));
            }
        }

        return result;
    }

    public async Task<IDictionary<string, Dictionary<string, int>>> GetGroupedReactionsAsync(IEnumerable<string> keys)
    {
        var dict = new Dictionary<string, Dictionary<string, int>>(keys.Count());
        foreach (var key in keys)
        {
            var stats = (Dictionary<string, int>) await GetStats(key);
            if (stats.Count != 0)
            {
                dict[key] = stats;
            }
        }

        return dict;
    }

    public async Task UpsertAsync(string key, string author, string type)
    {
        var oldType = await logger.LogExecutionInfo(() =>
            db.HashGetAsync("u:" + key, author));
        if (oldType.HasValue && (string) oldType == type)
        {
            return;
        }

        if (await logger.LogExecutionInfo(() => db.HashSetAsync("u:" + key, author, type)))
        {
            await logger.LogExecutionInfo(() => db.SortedSetIncrementAsync(key, type, 1));
        }
        else
        {
            await logger.LogExecutionInfo(() => db.SortedSetDecrementAsync(key, (string) oldType, 1));
            await logger.LogExecutionInfo(() => db.SortedSetIncrementAsync(key, type, 1));
        }
    }

    public async Task DeleteByAuthorAsync(string key, string author)
    {
        var value = await logger.LogExecutionInfo(() => db.HashGetAsync("u:" + key, author));
        if (value.HasValue)
        {
            await logger.LogExecutionInfo(() => db.HashDeleteAsync("u:" + key, author));
            await logger.LogExecutionInfo(() => db.SortedSetDecrementAsync(key, value, 1));
        }
    }

    public async Task DeleteAsync(string key)
    {
        await logger.LogExecutionInfo(() => db.KeyDeleteAsync(key));
        await logger.LogExecutionInfo(() => db.KeyDeleteAsync($"u:{key}"));
    }
}
