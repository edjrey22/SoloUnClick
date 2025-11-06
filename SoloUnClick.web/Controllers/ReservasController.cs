using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloUnClick.Domain.Entities;
using SoloUnClick.Domain.Interfaces;
using SoloUnClick.web.Models.ViewModels;
using System.Security.Claims;

namespace SoloUnClick.web.Controllers;

[Authorize]
public class ReservasController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReservasController> _logger;

    public ReservasController(IUnitOfWork unitOfWork, ILogger<ReservasController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // GET: Reservas/Crear/5
    [HttpGet]
    public async Task<IActionResult> Crear(int paqueteId)
    {
        try
        {
            // Obtener el paquete
            var paquete = await _unitOfWork.PaquetesTuristicos.GetByIdAsync(paqueteId);

            if (paquete == null)
            {
                TempData["Error"] = "El paquete turístico no existe.";
                return RedirectToAction("Index", "Paquetes");
            }

            // Crear el ViewModel
            var viewModel = new ReservaCrearViewModel
            {
                Paquete = paquete,
                Formulario = new ReservaFormViewModel
                {
                    PaqueteTuristicoId = paqueteId,
                    FechaViaje = DateTime.Now.AddDays(7), // Por defecto 7 días desde hoy
                    NumeroPasajeros = 1,
                    PrecioTotal = paquete.PrecioPorPersona
                }
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el formulario de reserva para el paquete {PaqueteId}", paqueteId);
            TempData["Error"] = "Ocurrió un error al cargar el formulario de reserva.";
            return RedirectToAction("Index", "Paquetes");
        }
    }

    // POST: Reservas/Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(ReservaFormViewModel model)
    {
        try
        {
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                // Recargar el paquete para mostrar el formulario nuevamente
                var paqueteError = await _unitOfWork.PaquetesTuristicos.GetByIdAsync(model.PaqueteTuristicoId);
                if (paqueteError == null)
                {
                    TempData["Error"] = "El paquete turístico no existe.";
                    return RedirectToAction("Index", "Paquetes");
                }

                var viewModelError = new ReservaCrearViewModel
                {
                    Paquete = paqueteError,
                    Formulario = model
                };

                return View(viewModelError);
            }

            // Verificar que la fecha de viaje sea futura
            if (model.FechaViaje.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("Formulario.FechaViaje", "La fecha de viaje debe ser posterior a hoy.");
                
                var paqueteError = await _unitOfWork.PaquetesTuristicos.GetByIdAsync(model.PaqueteTuristicoId);
                var viewModelError = new ReservaCrearViewModel
                {
                    Paquete = paqueteError!,
                    Formulario = model
                };
                
                return View(viewModelError);
            }

            // SEGURIDAD CRÍTICA: Cargar el paquete desde la base de datos - NO confiar en datos del cliente
            var paquete = await _unitOfWork.PaquetesTuristicos.GetByIdAsync(model.PaqueteTuristicoId);
            
            if (paquete == null)
            {
                TempData["Error"] = "El paquete turístico no existe.";
                return RedirectToAction("Index", "Paquetes");
            }

            // Verificar que el paquete esté activo
            if (!paquete.Activo)
            {
                TempData["Error"] = "Este paquete turístico no está disponible actualmente.";
                return RedirectToAction("Details", "Paquetes", new { id = model.PaqueteTuristicoId });
            }

            // VALIDACIÓN DE INVENTARIO: Verificar plazas disponibles
            if (paquete.PlazasDisponibles < model.NumeroPasajeros)
            {
                TempData["Error"] = $"Lo sentimos, solo hay {paquete.PlazasDisponibles} plazas disponibles para este paquete.";
                return RedirectToAction(nameof(Crear), new { paqueteId = model.PaqueteTuristicoId });
            }

            // Obtener el ID del usuario autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Debe iniciar sesión para realizar una reserva.";
                return RedirectToAction("Login", "Account");
            }

            // SEGURIDAD CRÍTICA: Recalcular el precio total en el servidor
            var precioTotal = paquete.PrecioPorPersona * model.NumeroPasajeros;

            // Crear la reserva con estado Pendiente por defecto
            var reserva = new Reserva
            {
                FechaReserva = DateTime.Now,
                FechaViaje = model.FechaViaje,
                NumeroPasajeros = model.NumeroPasajeros,
                PrecioTotal = precioTotal,
                Estado = SoloUnClick.Domain.Enums.EstadoReserva.Confirmada, // En producción, sería Pendiente hasta confirmar pago
                PaqueteTuristicoId = model.PaqueteTuristicoId,
                ApplicationUserId = userId
            };

            // Iniciar transacción
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Agregar la reserva
                await _unitOfWork.Reservas.AddAsync(reserva);
                
                // ACTUALIZAR INVENTARIO: Descontar las plazas reservadas
                paquete.PlazasDisponibles -= model.NumeroPasajeros;
                _unitOfWork.PaquetesTuristicos.Update(paquete);
                
                // Confirmar la transacción
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Reserva creada exitosamente. ReservaId: {ReservaId}, UserId: {UserId}, PaqueteId: {PaqueteId}, PlazasReservadas: {Plazas}", 
                    reserva.Id, userId, model.PaqueteTuristicoId, model.NumeroPasajeros);

                TempData["Success"] = "¡Reserva realizada con éxito!";
                return RedirectToAction(nameof(Confirmacion), new { id = reserva.Id });
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la reserva para el paquete {PaqueteId}", model.PaqueteTuristicoId);
            TempData["Error"] = "Ocurrió un error al procesar tu reserva. Por favor, intenta nuevamente.";
            
            var paquete = await _unitOfWork.PaquetesTuristicos.GetByIdAsync(model.PaqueteTuristicoId);
            if (paquete != null)
            {
                var viewModel = new ReservaCrearViewModel
                {
                    Paquete = paquete,
                    Formulario = model
                };
                return View(viewModel);
            }
            
            return RedirectToAction("Index", "Paquetes");
        }
    }

    // GET: Reservas/Confirmacion/5
    [HttpGet]
    public async Task<IActionResult> Confirmacion(int id)
    {
        try
        {
            // Obtener la reserva
            var reserva = await _unitOfWork.Reservas.GetByIdAsync(id);
            
            if (reserva == null)
            {
                TempData["Error"] = "La reserva no existe.";
                return RedirectToAction("Index", "Home");
            }

            // Verificar que la reserva pertenece al usuario actual
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (reserva.ApplicationUserId != userId)
            {
                TempData["Error"] = "No tienes permiso para ver esta reserva.";
                return RedirectToAction("Index", "Home");
            }

            // Obtener el paquete
            var paquete = await _unitOfWork.PaquetesTuristicos.GetByIdAsync(reserva.PaqueteTuristicoId);
            
            if (paquete == null)
            {
                TempData["Error"] = "El paquete turístico no existe.";
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new ReservaConfirmacionViewModel
            {
                Reserva = reserva,
                Paquete = paquete
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la confirmación de la reserva {ReservaId}", id);
            TempData["Error"] = "Ocurrió un error al cargar la confirmación de tu reserva.";
            return RedirectToAction("Index", "Home");
        }
    }

    // GET: Reservas/MisReservas
    [HttpGet]
    public async Task<IActionResult> MisReservas()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Obtener todas las reservas del usuario
            var reservas = await _unitOfWork.Reservas.FindAsync(r => r.ApplicationUserId == userId);
            var reservasOrdenadas = reservas.OrderByDescending(r => r.FechaReserva).ToList();

            return View(reservasOrdenadas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar las reservas del usuario");
            TempData["Error"] = "Ocurrió un error al cargar tus reservas.";
            return RedirectToAction("Index", "Home");
        }
    }
}
