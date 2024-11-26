using MongoDB.Bson.Serialization.Attributes;

namespace CommitChroniclesAPI.Models
{
    public class Missao
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public int PontosDeExperiencia {  get; set; }
        public Boolean StatusConclusao { get; set; } = false;

    }
}
