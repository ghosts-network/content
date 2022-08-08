using System;
using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Content.Api.Models;

public class CreateMediaModel
{
    [Required]
    public string Key { get; set; }

    public string Link { get; set; }
}