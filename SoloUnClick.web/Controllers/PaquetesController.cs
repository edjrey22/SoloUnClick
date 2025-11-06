using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloUnClick.Domain.Entities;
using SoloUnClick.Domain.Enums;
using SoloUnClick.Domain.Interfaces;
using SoloUnClick.web.Models.ViewModels;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace SoloUnClick.web.Controllers;

public class PaquetesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PaquetesController> _logger;
    private const int TamanoPagina = 12; // Número de paquetes por página

    public PaquetesController(IUnitOfWork unitOfWork, ILogger<PaquetesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // GET: Paquetes
    public async Task<IActionResult> Index(
        string? busqueda, 
        TipoViaje? tipoViaje, 
        decimal? precioMax,
        string? ordenarPor,
        int? pagina)
    {
        var viewModel = new PaquetesBusquedaViewModel
        {
            Busqueda = busqueda,
            TipoViaje = tipoViaje,
            PrecioMax = precioMax,
            OrdenarPor = ordenarPor,
            PaginaActual = pagina ?? 1
        };

        try
        {
            // Usar el repositorio especializado para filtrar
            var paquetes = await _unitOfWork.PaquetesTuristicos.GetPaquetesFiltradosAsync(
                busqueda: busqueda,
                tipoViaje: tipoViaje,
                precioMax: precioMax,
                soloActivos: true);

            var paquetesList = paquetes.ToList();

            // Aplicar ordenamiento
            paquetesList = ordenarPor switch
            {
                "precio_asc" => paquetesList.OrderBy(p => p.PrecioPorPersona).ToList(),
                "precio_desc" => paquetesList.OrderByDescending(p => p.PrecioPorPersona).ToList(),
                "duracion_asc" => paquetesList.OrderBy(p => p.DuracionDias).ToList(),
                "duracion_desc" => paquetesList.OrderByDescending(p => p.DuracionDias).ToList(),
                "popularidad" => paquetesList.OrderByDescending(p => p.Reservas?.Count ?? 0)
                                             .ThenByDescending(p => p.ObtenerCalificacionPromedio()).ToList(),
                _ => paquetesList.OrderBy(p => p.Nombre).ToList()
            };

            viewModel.TotalResultados = paquetesList.Count;

            // Paginar resultados
            var paginaActual = pagina ?? 1;
            viewModel.Paquetes = paquetesList.ToPagedList(paginaActual, TamanoPagina);

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar paquetes turísticos");
            TempData["Error"] = "Ocurrió un error al buscar los paquetes. Por favor, intente nuevamente.";
            return View(viewModel);
        }
    }

    // GET: Paquetes/Detalle/5 (mantener por compatibilidad)
    public async Task<IActionResult> Detalle(int id)
    {
        return await Details(id);
    }

    // GET: Paquetes/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            // Obtener el paquete completo
            var paquete = await _unitOfWork.PaquetesTuristicos.GetPaqueteCompletoAsync(id);

            if (paquete == null)
            {
                return NotFound();
            }

            // Obtener opiniones con información de usuarios
            var opinionesViewModel = new List<OpinionViewModel>();
            
            foreach (var opinion in paquete.Opiniones.OrderByDescending(o => o.FechaOpinion))
            {
                opinionesViewModel.Add(new OpinionViewModel
                {
                    Id = opinion.Id,
                    Titulo = opinion.Titulo,
                    Comentario = opinion.Comentario,
                    Calificacion = opinion.Calificacion,
                    FechaOpinion = opinion.FechaOpinion,
                    NombreUsuario = opinion.ApplicationUser?.Nombre ?? "Usuario",
                    ApellidoUsuario = opinion.ApplicationUser?.Apellido ?? "Anónimo"
                });
            }

            // Verificar si el usuario actual puede opinar
            bool puedeOpinar = false;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                // Verificar si tiene una reserva confirmada y completada para este paquete
                var tieneReservaCompletada = await _unitOfWork.Reservas.ExistsAsync(r =>
                    r.ApplicationUserId == userId &&
                    r.PaqueteTuristicoId == id &&
                    r.Estado == SoloUnClick.Domain.Enums.EstadoReserva.Confirmada &&
                    r.FechaViaje < DateTime.Now);

                // Verificar si ya ha opinado
                var yaOpino = await _unitOfWork.Opiniones.ExistsAsync(o =>
                    o.PaqueteTuristicoId == id &&
                    o.ApplicationUserId == userId);

                puedeOpinar = tieneReservaCompletada && !yaOpino;
            }

            var viewModel = new PaqueteDetalleViewModel
            {
                Paquete = paquete,
                Opiniones = opinionesViewModel,
                TotalOpiniones = opinionesViewModel.Count,
                CalificacionPromedio = paquete.ObtenerCalificacionPromedio(),
                PuedeOpinar = puedeOpinar,
                NuevaOpinion = new NuevaOpinionViewModel
                {
                    PaqueteTuristicoId = id
                }
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el detalle del paquete {PaqueteId}", id);
            TempData["Error"] = "Ocurrió un error al cargar los detalles del paquete.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Paquetes/AgregarOpinion
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarOpinion(PaqueteDetalleViewModel model)
    {
        try
        {
            // Validar solo el modelo de nueva opinión
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Por favor, completa todos los campos correctamente.";
                return RedirectToAction(nameof(Details), new { id = model.NuevaOpinion.PaqueteTuristicoId });
            }

            // Verificar que el paquete existe
            var paqueteExiste = await _unitOfWork.PaquetesTuristicos
                .ExistsAsync(p => p.Id == model.NuevaOpinion.PaqueteTuristicoId);

            if (!paqueteExiste)
            {
                TempData["Error"] = "El paquete turístico no existe.";
                return RedirectToAction(nameof(Index));
            }

            // Obtener el ID del usuario actual
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Debe iniciar sesión para agregar una opinión.";
                return RedirectToAction(nameof(Details), new { id = model.NuevaOpinion.PaqueteTuristicoId });
            }

            // SEGURIDAD: Verificar que el usuario tenga una reserva confirmada y completada
            var tieneReservaCompletada = await _unitOfWork.Reservas.ExistsAsync(r =>
                r.ApplicationUserId == userId &&
                r.PaqueteTuristicoId == model.NuevaOpinion.PaqueteTuristicoId &&
                r.Estado == SoloUnClick.Domain.Enums.EstadoReserva.Confirmada &&
                r.FechaViaje < DateTime.Now);

            if (!tieneReservaCompletada)
            {
                TempData["Error"] = "Solo puedes opinar sobre paquetes que hayas reservado y completado.";
                return RedirectToAction(nameof(Details), new { id = model.NuevaOpinion.PaqueteTuristicoId });
            }

            // Verificar si el usuario ya opinó sobre este paquete
            var yaOpino = await _unitOfWork.Opiniones.ExistsAsync(o => 
                o.PaqueteTuristicoId == model.NuevaOpinion.PaqueteTuristicoId && 
                o.ApplicationUserId == userId);

            if (yaOpino)
            {
                TempData["Warning"] = "Ya has enviado una opinión para este paquete.";
                return RedirectToAction(nameof(Details), new { id = model.NuevaOpinion.PaqueteTuristicoId });
            }

            // Crear la nueva opinión
            var nuevaOpinion = new Opinion
            {
                Titulo = model.NuevaOpinion.Titulo,
                Comentario = model.NuevaOpinion.Comentario,
                Calificacion = model.NuevaOpinion.Calificacion,
                FechaOpinion = DateTime.Now,
                PaqueteTuristicoId = model.NuevaOpinion.PaqueteTuristicoId,
                ApplicationUserId = userId
            };

            await _unitOfWork.Opiniones.AddAsync(nuevaOpinion);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "¡Gracias por tu opinión! Se ha agregado correctamente.";
            return RedirectToAction(nameof(Details), new { id = model.NuevaOpinion.PaqueteTuristicoId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar opinión para el paquete {PaqueteId}", model.NuevaOpinion.PaqueteTuristicoId);
            TempData["Error"] = "Ocurrió un error al agregar tu opinión. Por favor, intenta nuevamente.";
            return RedirectToAction(nameof(Details), new { id = model.NuevaOpinion.PaqueteTuristicoId });
        }
    }
}
