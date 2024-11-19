using CommitChroniclesAPI.Services;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace TestJogador
{
    public class JogadorTest
    {
        private readonly Mock<IMongoClient> _mockClient;
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<IMongoCollection<Jogador>> _mockJogadoresCollection;
        private readonly Mock<IAsyncCursor<Jogador>> _mockCursor;
        private readonly JogadorService _jogadorService;

        public JogadorTest()
        {
            // Inicializando os mocks
            _mockClient = new Mock<IMongoClient>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockJogadoresCollection = new Mock<IMongoCollection<Jogador>>();
            _mockCursor = new Mock<IAsyncCursor<Jogador>>();

            // Configuração do mock do client para retornar o database mockado
            _mockClient.Setup(c => c.GetDatabase("CommitChronicles", null))
                       .Returns(_mockDatabase.Object);

            // Configuração do mock do database para retornar a coleção mockada
            _mockDatabase.Setup(d => d.GetCollection<Jogador>("Jogadores", null))
                         .Returns(_mockJogadoresCollection.Object);

            // Instancia o serviço usando os mocks
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["ChaveSecreta"]).Returns("uma-chave-secreta");
            _jogadorService = new JogadorService(_mockClient.Object, mockConfig.Object);
        }

        [Fact]
        public async Task AdicionarJogadorAsync_DeveInserirJogador_QuandoEmailNaoExiste()
        {
            // Arrange
            var jogadorDTO = new JogadorDTO
            {
                UserName = "NovoJogador",
                UserEmail = "teste@exemplo.com"
            };

            // Configura o cursor para simular que nenhum jogador foi encontrado
            _mockCursor.Setup(c => c.Current).Returns(new List<Jogador>());
            _mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false);

            // Configura a coleção para retornar o cursor vazio
            _mockJogadoresCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<Jogador>>(),
                    It.IsAny<FindOptions<Jogador>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            // Act
            await _jogadorService.AdicionarJogadorAsync(jogadorDTO);

            // Assert
            _mockJogadoresCollection.Verify(
                c => c.InsertOneAsync(
                    It.Is<Jogador>(j => j.UserEmail == jogadorDTO.UserEmail && j.UserName == jogadorDTO.UserName),
                    null,
                    default),
                Times.Once);
        }
    }
}
