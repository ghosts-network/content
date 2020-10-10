using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostNetwork.Publications
{
    public class DomainResult
    {
        private DomainResult()
            : this(Enumerable.Empty<DomainError>())
        {
        }

        private DomainResult(IEnumerable<DomainError> errors)
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        public IEnumerable<DomainError> Errors { get; }

        public bool Success => !Errors.Any();

        /// <summary>
        /// Build success result
        /// </summary>
        public static DomainResult Successed() => new DomainResult(Enumerable.Empty<DomainError>());

        /// <summary>
        /// Build unsucceeded result from string message
        /// </summary>
        public static DomainResult Error(string error) => Error(new[] { error }.Select(e => new DomainError(e)));

        /// <summary>
        /// Build unsucceeded result from DomainError
        /// </summary>
        public static DomainResult Error(DomainError error) => Error(new[] { error });

        /// <summary>
        /// Build unsucceeded result from array of errors
        /// </summary>
        public static DomainResult Error(IEnumerable<DomainError> errors) => new DomainResult(errors);
    }
}
