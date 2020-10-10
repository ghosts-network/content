using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GhostNetwork.Publications.Api.Helpers;
using GhostNetwork.Publications.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Publications.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PublicationsController : ControllerBase
    {
        private readonly IPublicationService publicationService;

        public PublicationsController(IPublicationService publicationService)
        {
            this.publicationService = publicationService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Publication>> GetByIdAsync([FromRoute] string id)
        {
            var publication = await publicationService.GetByIdAsync(id);

            if (publication == null)
            {
                return NotFound();
            }

            return Ok(publication);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Publication>>> SearchAsync([FromQuery, Range(0, int.MaxValue)] int skip, [FromQuery, Range(1, 100)] int take, [FromQuery] List<string> tags)
        {
            var (list, totalCount) = await publicationService.SearchAsync(skip, take, tags);
            Response.Headers.Add("X-TotalCount", totalCount.ToString());

            return Ok(list);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Publication>> CreateAsync([FromBody] CreatePublicationModel model)
        {
            var authorId = model.AuthorId ?? "Unauthorized";
            var (result, id) = await publicationService.CreateAsync(model.Content, authorId);

            if (!result.Success)
            {
                return BadRequest(result.ToProblemDetails());
            }

            return Created(Url.Action("Find", new { id }), await publicationService.GetByIdAsync(id));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateAsync([FromRoute] string id, [FromBody] UpdatePublicationModel model)
        {
            if (await publicationService.GetByIdAsync(id) == null)
            {
                return NotFound();
            }

            var result = await publicationService.UpdateAsync(id, model.Content);

            if (!result.Success)
            {
                return BadRequest(result.ToProblemDetails());
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAsync([FromRoute] string id)
        {
            if (await publicationService.GetByIdAsync(id) == null)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}