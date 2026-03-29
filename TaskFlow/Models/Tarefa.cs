namespace TaskFlow.Models
{
    public class Tarefa
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public string? Prioridade { get; set; }
        public DateTime Prazo { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } // navegação de volta
    }
}