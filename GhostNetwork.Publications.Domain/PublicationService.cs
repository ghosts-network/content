using System;
using System.Collections.Generic;
using System.Text;

namespace GhostNetwork.Publications.Domain
{
    public class PublicationService : IPublicationService
    {
        private ILengthValidator lengthValidator;

        public PublicationService(ILengthValidator lengthValidator)
        {
            this.lengthValidator = lengthValidator;
        }

        public bool ValidateLength(string text)
        {
            return lengthValidator.Validate(text);
        }
    }
}
