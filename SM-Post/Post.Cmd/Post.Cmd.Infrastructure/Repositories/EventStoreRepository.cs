using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Config;

namespace Post.Cmd.Infrastructure.Repositories;

public class EventStoreRepository(IOptions<MongoDbConfig> config) : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventStoreCollection = new MongoClient(config.Value.ConnectionString)
        .GetDatabase(config.Value.Database)
        .GetCollection<EventModel>(config.Value.Collection);

    public async Task SaveAsync(EventModel @event)
    {
        await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
    }

    public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
        => await _eventStoreCollection
            .Find(x => x.AggregateIdentifier.Equals(aggregateId))
            .ToListAsync()
            .ConfigureAwait(false);
}