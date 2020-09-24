using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostNetwork.Publications.Domain.ContentValidation
{
    public interface IContentValidator
    {
        bool FindeForbiddenWords(string content);
    }
}
