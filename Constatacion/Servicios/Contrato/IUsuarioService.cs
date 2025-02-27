using Microsoft.EntityFrameworkCore;
using Persistencia.Models;

namespace Constatacion.Servicios.Contrato
{
    public interface IUsuarioService
    {
        Task<Usuario> GetUsuario(string correo, string clave);
        Task<Usuario> SaveUsuario(Usuario modelo);
        Task<Usuario> ObtenerUsuarioPorId(int userId);
        Task<Usuario> ActualizarUsuario(Usuario usuario);

    }
}
