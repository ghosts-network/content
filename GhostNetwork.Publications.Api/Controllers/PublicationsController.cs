using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Publications.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PublicationsController : ControllerBase
    {
        private static PublicationStorage storage = new PublicationStorage();
        
        [HttpGet("{id}")]
        public ActionResult GetAsync([FromRoute] string id)
        {
            var publication = storage.FindOneById(id);

            if (publication == null)
            {
                return NotFound();
            }

            return Ok(publication);
        }

        [HttpPost]
        public ActionResult PostAsync([FromBody] CreatePublicationModel model)
        {
            var publication = new Publication(string.Empty, model.Content);

            var id = storage.InsertOne(publication);

            return NoContent();
        }
    }

    public class PublicationStorage
    {
        private readonly IList<Publication> publications = new List<Publication>();

        public Publication FindOneById(string id)
        {
            return publications.FirstOrDefault(p => p.Id == id);
        }

        public string InsertOne(Publication publication)
        {
            publication = new Publication(Guid.NewGuid().ToString(), publication.Content);
            publications.Add(publication);

            return publication.Id;
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