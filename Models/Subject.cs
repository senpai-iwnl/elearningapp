﻿using System.ComponentModel.DataAnnotations;

namespace e_learning_app.Models;

public class Subject
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Class> Classes { get; set; } = new List<Class>();
    [Required]
    public Guid CreatorId { get; set; }
    public User? Creator { get; set; }

    // Studenci zapisani do klasy (relacja wiele-do-wielu)
    public List<User> Students { get; set; } = new List<User>();
    
    // 🔹 Relacja jeden-do-wielu: Przedmiot może mieć wiele grup
    public List<Group> Groups { get; set; } = new List<Group>();
    
    // Kod dołączenia do kursu - 10 losowych znaków
    [Required] 
    [MaxLength(10)] public string JoinCode { get; set; } = GenerateJoinCode();
    
    // 🔹 Nowe pole: Wiadomości powiązane z przedmiotem
    public List<Message> Messages { get; set; } = new List<Message>();
    
    private static string GenerateJoinCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, 10)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }

}