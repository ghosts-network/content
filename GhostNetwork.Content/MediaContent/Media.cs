namespace GhostNetwork.Content.MediaContent;

public class Media
{
    public Media(string id, string link, string key)
    {
        Id = id;
        Link = link;
        Key = key;
    }

    public string Id { get; set; }

    public string Link { get; set; }

    public string Key { get; set; }

    public static Media New(string link, string key)
    {
        return new Media(default, link, key);
    }
}