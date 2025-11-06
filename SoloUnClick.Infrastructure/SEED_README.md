# Sistema de Seed de Datos - SoloUnClick

## ?? Implementación Completada

### ?? Funcionalidad Principal

Se ha implementado un sistema completo de **seed de datos** que popula automáticamente la base de datos con información de ejemplo al iniciar la aplicación.

---

## ??? Arquitectura

### Clase DataSeeder

**Ubicación**: `SoloUnClick.Infrastructure\Seed\DataSeeder.cs`

**Responsabilidades**:
1. Crear usuarios de ejemplo con UserManager
2. Crear paquetes turísticos atractivos y realistas
3. Crear opiniones de ejemplo con calificaciones y comentarios
4. Gestionar el proceso completo de seed

### Método de Extensión

**Ubicación**: `SoloUnClick.web\Extensions\WebApplicationExtensions.cs`

**Funcionalidad**:
- Aplica migraciones pendientes automáticamente
- Ejecuta el seed de datos
- Manejo de errores con logging
- Se ejecuta al inicio de la aplicación

---

## ?? Usuarios de Ejemplo Creados

Se crean **5 usuarios** con las siguientes credenciales:

| Email | Nombre | Apellido | Contraseña |
|-------|--------|----------|------------|
| maria.garcia@example.com | María | García | Pass123! |
| carlos.rodriguez@example.com | Carlos | Rodríguez | Pass123! |
| ana.martinez@example.com | Ana | Martínez | Pass123! |
| luis.lopez@example.com | Luis | López | Pass123! |
| sofia.fernandez@example.com | Sofía | Fernández | Pass123! |

**Características**:
- ? Todos los usuarios tienen `EmailConfirmed = true`
- ? Contraseña común para desarrollo: `Pass123!`
- ? Creados usando `UserManager` de Identity
- ? Listos para hacer login y crear reservas/opiniones

---

## ?? Paquetes Turísticos Creados

Se crean **10 paquetes turísticos** distribuidos por tipo:

### ??? AVENTURA (3 paquetes)

#### 1. Aventura Inca - Machu Picchu 4 Días
- **Ubicación**: Cusco, Perú
- **Región**: Sierra
- **Precio**: S/ 1,850.00
- **Duración**: 4 días
- **Descripción**: Camino Inca hasta Machu Picchu con camping en sitios arqueológicos
- **Incluye**: Visitas a Ollantaytambo, Valle Sagrado, aguas termales

#### 2. Trekking Cordillera Blanca
- **Ubicación**: Huaraz, Ancash
- **Región**: Sierra
- **Precio**: S/ 1,450.00
- **Duración**: 5 días
- **Descripción**: Exploración de nevados, lagunas turquesas y flora andina
- **Incluye**: Lagunas 69 y Parón, acampada bajo las estrellas

#### 3. Rafting en el Río Urubamba
- **Ubicación**: Valle Sagrado, Cusco
- **Región**: Sierra
- **Precio**: S/ 680.00
- **Duración**: 2 días
- **Descripción**: Rápidos del río sagrado con diferentes niveles de dificultad
- **Incluye**: Equipo completo, instructores certificados, lodge junto al río

### ??? CULTURAL (3 paquetes)

#### 4. Lima Colonial y Moderna
- **Ubicación**: Lima, Perú
- **Región**: Costa
- **Precio**: S/ 450.00
- **Duración**: 3 días
- **Descripción**: Centro histórico Patrimonio de la Humanidad
- **Incluye**: Catedral, Palacio de Gobierno, Miraflores, Barranco, tour gastronómico

#### 5. Cusco Imperial - Ciudad de los Incas
- **Ubicación**: Cusco, Perú
- **Región**: Sierra
- **Precio**: S/ 980.00
- **Duración**: 4 días
- **Descripción**: Historia del Imperio Inca
- **Incluye**: Sacsayhuamán, Valle Sagrado, museo del chocolate, degustación de pisco

#### 6. Misterios de las Líneas de Nazca
- **Ubicación**: Nazca, Ica
- **Región**: Costa
- **Precio**: S/ 750.00
- **Duración**: 2 días
- **Descripción**: Sobrevuelo de las enigmáticas Líneas de Nazca
- **Incluye**: Vuelo en avioneta 30 min, mirador, museo Maria Reiche, acueductos

### ??? PLAYA (2 paquetes)

#### 7. Relax Total en Máncora
- **Ubicación**: Máncora, Piura
- **Región**: Costa
- **Precio**: S/ 1,250.00
- **Duración**: 5 días
- **Descripción**: Paraíso tropical del norte peruano
- **Incluye**: Resort frente al mar, spa, clases de surf, yoga, observación de tortugas

#### 8. Paracas y las Islas Ballestas
- **Ubicación**: Paracas, Ica
- **Región**: Costa
- **Precio**: S/ 580.00
- **Duración**: 2 días
- **Descripción**: 'Pequeñas Galápagos' del Perú
- **Incluye**: Navegación, leones marinos, pingüinos de Humboldt, playas

### ??? GASTRONÓMICO (2 paquetes)

#### 9. Tour Gastronómico Lima - Capital Culinaria
- **Ubicación**: Lima, Perú
- **Región**: Costa
- **Precio**: S/ 890.00
- **Duración**: 3 días
- **Descripción**: Capital gastronómica de América
- **Incluye**: Mercados locales, clase de cocina, restaurantes galardonados, pisco sour

#### 10. Ruta del Pisco y Vinos - Valle de Ica
- **Ubicación**: Ica, Perú
- **Región**: Costa
- **Precio**: S/ 620.00
- **Duración**: 2 días
- **Descripción**: Cuna del pisco peruano
- **Incluye**: Bodegas artesanales, degustaciones, almuerzos maridados, Oasis de Huacachina

---

## ? Sistema de Opiniones

### Generación Automática

Para cada paquete turístico se crean **2 a 4 opiniones** con las siguientes características:

**Distribución de Calificaciones**:
- ? **Rango**: 3 a 5 estrellas
- ? **Ponderación**: Más opiniones con 4-5 estrellas (positivas)
- ? **Realismo**: Mezcla de excelentes y buenas experiencias

**Características de las Opiniones**:
- ? **Usuarios únicos** por paquete (no se repiten)
- ? **Comentarios variados** según la calificación
- ? **Fechas aleatorias** en los últimos 6 meses
- ? **Asignación aleatoria** de usuarios a paquetes

### Comentarios de Ejemplo

#### Opiniones Positivas (4-5 estrellas):
- "¡Experiencia inolvidable! Todo estuvo perfectamente organizado y los guías fueron excepcionales."
- "Superó todas mis expectativas. Los paisajes son impresionantes y el servicio de primera calidad."
- "Una aventura maravillosa de principio a fin. Totalmente recomendable para toda la familia."
- "Me encantó cada momento del viaje. La atención al detalle fue extraordinaria."
- "¡Increíble! Definitivamente volvería a tomar este tour. Vale cada centavo."
- Y más...

#### Opiniones Buenas (3 estrellas):
- "Buena experiencia en general. Algunos detalles podrían mejorar pero en general satisfactorio."
- "Tour agradable con buenos momentos. El guía fue amable aunque podría dar más información."
- "Bien organizado, cumplió con lo prometido. Una buena opción para conocer el lugar."
- Y más...

---

## ?? Proceso de Seed

### Flujo de Ejecución

```
Inicio de la Aplicación (Program.cs)
    ?
UseDatabaseMigrationAndSeedAsync()
    ?
Verificar Migraciones Pendientes
    ?
Aplicar Migraciones (si hay)
    ?
Verificar si ya hay datos
    ?
Si no hay datos:
    ?? Crear 5 Usuarios con UserManager
    ?? Crear 10 Paquetes Turísticos
    ?? Crear 2-4 Opiniones por Paquete
    ?
Logging de resultados
    ?
Aplicación lista para usar
```

### Lógica de Verificación

```csharp
// Solo ejecuta seed si la base de datos está vacía
if (await _context.Users.AnyAsync())
{
    _logger.LogInformation("La base de datos ya contiene datos. Saltando seed.");
    return;
}
```

**Ventajas**:
- ? No duplica datos en ejecuciones subsiguientes
- ? Seguro para producción
- ? Puede ejecutarse múltiples veces sin problemas

---

## ?? Logging Implementado

### Mensajes de Log

#### Inicio del Proceso:
```
Verificando migraciones de base de datos...
Aplicando migraciones pendientes...
Migraciones aplicadas exitosamente
```

#### Durante el Seed:
```
Iniciando proceso de seed de datos...
Usuarios creados: 5
Paquetes turísticos creados: 10
Opiniones creadas exitosamente
```

#### Finalización:
```
Proceso de seed completado exitosamente
Proceso de inicialización de base de datos completado
```

#### Errores:
```
Error al crear usuario {Email}: {Errors}
Error durante el proceso de seed de datos
Error durante la inicialización de la base de datos
```

---

## ??? Configuración en Program.cs

### Línea Agregada

```csharp
var app = builder.Build();

// Ejecutar migraciones y seed de datos
await app.UseDatabaseMigrationAndSeedAsync();
```

**Ubicación**: Inmediatamente después de construir la aplicación

**Características**:
- ? Se ejecuta **antes** de configurar el pipeline HTTP
- ? Asíncrono (no bloquea el inicio)
- ? Maneja errores con logging
- ? En desarrollo, lanza excepción si falla
- ? En producción, continúa la ejecución

---

## ?? Seguridad

### Consideraciones

#### Contraseñas de Ejemplo:
```csharp
// Contraseña común: Pass123!
var result = await _userManager.CreateAsync(usuario, "Pass123!");
```

**?? Advertencia**: Estas contraseñas son solo para desarrollo.

**Recomendaciones para Producción**:
1. Cambiar todas las contraseñas
2. No usar seed en producción
3. Usar secretos de configuración
4. Implementar política de contraseñas más fuerte

---

## ?? Estadísticas de Datos

### Resumen de Datos Generados

| Tipo de Dato | Cantidad | Detalles |
|--------------|----------|----------|
| **Usuarios** | 5 | Distribuidos para opiniones |
| **Paquetes** | 10 | 5 tipos diferentes |
| **Opiniones** | 20-40 | 2-4 por paquete |

### Distribución por Tipo de Viaje

| Tipo | Cantidad | Porcentaje |
|------|----------|------------|
| Aventura | 3 | 30% |
| Cultural | 3 | 30% |
| Playa | 2 | 20% |
| Gastronómico | 2 | 20% |

### Distribución de Precios

| Rango | Cantidad | Ejemplos |
|-------|----------|----------|
| S/ 400-700 | 4 | Lima Colonial, Rafting, Paracas, Ruta Pisco |
| S/ 700-1000 | 3 | Nazca, Tour Gastronómico, Cusco Imperial |
| S/ 1000-1500 | 2 | Máncora, Cordillera Blanca |
| S/ 1500-2000 | 1 | Aventura Inca |

---

## ?? URLs de Imágenes

### Fuente: Unsplash

Todas las imágenes son de **Unsplash** (free to use):

```csharp
RutaImagenPrincipal = "https://images.unsplash.com/photo-{id}?w=800"
```

**Características**:
- ? Alta calidad
- ? Uso gratuito
- ? Optimizadas para web (800px width)
- ? Temáticas apropiadas

**Nota**: Para producción, se recomienda:
1. Descargar y hospedar las imágenes localmente
2. Usar un CDN propio
3. Optimizar para diferentes tamaños de pantalla

---

## ?? Ventajas del Sistema

### 1. Desarrollo Rápido
- ? No necesitas crear datos manualmente
- ? Base de datos lista al iniciar
- ? Testing inmediato de funcionalidades

### 2. Demostración
- ? Datos realistas para presentaciones
- ? Variedad de tipos de viaje
- ? Opiniones con diferentes calificaciones

### 3. Testing
- ? Datos consistentes en todos los entornos
- ? Escenarios de prueba predefinidos
- ? Usuarios de ejemplo para login

### 4. Mantenimiento
- ? Fácil de modificar
- ? Centralizado en una clase
- ? Bien documentado con logging

---

## ?? Personalización

### Agregar Más Usuarios

```csharp
usuarios.Add(new ApplicationUser
{
    UserName = "nuevo.usuario@example.com",
    Email = "nuevo.usuario@example.com",
    Nombre = "Nuevo",
    Apellido = "Usuario",
    EmailConfirmed = true
});
```

### Agregar Más Paquetes

```csharp
paquetes.Add(new PaqueteTuristico
{
    Nombre = "Tu Nuevo Paquete",
    DescripcionDetallada = "Descripción detallada...",
    Ubicacion = "Ciudad, Región",
    Region = "Costa/Sierra/Selva",
    PrecioPorPersona = 999.00m,
    DuracionDias = 3,
    TipoViaje = TipoViaje.Aventura,
    RutaImagenPrincipal = "https://..."
});
```

### Modificar Comentarios

```csharp
var comentariosPersonalizados = new[]
{
    "Tu comentario personalizado aquí",
    "Otro comentario...",
    // Agrega más...
};
```

---

## ?? Comandos Útiles

### Crear Nueva Migración

```bash
dotnet ef migrations add NombreMigracion --project SoloUnClick.Infrastructure --startup-project SoloUnClick.web
```

### Aplicar Migraciones Manualmente

```bash
dotnet ef database update --project SoloUnClick.Infrastructure --startup-project SoloUnClick.web
```

### Limpiar Base de Datos

```bash
dotnet ef database drop --project SoloUnClick.Infrastructure --startup-project SoloUnClick.web
```

**Nota**: Después de `drop`, simplemente reinicia la aplicación para recrear y sembrar.

---

## ? Checklist de Implementación

- [x] Crear clase DataSeeder en Infrastructure
- [x] Implementar método SeedUsuariosAsync (5 usuarios)
- [x] Implementar método SeedPaquetesTuristicosAsync (10 paquetes)
- [x] Implementar método SeedOpinionesAsync (2-4 por paquete)
- [x] Crear WebApplicationExtensions en proyecto web
- [x] Implementar UseDatabaseMigrationAndSeedAsync
- [x] Modificar Program.cs para ejecutar seed al inicio
- [x] Agregar Microsoft.EntityFrameworkCore.Tools
- [x] Implementar logging completo
- [x] Implementar verificación de datos existentes
- [x] Manejo de errores con try-catch
- [x] Comentarios variados según calificación
- [x] Fechas aleatorias en opiniones
- [x] Usuarios únicos por paquete en opiniones
- [x] Contraseñas de ejemplo documentadas
- [x] URLs de imágenes de Unsplash
- [x] Distribución realista de tipos de viaje
- [x] Precios realistas por duración
- [x] Descripciones detalladas y atractivas
- [x] Compilación exitosa

---

## ?? Resultado Final

El sistema de seed está **completamente implementado y funcional**, proporcionando:

? **5 usuarios de ejemplo** listos para login (Pass123!)  
? **10 paquetes turísticos** atractivos y realistas  
? **20-40 opiniones** distribuidas con calificaciones 3-5 estrellas  
? **Migraciones automáticas** al iniciar la aplicación  
? **Logging completo** del proceso  
? **Verificación de datos** para evitar duplicados  
? **Imágenes de alta calidad** de Unsplash  
? **Datos realistas** para demostración y testing  

**¡La base de datos se pobla automáticamente al iniciar la aplicación!** ??

### Próximos Pasos

1. **Ejecutar la aplicación** por primera vez
2. **Verificar logs** en la consola
3. **Hacer login** con cualquiera de los usuarios de ejemplo
4. **Explorar paquetes** creados
5. **Ver opiniones** en cada paquete
6. **Crear reservas** con los usuarios de ejemplo
