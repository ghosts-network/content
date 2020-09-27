using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public class CommentService : ICommentService
    {
        private readonly ICommentStorage commentStorage;
        private readonly IPublicationStorage publicationStorage;
        private readonly ICommentLengthValidator lengthValidator;

        public CommentService(ICommentStorage commentStorage, IPublicationStorage publicationStorage, ICommentLengthValidator lengthValidator)
        {
            this.commentStorage = commentStorage;
            this.publicationStorage = publicationStorage;
            this.lengthValidator = lengthValidator;
        }

        public async Task<(DomainResult, string)> CreateAsync(string publicationId, string text, string replyCommentId)
        {
            var publication = await publicationStorage.FindOneByIdAsync(publicationId);
            if (publication == null)
            {
                var error = await Task.FromResult(DomainResult.Error("Publication not found"));
                return (error, null);
            }

            if (replyCommentId == null || await commentStorage.FindCommentInPublicationById(replyCommentId, publicationId))
            {
                var result = await lengthValidator.ValidateAsync(new CommentContext(text));
                if (!result.Success)
                {
                    return (result, null);
                }

                var comment = new Comment(string.Empty, text, DateTimeOffset.Now, publicationId, replyCommentId);
                var id = await commentStorage.InsertOneAsync(comment);
                return (result, id);
            }

            return (await Task.FromResult(DomainResult.Error("Comment id not found")), null);
        }

        public async Task<Comment> FindOneByIdAsync(string id)
        {
            var comment = await commentStorage.FindOneByIdAsync(id);
            return comment;
        }

        public async Task<IEnumerable<Comment>> FindManyAsync(string publicationId, int skip, int take)
        {
            var publications = await publicationStorage.FindOneByIdAsync(publicationId);

            if (publications == null)
            {
                return null;
            }

            var comments = await commentStorage.FindManyAsync(publicationId, skip, take);
            return comments;
        }
    }
}
