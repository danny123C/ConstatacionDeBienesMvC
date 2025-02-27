using Constatacion.Models;

using Dominio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistencia.Models;
using System.Drawing;
using System.Security.Claims;
using ZXing;




namespace Constatacion.Controllers
{
    //[Authorize]
    public class ConstatacionController : Controller
    {

        EntitiesDomain entitiesDomain;
        EntitiesDomainnn entitiesDomainnn;
        ILogger logger;
        //readonly IMapper _mapper;

        ErrorViewModel errorVM;
        public ConstatacionController(DbContextOptions<ConstatacionContext> options, ILoggerFactory log)
        {

            entitiesDomain = new EntitiesDomain(options);
            entitiesDomainnn = new EntitiesDomainnn(options);

        }

        public IActionResult ConstatacionIndex()
        {
            return View();
        }
        public IActionResult BuscarPorCodigoEsbay(string codigoEsbay)
        {
            IQueryable<Bien> resultadosQuery = entitiesDomain.BienesRepositorio.ObtenerTodos().AsQueryable();

            if (int.TryParse(codigoEsbay, out int codigoEsbayInt))// Filtrar por Código Esbay si se proporciona y es un número válido
            {
                resultadosQuery = resultadosQuery.Where(b => b.CodigoEsbay == codigoEsbayInt);
                var tabla = resultadosQuery.ToList();// Obtener los resultados
                return View("ConstatacionIndex", tabla);
            }
           
            else
            {
                TempData["Mensaje"] = "Error: El bien no se encuentra.";
                return RedirectToAction("ConstatacionIndex");
            }
        }




        [HttpPost]
        public IActionResult Constatar(string codigoEsbay)
        {
            if (!int.TryParse(codigoEsbay, out int codigoEsbayInt))
            {
                TempData["Mensaje"] = "Error: El códigoEsbay proporcionado no es válido.";
                return RedirectToAction("ConstatacionIndex");
            }

            var bien = entitiesDomain.BienesRepositorio.BuscarPor(x => x.CodigoEsbay == codigoEsbayInt).FirstOrDefault();

            if (bien != null)
            {
                if (bien.Constatado)
                {
                    TempData["Mensaje"] = "Advertencia: El bien ya ha sido constatado anteriormente.";
                    return RedirectToAction("ConstatacionIndex");
                }

                ConstatarBien(bien);

                TempData["Mensaje"] = "Éxito: El bien ha sido constatado correctamente.";
                return RedirectToAction("ConstatacionIndex");
            }

            TempData["Mensaje"] = "Error: El bien no se encuentra.";
            return RedirectToAction("ConstatacionIndex");
        }

        private void ConstatarBien(Bien bien)
        {
            bien.Constatado = true;
            bien.FechaConstatacion = DateTime.Now;

            entitiesDomain.BienesRepositorio.Actualizar(bien);


            entitiesDomain.GuardarTransacciones();

            // Registrar actividad de constatación en BienesConstatado
            var bienConstatado = new BienesConstatado
                {
                    CodigoEsbay = bien.CodigoEsbay,
                    CodigoOlimpo = bien.CodigoOlimpo,
                    Descripcion = bien.Descripcion,
                    Serie = bien.Serie,
                    Modelo = bien.Modelo,
                    Marca = bien.Marca,
                    Custodio = bien.Custodio,
                    FechaIngreso = bien.FechaIngreso,
                    IdCampus = (int)bien.IdCampus,
                    FechaConstatacion = bien.FechaConstatacion,
                   
                };

                entitiesDomainnn.BienesConstatadoRepositorio.Insertar(bienConstatado);
                entitiesDomainnn.GuardarTransacciones();
            
        }





    }
}
