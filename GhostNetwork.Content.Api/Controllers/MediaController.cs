using System.Collections.Generic;
using System.Threading.Tasks;
using GhostNetwork.Content.Api.Helpers;
using GhostNetwork.Content.Api.Models;
using GhostNetwork.Content.MediaContent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Content.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class MediaController : ControllerBase
{
    private readonly IMediaService mediaService;

    public MediaController(IMediaService mediaService)
    {
        this.mediaService = mediaService;
    }

    /// <summary>
    /// Get media content by id
    /// </summary>
    /// <param name="id">Media id</param>
    /// <returns>Media content</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Media>> GetByIdAsync([FromRoute] string id)
    {
        var media = await mediaService.GetByIdAsync(id);

        if (media is null)
        {
            return NotFound();
        }

        return Ok(media);
    }

    /// <summary>
    /// Search media content by key
    /// </summary>
    /// <param name="key">Media key (publication id or comment id)</param>
    /// <returns>Media content</returns>
    [HttpGet("bykey/{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<Media>>> SearchByKeyAsync(
        [FromRoute] string key)
    {
        var media = await mediaService.SearchByKeyAsync(key);

        return Ok(media);
    }

    /// <summary>
    /// Find grouped media for keys
    /// </summary>
    /// <param name="model">Media query model</param>
    /// <returns>Grouped media related to keys</returns>
    [HttpPost("grouped")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Dictionary<string, GroupedMedia>>> FindGroupedMediaAsync([FromBody] MediaQuery model)
    {
        var result = await mediaService.FindGroupedMediaAsync(model.Keys);

        return Ok(result);
    }

    /// <summary>
    /// Insert media
    /// </summary>
    /// <param name="model">Create media model</param>
    /// <returns>Created media</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Media>> InsertMediaAsync(
        [FromBody] CreateMediaModel model)
    {
        var (result, id) = await mediaService.InsertAsync(model.Link, model.Key);

        if (!result.Successed)
        {
            return BadRequest(result.ToProblemDetails());
        }

        return Created(Url.Action("GetById", new { id })!, await mediaService.GetByIdAsync(id));
    }

    /// <summary>
    /// Delete media by id
    /// </summary>
    /// <param name="id">Media id</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync([FromRoute] string id)
    {
        var media = await mediaService.GetByIdAsync(id);

        if (media is null)
        {
            return NotFound();
        }

        await mediaService.DeleteAsync(id);

        return NoContent();
    }

    /// <summary>
    /// Delete all media by key
    /// </summary>
    /// <param name="key">Media key (publication id or comment id)</param>
    [HttpDelete("bykey/{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteByKeyAsync([FromRoute] string key)
    {
        await mediaService.DeleteByKeyAsync(key);

        return NoContent();
    }
}