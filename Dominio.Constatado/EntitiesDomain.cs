using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Persistencia.Models;
using Biblioteca.Dominio;

namespace Dominio
{
    public class EntitiesDomainnn : IDisposable
    {
        ConstatacionContext contexto;

        public EntitiesDomainnn(DbContextOptions<ConstatacionContext> options)
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

       

        public IList<T> ExecuteStoredProcedure<T>(string name, params (string, object)[] nameValueParams) where T : class
        {
            DbCommand command = contexto.LoadStoredProcedure(name).WithSqlParams(nameValueParams);

            if (command.Connection.State == System.Data.ConnectionState.Closed)
                command.Connection.Open();
            try
            {
                using (var reader = command.ExecuteReader())
                {
                    return reader.MapToList<T>();
                }
            }
            finally
            {
                command.Connection.Close();
            }
        }

        public List<T> ExecuteStoredProcedureSingleFieldResult<T>(string name, params (string, object)[] nameValueParams)// where T : class
        {
            List<T> result = new List<T>();
            DbCommand command = contexto.LoadStoredProcedure(name).WithSqlParams(nameValueParams);

            if (command.Connection.State == System.Data.ConnectionState.Closed)
                command.Connection.Open();
            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetFieldValue<T>(0));
                    }
                    //return reader.MapToList<T>();
                }
            }
            finally
            {
                command.Connection.Close();
            }
            return result;
        }

        private Repositorio<BienesConstatado> bienesConstatadoRepositorio;

        public Repositorio<BienesConstatado> BienesConstatadoRepositorio
        {
            get
            {
                if (bienesConstatadoRepositorio == null)
                {
                    bienesConstatadoRepositorio = new Repositorio<BienesConstatado>(contexto);
                }
                return bienesConstatadoRepositorio;
            }
        }
      

    }
}
