namespace GhostNetwork.Publications.Domain
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
