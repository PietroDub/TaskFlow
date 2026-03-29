using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _context;

        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Usuario>> ListarAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .OrderBy(u => u.Nome)
                .ToListAsync(cancellationToken);
        }

        public async Task<Usuario?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Usuarios.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task CriarAsync(Usuario usuario, CancellationToken cancellationToken = default)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task AtualizarAsync(Usuario usuario, CancellationToken cancellationToken = default)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken = default)
        {
            var usuario = await _context.Usuarios.FindAsync(new object[] { id }, cancellationToken);
            if (usuario is null)
                return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
