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
        public async Task<ActionResult<Publication>> FindAsync([FromRoute] string id)
        {
            var publication = await publicationService.FindOneByIdAsync(id);

            if (publication == null)
            {
                return NotFound();
            }

            return Ok(publication);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Publication>>> FindManyAsync([FromQuery, Range(0, int.MaxValue)] int skip, [FromQuery, Range(1, 100)] int take, [FromQuery] List<string> tags)
        {
            return Ok(await publicationService.FindManyAsync(skip, take, tags));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Publication>> CreateAsync([FromBody] CreatePublicationModel model)
        {
            var (result, id) = await publicationService.CreateAsync(model.Content);

            if (!result.Success)
            {
                BadRequest();
            }

            return Created(Url.Action("Find", new { id }), await publicationService.FindOneByIdAsync(id));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateAsync([FromRoute] string id, [FromBody] UpdatePublicationModel model)
        {
            var result = await publicationService.UpdateOneAsync(id, model.Content);

            if (!result.Success)
            {
                BadRequest();
            }

            return NoContent();
        }
    }
}