using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TaskFlow.Helpers;
using TaskFlow.Models;
using TaskFlow.Models.ViewModels;
using TaskFlow.Services;

namespace TaskFlow.Controllers
{
    public class HomeController : Controller
    {
        private const string MsgErroCarregar = "Não foi possível carregar os dados.";

        private readonly ITarefaService _tarefaService;

        public HomeController(ITarefaService tarefaService)
        {
            _tarefaService = tarefaService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Relatorio(CancellationToken cancellationToken)
        {
            try
            {
                var vm = await _tarefaService.ObterRelatorioAsync(cancellationToken);
                return View(vm);
            }
            catch (Exception ex) when (DatabaseAccessHelper.IsLikelyDatabaseAccessFailure(ex))
            {
                ViewBag.ErroCarregamento = MsgErroCarregar;
                return View(new RelatorioTarefasViewModel());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
