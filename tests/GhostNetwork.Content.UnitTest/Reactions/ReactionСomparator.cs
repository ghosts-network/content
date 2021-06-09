using System.Collections.Generic;
using System.Linq;
using GhostNetwork.Content.Reactions;

namespace GhostNetwork.Content.UnitTest.Reactions
{
    public static class ReactionСomparator
    {
        public static bool Compare(Reaction obj1, Reaction obj2)
        {
            if (obj1 == null && obj2 == null)
            {
                return true;
            }

            if ((obj1 == null && obj2 != null) || (obj2 == null && obj1 != null))
            {
                return false;
            }

            if (obj1.Key != obj2.Key && obj1.Type != obj2.Type)
            {
                return false;
            }

            return true;
        }

        public static bool Compare(IEnumerable<Reaction> collection1, IEnumerable<Reaction> collection2)
        {
            if (collection1.Count() != collection2.Count())
            {
                return false;
            }

            foreach (var item in collection1.Zip(collection2, (one, two) => new { one, two }))
            {
                if (!Compare(item.one, item.two))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CompareStats(IDictionary<string, int> stats1, IDictionary<string, int> stats2)
        {
            if (stats1.Count() != stats2.Count())
            {
                return false;
            }

            foreach (var item in stats1.Zip(stats2, (one, two) => new { one, two }))
            {
                if (item.one.Key != item.two.Key || item.one.Value != item.two.Value)
                {
                    return false;
                }
            }

            return true;
        }
    }
}