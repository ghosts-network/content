using System.Collections.Generic;

namespace GhostNetwork.Content.MediaContent;

public class GroupedMedia
{
    public GroupedMedia(IEnumerable<Media> media, int totalCount)
    {
        Media = media;
        TotalCount = totalCount;
    }

    public IEnumerable<Media> Media { get; }

    public int TotalCount { get; }
}