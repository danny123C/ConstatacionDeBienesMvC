
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Persistencia.Models;
using Biblioteca.Dominio;

namespace Dominio
{
    public class EntitiesDomainM : IDisposable
    {
        ConstatacionContext contexto;

        public EntitiesDomainM(DbContextOptions<ConstatacionContext> options)
        {
            contexto = new ConstatacionContext(options);
        }

        public void GuardarTransacciones()
        {
            contexto.SaveChanges();
        }

        #region Dispose
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

       

      
 

        private Repositorio<Mantenimiento> mantenimientoRepositorio;

        public Repositorio<Mantenimiento> MantenimientoRepositorio
        {
            get
            {
                if (mantenimientoRepositorio == null)
                {
                    mantenimientoRepositorio = new Repositorio<Mantenimiento>(contexto);
                }
                return mantenimientoRepositorio;
            }
        }
      

    }
}
