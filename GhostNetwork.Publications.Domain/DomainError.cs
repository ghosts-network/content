namespace GhostNetwork.Publications
{
    public class DomainError
    {
        public DomainError(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
