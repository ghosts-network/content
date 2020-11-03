using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GhostNetwork.Publications.Api.Helpers;
using GhostNetwork.Publications.Api.Models;
using GhostNetwork.Publications.Comments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Publications.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService commentService;

        public CommentsController(ICommentsService commentService)
        {
            this.commentService = commentService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Comment>> CreateAsync([FromBody] CreateCommentModel model)
        {
            var authorId = model.AuthorId ?? "Unauthorized";
            var (domainResult, id) = await commentService.CreateAsync(model.PublicationId, model.Content, model.ReplyCommentId, authorId);

            if (domainResult.Successed)
            {
                return Created(Url.Action("GetById", new { id }), await commentService.GetByIdAsync(id));
            }

            return BadRequest(domainResult.ToProblemDetails());
        }

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

        [HttpGet("bypublication/{publicationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Comment>>> SearchAsync(
            [FromRoute] string publicationId,
            [FromQuery, Range(0, int.MaxValue)] int skip,
            [FromQuery, Range(0, 100)] int take = 10)
        {
            var (comments, totalCount) = await commentService.SearchAsync(publicationId, skip, take);
            Response.Headers.Add("X-TotalCount", totalCount.ToString());

            return Ok(comments);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Comment>>> DeleteAsync([FromRoute] string id)
        {
            if (await commentService.GetByIdAsync(id) == null)
            {
                return NotFound();
            }

            await commentService.DeleteAsync(id);

            return Ok();
        }
    }
}
