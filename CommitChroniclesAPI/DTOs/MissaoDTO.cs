using MongoDB.Bson.Serialization.Attributes;

namespace CommitChroniclesAPI.DTOs
{
    public class MissaoDTO
    {
        [BsonId]
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string ComandoEsperado { get; set; }
        public string Objetivo { get; set; }
        public int PontosDeExperiencia { get; set; }
        public bool StatusConclusao { get; set; }

    }
}
