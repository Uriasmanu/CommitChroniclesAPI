using CommitChroniclesAPI.Models;
using MongoDB.Driver;

namespace CommitChroniclesAPI.Services
{
    public class MissaoService
    {
        private readonly IMongoCollection<Missao> _MissoesCollection;
    }
}
