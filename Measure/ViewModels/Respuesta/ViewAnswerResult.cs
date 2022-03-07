using Measure.ViewModels.Grupo;
using System;
using System.Collections.Generic;

namespace Measure.ViewModels.Respuesta
{
    public class ViewAnswerResult
    {
        public Models.Usuario Cliente { get; set; }

        public ViewPoll DataEncuesta { get; set; }

        public ViewPresentation Wellcome { get; set; }

        public List<ViewAnswerGroupPoll> Grupos { get; set; }

        public List<Tuple<int, string>> ConsolidadoMatriz { get; set; }

        public bool Report { get; set; }

        public bool Email { get; set; }
    }
}