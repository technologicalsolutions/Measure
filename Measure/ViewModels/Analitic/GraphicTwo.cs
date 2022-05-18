using System.Collections.Generic;

namespace Measure.ViewModels.Analitic
{
    public class GraphicTwo
    {
        public List<string> Grupos { get; set; }
        public DataGeneral GenPais { get; set; }
        public DataGeneral GenSucursal { get; set; }
        public DataGeneral GenGerencia { get; set; }
        public DataGeneral GenRol { get; set; }
        public DataGeneral GenGrupo { get; set; }
        public List<GraphicTwoBase> GerenciaGrupo { get; set; }

    }
}