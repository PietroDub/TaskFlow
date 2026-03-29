using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Extensions;
using TaskFlow.Helpers;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Controllers
{
    public class TarefasController : Controller
    {
        private const string MsgErroCarregar = "Não foi possível carregar os dados.";
        private const string MsgErroSalvar = "Não foi possível salvar os dados. Verifique a conexão com o banco.";

        private readonly ITarefaService _tarefaService;
        private readonly IUsuarioService _usuarioService;

        public TarefasController(ITarefaService tarefaService, IUsuarioService usuarioService)
        {
            _tarefaService = tarefaService;
            _usuarioService = usuarioService;
        }

        public async Task<IActionResult> Index(
            int? usuarioId,
            int? status,
            int? prioridade,
            CancellationToken cancellationToken)
        {
            StatusTarefa? statusEnum = status.HasValue && Enum.IsDefined(typeof(StatusTarefa), status.Value)
                ? (StatusTarefa)status.Value
                : null;
            PrioridadeTarefa? prioridadeEnum = prioridade.HasValue && Enum.IsDefined(typeof(PrioridadeTarefa), prioridade.Value)
                ? (PrioridadeTarefa)prioridade.Value
                : null;

            try
            {
                var tarefas = await _tarefaService.ListarFiltradasAsync(usuarioId, statusEnum, prioridadeEnum, cancellationToken);
                await PreencherViewBagsFiltrosAsync(usuarioId, statusEnum, prioridadeEnum, cancellationToken);
                return View(tarefas);
            }
            catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
            {
                ViewBag.ErroCarregamento = MsgErroCarregar;
                PreencherFiltrosSemBanco(usuarioId, statusEnum, prioridadeEnum);
                return View(Array.Empty<Tarefa>());
            }
        }

        public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
                return NotFound();

            try
            {
                var tarefa = await _tarefaService.ObterPorIdAsync(id.Value, cancellationToken);
                if (tarefa == null)
                    return NotFound();

                return View(tarefa);
            }
            catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
            {
                ViewBag.ErroCarregamento = MsgErroCarregar;
                return View(new Tarefa());
            }
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            await PreencherSelectUsuariosAsync(null, cancellationToken);
            return View(new Tarefa
            {
                Prazo = DateTime.Today,
                Status = StatusTarefa.AFazer,
                Prioridade = PrioridadeTarefa.Media
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Titulo,Descricao,Prioridade,Prazo,Status,UsuarioId")] Tarefa tarefa,
            CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _tarefaService.CriarAsync(tarefa, cancellationToken);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
                {
                    ViewBag.ErroOperacao = MsgErroSalvar;
                    await PreencherSelectUsuariosAsync(tarefa.UsuarioId, cancellationToken);
                    return View(tarefa);
                }
            }
            await PreencherSelectUsuariosAsync(tarefa.UsuarioId, cancellationToken);
            return View(tarefa);
        }

        public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
                return NotFound();

            try
            {
                var tarefa = await _tarefaService.ObterPorIdAsync(id.Value, cancellationToken);
                if (tarefa == null)
                    return NotFound();

                await PreencherSelectUsuariosAsync(tarefa.UsuarioId, cancellationToken);
                return View(tarefa);
            }
            catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
            {
                ViewBag.ErroCarregamento = MsgErroCarregar;
                await PreencherSelectUsuariosAsync(null, cancellationToken);
                return View(new Tarefa { Id = id.Value, Prazo = DateTime.Today });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Titulo,Descricao,Prioridade,Prazo,Status,UsuarioId")] Tarefa tarefa,
            CancellationToken cancellationToken)
        {
            if (id != tarefa.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _tarefaService.AtualizarAsync(tarefa, cancellationToken);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    try
                    {
                        if (await _tarefaService.ObterPorIdAsync(tarefa.Id, cancellationToken) == null)
                            return NotFound();
                    }
                    catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
                    {
                        ViewBag.ErroOperacao = MsgErroSalvar;
                        await PreencherSelectUsuariosAsync(tarefa.UsuarioId, cancellationToken);
                        return View(tarefa);
                    }
                    throw;
                }
                catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
                {
                    ViewBag.ErroOperacao = MsgErroSalvar;
                    await PreencherSelectUsuariosAsync(tarefa.UsuarioId, cancellationToken);
                    return View(tarefa);
                }
            }
            await PreencherSelectUsuariosAsync(tarefa.UsuarioId, cancellationToken);
            return View(tarefa);
        }

        public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
                return NotFound();

            try
            {
                var tarefa = await _tarefaService.ObterPorIdAsync(id.Value, cancellationToken);
                if (tarefa == null)
                    return NotFound();

                return View(tarefa);
            }
            catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
            {
                ViewBag.ErroCarregamento = MsgErroCarregar;
                return View(new Tarefa());
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _tarefaService.ExcluirAsync(id, cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
            {
                TempData["ErroOperacao"] = MsgErroSalvar;
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task PreencherSelectUsuariosAsync(int? usuarioIdSelecionado, CancellationToken cancellationToken)
        {
            try
            {
                var usuarios = await _usuarioService.ListarAsync(cancellationToken);
                ViewData["UsuarioId"] = new SelectList(usuarios, "Id", "Nome", usuarioIdSelecionado);
            }
            catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
            {
                ViewData["UsuarioId"] = new SelectList(Array.Empty<Usuario>(), "Id", "Nome", usuarioIdSelecionado);
                ViewBag.ErroCarregamento = MsgErroCarregar;
            }
        }

        private async Task PreencherViewBagsFiltrosAsync(
            int? usuarioId,
            StatusTarefa? status,
            PrioridadeTarefa? prioridade,
            CancellationToken cancellationToken)
        {
            var usuarios = await _usuarioService.ListarAsync(cancellationToken);
            var listaUsuarios = new List<SelectListItem> { new("Todos", "", !usuarioId.HasValue) };
            listaUsuarios.AddRange(usuarios.Select(u => new SelectListItem(u.Nome, u.Id.ToString(), usuarioId == u.Id)));
            ViewBag.UsuariosFiltro = listaUsuarios;

            PreencherFiltrosEnum(status, prioridade);
        }

        private void PreencherFiltrosSemBanco(int? usuarioId, StatusTarefa? status, PrioridadeTarefa? prioridade)
        {
            ViewBag.UsuariosFiltro = new List<SelectListItem> { new("Todos", "", !usuarioId.HasValue) };
            PreencherFiltrosEnum(status, prioridade);
        }

        private void PreencherFiltrosEnum(StatusTarefa? status, PrioridadeTarefa? prioridade)
        {
            var listaStatus = new List<SelectListItem> { new("Todos", "", !status.HasValue) };
            listaStatus.AddRange(Enum.GetValues<StatusTarefa>().Select(s =>
                new SelectListItem(s.GetDisplayName(), ((int)s).ToString(), status == s)));
            ViewBag.StatusFiltro = listaStatus;

            var listaPrior = new List<SelectListItem> { new("Todas", "", !prioridade.HasValue) };
            listaPrior.AddRange(Enum.GetValues<PrioridadeTarefa>().Select(p =>
                new SelectListItem(p.GetDisplayName(), ((int)p).ToString(), prioridade == p)));
            ViewBag.PrioridadeFiltro = listaPrior;
        }
    }
}

