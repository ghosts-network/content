using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GhostNetwork.Publications.Api.Models;
using GhostNetwork.Publications.Domain;
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
            var (domainResult, id) = await commentService.CreateAsync(model.PublicationId, model.Content, model.ReplyCommentId);

            if (domainResult.Success)
            {
                return Created(Url.Action("Find", new { id }), await commentService.FindOneByIdAsync(id));
            }

            return BadRequest();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Comment>> FindAsync([FromRoute] string id)
        {
            var comment = await commentService.FindOneByIdAsync(id);
            if (comment != null)
            {
                return Ok(comment);
            }

            return NotFound();
        }

        [HttpGet("bypublication/{publicationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Comment>>> FindManyAsync(
            [FromRoute] string publicationId,
            [FromQuery, Range(0, int.MaxValue)] int skip,
            [FromQuery, Range(0, 100)] int take = 10)
        {
            var comments = await commentService.FindManyAsync(publicationId, skip, take);

            if (comments == null)
            {
                return NotFound();
            }

            return Ok(comments);
        }
    }
}
