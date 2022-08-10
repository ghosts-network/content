using System;

namespace GhostNetwork.Content.Api.Models;

public class UpdateMediaModel
{
    public string Link { get; set; }

    public static explicit operator Media(UpdateMediaModel model)
    {
        return model == null
            ? null
            : new Media(Guid.NewGuid(), model.Link);
    }
}