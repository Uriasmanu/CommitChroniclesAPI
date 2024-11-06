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

        // Método para adicionar um novo jogador
        public async Task AdicionarJogadorAsync(Jogador jogador)
        {
            // Verificar se já existe um jogador com o mesmo UserEmail
            var jogadorExistente = await _jogadoresCollection.Find(j => j.UserEmail == jogador.UserEmail).FirstOrDefaultAsync();
            if (jogadorExistente != null)
            {
                throw new InvalidOperationException($"O e-mail '{jogador.UserEmail}' já está em uso.");
            }

            // Caso o jogador não tenha um ID, gerar um novo Guid
            if (jogador.Id == Guid.Empty)
            {
                jogador.Id = Guid.NewGuid();
            }

            // Inserir o jogador na coleção
            await _jogadoresCollection.InsertOneAsync(jogador);
        }


        // Método para buscar todos os jogadores
        public async Task<List<Jogador>> ObterTodosJogadoresAsync()
        {
            return await _jogadoresCollection.Find(j => true).ToListAsync();
        }

        // Método para buscar um jogador por ID
        public async Task<Jogador> ObterJogadorPorIdAsync(Guid id)
        {
            var jogador = await _jogadoresCollection.Find(j => j.Id == id).FirstOrDefaultAsync();
            if (jogador == null)
            {
                throw new KeyNotFoundException($"Jogador com o ID '{id}' não foi encontrado.");
            }
            return jogador;
        }

        // Método para atualizar um jogador
        public async Task AtualizarJogadorAsync(Guid id, Jogador jogadorAtualizado)
        {
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
