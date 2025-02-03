using System.ComponentModel.DataAnnotations;

namespace e_learning_app.Models
{
    public class Document
    {
        public Guid Id { get; set; }

        [Required]
        public string FileName { get; set; } // Nazwa pliku

        [Required]
        public string ContentType { get; set; } // Typ MIME pliku (np. "application/pdf")

        [Required]
        public byte[] Data { get; set; } // Przechowywanie pliku w bazie danych

        public Guid ClassId { get; set; } // Klucz obcy powiązany z klasą
        public Class Class { get; set; } // Relacja do klasy
    }
}