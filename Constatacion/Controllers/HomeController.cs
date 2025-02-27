using Constatacion.Models;
using Dominio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistencia.Models;
using SkiaSharp;
using System.Diagnostics;
using Constatacion.ViewModels;


namespace Constatacion.Controllers
{
  
    public class HomeController : Controller
    {

        EntitiesDomain entitiesDomain;

        EntitiesDomainM entitiesDomainM;
        ILogger logger;
        //readonly IMapper _mapper;

        ErrorViewModel errorVM;
        public HomeController(DbContextOptions<ConstatacionContext> options, ILoggerFactory log)
        {
            entitiesDomainM = new EntitiesDomainM(options);
            entitiesDomain = new EntitiesDomain(options);
         
        }


        public IActionResult Index()
        {

            //var Consultas = entitiesDomain.BienesRepositorio.ObtenerTodos().ToList();


            return View();
        }
        public IActionResult BuscarPorCodigoEsbay(string codigoEsbay)
        {
            if (!int.TryParse(codigoEsbay, out int codigoEsbayInt)) // Intentar convertir a número
            {
                TempData["MensajeNoEncontrado"] = "El código Esbay proporcionado no es válido.";
                return View("Index", new List<Bien>()); // Devolver una lista vacía en este caso
            }

            IQueryable<Bien> resultadosQuery = entitiesDomain.BienesRepositorio.ObtenerTodos().AsQueryable();
            resultadosQuery = resultadosQuery.Where(b => b.CodigoEsbay == codigoEsbayInt);

            var resultados = resultadosQuery.ToList();
            if (resultados.Count == 0)
            {
                TempData["MensajeNoEncontrado"] = "No se encontraron resultados.";
            }
            return View("Index", resultados);
        }




        [HttpGet]
        public IActionResult Eliminar(int codigoEsbay)
        {
            var bien = entitiesDomain.BienesRepositorio.BuscarPor(x => x.CodigoEsbay == codigoEsbay).FirstOrDefault();

            if (bien != null)
            {
                // Muestra la vista de confirmación para solicitudes GET
                return View(bien);
            }
            else
            {
                // Manejar el caso donde no se encuentra el objeto
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Eliminar(Bien bien)
        {
            if (bien != null)
            {
                // Realiza la lógica de eliminación para solicitudes POST
                entitiesDomain.BienesRepositorio.Eliminar(bien);
                entitiesDomain.GuardarTransacciones();
                return RedirectToAction("Index");
            }
            else
            {
                // Manejar el caso donde el objeto es nulo
                return NotFound();
            }
        }

        [HttpPost]

        public IActionResult Actualizar(Bien bien) // guarda cuando doy en el boton de actualizar
        {
            entitiesDomain.BienesRepositorio.Actualizar(bien);
            entitiesDomain.GuardarTransacciones();

            return RedirectToAction("Index");
        }
        [HttpGet]

        public IActionResult Actualizar(int codigoEsbay)
        {
            var bien = entitiesDomain.BienesRepositorio.BuscarPor(x => x.CodigoEsbay == codigoEsbay).FirstOrDefault();
            return View(bien);
        }

        public IActionResult Detalle(int codigoEsbay)
        {
            var bien = entitiesDomain.BienesRepositorio.BuscarPor(x => x.CodigoEsbay == codigoEsbay).FirstOrDefault();

            if (bien == null)
            {
                // Puedes manejar el caso donde no se encuentra el bien, por ejemplo, redirigiendo a una página de error.
                return RedirectToAction("Error");
            }

            return View(bien);
        }



        [HttpPost]

        public IActionResult Editar(Bien bien) // guarda cuando doy en el boton de actualizar
        {
            entitiesDomain.BienesRepositorio.Actualizar(bien);
            entitiesDomain.GuardarTransacciones();

            return RedirectToAction("Index");
        }
        [HttpGet]

        public IActionResult Editar(int codigoEsbay)
        {
            var bien = entitiesDomain.BienesRepositorio.BuscarPor(x => x.CodigoEsbay == codigoEsbay).FirstOrDefault();
            return View(bien);
        }

        [HttpPost]
        public IActionResult AgregarBien(Bien bien)
        {
            // Verificar si ya existe un Bien con el mismo CodigoEsbay
            var existingBien = entitiesDomain.BienesRepositorio.ObtenerPorCampo<int>(
                b => EF.Property<int>(b, "CodigoEsbay") == bien.CodigoEsbay
            );

            if (existingBien != null)
            {
                // Ya existe un Bien con el mismo CodigoEsbay, manejar el error
                ViewData["ErrorCodigoEsbay"] = "Ya existe un Bien con este código.";
                return View(); // Devolver la vista con el modelo para que el usuario corrija la entrada
            }

            // No existe un Bien con el mismo CodigoEsbay, continuar con la lógica para agregar el Bien
            entitiesDomain.BienesRepositorio.Insertar(bien);
            entitiesDomain.GuardarTransacciones();

            return RedirectToAction("Index");
        }

        [HttpGet]

        public IActionResult AgregarBien()
        {


            return View(new Bien());
        }



        [HttpPost]
        public IActionResult ActualizarMantenimiento(Mantenimiento mantenimiento)
        {
            // Verificar si el ModelState es válido antes de continuar
            if (!ModelState.IsValid)
            {
                // ModelState no es válido, devolver la vista con errores
                return View(mantenimiento);
            }

            // Actualizar el Mantenimiento en la base de datos
            if (mantenimiento.Id > 0)
            {
                // Si el mantenimiento ya existe, actualizar sus propiedades
                entitiesDomainM.MantenimientoRepositorio.Actualizar(mantenimiento);
            }
            else
            {
                // Si el mantenimiento no existe, insertarlo en la base de datos
                entitiesDomainM.MantenimientoRepositorio.Insertar(mantenimiento);
            }

            // Guardar transacciones en el contexto
            entitiesDomainM.GuardarTransacciones();
       
            return RedirectToAction("Index", new { codigoEsbay = mantenimiento.CodigoEsbay });


        }


        [HttpGet]
        public IActionResult ActualizarMantenimiento(int codigoEsbay)
        {
            // Obtener el Bien correspondiente al CodigoEsbay
            var bien = entitiesDomain.BienesRepositorio.BuscarPor(b => b.CodigoEsbay == codigoEsbay).FirstOrDefault();

            if (bien != null)
            {
                // Obtener el Mantenimiento correspondiente al CodigoEsbay (si ya existe)
                var mantenimientoExistente = entitiesDomainM.MantenimientoRepositorio.BuscarPor(m => m.CodigoEsbay == codigoEsbay).FirstOrDefault();

                // Si el mantenimiento existe, devolver la vista con la información para mostrar y editar
                if (mantenimientoExistente != null)
                {
                    return View(mantenimientoExistente);
                }

                // Si no existe mantenimiento, devolver la vista con la información del Bien
                var nuevoMantenimiento = new Mantenimiento
                {
                    CodigoEsbay = bien.CodigoEsbay,
                    // Inicializar otros campos según sea necesario
                };

                return View(nuevoMantenimiento);
            }

            // Manejar el caso donde no se encuentra el Bien
            return NotFound();
        }


        public IActionResult BuscarPorModelo(string modelo, string marca, int idDependencia, int idCampus)
        {
            IQueryable<Bien> resultadosQuery = entitiesDomain.BienesRepositorio.ObtenerTodos().AsQueryable();
            if (!string.IsNullOrEmpty(marca))
            {
                resultadosQuery = resultadosQuery.Where(b => b.Marca == marca);
            }
            if (!string.IsNullOrEmpty(modelo))
            {
                resultadosQuery = resultadosQuery.Where(b => b.Modelo == modelo);
            }

         

            if (idDependencia > 0)
            {
                resultadosQuery = resultadosQuery.Where(b => b.IdDependencia == idDependencia);
            }

            if (idCampus > 0)
            {
                resultadosQuery = resultadosQuery.Where(b => b.IdCampus == idCampus);
            }

            var resultados = resultadosQuery.ToList();
            if (resultados.Count == 0)
            {
                TempData["MensajeNoEncontrado"] = "No se encontraron resultados.";
            }
            return View("Index", resultados);
        }
        [HttpGet]
        public IActionResult ObtenerModelos()
        {

            var modelos = entitiesDomain.BienesRepositorio
                .ObtenerTodos()
                .Select(b => b.Modelo)
                .Distinct()
                .ToList(); // No necesitas ToListAsync() aquí

            return Json(modelos);
        }

        [HttpGet]
        public IActionResult ObtenerMarcas()
        {
            var marcas = entitiesDomain.BienesRepositorio
                .ObtenerTodos()
                .Select(b => b.Marca)
                .Distinct()
                .OrderBy(m => m)
                .ToList();

            return Json(marcas);
        }




        [HttpGet]
        public async Task<IActionResult> ObtenerDependencias()
        {
            var dependencias = entitiesDomain.BienesRepositorio
             .ObtenerTodos()
             .Select(b => b.IdDependencia)
             .Distinct()
             .ToList();

            return Json(dependencias);
        }





        [HttpGet]
        public IActionResult ObtenerCampus()
        {
            var campus = entitiesDomain.BienesRepositorio
                .ObtenerTodos()
                .Select(b => b.IdCampus)
                .Distinct()
                .ToList();

            return Json(campus);
        }

    

    


       
        [HttpGet]
        public IActionResult ObtenerDependenciasPorModelo(string modelo)
        {
            var dependencias = entitiesDomain.BienesRepositorio
                .ObtenerTodos()
                .Where(b => b.Modelo == modelo)
                .Select(b => b.IdDependencia)
                .Distinct()
                .ToList();

            return Json(dependencias);
        }

        [HttpGet]
        public IActionResult ObtenerMarcasPorModelo(string modelo)
        {
            var marcas = entitiesDomain.BienesRepositorio
                .ObtenerTodos()
                .Where(b => b.Modelo == modelo)
                .Select(b => b.Marca)
                .Distinct()
                .ToList();



            return Json(marcas);
        }
        [HttpGet]
        public IActionResult ObtenerModelosPorMarca(string marca)
        {
            try
            {
                var modelos = entitiesDomain.BienesRepositorio
                    .ObtenerTodos()
                    .Where(b => b.Marca == marca && !string.IsNullOrEmpty(b.Modelo))
                    .Select(b => b.Modelo)
                    .Distinct()
                    .ToList();

                return Json(modelos);
            }
            catch (Exception ex)
            {
                // Manejar la excepción de manera adecuada, por ejemplo, loguearla
                // y devolver un mensaje de error genérico al cliente.
                return Json(new { Error = "Error al obtener modelos." });
            }
        }
        [HttpGet]
        public IActionResult ObtenerCampusPorModelo(string modelo)
        {
            var campus = entitiesDomain.BienesRepositorio
              .ObtenerTodos()
              .Where(b => b.Modelo == modelo)
              .Select(b => b.IdCampus)
              .Distinct()
              .ToList();

            return Json(campus);
        }



    }
}

