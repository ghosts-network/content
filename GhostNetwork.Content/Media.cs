using System;

namespace GhostNetwork.Content;

public class Media
{
    public Media(Guid id, string link)
    {
        Id = id;
        Link = link;
    }

    public Guid Id { get; set; }

    public string Link { get; set; }
}