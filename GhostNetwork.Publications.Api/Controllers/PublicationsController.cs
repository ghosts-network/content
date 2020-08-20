using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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

    public interface IPublicationStorage
    {
        Task<Publication> FindOneByIdAsync(string id);
        Task<string> InsertOneAsync(Publication publication);
    }

    public class PublicationStorage : IPublicationStorage
    {
        private readonly IList<Publication> publications = new List<Publication>();

        public Task<Publication> FindOneByIdAsync(string id)
        {
            return Task.FromResult(publications.FirstOrDefault(p => p.Id == id));
        }

        public Task<string> InsertOneAsync(Publication publication)
        {
            publication = new Publication(Guid.NewGuid().ToString(), publication.Content);
            publications.Add(publication);

            return Task.FromResult(publication.Id);
        }
    }

    public class Publication
    {
        public string Id { get; }
        public string Content { get; }

        public Publication(string id, string content)
        {
            Id = id;
            Content = content;
        }
    }

    public class CreatePublicationModel
    {
        [Required]
        public string Content { get; set; }
    }
}