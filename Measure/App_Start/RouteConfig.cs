using System.Web.Mvc;
using System.Web.Routing;

namespace Measure
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            #region Base

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Login", action = "Index" }
            );

            routes.MapRoute(
                name: "Home",
                url: "Home",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "UserHome",
                url: "UserHome",
                defaults: new { controller = "Home", action = "UserIndex" }
            );

            routes.MapRoute(
                name: "Error",
                url: "Error",
                defaults: new { controller = "Home", action = "Error" }
            );

            #endregion

            #region Usuarios
            routes.MapRoute(
                name: "Usuarios",
                url: "Usuarios",
                defaults: new { controller = "Usuarios", action = "Index" }
            );

            routes.MapRoute(
                name: "Usuario",
                url: "Usuario",
                defaults: new { controller = "Usuarios", action = "Usuario" }
            );

            routes.MapRoute(
                name: "UserAcciones",
                url: "UserAcciones",
                defaults: new { controller = "Usuarios", action = "Acciones" }
            );

            routes.MapRoute(
                name: "BuscarUsuarios",
                url: "BuscarUsuarios",
                defaults: new { controller = "Usuarios", action = "BuscarUsuarios" }
            );

            routes.MapRoute(
                name: "Busqueda",
                url: "Busqueda",
                defaults: new { controller = "Usuarios", action = "Busqueda" }
            );

            routes.MapRoute(
                name: "AsignarUsuarios",
                url: "AsignarUsuarios",
                defaults: new { controller = "Usuarios", action = "AsignarUsuarios" }
            );

            routes.MapRoute(
                name: "DesAsignarUsuarios",
                url: "DesAsignarUsuarios",
                defaults: new { controller = "Usuarios", action = "DesAsignarUsuarios" }
            );

            routes.MapRoute(
               name: "ListaUsuarios",
               url: "ListaUsuarios",
               defaults: new { controller = "Usuarios", action = "ListaUsuarios" }
            );           

            routes.MapRoute(
               name: "ActualizarUsuario",
               url: "ActualizarUsuario",
               defaults: new { controller = "Usuarios", action = "UpdateUser" }
            );
            #endregion

            #region Encuestas
            routes.MapRoute(
               name: "Encuestas",
               url: "Encuestas",
               defaults: new { controller = "Encuestas", action = "Index" }
            );

            routes.MapRoute(
                name: "EncuestaAcciones",
                url: "EncuestaAcciones",
                defaults: new { controller = "Encuestas", action = "Acciones" }
            );

            routes.MapRoute(
                name: "GuardarEncuesta",
                url: "GuardarEncuesta",
                defaults: new { controller = "Encuestas", action = "GuardarEncuesta" }
            );

            #endregion

            #region ContenidoEncuesta

            routes.MapRoute(
                name: "ContenidoEncuesta",
                url: "ContenidoEncuesta",
                defaults: new { controller = "ContenidoEncuesta", action = "Index" }
            );

            #endregion 

            #region PreguntasPorGrupo 

            routes.MapRoute(
                name: "PreguntasPorGrupo",
                url: "PreguntasPorGrupo",
                defaults: new { controller = "PreguntasPorGrupo", action = "Index" }
            );

            #endregion

            #region Contenido

            routes.MapRoute(
                name: "Contenido",
                url: "Contenido",
                defaults: new { controller = "Contenido", action = "Index" }
            );

            #endregion

            #region Grupos
            routes.MapRoute(
               name: "Categorias",
               url: "Categorias",
               defaults: new { controller = "Grupos", action = "Index" }
            );

            routes.MapRoute(
               name: "Categoria",
               url: "Categoria",
               defaults: new { controller = "Grupos", action = "Grupo" }
            );

            routes.MapRoute(
               name: "CategoriasAcciones",
               url: "CategoriasAcciones",
               defaults: new { controller = "Grupos", action = "Acciones" }
            );

            routes.MapRoute(
               name: "PreguntasCategoria",
               url: "PreguntasCategoria",
               defaults: new { controller = "Grupos", action = "PreguntasCategoria" }
            );
            #endregion

            #region Reporte

            routes.MapRoute(
               name: "Reportes",
               url: "Reportes",
               defaults: new { controller = "Resporte", action = "Index" }
            );

            routes.MapRoute(
               name: "ReporteAcciones",
               url: "ReporteAcciones",
               defaults: new { controller = "Resporte", action = "Acciones" }
            );

            routes.MapRoute(
               name: "EliminarReporte",
               url: "EliminarReporte",
               defaults: new { controller = "Resporte", action = "Delete" }
            );

            routes.MapRoute(
               name: "ContenidosReporte",
               url: "ContenidosReporte",
               defaults: new { controller = "Resporte", action = "Contenido" }
            );

            routes.MapRoute(
               name: "ContenidoReporte",
               url: "ContenidoReporte",
               defaults: new { controller = "Resporte", action = "CreateOrEdit" }
            );

            routes.MapRoute(
               name: "AccionesContenido",
               url: "AccionesContenido",
               defaults: new { controller = "Resporte", action = "ActionContent" }
            );

            routes.MapRoute(
               name: "EliminarContenido",
               url: "EliminarContenido",
               defaults: new { controller = "Resporte", action = "DeleteContent" }
            );

            #endregion

            #region Dashboard

            routes.MapRoute(
              name: "DashboardGeneral",
              url: "DashboardGeneral/{Id}",
              defaults: new { controller = "Dashboard", action = "Index", Id = UrlParameter.Optional }
            );

            routes.MapRoute(
              name: "DashboardGeneralDescripcion",
              url: "DashboardGeneralDescripcion/{Id}",
              defaults: new { controller = "Dashboard", action = "Descripcion", Id = UrlParameter.Optional }
            );

            routes.MapRoute(
              name: "BasicGraph",
              url: "BasicGraph",
              defaults: new { controller = "Dashboard", action = "BasicGraphic" }
            );

            routes.MapRoute(
              name: "DashboardAnalitica",
              url: "DashboardAnalitica/{Id}",
              defaults: new { controller = "Dashboard", action = "AnaliticaIndex", Id = UrlParameter.Optional }
            );

            #endregion

            #region Preguntas
            routes.MapRoute(
               name: "Preguntas",
               url: "Preguntas",
               defaults: new { controller = "Preguntas", action = "Index" }
            );

            routes.MapRoute(
               name: "Pregunta",
               url: "Pregunta",
               defaults: new { controller = "Preguntas", action = "Pregunta" }
            );

            #endregion

            #region Respuestas
            routes.MapRoute(
               name: "Respuestas",
               url: "Respuestas",
               defaults: new { controller = "Respuestas", action = "Index" }
            );

            routes.MapRoute(
               name: "FiltrarRespuestas",
               url: "FiltrarRespuestas",
               defaults: new { controller = "Respuestas", action = "FiltrarRespuestas" }
            );

            routes.MapRoute(
               name: "Respuesta",
               url: "Respuesta",
               defaults: new { controller = "Respuestas", action = "Respuesta" }
            );

            routes.MapRoute(
               name: "GeneratePdf",
               url: "GeneratePdf",
               defaults: new { controller = "Respuestas", action = "GeneratePdf" }
            );

            routes.MapRoute(
               name: "Email",
               url: "Email",
               defaults: new { controller = "Respuestas", action = "RespuestaEmail" }
            );

            routes.MapRoute(
               name: "SendEMail",
               url: "SendEMail",
               defaults: new { controller = "Respuestas", action = "SendEMail" }
            );

            routes.MapRoute(
               name: "PruebaEmail",
               url: "PruebaEmail",
               defaults: new { controller = "Respuestas", action = "PruebaEmail" }
            );
            #endregion

            #region Maestra

            routes.MapRoute(
                name: "Maestras",
                url: "Maestras",
                defaults: new { controller = "Maestra", action = "Index" }
            );

            routes.MapRoute(
                name: "Detalle",
                url: "Detalle/{Id}",
                defaults: new { controller = "Maestra", action = "Detalles", Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Idiomas",
                url: "Idiomas",
                defaults: new { controller = "Maestra", action = "Idiomas" }
            );            

            routes.MapRoute(
                name: "Paises",
                url: "Paises",
                defaults: new { controller = "Maestra", action = "Index" }
            );
            
            #endregion
        }
    }
}
