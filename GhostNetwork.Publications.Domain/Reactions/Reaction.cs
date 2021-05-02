namespace GhostNetwork.Publications.Reactions
{
    public class Reaction
    {
        public Reaction(string key, string type)
        {
            Key = key;
            Type = type;
        }

        public string Key { get; set; }

        public string Type { get; set; }
    }
}