using MongoDB.Bson;
using MongoDB.Driver;

namespace CommitChroniclesAPI.Data
{
    public class MeuServicoMongoDb
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        public MeuServicoMongoDb(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("CommitChronicles");
        }

        public IMongoCollection<BsonDocument> MinhaColecao => _database.GetCollection<BsonDocument>("MinhaColecao");
    }
}
