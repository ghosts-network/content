using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public class CommentsService : ICommentsService
    {
        private readonly ICommentsStorage commentStorage;
        private readonly IPublicationStorage publicationStorage;
        private readonly ICommentLengthValidator lengthValidator;

        public CommentsService(ICommentsStorage commentStorage, IPublicationStorage publicationStorage, ICommentLengthValidator lengthValidator)
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
                return (DomainResult.Error("Publication not found"), null);
            }

            if (replyCommentId == null || await commentStorage.IsCommentInPublicationAsync(replyCommentId, publicationId))
            {
                var result = await lengthValidator.ValidateAsync(new CommentContext(text));
                if (!result.Success)
                {
                    return (result, null);
                }

                var comment = new Comment(string.Empty, text, DateTimeOffset.Now, publicationId, replyCommentId);

                return (result, await commentStorage.InsertOneAsync(comment));
            }

            return (DomainResult.Error("Comment id not found"), null);
        }

        public Task<Comment> FindOneByIdAsync(string id)
        {
            return commentStorage.FindOneByIdAsync(id);
        }

        public async Task<IEnumerable<Comment>> FindManyAsync(string publicationId, int skip, int take)
        {
            if (await publicationStorage.FindOneByIdAsync(publicationId) == null)
            {
                return null;
            }

            return await commentStorage.FindManyAsync(publicationId, skip, take);
        }
    }
}
