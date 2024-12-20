﻿using CommitChroniclesAPI.Models;
using MongoDB.Bson.Serialization.Attributes;

public class Jogador
{
    [BsonId]
    public Guid Id { get; set; } = Guid.NewGuid(); 
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public int Nivel { get; set; } = 1;
    public int Experiencia { get; set; } = 0;
    public ICollection<Missao> MissoesConcluidas { get; set; } = new List<Missao>();

}
