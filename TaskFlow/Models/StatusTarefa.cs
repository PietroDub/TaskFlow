using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public enum StatusTarefa
    {
        [Display(Name = "A Fazer")]
        AFazer = 0,

        [Display(Name = "Em Andamento")]
        EmAndamento = 1,

        [Display(Name = "Concluído")]
        Concluido = 2
    }
}
