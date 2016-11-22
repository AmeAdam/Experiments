using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GiadaServer.Model
{
    public interface IAlarmProvider
    {
        AlarmState GetAlarmState();
        void UpdateAlarmState(bool armedState, DateTime updateTimeUtc);
    }

    public class AlarmProvider : IAlarmProvider
    {
        private readonly IMongoCollection<AlarmState> stateCollection;

        public AlarmProvider(IOptions<GiadaSettings> settings)
        {
            var mongoClient = new MongoClient(new MongoClientSettings
            {
                Server = new MongoServerAddress(settings.Value.DatabaseHost)
            });
            stateCollection = mongoClient.GetDatabase(settings.Value.DatabaseName).GetCollection<AlarmState>(nameof(AlarmState));
        }

        public AlarmState GetAlarmState()
        {
            return stateCollection.FindSync(_ => true).FirstOrDefault();
        }

        public void UpdateAlarmState(bool armedState, DateTime updateTimeUtc)
        {
            var update = Builders<AlarmState>.Update
                .Set(a => a.Armed, armedState)
                .Set(a => a.UpdateTime, updateTimeUtc);
            stateCollection.UpdateOne(_ => true, update);
        }
    }
}
