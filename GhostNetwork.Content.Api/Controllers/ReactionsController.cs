using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Content.Api.Models;
using GhostNetwork.Content.Reactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Content.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReactionsController : ControllerBase
    {
        private readonly IReactionStorage reactionStorage;

        public ReactionsController(IReactionStorage reactionStorage)
        {
            this.reactionStorage = reactionStorage;
        }

        /// <summary>
        /// Returns stats for one entity.
        /// </summary>
        /// <param name="key">Entity key</param>
        /// <response code="200">Returns stats for one entity by key.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{key}")]
        public async Task<ActionResult<IDictionary<string, int>>> GetAsync([FromRoute] string key)
        {
            var result = await reactionStorage.GetStats(key);

            if (!result.Any())
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Returns reaction by author.
        /// </summary>
        /// <param name="key">Entity key</param>
        /// <param name="author">Author of reaction</param>
        /// <response code="200">Returns reaction by author and key.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{key}/author")]
        public async Task<ActionResult<Reaction>> GetReactionByAuthor(
            [FromRoute] string key,
            [Required, FromHeader] string author)
        {
            var result = await reactionStorage.GetReactionByAuthorAsync(key, author);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Returns many reactions by author.
        /// </summary>
        /// <param name="author">Author of reaction</param>
        /// <param name="model">Array of publications ids</param>
        /// <response code="200">Returns reactions filtered by author and keys.</response>
        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<Reaction>>> SearchAsync(
            [Required, FromQuery] string author,
            [FromBody] ReactionsQuery model)
        {
            return Ok(await reactionStorage.GetReactionsByAuthorAsync(author, model.Keys));
        }

        /// <summary>
        /// Returns reactions stats for many publications.
        /// </summary>
        /// <param name="model">Array of publications ids</param>
        /// <response code="200">Returns reactions stats for many publications by publications ids.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("grouped")]
        public async Task<ActionResult<IDictionary<string, IDictionary<string, int>>>> GetGroupedReactionsAsync([FromBody]ReactionsQuery model)
        {
            return Ok(await reactionStorage.GetGroupedReactionsAsync(model.Keys));
        }

        /// <summary>
        /// Add type of reaction to entity.
        /// </summary>
        /// <param name="key">Entity key</param>
        /// <param name="type">Reaction type</param>
        /// <param name="author">Author of reaction</param>
        /// <response code="201">Reaction is added.</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{key}/{type}")]
        public async Task<ActionResult<IDictionary<string, int>>> UpsertAsync(
            [FromRoute] string key,
            [FromRoute] string type,
            [Required, FromHeader] string author)
        {
            await reactionStorage.UpsertAsync(key, author, type);

            return Created(string.Empty, await reactionStorage.GetStats(key));
        }

        /// <summary>
        /// Remove reaction by author and key.
        /// </summary>
        /// <param name="key">Entity key</param>
        /// <param name="author">Author of reaction</param>
        /// <response code="200">Remove reaction by key and author.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{key}/author")]
        public async Task<ActionResult<IDictionary<string, int>>> DeleteByAuthorAsync(
            [FromRoute] string key,
            [Required, FromHeader] string author)
        {
            if (!(await reactionStorage.GetStats(key)).Any())
            {
                return NotFound();
            }

            await reactionStorage.DeleteByAuthorAsync(key, author);

            return Ok(await reactionStorage.GetStats(key));
        }

        /// <summary>
        /// Remove all reactions.
        /// </summary>
        /// <param name="key">Entity key</param>
        /// <response code="200">Remove all reactions by key.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("{key}")]
        public async Task<ActionResult<IDictionary<string, int>>> DeleteAsync([FromRoute] string key)
        {
            await reactionStorage.DeleteAsync(key);

            return Ok(await reactionStorage.GetStats(key));
        }
    }
}