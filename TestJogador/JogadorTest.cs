using CommitChroniclesAPI.Services;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
            mockConfig.Setup(c => c["ChaveSecreta"]).Returns("super-secreta-chave-com-32-caracteres!");
            _jogadorService = new JogadorService(_mockClient.Object, mockConfig.Object);
        }

        [Fact]
        public async Task LogarJogadorAsync_DeveRetornarToken_QuandoCredenciaisCorretas()
        {
            // Arrange
            var jogadorDTO = new JogadorDTO
            {
                UserName = "JogadorCorreto",
                UserEmail = "email@correto.com"
            };

            var jogadorExistente = new Jogador
            {
                Id = Guid.NewGuid(),
                UserName = jogadorDTO.UserName,
                UserEmail = jogadorDTO.UserEmail
            };

            // Configura o cursor para simular que o jogador foi encontrado
            _mockCursor.Setup(c => c.Current).Returns(new List<Jogador> { jogadorExistente });
            _mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockJogadoresCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<Jogador>>(),
                    It.IsAny<FindOptions<Jogador>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            // Act
            var token = await _jogadorService.LogarJogadorAsync(jogadorDTO);

            // Assert
            Assert.NotNull(token); // Verifica se o token foi gerado
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            Assert.Equal(jogadorExistente.Id.ToString(), jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Assert.Equal(jogadorExistente.UserName, jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);
            Assert.Equal(jogadorExistente.UserEmail, jwtToken.Claims.First(c => c.Type == ClaimTypes.Email).Value);
        }

        [Fact]
        public async Task LogarJogadorAsync_DeveLancarExcecao_QuandoCredenciaisInvalidas()
        {
            // Arrange
            var jogadorDTO = new JogadorDTO
            {
                UserName = "JogadorInvalido",
                UserEmail = "email@invalido.com"
            };

            // Configura o cursor para simular que nenhum jogador foi encontrado
            _mockCursor.Setup(c => c.Current).Returns(new List<Jogador>());
            _mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false);

            _mockJogadoresCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<Jogador>>(),
                    It.IsAny<FindOptions<Jogador>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _jogadorService.LogarJogadorAsync(jogadorDTO));

            Assert.Equal("Credenciais inválidas. Tente novamente.", exception.Message);

            _mockJogadoresCollection.Verify(
                c => c.FindAsync(
                    It.IsAny<FilterDefinition<Jogador>>(),
                    It.IsAny<FindOptions<Jogador>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once); // Garante que a busca foi realizada
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

        [Fact]
        public async Task AdicionarJogadorAsync_DeveRecusarJogador_QuandoEmailJaExiste()
        {
            // Arrange
            var jogadorDTO = new JogadorDTO
            {
                UserName = "JogadorExistente",
                UserEmail = "email@existente.com"
            };

            var jogadorExistente = new Jogador
            {
                UserName = "OutroJogador",
                UserEmail = "email@existente.com"
            };

            // Configura o cursor para simular que o e-mail já existe
            _mockCursor.Setup(c => c.Current).Returns(new List<Jogador> { jogadorExistente });
            _mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockJogadoresCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<Jogador>>(),
                    It.IsAny<FindOptions<Jogador>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _jogadorService.AdicionarJogadorAsync(jogadorDTO));

            // Assert
            _mockJogadoresCollection.Verify(
                c => c.InsertOneAsync(
                    It.IsAny<Jogador>(),
                    null,
                    default),
                Times.Never); // Garante que nenhum jogador foi inserido
        }

        [Fact]
        public async Task RemoverJogadorAsync_DeveRemoverJogador_QuandoIdExiste()
        {
            // Arrange
            var jogadorId = Guid.NewGuid();

            _mockJogadoresCollection
                .Setup(c => c.DeleteOneAsync(
                    It.IsAny<FilterDefinition<Jogador>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            // Act
            await _jogadorService.RemoverJogadorAsync(jogadorId);

            // Assert
            _mockJogadoresCollection.Verify(
                c => c.DeleteOneAsync(
                    It.IsAny<FilterDefinition<Jogador>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [Fact]
        public async Task RemoverJogadorAsync_DeveLancarExcecao_QuandoIdNaoExiste()
        {
            // Arrange
            var jogadorId = Guid.NewGuid();

            // Configuração para simular que nenhum documento foi removido
            _mockJogadoresCollection
                .Setup(c => c.DeleteOneAsync(
                    It.IsAny<FilterDefinition<Jogador>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteResult.Acknowledged(0)); // Simula que 0 documentos foram removidos

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _jogadorService.RemoverJogadorAsync(jogadorId));

            Assert.Equal($"Jogador com o ID '{jogadorId}' não foi encontrado para remoção.", exception.Message);

            _mockJogadoresCollection.Verify(
                c => c.DeleteOneAsync(
                    It.IsAny<FilterDefinition<Jogador>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once); // Garante que a tentativa de exclusão foi feita
        }
    }
}