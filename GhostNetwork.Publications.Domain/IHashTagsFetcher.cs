using System.Collections.Generic;

namespace GhostNetwork.Publications.Domain
{
    public interface IHashTagsFetcher
    {
        IEnumerable<string> Fetch(string text);
    }

    public class DefaultHashTagsFetcher : IHashTagsFetcher
    {
        public IEnumerable<string> Fetch(string text)
        {
            var tags = new List<string>();

            var hashTagStart = -1;
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '#')
                {
                    if (hashTagStart != -1)
                    {
                        tags.Add(text.Substring(hashTagStart + 1, i - hashTagStart - 1));
                    }

                    hashTagStart = i;
                    continue;
                }

                if (hashTagStart == -1 || char.IsLetter(text[i]) || text[i] == '_')
                {
                    continue;
                }

                tags.Add(text.Substring(hashTagStart + 1, i - hashTagStart - 1));
                hashTagStart = -1;
            }

            if (hashTagStart != -1)
            {
                tags.Add(text.Substring(hashTagStart + 1, text.Length - hashTagStart - 1));
            }

            return tags;
        }
    }
}