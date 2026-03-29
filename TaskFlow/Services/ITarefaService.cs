using TaskFlow.Models;
using TaskFlow.Models.ViewModels;

namespace TaskFlow.Services
{
    public interface ITarefaService
    {
        Task<IReadOnlyList<Tarefa>> ListarFiltradasAsync(
            int? usuarioId,
            StatusTarefa? status,
            PrioridadeTarefa? prioridade,
            CancellationToken cancellationToken = default);

        Task<Tarefa?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task CriarAsync(Tarefa tarefa, CancellationToken cancellationToken = default);
        Task AtualizarAsync(Tarefa tarefa, CancellationToken cancellationToken = default);
        Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken = default);

        Task<int> ContarConcluidasAsync(CancellationToken cancellationToken = default);
        Task<RelatorioTarefasViewModel> ObterRelatorioAsync(CancellationToken cancellationToken = default);
    }
}
