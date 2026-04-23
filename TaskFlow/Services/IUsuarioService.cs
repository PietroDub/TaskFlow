using TaskFlow.Models;

namespace TaskFlow.Services
{
    public interface IUsuarioService
    {
        Task<IReadOnlyList<Usuario>> ListarAsync(CancellationToken cancellationToken = default);
        Task<Usuario?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task CriarAsync(Usuario usuario, CancellationToken cancellationToken = default);
        Task<bool> AtualizarAsync(Usuario usuario, CancellationToken cancellationToken = default);
        Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExisteAsync(int id, CancellationToken cancellationToken = default);
    }
}
