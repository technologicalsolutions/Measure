using Measure.Enums;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Grupo
{
    public class ViewGrupoEncuestas
    {
        public List<ViewGroupPoll> Lista { get; set; }
        public List<SelectListItem> Clientes { get; set; }        
        public Guid? ClienteId { get; set; }
        public ViewGroupPoll Modelo { get; set; }
        public DbAcciones Accion { get; set; }
    }
}