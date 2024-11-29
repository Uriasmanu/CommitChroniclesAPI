using CommitChroniclesAPI.DTOs;
using CommitChroniclesAPI.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MissaoService
{
    private readonly IMongoCollection<Missao> _missoesCollection;

    public MissaoService(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("CommitChronicles");
        _missoesCollection = database.GetCollection<Missao>("Missoes");
    }

    public async Task<MissaoDTO> CriarMissaoAsync(MissaoDTO missaoDTO)
    {
        var missao = new Missao
        {
            Titulo = missaoDTO.Titulo,
            Descricao = missaoDTO.Descricao,
            ComandoEsperado = missaoDTO.ComandoEsperado,
            Objetivo = missaoDTO.Objetivo,
            PontosDeExperiencia = missaoDTO.PontosDeExperiencia,
            StatusConclusao = false // Inicializa a missão com status de não concluída
        };

        await _missoesCollection.InsertOneAsync(missao);
        return missaoDTO;
    }

    public async Task<List<MissaoDTO>> ObterMissoesAsync()
    {
        var missoes = await _missoesCollection.Find(_ => true).ToListAsync();
        return missoes.Select(m => new MissaoDTO
        {
            Titulo = m.Titulo,
            Descricao = m.Descricao,
            ComandoEsperado = m.ComandoEsperado,
            Objetivo = m.Objetivo,
            PontosDeExperiencia = m.PontosDeExperiencia,
            StatusConclusao = m.StatusConclusao,
        }).ToList();
    }

    public async Task<MissaoDTO> ObterMissaoPorIdAsync(string missaoId)
    {
        var missao = await _missoesCollection.Find(m => m.Titulo == missaoId).FirstOrDefaultAsync();

        if (missao == null)
        {
            throw new MissaoNaoEncontradaException("Missão não encontrada.");
        }

        return new MissaoDTO
        {
            Titulo = missao.Titulo,
            Descricao = missao.Descricao,
            ComandoEsperado = missao.ComandoEsperado,
            Objetivo = missao.Objetivo,
            PontosDeExperiencia = missao.PontosDeExperiencia,
            StatusConclusao = missao.StatusConclusao,
        };
    }

    public async Task<MissaoDTO> AtualizarMissaoAsync(string missaoId, MissaoDTO missaoDTO)
    {
        var update = Builders<Missao>.Update
            .Set(m => m.Titulo, missaoDTO.Titulo)
            .Set(m => m.Descricao, missaoDTO.Descricao)
            .Set(m => m.ComandoEsperado, missaoDTO.ComandoEsperado)
            .Set(m => m.Objetivo, missaoDTO.Objetivo)
            .Set(m => m.PontosDeExperiencia, missaoDTO.PontosDeExperiencia);

        var result = await _missoesCollection.UpdateOneAsync(m => m.Titulo == missaoId, update);

        if (result.MatchedCount == 0)
        {
            throw new MissaoNaoEncontradaException("Missão não encontrada.");
        }

        if (result.ModifiedCount == 0)
        {
            throw new Exception("A atualização não foi realizada.");
        }

        return missaoDTO;
    }

    public async Task DeletarMissaoAsync(string missaoId)
    {
        var result = await _missoesCollection.DeleteOneAsync(m => m.Titulo == missaoId);

        if (result.DeletedCount == 0)
        {
            throw new MissaoNaoEncontradaException("Missão não encontrada.");
        }
    }

    // Método para alterar o StatusConclusao de false para true
    public async Task<MissaoDTO> AlterarStatusConclusaoAsync(string titulo)
    {
        var filtro = Builders<Missao>.Filter.Eq(m => m.Titulo, titulo);

        // Recuperando a missão atual para verificar o status
        var missaoAtualizada = await _missoesCollection.Find(filtro).FirstOrDefaultAsync();

        if (missaoAtualizada == null)
        {
            throw new MissaoNaoEncontradaException("Missão não encontrada.");
        }

        // Invertendo o status de StatusConclusao
        bool novoStatus = !missaoAtualizada.StatusConclusao;

        var update = Builders<Missao>.Update.Set(m => m.StatusConclusao, novoStatus);

        var result = await _missoesCollection.UpdateOneAsync(filtro, update);

        // Logs para depuração
        Console.WriteLine($"Título da missão: {titulo}");
        Console.WriteLine($"Filtro aplicado: {filtro}");
        Console.WriteLine($"Resultado do update: MatchedCount = {result.MatchedCount}, ModifiedCount = {result.ModifiedCount}");

        if (result.MatchedCount == 0)
        {
            throw new MissaoNaoEncontradaException("Missão não encontrada.");
        }

        // Verificando se a missão foi modificada
        if (result.ModifiedCount == 0)
        {
            throw new Exception("A atualização não foi realizada. O status já pode ser invertido.");
        }

        // Recuperando a missão atualizada
        missaoAtualizada = await _missoesCollection.Find(filtro).FirstOrDefaultAsync();

        return new MissaoDTO
        {
            Titulo = missaoAtualizada.Titulo,
            Descricao = missaoAtualizada.Descricao,
            ComandoEsperado = missaoAtualizada.ComandoEsperado,
            Objetivo = missaoAtualizada.Objetivo,
            PontosDeExperiencia = missaoAtualizada.PontosDeExperiencia,
            StatusConclusao = missaoAtualizada.StatusConclusao, // Retorna o valor atualizado do banco
        };
    }




    // Exceção customizada para Missão não encontrada (404)
    public class MissaoNaoEncontradaException : Exception
    {
        public MissaoNaoEncontradaException(string message) : base(message) { }
    }
}
