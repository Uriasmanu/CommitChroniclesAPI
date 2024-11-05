namespace CommitChroniclesAPI.Models
{
    public class Jogador
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int Nivel { get; set; }

    }
}
