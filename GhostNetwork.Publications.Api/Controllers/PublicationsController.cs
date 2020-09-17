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
        private readonly IPublicationStorage storage;
        private readonly PublicationBuilder publicationBuilder;

        public PublicationsController(IPublicationStorage storage, PublicationBuilder publicationBuilder)
        {
            this.storage = storage;
            this.publicationBuilder = publicationBuilder;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Publication>> FindAsync([FromRoute] string id)
        {
            var publication = await storage.FindOneByIdAsync(id);

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
            return Ok(await storage.FindManyAsync(skip, take, tags));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Publication>> CreateAsync([FromBody] CreatePublicationModel model)
        {
            var publication = publicationBuilder.Build(model.Content);

            var id = await storage.InsertOneAsync(publication);

            return Created(Url.Action("Find", new { id }), await storage.FindOneByIdAsync(id));
        }
    }
}