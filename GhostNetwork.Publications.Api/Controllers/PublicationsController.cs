using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GhostNetwork.Publications.Api.Models;
using GhostNetwork.Publications.Domain;
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
        public async Task<ActionResult> GetAsync([FromRoute] string id)
        {
            var publication = await storage.FindOneByIdAsync(id);

            if (publication == null)
            {
                return NotFound();
            }

            return Ok(publication);
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync([FromQuery, Range(0, int.MaxValue)] int skip, [FromQuery, Range(1, 100)] int take)
        {
            return Ok(await storage.FindManyAsync(skip, take));
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] CreatePublicationModel model)
        {
            var publication = publicationBuilder.Build(model.Content);

            var id = await storage.InsertOneAsync(publication);

            return Created(Url.Action("Get", new { id }), new { Id = id });
        }
    }
}