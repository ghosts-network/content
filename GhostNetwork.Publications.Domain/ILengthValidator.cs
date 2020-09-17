using System;
using System.Collections.Generic;
using System.Text;

namespace GhostNetwork.Publications.Domain
{
    public interface ILengthValidator
    {
        bool Validate(string text);
    }

    public class DefaultLengthValidator : ILengthValidator
    {
        private readonly int? length;

        public DefaultLengthValidator(int? length = null)
        {
            this.length = length;
        }

        public bool Validate(string text)
        {
            if (length == null)
            {
                return true;
            }

            if (text.Length < length)
            {
                return false;
            }

            return true;
        }
    }
}
