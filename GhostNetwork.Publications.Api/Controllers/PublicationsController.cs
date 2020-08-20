using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GhostNetwork.Publications.Domain;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Publications.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PublicationsController : ControllerBase
    {
        private static IPublicationStorage storage = new PublicationStorage();
        
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

            return NoContent();
        }
    }

    public class CreatePublicationModel
    {
        [Required]
        public string Content { get; set; }
    }
}