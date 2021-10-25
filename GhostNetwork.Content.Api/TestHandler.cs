using System.Threading.Tasks;
using GhostNetwork.Content.Publications;

namespace GhostNetwork.Content.Api
{
    public class TestHandler : IEventHandler<Publications.CreatedEvent>
    {
        public async Task ProcessAsync(CreatedEvent @event)
        {
            
        }
    }
}