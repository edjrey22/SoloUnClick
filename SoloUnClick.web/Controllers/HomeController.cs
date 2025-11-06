using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SoloUnClick.web.Models;
using SoloUnClick.web.Models.ViewModels;
using SoloUnClick.Domain.Interfaces;

namespace SoloUnClick.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeIndexViewModel();

            try
            {
                // Obtener todos los paquetes
                var todosPaquetes = await _unitOfWork.PaquetesTuristicos.GetAllAsync();
                var paquetesList = todosPaquetes.ToList();

                // Destinos Populares: Primeros 6 paquetes (podrías ordenar por número de reservas en el futuro)
                viewModel.DestinosPopulares = paquetesList.Take(6).ToList();

                // Ofertas de Temporada: Siguientes 4 paquetes (podrías filtrar por ofertas especiales en el futuro)
                viewModel.OfertasTemporada = paquetesList.Skip(6).Take(4).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar los paquetes turísticos en la página de inicio");
                // Continuar con listas vacías
            }

            return View(viewModel);
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
