using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public class Usuario
    {
        [Key]
        [Display(Name ="UsuarioId")]
        public int Id { get; set; }

        public string? Nome { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Funcao { get; set; }
    }
}