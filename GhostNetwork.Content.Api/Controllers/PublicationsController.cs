using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GhostNetwork.Content.Api.Helpers;
using GhostNetwork.Content.Api.Models;
using GhostNetwork.Content.Publications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Content.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PublicationsController : ControllerBase
    {
        private readonly IPublicationService publicationService;
        private readonly IUserProvider userProvider;

        public PublicationsController(IPublicationService publicationService, IUserProvider userProvider)
        {
            this.publicationService = publicationService;
            this.userProvider = userProvider;
        }

        /// <summary>
        /// Get one publication by id
        /// </summary>
        /// <param name="id">Publication id</param>
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

        /// <summary>
        /// Search publication by authorId
        /// </summary>
        /// <param name="skip">Skip publications up to a specified position</param>
        /// <param name="take">Take publications up to a specified position</param>
        /// <param name="authorId">Filters publications by authorId</param>
        /// <param name="order">Order by creation date</param>
        /// <returns>Filtered sequence of publications</returns>
        [HttpGet("publications/{authorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "X-TotalCount", "Number", "Total number of author publications")]
        public async Task<ActionResult<IEnumerable<Publication>>> SearchByAuthorAsync(
            [FromQuery, Range(0, int.MaxValue)] int skip,
            [FromQuery, Range(1, 100)]int take,
            [FromRoute] Guid authorId,
            [FromQuery] Ordering order = Ordering.Asc)
        {
            var (publications, totalCount) = await publicationService.SearchByAuthor(skip, take, authorId, order);
            Response.Headers.Add("X-TotalCount", totalCount.ToString());

            return Ok(publications);
        }

        /// <summary>
        /// Search publications
        /// </summary>
        /// <param name="skip">Skip publications up to a specified position</param>
        /// <param name="cursor">Skip publications up to a specified position by cursor</param>
        /// <param name="take">Take publications up to a specified position</param>
        /// <param name="tags">Filters publications by tags</param>
        /// <param name="order">Order by creation date</param>
        /// <returns>Filtered sequence of publications</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "X-TotalCount", "Number", "Total count of publications with applied filters")]
        public async Task<ActionResult<IEnumerable<Publication>>> SearchAsync(
            [FromQuery, Range(0, int.MaxValue)] int skip,
            [FromQuery, Range(0, long.MaxValue)] long cursor,
            [FromQuery, Range(1, 100)] int take,
            [FromQuery] List<string> tags,
            [FromQuery] Ordering order = Ordering.Asc)
        {
            var (list, totalCount) = await publicationService.SearchAsync(skip, cursor, take, tags, order);
            Response.Headers.Add("X-TotalCount", totalCount.ToString());

            return Ok(list);
        }

        /// <summary>
        /// Search publications by cursor
        /// </summary>
        /// <param name="timePoint">Skip publications up to a specified position</param>
        /// <param name="take">Take publications up to a specified position</param>
        /// <param name="tags">Filters publications by tags</param>
        /// <param name="order">Order by creation date</param>
        /// <returns>Filtered sequence of publications</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Publication>>> SearchByCursorAsync(
            [FromQuery, Required] DateTimeOffset timePoint,
            [FromQuery, Range(1, 100), Required] int take,
            [FromQuery] List<string> tags,
            [FromQuery] Ordering order = Ordering.Asc)
        {
            var (publications, totalCount) = await publicationService.SearchByCursorAsync(timePoint.ToUnixTimeMilliseconds(), take, tags, order);
            Response.Headers.Add("X-TotalCount", totalCount.ToString());

            return Ok(publications);
        }

        /// <summary>
        /// Create one publication
        /// </summary>
        /// <param name="model">Publication</param>
        /// <returns>Created publication</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Publication>> CreateAsync([FromBody] CreatePublicationModel model)
        {
            var author = await userProvider.GetByIdAsync(model.AuthorId);
            var (result, id) = await publicationService.CreateAsync(model.Content, author);

            if (!result.Successed)
            {
                return BadRequest(result.ToProblemDetails());
            }

            return Created(Url.Action("GetById", new { id }), await publicationService.GetByIdAsync(id));
        }

        /// <summary>
        /// Update one publication
        /// </summary>
        /// <param name="id">Publication id</param>
        /// <param name="model">Updated model</param>
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

            if (!result.Successed)
            {
                return BadRequest(result.ToProblemDetails());
            }

            return NoContent();
        }

        /// <summary>
        /// Delete one publication
        /// </summary>
        /// <param name="id">Publication id</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAsync([FromRoute] string id)
        {
            if (await publicationService.GetByIdAsync(id) == null)
            {
                return NotFound();
            }

            await publicationService.DeleteAsync(id);

            return NoContent();
        }
    }
}