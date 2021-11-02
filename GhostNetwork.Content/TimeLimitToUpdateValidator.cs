using System;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;
using GhostNetwork.Content.Comments;
using GhostNetwork.Content.Publications;

namespace GhostNetwork.Content
{
    public class TimeLimitToUpdateValidator : IValidator<Publication>, IValidator<Comment>
    {
        private readonly TimeSpan timeLimit;

        public TimeLimitToUpdateValidator(TimeSpan timeLimit)
        {
            this.timeLimit = timeLimit;
        }

        public Task<DomainResult> ValidateAsync(Publication publication)
        {
            if (publication == null)
            {
                throw new ArgumentNullException();
            }

            return Task.FromResult(Validate(publication.CreatedOn));
        }

        public Task<DomainResult> ValidateAsync(Comment comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException();
            }

            return Task.FromResult(Validate(comment.CreatedOn));
        }

        private DomainResult Validate(DateTimeOffset createdOn)
        {
            return createdOn.Add(timeLimit) < DateTimeOffset.UtcNow ?
                DomainResult.Error($"Entity cannot update after {timeLimit.TotalMinutes} minutes after it was created") :
                DomainResult.Success();
        }
    }
}