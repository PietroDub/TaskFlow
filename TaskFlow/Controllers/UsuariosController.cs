using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Helpers;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Controllers
{
    public class UsuariosController : Controller
    {
        private const string MsgErroCarregar = "Não foi possível carregar os dados.";
        private const string MsgErroSalvar = "Não foi possível salvar os dados. Verifique a conexão com o banco.";

        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            try
            {
                var list = await _usuarioService.ListarAsync(cancellationToken);
                return View(list);
            }
            catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
            {
                ViewBag.ErroCarregamento = MsgErroCarregar;
                return View(Array.Empty<Usuario>());
            }
        }

        public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
                return NotFound();

            try
            {
                var usuario = await _usuarioService.ObterPorIdAsync(id.Value, cancellationToken);
                if (usuario == null)
                    return NotFound();

                return View(usuario);
            }
            catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
            {
                ViewBag.ErroCarregamento = MsgErroCarregar;
                return View(new Usuario());
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Email,Funcao")] Usuario usuario, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _usuarioService.CriarAsync(usuario, cancellationToken);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
                {
                    ViewBag.ErroOperacao = MsgErroSalvar;
                    return View(usuario);
                }
            }
            return View(usuario);
        }

        public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
                return NotFound();

            try
            {
                var usuario = await _usuarioService.ObterPorIdAsync(id.Value, cancellationToken);
                if (usuario == null)
                    return NotFound();

                return View(usuario);
            }
            catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
            {
                ViewBag.ErroCarregamento = MsgErroCarregar;
                return View(new Usuario { Id = id.Value });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,Funcao")] Usuario usuario, CancellationToken cancellationToken)
        {
            if (id != usuario.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _usuarioService.AtualizarAsync(usuario, cancellationToken);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    try
                    {
                        if (await _usuarioService.ObterPorIdAsync(usuario.Id, cancellationToken) == null)
                            return NotFound();
                    }
                    catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
                    {
                        ViewBag.ErroOperacao = MsgErroSalvar;
                    return View(usuario);
                    }
                    throw;
                }
                catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
                {
                    ViewBag.ErroOperacao = MsgErroSalvar;
                    return View(usuario);
                }
            }
            return View(usuario);
        }

        public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
                return NotFound();

            try
            {
                var usuario = await _usuarioService.ObterPorIdAsync(id.Value, cancellationToken);
                if (usuario == null)
                    return NotFound();

                return View(usuario);
            }
            catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
            {
                ViewBag.ErroCarregamento = MsgErroCarregar;
                return View(new Usuario());
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _usuarioService.ExcluirAsync(id, cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
            {
                TempData["ErroOperacao"] = MsgErroSalvar;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}


