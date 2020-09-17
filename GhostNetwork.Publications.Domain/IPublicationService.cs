using System;
using System.Collections.Generic;
using System.Text;

namespace GhostNetwork.Publications.Domain
{
    public interface IPublicationService
    {
        bool ValidateLength(string text);
    }
}
