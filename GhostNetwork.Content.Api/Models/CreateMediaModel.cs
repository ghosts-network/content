using System;

namespace GhostNetwork.Content.Api.Models;

public class CreateMediaModel
{
    public string Link { get; set; }

    public static explicit operator Media(CreateMediaModel model)
    {
        return model == null
            ? null
            : new Media(Guid.NewGuid(), model.Link);
    }
}