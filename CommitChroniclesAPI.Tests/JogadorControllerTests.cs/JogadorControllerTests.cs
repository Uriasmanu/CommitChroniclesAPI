using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using CommitChroniclesAPI;
using Microsoft.VisualStudio.TestPlatform.TestHost; // Namespace do seu projeto principal

namespace CommitChroniclesAPI.Tests
{
    public class JogadorControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public JogadorControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_Jogador_ReturnsSuccessStatusCode()
        {
            // Faz uma solicitação para o endpoint da API
            var response = await _client.GetAsync("/api/jogador");

            // Verifica se o status da resposta é 200 OK
            response.EnsureSuccessStatusCode();
        }
    }
}
