using Measure.Enums;
using Measure.Models;
using System.Collections.Generic;

namespace Measure.ViewModels.Maestra
{
    public class ViewDetailsMaster
    {        
        public DbAcciones Acciones { get; set; }
        public Maestras Maestra { get; set; }
        public List<MaestrasDetalle> Lista { get; set; }
        public MaestrasDetalle Modelo { get; set; }
    }
}