using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Content.Api.Helpers;
using GhostNetwork.Content.Api.Models;
using GhostNetwork.Content.Comments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Content.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService commentService;
        private readonly IUserProvider userProvider;

        public CommentsController(ICommentsService commentService, IUserProvider userProvider)
        {
            this.commentService = commentService;
            this.userProvider = userProvider;
        }

        /// <summary>
        /// Create comment
        /// </summary>
        /// <param name="model">Comment</param>
        /// <returns>Created comment</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Comment>> CreateAsync([FromBody] CreateCommentModel model)
        {
            var author = await userProvider.GetByIdAsync(model.AuthorId);

            var (domainResult, id) = await commentService
                .CreateAsync(model.Key, model.Content, model.ReplyCommentId, author);

            if (domainResult.Successed)
            {
                return Created(Url.Action("GetById", new { id }), await commentService.GetByIdAsync(id));
            }

            return BadRequest(domainResult.ToProblemDetails());
        }

        /// <summary>
        /// Update comment content
        /// </summary>
        /// <param name="commentId">Existing comment ID</param>
        /// <param name="content">New content</param>
        /// <returns>Updated comment</returns>
        [HttpPut("{commentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Comment>> UpdateAsync([FromRoute] string commentId, [FromBody][Required] string content)
        {
            var (domainResult, (comment, updateCount)) = await commentService.UpdateAsync(commentId, content);

            if (!domainResult.Successed)
            {
                return BadRequest();
            }

            return comment == null ? NotFound() : updateCount == 0 ? NoContent() : Ok(comment);
        }

        /// <summary>
        /// Get one comment by id
        /// </summary>
        /// <param name="id">Comment id</param>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Comment>> GetByIdAsync([FromRoute] string id)
        {
            var comment = await commentService.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        /// <summary>
        /// Search featured comments for keys
        /// </summary>
        /// <param name="model">Array of keys</param>
        /// <returns>Featured comments related to keys</returns>
        [HttpPost("comments/featured")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Dictionary<string, FeaturedInfo>>> SearchFeaturedAsync(
            [FromBody] FeaturedQuery model)
        {
            var result = await commentService.SearchFeaturedAsync(model.Keys);
            return Ok(result);
        }

        /// <summary>
        /// Search comments for key
        /// </summary>
        /// <param name="key">Comment key</param>
        /// <param name="skip">Skip comments up to a specified position</param>
        /// <param name="take">Take comments up to a specified position</param>
        /// <returns>Comments related to key</returns>
        [HttpGet("bykey/{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Comment>>> SearchByKeyAsync(
            [FromRoute] string key,
            [FromQuery, Range(0, int.MaxValue)] int skip,
            [FromQuery, Range(0, 100)] int take = 10)
        {
            var (comments, totalCount) = await commentService.SearchAsync(key, skip, take);
            Response.Headers.Add("X-TotalCount", totalCount.ToString());

            return Ok(comments);
        }

        /// <summary>
        /// Delete one comment
        /// </summary>
        /// <param name="id">Comment id</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Comment>>> DeleteAsync([FromRoute] string id)
        {
            if (await commentService.GetByIdAsync(id) == null)
            {
                return NotFound();
            }

            await commentService.DeleteAsync(id);

            return NoContent();
        }

        /// <summary>
        /// Delete all comments by key
        /// </summary>
        /// <param name="key">Comments key</param>
        [HttpDelete("bykey/{key}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Comment>>> DeleteByKeyAsync([FromRoute] string key)
        {
            await commentService.DeleteByKeyAsync(key);

            return NoContent();
        }
    }
}