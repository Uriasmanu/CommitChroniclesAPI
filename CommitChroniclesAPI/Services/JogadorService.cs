using MongoDB.Driver;

namespace CommitChroniclesAPI.Services
{
    public class JogadorService
    {
        private readonly IMongoCollection<Jogador> _jogadoresCollection;

        public JogadorService(IMongoClient client)
        {
            // Acessa o banco "CommitChronicles" e a coleção "Jogadores"
            var database = client.GetDatabase("CommitChronicles");
            _jogadoresCollection = database.GetCollection<Jogador>("Jogadores");
        }

        // Método para adicionar um novo jogador (usando DTO)
        public async Task AdicionarJogadorAsync(JogadorDTO jogadorDTO)
        {
            // Verificar se já existe um jogador com o mesmo UserEmail
            var jogadorExistente = await _jogadoresCollection.Find(j => j.UserEmail == jogadorDTO.UserEmail).FirstOrDefaultAsync();
            if (jogadorExistente != null)
            {
                throw new InvalidOperationException($"O e-mail '{jogadorDTO.UserEmail}' já está em uso.");
            }

            // Converter o DTO para o modelo Jogador
            var jogador = new Jogador
            {
                Id = Guid.NewGuid(),
                UserName = jogadorDTO.UserName,
                UserEmail = jogadorDTO.UserEmail,
                Nivel = 0 // Pode ser ajustado se necessário
            };

            // Inserir o jogador na coleção
            await _jogadoresCollection.InsertOneAsync(jogador);
        }

        // Método para buscar todos jogadores (retornando DTOs)
        public async Task<List<JogadorDTO>> ObterTodosJogadoresAsync()
        {
            var jogadores = await _jogadoresCollection.Find(j => true).ToListAsync();

            // Converter lista de jogadores para DTOs
            var jogadoresDTO = jogadores.Select(j => new JogadorDTO
            {
                UserName = j.UserName,
                UserEmail = j.UserEmail
            }).ToList();

            return jogadoresDTO;
        }

        // Método para buscar um jogador por ID (retornando DTO)
        public async Task<JogadorDTO> ObterJogadorPorIdAsync(Guid id)
        {
            var jogador = await _jogadoresCollection.Find(j => j.Id == id).FirstOrDefaultAsync();
            if (jogador == null)
            {
                throw new KeyNotFoundException($"Jogador com o ID '{id}' não foi encontrado.");
            }

            // Converter jogador para DTO
            var jogadorDTO = new JogadorDTO
            {
                UserName = jogador.UserName,
                UserEmail = jogador.UserEmail
            };

            return jogadorDTO;
        }

        // Método para atualizar um jogador (usando DTO)
        public async Task AtualizarJogadorAsync(Guid id, JogadorDTO jogadorDTO)
        {
            // Converter o DTO para o modelo Jogador
            var jogadorAtualizado = new Jogador
            {
                Id = id,
                UserName = jogadorDTO.UserName,
                UserEmail = jogadorDTO.UserEmail,
                Nivel = 0 // Pode ser ajustado se necessário
            };

            var resultado = await _jogadoresCollection.ReplaceOneAsync(j => j.Id == id, jogadorAtualizado);
            if (resultado.MatchedCount == 0)
            {
                throw new KeyNotFoundException($"Jogador com o ID '{id}' não foi encontrado para atualização.");
            }
        }

        // Método para remover um jogador
        public async Task RemoverJogadorAsync(Guid id)
        {
            var resultado = await _jogadoresCollection.DeleteOneAsync(j => j.Id == id);
            if (resultado.DeletedCount == 0)
            {
                throw new KeyNotFoundException($"Jogador com o ID '{id}' não foi encontrado para remoção.");
            }
        }
    }
}
