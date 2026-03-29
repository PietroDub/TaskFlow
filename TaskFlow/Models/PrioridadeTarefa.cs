using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public enum PrioridadeTarefa
    {
        [Display(Name = "Baixa")]
        Baixa = 0,

        [Display(Name = "Média")]
        Media = 1,

        [Display(Name = "Alta")]
        Alta = 2
    }
}
