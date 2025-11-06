using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SoloUnClick.Domain.Entities;
using SoloUnClick.Domain.Enums;
using SoloUnClick.Infrastructure.Data;

namespace SoloUnClick.Infrastructure.Seed;

public class DataSeeder
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<DataSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Iniciando proceso de seed de datos...");

            // Verificar si ya hay datos
            if (await _context.Users.AnyAsync())
            {
                _logger.LogInformation("La base de datos ya contiene datos. Saltando seed.");
                return;
            }

            // Seed de usuarios
            var usuarios = await SeedUsuariosAsync();
            _logger.LogInformation("Usuarios creados: {Count}", usuarios.Count);

            // Seed de paquetes turísticos
            var paquetes = await SeedPaquetesTuristicosAsync();
            _logger.LogInformation("Paquetes turísticos creados: {Count}", paquetes.Count);

            // Seed de reservas históricas (antes de opiniones)
            await SeedReservasHistoricasAsync(usuarios, paquetes);
            _logger.LogInformation("Reservas históricas creadas exitosamente");

            // Seed de opiniones
            await SeedOpinionesAsync(usuarios, paquetes);
            _logger.LogInformation("Opiniones creadas exitosamente");

            _logger.LogInformation("Proceso de seed completado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el proceso de seed de datos");
            throw;
        }
    }

    private async Task<List<ApplicationUser>> SeedUsuariosAsync()
    {
        var usuarios = new List<ApplicationUser>
        {
            // Usuario con historial de compras - Cliente VIP
            new ApplicationUser
            {
                UserName = "maria.garcia@example.com",
                Email = "maria.garcia@example.com",
                Nombre = "María",
                Apellido = "García",
                EmailConfirmed = true,
                TotalComprasHistoricas = 3500.00m,
                NivelDescuento = 0.05m // 5% de descuento
            },
            // Usuario con historial de compras - Cliente Frecuente
            new ApplicationUser
            {
                UserName = "carlos.rodriguez@example.com",
                Email = "carlos.rodriguez@example.com",
                Nombre = "Carlos",
                Apellido = "Rodríguez",
                EmailConfirmed = true,
                TotalComprasHistoricas = 5800.00m,
                NivelDescuento = 0.10m // 10% de descuento
            },
            // Usuarios nuevos sin historial
            new ApplicationUser
            {
                UserName = "ana.martinez@example.com",
                Email = "ana.martinez@example.com",
                Nombre = "Ana",
                Apellido = "Martínez",
                EmailConfirmed = true,
                TotalComprasHistoricas = 0m,
                NivelDescuento = 0m
            },
            new ApplicationUser
            {
                UserName = "luis.lopez@example.com",
                Email = "luis.lopez@example.com",
                Nombre = "Luis",
                Apellido = "López",
                EmailConfirmed = true,
                TotalComprasHistoricas = 0m,
                NivelDescuento = 0m
            },
            new ApplicationUser
            {
                UserName = "sofia.fernandez@example.com",
                Email = "sofia.fernandez@example.com",
                Nombre = "Sofía",
                Apellido = "Fernández",
                EmailConfirmed = true,
                TotalComprasHistoricas = 0m,
                NivelDescuento = 0m
            }
        };

        foreach (var usuario in usuarios)
        {
            var result = await _userManager.CreateAsync(usuario, "Pass123!");
            if (!result.Succeeded)
            {
                _logger.LogError("Error al crear usuario {Email}: {Errors}",
                    usuario.Email,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        return usuarios;
    }

    private async Task<List<PaqueteTuristico>> SeedPaquetesTuristicosAsync()
    {
        var paquetes = new List<PaqueteTuristico>
        {
            new PaqueteTuristico
            {
                Nombre = "Cusco Mágico y Machu Picchu",
                DescripcionDetallada = "Descubre la magia del antiguo Imperio Inca en este tour de 5 días que combina la belleza " +
                    "histórica de Cusco con la majestuosidad de Machu Picchu. Explora el Valle Sagrado, visita mercados locales, " +
                    "degusta la gastronomía andina y maravíllate con las ruinas incas. Incluye transporte en tren panorámico, " +
                    "guías especializados, entradas a todos los sitios arqueológicos y alojamiento en hoteles boutique.",
                Ubicacion = "Cusco",
                Region = "Sierra",
                PrecioPorPersona = 1850.00m,
                DuracionDias = 5,
                TipoViaje = TipoViaje.Cultural,
                PlazasDisponibles = 50,
                Activo = true,
                Tags = "machupicchu,inca,maravilla,cusco,vallesagrado",
                RutaImagenPrincipal = "https://images.unsplash.com/photo-1587595431973-160d0d94add1?w=800"
            },
            new PaqueteTuristico
            {
                Nombre = "Aventura en la Selva de Iquitos",
                DescripcionDetallada = "Sumérgete en la biodiversidad del Amazonas peruano con este tour de 4 días en Iquitos. " +
                    "Navega por el río Amazonas, observa delfines rosados, explora la flora y fauna exótica, visita comunidades nativas " +
                    "y duerme en un lodge ecológico en plena selva. Incluye excursiones en canoa, caminatas nocturnas, pesca de pirañas " +
                    "y avistamiento de aves. Una experiencia única en contacto con la naturaleza.",
                Ubicacion = "Iquitos",
                Region = "Selva",
                PrecioPorPersona = 1650.00m,
                DuracionDias = 4,
                TipoViaje = TipoViaje.Aventura,
                PlazasDisponibles = 50,
                Activo = true,
                Tags = "amazonas,naturaleza,rio,selva,delfines",
                RutaImagenPrincipal = "https://images.unsplash.com/photo-1516026672322-bc52d61a55d5?w=800"
            },
            new PaqueteTuristico
            {
                Nombre = "Relax en las Playas de Máncora",
                DescripcionDetallada = "Escapa al paraíso tropical de Máncora, famoso por sus playas de arena blanca y aguas cálidas. " +
                    "Este paquete de 3 días incluye alojamiento en resort frente al mar, clases de surf para todos los niveles, " +
                    "masajes relajantes, yoga al atardecer y excursiones a playas vírgenes. Disfruta de la vida nocturna, la gastronomía " +
                    "marina y el clima perfecto todo el año. Ideal para desconectar y recargar energías.",
                Ubicacion = "Máncora",
                Region = "Costa",
                PrecioPorPersona = 980.00m,
                DuracionDias = 3,
                TipoViaje = TipoViaje.Playa,
                PlazasDisponibles = 50,
                Activo = true,
                Tags = "playa,surf,relax,norteperuano,tropical",
                RutaImagenPrincipal = "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=800"
            },
            new PaqueteTuristico
            {
                Nombre = "Trekking en Huaraz: Laguna 69",
                DescripcionDetallada = "Vive una aventura de montaña inolvidable en Huaraz, conocida como la 'Suiza peruana'. " +
                    "Este tour de 4 días incluye trekking a la espectacular Laguna 69, con sus aguas turquesas rodeadas de nevados. " +
                    "Explora el Parque Nacional Huascarán, visita las lagunas de Llanganuco y acampa bajo un cielo estrellado. " +
                    "Incluye equipo de camping, guías expertos, transporte y todas las comidas. Para aventureros en buena condición física.",
                Ubicacion = "Huaraz",
                Region = "Sierra",
                PrecioPorPersona = 1450.00m,
                DuracionDias = 4,
                TipoViaje = TipoViaje.Aventura,
                PlazasDisponibles = 50,
                Activo = true,
                Tags = "montaña,trekking,nevados,cordillerablanca,naturaleza",
                RutaImagenPrincipal = "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?w=800"
            },
            new PaqueteTuristico
            {
                Nombre = "Tour Gastronómico en Lima",
                DescripcionDetallada = "Descubre por qué Lima es considerada la capital gastronómica de Sudamérica en este tour de 2 días. " +
                    "Visita mercados tradicionales, participa en una clase de cocina peruana, cena en restaurantes galardonados y " +
                    "aprende a preparar el pisco sour perfecto. Degusta ceviche, anticuchos, causa limeña y picarones. " +
                    "Incluye tour por Miraflores y Barranco, el distrito bohemio de Lima. Una experiencia culinaria inolvidable.",
                Ubicacion = "Lima",
                Region = "Costa",
                PrecioPorPersona = 750.00m,
                DuracionDias = 2,
                TipoViaje = TipoViaje.Gastronomico,
                PlazasDisponibles = 50,
                Activo = true,
                Tags = "comida,restaurantes,ciudad,ceviche,gastronomia",
                RutaImagenPrincipal = "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=800"
            },
            new PaqueteTuristico
            {
                Nombre = "Ruta del Sillar y Cañón del Colca",
                DescripcionDetallada = "Explora la 'Ciudad Blanca' de Arequipa y sus maravillas naturales en este tour de 3 días. " +
                    "Visita las canteras de sillar, descubre la arquitectura colonial, y aventúrate al impresionante Cañón del Colca, " +
                    "uno de los más profundos del mundo. Observa el majestuoso vuelo del cóndor, relájate en aguas termales naturales " +
                    "y visita pueblos tradicionales. Incluye transporte, guía, entradas y alojamiento en hotel con vista al volcán Misti.",
                Ubicacion = "Arequipa",
                Region = "Sierra",
                PrecioPorPersona = 1280.00m,
                DuracionDias = 3,
                TipoViaje = TipoViaje.Cultural,
                PlazasDisponibles = 50,
                Activo = true,
                Tags = "arquitectura,ciudadblanca,volcan,colca,condor",
                RutaImagenPrincipal = "https://images.unsplash.com/photo-1531968455429-8c233a35b44c?w=800"
            },
            new PaqueteTuristico
            {
                Nombre = "Misterio en las Líneas de Nazca",
                DescripcionDetallada = "Sobrevuela uno de los enigmas más fascinantes de la humanidad: las Líneas de Nazca. " +
                    "Este tour de 2 días incluye vuelo en avioneta sobre las famosas figuras geométricas y geoglifos, visita al museo " +
                    "María Reiche, excursión al Oasis de Huacachina con sandboarding, y tour por las islas Ballestas. " +
                    "Descubre teorías sobre el origen de estas misteriosas líneas y maravíllate con la ingeniería antigua. " +
                    "Incluye transporte, vuelo certificado, guía arqueólogo y alojamiento.",
                Ubicacion = "Ica/Nazca",
                Region = "Costa",
                PrecioPorPersona = 890.00m,
                DuracionDias = 2,
                TipoViaje = TipoViaje.Cultural,
                PlazasDisponibles = 50,
                Activo = true,
                Tags = "desierto,misterio,nazca,lineas,arqueologia",
                RutaImagenPrincipal = "https://images.unsplash.com/photo-1580837119756-563d608dd119?w=800"
            },
            new PaqueteTuristico
            {
                Nombre = "Reserva Nacional de Paracas",
                DescripcionDetallada = "Descubre la biodiversidad marina de Paracas en este tour de 2 días. Navega hacia las Islas Ballestas, " +
                    "conocidas como las 'pequeñas Galápagos', donde observarás leones marinos, pingüinos de Humboldt y miles de aves. " +
                    "Explora la Reserva Nacional de Paracas en buggy, visita la Catedral (formación rocosa), playas Roja y Lagunillas. " +
                    "Incluye tour en lancha, transporte, guía naturalista, almuerzo con vista al mar y alojamiento en hotel boutique. " +
                    "Perfecto para amantes de la naturaleza y fotografía.",
                Ubicacion = "Ica/Paracas",
                Region = "Costa",
                PrecioPorPersona = 680.00m,
                DuracionDias = 2,
                TipoViaje = TipoViaje.Aventura,
                PlazasDisponibles = 50,
                Activo = true,
                Tags = "islasballestas,reserva,naturaleza,marinos,paracas",
                RutaImagenPrincipal = "https://images.unsplash.com/photo-1559827260-dc66d52bef19?w=800"
            },
            new PaqueteTuristico
            {
                Nombre = "Kuelap y la Fortaleza Oculta",
                DescripcionDetallada = "Viaja a Chachapoyas y descubre Kuelap, la impresionante fortaleza pre-inca construida por la cultura Chachapoya. " +
                    "Este tour de 4 días incluye teleférico moderno hasta la ciudadela, caminatas por bosques nubosos, visita a las Cataratas " +
                    "de Gocta (una de las más altas del mundo), exploración de sarcófagos de Karajía y descenso a la Caverna de Quiocta. " +
                    "Alojamiento en lodge con vista panorámica, guías expertos en historia local, todas las entradas y transporte. " +
                    "Una aventura arqueológica fuera de lo común.",
                Ubicacion = "Chachapoyas",
                Region = "Selva",
                PrecioPorPersona = 1580.00m,
                DuracionDias = 4,
                TipoViaje = TipoViaje.Cultural,
                PlazasDisponibles = 50,
                Activo = true,
                Tags = "arqueologia,chachapoyas,selvaalta,kuelap,catarata",
                RutaImagenPrincipal = "https://images.unsplash.com/photo-1501594907352-04cda38ebc29?w=800"
            },
            new PaqueteTuristico
            {
                Nombre = "Ayacucho: Iglesias y Artesanía",
                DescripcionDetallada = "Sumérgete en la rica historia colonial y artesanal de Ayacucho, conocida como la 'Ciudad de las 33 Iglesias'. " +
                    "Este tour de 3 días incluye visitas a templos coloniales con retablos barrocos dorados, talleres de artesanos locales " +
                    "especializados en retablos ayacuchanos y piedra de Huamanga, tour gastronómico degustando puca picante y mondongo ayacuchano, " +
                    "y excursión al complejo arqueológico de Wari. Incluye alojamiento en hotel colonial restaurado, guía historiador, " +
                    "todas las entradas y transporte. Perfecto para amantes del arte y la cultura.",
                Ubicacion = "Ayacucho",
                Region = "Sierra",
                PrecioPorPersona = 850.00m,
                DuracionDias = 3,
                TipoViaje = TipoViaje.Cultural,
                PlazasDisponibles = 50,
                Activo = true,
                Tags = "artesania,iglesias,historia,colonial,wari",
                RutaImagenPrincipal = "https://images.unsplash.com/photo-1526392060635-9d6019884377?w=800"
            }
        };

        await _context.PaquetesTuristicos.AddRangeAsync(paquetes);
        await _context.SaveChangesAsync();

        return paquetes;
    }

    private async Task SeedReservasHistoricasAsync(List<ApplicationUser> usuarios, List<PaqueteTuristico> paquetes)
    {
        var reservas = new List<Reserva>();

        // Usuario 1: María García (TotalComprasHistoricas = 3500.00)
        var maria = usuarios[0];
        reservas.Add(new Reserva
        {
            FechaReserva = DateTime.Now.AddMonths(-6),
            FechaViaje = DateTime.Now.AddMonths(-5).AddDays(-10),
            NumeroPasajeros = 2,
            PrecioTotal = 1850.00m * 2, // Cusco Mágico
            Estado = EstadoReserva.Confirmada,
            PaqueteTuristicoId = paquetes[0].Id,
            ApplicationUserId = maria.Id
        });

        // Usuario 2: Carlos Rodríguez (TotalComprasHistoricas = 5800.00)
        var carlos = usuarios[1];
        reservas.Add(new Reserva
        {
            FechaReserva = DateTime.Now.AddMonths(-8),
            FechaViaje = DateTime.Now.AddMonths(-7).AddDays(-15),
            NumeroPasajeros = 2,
            PrecioTotal = 1650.00m * 2, // Iquitos
            Estado = EstadoReserva.Confirmada,
            PaqueteTuristicoId = paquetes[1].Id,
            ApplicationUserId = carlos.Id
        });

        reservas.Add(new Reserva
        {
            FechaReserva = DateTime.Now.AddMonths(-4),
            FechaViaje = DateTime.Now.AddMonths(-3).AddDays(-5),
            NumeroPasajeros = 1,
            PrecioTotal = 1280.00m, // Arequipa
            Estado = EstadoReserva.Confirmada,
            PaqueteTuristicoId = paquetes[5].Id,
            ApplicationUserId = carlos.Id
        });

        await _context.Reservas.AddRangeAsync(reservas);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Creadas {Count} reservas históricas", reservas.Count);
    }

    private async Task SeedOpinionesAsync(List<ApplicationUser> usuarios, List<PaqueteTuristico> paquetes)
    {
        var random = new Random();
        var opiniones = new List<Opinion>();

        var titulosYComentarios = new[]
        {
            new { 
                Titulo = "¡Experiencia inolvidable!", 
                Comentario = "Todo estuvo perfectamente organizado y los guías fueron excepcionales. Los paisajes superaron mis expectativas. Definitivamente volvería."
            },
            new { 
                Titulo = "Superó todas mis expectativas", 
                Comentario = "Los paisajes son impresionantes y el servicio de primera calidad. La atención al detalle fue extraordinaria. Muy recomendable."
            },
            new { 
                Titulo = "Una aventura maravillosa", 
                Comentario = "Experiencia de principio a fin totalmente recomendable para toda la familia. Los niños se divirtieron muchísimo."
            },
            new { 
                Titulo = "Me encantó cada momento", 
                Comentario = "La atención al detalle fue extraordinaria. Los guías conocían muy bien la historia y cultura local. Un viaje perfecto."
            },
            new { 
                Titulo = "¡Increíble! Vale cada centavo", 
                Comentario = "Definitivamente volvería a tomar este tour. Los lugares visitados son espectaculares y el clima fue perfecto. Excelente inversión."
            },
            new { 
                Titulo = "Excelente organización", 
                Comentario = "Guías muy profesionales y conocedores. Una experiencia que recordaré siempre. Todo salió según lo planeado."
            },
            new { 
                Titulo = "Todo fue perfecto", 
                Comentario = "El itinerario, las comidas, el alojamiento. Muy satisfecho con el servicio. No cambiaría nada."
            },
            new { 
                Titulo = "Una experiencia única", 
                Comentario = "Vale totalmente la pena. Los lugares visitados son espectaculares. La fotografía que tomé es increíble."
            },
            new { 
                Titulo = "Muy buena experiencia", 
                Comentario = "Todo según lo programado. Los guías conocen muy bien la historia local. Aprendí mucho sobre la cultura."
            },
            new { 
                Titulo = "Hermoso tour bien planificado", 
                Comentario = "Las vistas son espectaculares y el clima fue perfecto. El hotel superó mis expectativas. Muy recomendado."
            },
            new { 
                Titulo = "Buena experiencia en general", 
                Comentario = "Algunos detalles podrían mejorar pero en general satisfactorio. Los lugares son hermosos y el guía fue amable."
            },
            new { 
                Titulo = "Tour agradable", 
                Comentario = "Buenos momentos. El guía fue amable aunque podría dar más información histórica. En general, una buena opción."
            },
            new { 
                Titulo = "Bien organizado", 
                Comentario = "Cumplió con lo prometido. Una buena opción para conocer el lugar. El tiempo fue un poco limitado pero disfrutamos."
            },
            new { 
                Titulo = "Experiencia positiva", 
                Comentario = "Los lugares son hermosos aunque el tiempo fue un poco limitado. Aún así, valió la pena el viaje."
            },
            new { 
                Titulo = "Buen servicio", 
                Comentario = "Aunque hubo pequeños retrasos, en general cumplieron con lo esperado. Los lugares visitados fueron increíbles."
            }
        };

        // Crear 15 opiniones distribuidas entre los paquetes
        var opinionesCreadas = 0;
        while (opinionesCreadas < 15)
        {
            var paquete = paquetes[random.Next(paquetes.Count)];
            var usuario = usuarios[random.Next(usuarios.Count)];
            
            // Evitar opiniones duplicadas del mismo usuario en el mismo paquete
            if (opiniones.Any(o => o.PaqueteTuristicoId == paquete.Id && o.ApplicationUserId == usuario.Id))
                continue;

            var tituloComentario = titulosYComentarios[random.Next(titulosYComentarios.Length)];
            var calificacion = random.Next(3, 6); // 3-5 estrellas
            var diasAtras = random.Next(1, 180);

            opiniones.Add(new Opinion
            {
                Titulo = tituloComentario.Titulo,
                Comentario = tituloComentario.Comentario,
                Calificacion = calificacion,
                FechaOpinion = DateTime.Now.AddDays(-diasAtras),
                PaqueteTuristicoId = paquete.Id,
                ApplicationUserId = usuario.Id
            });

            opinionesCreadas++;
        }

        await _context.Opiniones.AddRangeAsync(opiniones);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Creadas {Count} opiniones", opiniones.Count);
    }
}
