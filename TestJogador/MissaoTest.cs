using CommitChroniclesAPI.Models;
using CommitChroniclesAPI.Services;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitChroniclesTest
{
    class MissaoTest
    {
        private readonly Mock<IMongoClient> _mockClient;
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<IMongoCollection<Missao>> _mockMissoesCollection;
        private readonly Mock<IAsyncCursor<Missao>> _mockCursor;
        private readonly MissaoService _missaoService;

        public MissaoTest()
        {
            // Inicializando os mocks
            _mockClient = new Mock<IMongoClient>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockMissoesCollection = new Mock<IMongoCollection<Missao>>();
            _mockCursor = new Mock<IAsyncCursor<Missao>>();

            // Configuração do mock do client para retornar o database mockado
            _mockClient.Setup(c => c.GetDatabase("CommitChronicles", null))
                       .Returns(_mockDatabase.Object);

            // Configuração do mock do database para retornar a coleção mockada
            _mockDatabase.Setup(d => d.GetCollection<Missao>("Missoes", null))
                         .Returns(_mockMissoesCollection.Object);
        }
    }
}
