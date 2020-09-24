using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostNetwork.Publications.Domain
{
    public interface IContentValidator
    {
        bool FindeForbiddenWords(string content);
    }

    public class ForbiddenWordModel
    {
        public string ForbiddenWord { get; set; }
    }

    public class ContentValidator : IContentValidator
    {
        private readonly List<ForbiddenWordModel> forbidden;

        public ContentValidator()
        {
            forbidden = new List<ForbiddenWordModel>();
            forbidden.Add(new ForbiddenWordModel { ForbiddenWord = "duck" });
            forbidden.Add(new ForbiddenWordModel { ForbiddenWord = "dog" });
            forbidden.Add(new ForbiddenWordModel { ForbiddenWord = "cat" });
        }

        public bool FindeForbiddenWords(string content)
        {
            foreach (var s in forbidden.Select(x => x.ForbiddenWord))
            {
                if (content.Contains(s))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
