using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public class Usuario
    {
        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(120)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A função é obrigatória.")]
        [StringLength(80)]
        [Display(Name = "Função")]
        public string Funcao { get; set; } = string.Empty;
    }
}
