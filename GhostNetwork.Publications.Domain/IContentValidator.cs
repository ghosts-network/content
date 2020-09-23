using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostNetwork.Publications.Domain
{
    public interface IContentValidator
    {
        bool FindenWords(string content);
    }

    public class ForbiddenWordEntity
    {
        public string ForbiddenWord { get; set; }
    }

    public class ContentValidator : IContentValidator
    {
        private readonly List<ForbiddenWordEntity> forbidden;

        public ContentValidator()
        {
            forbidden = new List<ForbiddenWordEntity>();
            forbidden.Add(new ForbiddenWordEntity { ForbiddenWord = "duck" });
            forbidden.Add(new ForbiddenWordEntity { ForbiddenWord = "dog" });
            forbidden.Add(new ForbiddenWordEntity { ForbiddenWord = "cat" });
        }

        public bool FindenWords(string content)
        {
            var list = new List<string>();

            foreach (var s in forbidden.Select(x => x.ForbiddenWord))
            {
                if (content.Contains(s))
                {
                    list.Add(s);
                }
            }

            if (list.Count > 0)
            {
                return false;
            }

            return true;
        }
    }
}
