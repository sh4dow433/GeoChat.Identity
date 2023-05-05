using GeoChat.Identity.Api.Entities;
using GeoChat.Identity.Api.EventBus.Events;
using GeoChat.Identity.Api.EventBus.Extensions;
using GeoChat.Identity.Api.Repo;
using Microsoft.AspNetCore.Identity;

namespace GeoChat.Identity.Api.EventBus.EventHandlers
{
    public class SyncCallEventHandler : IEventHandler<SyncCallEvent>
    {
        private readonly IEventBus _bus;
        private readonly IGenericRepo<AppUser> _repo;
        private readonly IConfiguration _cfg;

        public SyncCallEventHandler(IEventBus bus, IGenericRepo<AppUser> repo, IConfiguration configuration)
        {
            _bus = bus;
            _repo = repo;
            _cfg = configuration;
        }
        public async Task HandleAsync(SyncCallEvent @event)
        {
            var users = await _repo.GetAllAsync(u => u.LastUpdated >= @event.LastSyncCall);
            var count = users.Count();
            while (count > 0)
            {
                var batch = users.Take(15).Select(u => {
                    return new NewAccountCreatedEvent()
                    {
                        UserId = u.Id,
                        UserName = u.UserName!
                    };
                });
                var responseEvent = new SyncResponseEvent()
                {
                    AccountsCreatedOrUpdated = batch
                };
                _bus.PublishSyncResponseEvent(_cfg, responseEvent);
                count -= 15;
                users = users.Skip(15);
            }
        }
    }
}
