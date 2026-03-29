using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskFlow.Models
{
    public class Tarefa
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(200)]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Display(Name = "Prioridade")]
        public PrioridadeTarefa Prioridade { get; set; } = PrioridadeTarefa.Media;

        [Display(Name = "Status")]
        public StatusTarefa Status { get; set; } = StatusTarefa.AFazer;

        [DataType(DataType.Date)]
        [Display(Name = "Prazo")]
        public DateTime Prazo { get; set; }

        [Display(Name = "Responsável")]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }
    }
}
