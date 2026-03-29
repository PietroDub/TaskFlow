using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;
using TaskFlow.Models.ViewModels;

namespace TaskFlow.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly AppDbContext _context;

        public TarefaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Tarefa>> ListarFiltradasAsync(
            int? usuarioId,
            StatusTarefa? status,
            PrioridadeTarefa? prioridade,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Tarefas
                .Include(t => t.Usuario)
                .AsQueryable();

            if (usuarioId.HasValue)
                query = query.Where(t => t.UsuarioId == usuarioId.Value);

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (prioridade.HasValue)
                query = query.Where(t => t.Prioridade == prioridade.Value);

            return await query
                .OrderBy(t => t.Prazo)
                .ThenBy(t => t.Titulo)
                .ToListAsync(cancellationToken);
        }

        public async Task<Tarefa?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Tarefas
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task CriarAsync(Tarefa tarefa, CancellationToken cancellationToken = default)
        {
            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task AtualizarAsync(Tarefa tarefa, CancellationToken cancellationToken = default)
        {
            _context.Tarefas.Update(tarefa);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken = default)
        {
            var tarefa = await _context.Tarefas.FindAsync(new object[] { id }, cancellationToken);
            if (tarefa is null)
                return false;

            _context.Tarefas.Remove(tarefa);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<int> ContarConcluidasAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Tarefas
                .CountAsync(t => t.Status == StatusTarefa.Concluido, cancellationToken);
        }

        public async Task<RelatorioTarefasViewModel> ObterRelatorioAsync(CancellationToken cancellationToken = default)
        {
            var lista = await _context.Tarefas
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new RelatorioTarefasViewModel
            {
                TotalTarefas = lista.Count,
                Concluidas = lista.Count(t => t.Status == StatusTarefa.Concluido),
                EmAndamento = lista.Count(t => t.Status == StatusTarefa.EmAndamento),
                AFazer = lista.Count(t => t.Status == StatusTarefa.AFazer)
            };
        }
    }
}
