using Constatacion.Models;
using Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistencia.Models;
using Constatacion.ViewModels;

using Rotativa.AspNetCore;
using Constatacion.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Reporte.Controllers
{
  //  [Authorize]
    public class ReporteController : Controller
    {
        EntitiesDomainnn entitiesDomain;
        EntitiesDomainM entitiesDomainM;
        ILogger logger;
        //readonly IMapper _mapper;

        ErrorViewModel errorVM;
        public ReporteController(DbContextOptions<ConstatacionContext> options, ILoggerFactory log)
        {
            // logger
            logger = log.CreateLogger(typeof(HomeController));
            errorVM = new ErrorViewModel(log);

            //_mapper = mapper;
            entitiesDomain = new EntitiesDomainnn(options);
            entitiesDomainM = new EntitiesDomainM(options);
        }


        public IActionResult ImprimirVenta()
        {
            List<ConstatacionVM> listaDeBienes = entitiesDomain.BienesConstatadoRepositorio
                .ObtenerTodosEnOtraVista<ConstatacionVM>(
                    m => new ConstatacionVM
                    {
                      
                        CodigoEsbay = m.CodigoEsbay,
                        CodigoOlimpo = m.CodigoOlimpo,
                        Descripcion = m.Descripcion,
                        Serie = m.Serie,
                      
                        Modelo = m.Modelo,
                        Marca = m.Marca,
                        Custodio = m.Custodio,
                        Constatado = m.Constatado,
                        FechaConstatacion = m.FechaConstatacion
                    })
                .ToList();

            return new ViewAsPdf("ImprimirVenta", listaDeBienes)
            {
                FileName = "ReporteDeBienes.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }
        public IActionResult ImprimirMantenimiento()
        {
          

            List<MatenimientosVM> listaDeManteimiento = entitiesDomainM.MantenimientoRepositorio
                .ObtenerTodosEnOtraVista<MatenimientosVM>(
                    m => new MatenimientosVM
                    {

                        CodigoEsbay = m.CodigoEsbayNavigation.CodigoEsbay,
                        MotivoMantenimiento = m.MotivoMantenimiento,
                        AccionesRealizadas = m.AccionesRealizadas,
                        NombreTecnico = m.NombreTecnico,

                        Resultado = m.Resultado,

                        Marca = m.CodigoEsbayNavigation.Marca,
                        Descripcion = m.CodigoEsbayNavigation.Descripcion,
                        Serie = m.CodigoEsbayNavigation.Serie,
                        Modelo = m.CodigoEsbayNavigation.Modelo,
                
                        Custodio = m.CodigoEsbayNavigation.Custodio,
                        Nombre = m.IdTipoEstadoBienNavigation.Nombre,
                    })
                .ToList();

            return new ViewAsPdf("ImprimirMantenimiento", listaDeManteimiento)
            {
                FileName = "MantenimientoDeBienes.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }

    }

}
