using Persistencia.Models;

namespace Constatacion.ViewModels
{
    public class ConstatacionVM
    {
        public int Id { get; set; }

        public int CodigoEsbay { get; set; }

        public string CodigoOlimpo { get; set; }

        public string Descripcion { get; set; }

        public string Serie { get; set; }

        public int? IdUsuario { get; set; }

        public bool? Constatado { get; set; }

        public string Modelo { get; set; }

        public string Marca { get; set; }

        public string Custodio { get; set; }

    

        public int IdCampus { get; set; }

        public DateTime? FechaConstatacion { get; set; }

        public List<Bien> Bienes { get; set; }

        // Constructor para inicializar la lista de Tipos
        public ConstatacionVM()
        {
            Bienes = new List<Bien>();
        }

    }
}