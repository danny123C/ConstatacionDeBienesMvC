using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore; 
using Persistencia.Models;

namespace Dominio
{
    public class Repositorio<T> where T : class
    {
        ConstatacionContext contexto = null;

        private DbSet<T> entidades;

        public Repositorio(ConstatacionContext _context)
        {
            contexto = _context;
            entidades = contexto.Set<T>();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Buscar(int id)
        {
            return entidades.Find(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> BuscarAsync(int id)
        {
            return await entidades.FindAsync(id);
        }

        public void Actualizar(List<T> entidadesActualizar)
        {
            contexto.UpdateRange(entidadesActualizar);
        }



        /// <summary>
        /// Contar en base a filtro
        /// </summary>
        /// <param name="filtro">Filtro como expresión lambda (x=>x.Propiedad == 1)</param>
        /// <returns>Cantidad de elementos que cumplen el filtro</returns>
        public int Contar(Expression<Func<T, bool>> filtro = null)
        {
            IQueryable<T> query = entidades.AsNoTracking();
            if (filtro != null)
                query = query.Where(filtro);
            return query.Count();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtro">Expresión lambda que resentada Ej. x=>x.Id == 2</param>
        /// <param name="ordenarPor">y=>(y.OrderBy(z=>z.Propiedad))</param>
        /// <param name="entidadesRelacionadasAIncluir">x=>(x as Entidad).OtraEntidadRelacionada (Padre, Hijo)</param>
        /// <returns></returns>
        public List<T> BuscarPor(
    Expression<Func<T, bool>> filtro = null,
    Func<IQueryable<T>, IOrderedQueryable<T>> ordenarPor = null,
    params Expression<Func<T, object>>[] entidadesRelacionadasAIncluir)
        {
            IQueryable<T> query = entidades.AsNoTracking();

            if (filtro != null)
                query = query.Where(filtro);

            if (entidadesRelacionadasAIncluir != null)
                query = entidadesRelacionadasAIncluir.Aggregate(query, (current, include) => current.Include(include));

            // Paginación
            int pageSize = 100; // Ajusta según tus necesidades
            if (ordenarPor != null)
                query = ordenarPor(query);

            List<T> resultado = query.Take(pageSize).ToList();

            return resultado;
        }


        public List<T> ObtenerTop(Expression<Func<T, bool>> filtro = null,
                           Func<IQueryable<T>, IOrderedQueryable<T>> ordenarPor = null,
            int top=0)
        {
            List<T> resultado = null;
            IQueryable<T> query = entidades.AsNoTracking();

         
            if (filtro != null)
                query = query.Where(filtro);

            if (ordenarPor != null)
                resultado = ordenarPor(query).Take(top).ToList();
            else
                resultado = query.Take(top).ToList();


            return resultado;
        }


  
        public List<T> ObtenerTodos()
        {
            return entidades.AsNoTracking().AsEnumerable().ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="vista"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public List<T> ObtenerTodosEnVistaParcial(Expression<Func<T, T>> vista,
            Expression<Func<T, bool>> filtro = null)
        {
            IQueryable<T> query = entidades.AsNoTracking();
            if (filtro != null)
                query = query.Where(filtro);
            return query.Select(vista).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="vista"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public List<T2> ObtenerTodosEnOtraVista<T2>(Expression<Func<T, T2>> vista,
            Expression<Func<T, bool>> filtro = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> ordenarPor = null, int top = 0) where T2 : class
        {
            IQueryable<T> query = entidades.AsNoTracking();
            if (filtro != null)
                query = query.Where(filtro);

            //
            if (ordenarPor != null)
                query = ordenarPor(query);

            if (top > 0)
                return query.Select(vista).Take(top).ToList<T2>();
            else
                return query.Select(vista).ToList<T2>();
        }

        public void Insertar(T entidad)
        {
            entidades.Add(entidad);
        }

        public void Actualizar(T entidadActualizar)
        {
            entidades.Attach(entidadActualizar);
            contexto.Entry(entidadActualizar).State = EntityState.Modified;
        }

        public void Eliminar(T entidadAeliminar)
        {
            if (contexto.Entry(entidadAeliminar).State == EntityState.Detached)
            {
                entidades.Attach(entidadAeliminar);
            }
            entidades.Remove(entidadAeliminar);
        }

        public void Eliminar(int id)
        {
            var entidadAeliminar = Buscar(id);
            if (contexto.Entry(entidadAeliminar).State == EntityState.Detached)
            {
                entidades.Attach(entidadAeliminar);
            }
            entidades.Remove(entidadAeliminar);
        }

        public void Insertar(List<Usuario>? usuario)
        {
            throw new NotImplementedException();
        }

        // ... Otros métodos y propiedades ...

       
            public List<TResult> ObtenerDatosPersonalizados<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> filtro = null)
            {
                IQueryable<T> query = entidades.AsNoTracking();

                if (filtro != null)
                    query = query.Where(filtro);

                return query.Select(selector).ToList();
            }
        




    }
}
