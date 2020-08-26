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

        public PublicationsController(IPublicationStorage storage)
        {
            this.storage = storage;
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

        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] CreatePublicationModel model)
        {
            var publication = new Publication(string.Empty, model.Content);

            var id = await storage.InsertOneAsync(publication);

            return Created(Url.Action("Get", new { id }), new { Id = id });
        }
    }
}