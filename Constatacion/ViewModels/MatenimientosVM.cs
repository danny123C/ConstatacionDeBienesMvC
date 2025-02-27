using Persistencia.Models;

namespace Constatacion.ViewModels
{
    public class MatenimientosVM
    {

        public int CodigoEsbay { get; set; }
        public string Descripcion { get; set; }

        public string Serie { get; set; }

        public string Modelo { get; set; }

        public string Marca { get; set; }

        public string Custodio { get; set; }
        public string MotivoMantenimiento { get; set; }

        public string AccionesRealizadas { get; set; }

        public string NombreTecnico { get; set; }

        public string Resultado { get; set; }

        public string Nombre { get; set; }


        public List<Bien> Bienes { get; set; }

        // Constructor para inicializar la lista de Tipos
        public MatenimientosVM()
        {
            Bienes = new List<Bien>();
        }
    }
}
