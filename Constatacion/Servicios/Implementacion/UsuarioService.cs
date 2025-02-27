
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Constatacion.Servicios.Contrato;
using Persistencia.Models;

namespace Constatacion.Servicios.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ConstatacionContext _dbContext;
        public UsuarioService(ConstatacionContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Usuario> GetUsuario(string correo, string clave)
        {
            Usuario usuario_encontrado = await _dbContext.Usuarios.Where(u => u.Correo == correo && u.Clave == clave)
                 .FirstOrDefaultAsync();

            return usuario_encontrado;
        }

        public async Task<Usuario> SaveUsuario(Usuario modelo)
        {
            _dbContext.Usuarios.Add(modelo);
            await _dbContext.SaveChangesAsync();
            return modelo;
        }
        public async Task<Usuario> ObtenerUsuarioPorId(int userId)
        {
            // Lógica para obtener un usuario por su ID desde la base de datos
            return await _dbContext.Usuarios.FindAsync(userId);
        }
        public async Task<Usuario> ActualizarUsuario(Usuario usuario)
        {
            _dbContext.Entry(usuario).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return usuario;
        }
    }
}





