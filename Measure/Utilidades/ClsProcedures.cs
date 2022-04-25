using Measure.Models;
using Measure.ViewModels.Grupo;
using Measure.ViewModels.Respuesta;
using Measure.ViewModels.Usuario;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace Measure.Utilidades
{
    public class ClsProcedures : IDisposable
    {
        private bool disposing = false;

        public List<ViewPoll> MisAsignaciones(Guid UsuarioId, bool Todas)
        {
            List<ViewPoll> Result = new List<ViewPoll>();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                List<ViewPoll> Lista = (from A in db.UsuariosPorEncuenta
                                        join B in db.Encuesta on A.EncuestaId equals B.id
                                        join C in db.Usuario on A.UsuarioId equals C.Id
                                        join D in db.Usuario on C.ClienteId equals D.Id
                                        where A.UsuarioId == UsuarioId
                                        select new ViewPoll
                                        {
                                            id = B.id,
                                            ClienteId = D.ClienteId,
                                            NombreCliente = D.Nombres,
                                            Nombre = B.Nombre,
                                            FechaCreacion = B.FechaCreacion,
                                            FechaRespuesta = A.FechaResuelta,
                                            Finalizada = (A.FechaResuelta == null),
                                            IdAsignacion = A.Id,
                                            Estado = B.Estado,
                                        }).ToList();
                if (Todas)
                {
                    Result = Lista;
                }
                else
                {
                    if (Lista.Count() > 0)
                    {
                        Result.Add(Lista.FirstOrDefault());
                    }                    
                }
            }
            
            return Result;
        }

        public List<ViewResultList> ListaRespuestas(int Rol, Guid? ClienteId)
        {
            List<ViewResultList> Result = new List<ViewResultList>();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                using (DbConnection con = db.Database.Connection)
                {
                    con.Open();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(con);

                    using (DbCommand cmd = dbFactory.CreateCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "[DBO].[LISTARESPUESTAS]";
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter par = new SqlParameter
                        {
                            ParameterName = "@Rol",
                            SqlDbType = SqlDbType.Int,
                            Direction = ParameterDirection.Input,
                            SqlValue = Rol
                        };
                        cmd.Parameters.Add(par);

                        par = null;
                        par = new SqlParameter
                        {
                            ParameterName = "@ClienteId",
                            SqlDbType = SqlDbType.UniqueIdentifier,
                            Direction = ParameterDirection.Input
                        };
                        if (ClienteId != null)
                        {
                            par.SqlValue = ClienteId;
                        }
                        else
                        {
                            par.SqlValue = DBNull.Value;
                        }
                        cmd.Parameters.Add(par);

                        using (DbDataReader DataR = cmd.ExecuteReader())
                        {
                            while (DataR.Read())
                            {
                                ViewResultList data = new ViewResultList
                                {
                                    Correo = DataR.GetString(2),
                                    FechaRespuesta = DataR.GetDateTime(4),
                                    NombreCliente = DataR.GetString(1),
                                    Usuario = DataR.GetString(3),
                                };
                                Result.Add(data);
                            }
                        }
                    }
                    con.Close();
                }
            }

            return Result;
        }

        public ViewPoll DetalleEncuestaUsuario(Guid EncuestaId, Guid UsuarioId)
        {
            List<ViewPoll> Result = new List<ViewPoll>();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                using (DbConnection con = db.Database.Connection)
                {
                    con.Open();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(con);

                    using (DbCommand cmd = dbFactory.CreateCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "[DBO].[ENCUESTADETALLEUSUARIO]";
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter par = new SqlParameter
                        {
                            ParameterName = "@IdAsignacion",
                            SqlDbType = SqlDbType.UniqueIdentifier,
                            SqlValue = EncuestaId,
                            Direction = ParameterDirection.Input
                        };
                        cmd.Parameters.Add(par);

                        par = null;
                        par = new SqlParameter
                        {
                            ParameterName = "@UsuarioId",
                            SqlDbType = SqlDbType.UniqueIdentifier,
                            SqlValue = UsuarioId,
                            Direction = ParameterDirection.Input
                        };
                        cmd.Parameters.Add(par);

                        //DataTable dt = new DataTable(); 
                        //using (DbDataReader DataR = cmd.ExecuteReader())
                        //{
                        //    dt.Load(DataR);
                        //}

                        using (DbDataReader DataR = cmd.ExecuteReader())
                        {
                            while (DataR.Read())
                            {
                                ViewPoll data = new ViewPoll
                                {
                                    id = DataR.GetGuid(0),
                                    ClienteId = DataR.GetGuid(1),
                                    NombreCliente = DataR.GetString(2),
                                    Nombre = DataR.GetString(3),
                                    FechaCreacion = DataR.GetDateTime(4),
                                    Estado = DataR.GetBoolean(6),
                                    IdAsignacion = DataR.GetGuid(8),
                                    TipoReporteGeneral = DataR.GetGuid(9),
                                };

                                if (!DataR.IsDBNull(5))
                                {
                                    data.FechaRespuesta = DataR.GetDateTime(5);
                                    data.Finalizada = DataR.GetBoolean(7);
                                }

                                Result.Add(data);
                            }
                        }
                    }
                    con.Close();
                }
            }

            return Result.FirstOrDefault();
        }

        public List<ViewUser> BuscarEncustados(ViewFindUser Data)
        {
            List<ViewUser> Result = new List<ViewUser>();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                using (DbConnection con = db.Database.Connection)
                {
                    con.Open();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(con);

                    using (DbCommand cmd = dbFactory.CreateCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "[dbo].[BuscarEncustados]";
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter par = new SqlParameter
                        {
                            ParameterName = "@ClienteId",
                            SqlDbType = SqlDbType.UniqueIdentifier,
                            SqlValue = Data.ClienteId,
                            Direction = ParameterDirection.Input
                        };
                        cmd.Parameters.Add(par);

                        par = null;
                        par = new SqlParameter
                        {
                            ParameterName = "@PaisId",
                            SqlDbType = SqlDbType.Int,
                            SqlValue = DBNull.Value,
                            Direction = ParameterDirection.Input
                        };
                        if (Data.PaisId != 0)
                        {
                            par.SqlValue = Data.PaisId;
                        }
                        cmd.Parameters.Add(par);

                        par = null;
                        par = new SqlParameter
                        {
                            ParameterName = "@Idioma",
                            SqlDbType = SqlDbType.Int,
                            SqlValue = DBNull.Value,
                            Direction = ParameterDirection.Input
                        };
                        if (Data.Idioma != 0)
                        {
                            par.SqlValue = Data.Idioma;
                        }
                        cmd.Parameters.Add(par);

                        par = null;
                        par = new SqlParameter
                        {
                            ParameterName = "@Correo",
                            SqlDbType = SqlDbType.NVarChar,
                            SqlValue = DBNull.Value,
                            Direction = ParameterDirection.Input
                        };
                        if (!string.IsNullOrEmpty(Data.Correo))
                        {
                            par.SqlValue = Data.Correo;
                        }
                        cmd.Parameters.Add(par);

                        par = null;
                        par = new SqlParameter
                        {
                            ParameterName = "@Nombre",
                            SqlDbType = SqlDbType.NVarChar,
                            SqlValue = DBNull.Value,
                            Direction = ParameterDirection.Input
                        };
                        if (!string.IsNullOrEmpty(Data.Nombre))
                        {
                            par.SqlValue = Data.Nombre;
                        }
                        cmd.Parameters.Add(par);

                        using (DbDataReader DataR = cmd.ExecuteReader())
                        {
                            while (DataR.Read())
                            {
                                ViewUser data = new ViewUser
                                {
                                    Id = DataR.GetGuid(0),
                                    Correo = DataR.GetString(1),
                                    Clave = DataR.GetString(2),
                                    RolId = DataR.GetInt32(3),
                                    Cliente = DataR.GetValue(5).ToString(),
                                    Color = DataR.GetValue(8).ToString(),
                                    Idioma = DataR.GetInt32(9),
                                    NombreIdioma = DataR.GetString(10),
                                    Nombres = DataR.GetString(11),
                                    Apellidos = DataR.GetValue(12).ToString(),
                                    Telefono = DataR.GetValue(13).ToString(),
                                    Titulo = DataR.GetValue(14).ToString(),
                                    CorreoTrabajo = DataR.GetValue(15).ToString(),
                                    Empresa = DataR.GetValue(16).ToString(),
                                    Subsidiario = DataR.GetValue(19).ToString(),
                                    Pais = DataR.GetValue(21).ToString(),
                                    DireccionTrabajo = DataR.GetValue(22).ToString(),
                                    Estado = DataR.GetBoolean(23),
                                };
                                if (DataR.GetValue(4).GetType().ToString() != "System.DBNull")
                                {
                                    data.ClienteId = DataR.GetGuid(4);
                                }
                                if (DataR.GetValue(6).GetType().ToString() != "System.DBNull")
                                {
                                    data.AliadoId = DataR.GetGuid(6);
                                }
                                if (DataR.GetValue(7).GetType().ToString() != "System.DBNull")
                                {
                                    data.ByteIMage = (byte[])DataR.GetValue(7);
                                }
                                if (DataR.GetValue(17).GetType().ToString() != "System.DBNull")
                                {
                                    data.CEmpleadosId = DataR.GetInt32(17);
                                }
                                if (DataR.GetValue(18).GetType().ToString() != "System.DBNull")
                                {
                                    data.IndustriaId = DataR.GetInt32(18);
                                }
                                if (DataR.GetValue(20).GetType().ToString() != "System.DBNull")
                                {
                                    data.PaisId = DataR.GetInt32(20);
                                }
                                Result.Add(data);
                            }
                        }
                    }
                    con.Close();
                }
            }

            return Result;
        }

        public List<ViewUser> UsuariosPorRol(int? Rol, Guid? ClienteId, Guid? AliadoId, int? Idioma, Guid? EncuestaId)
        {
            List<ViewUser> Result = new List<ViewUser>();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                using (DbConnection con = db.Database.Connection)
                {
                    con.Open();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(con);

                    using (DbCommand cmd = dbFactory.CreateCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "[dbo].[USUARIOSPORROL]";
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter par = new SqlParameter
                        {
                            ParameterName = "@Rol",
                            SqlDbType = SqlDbType.Int,
                            SqlValue = DBNull.Value,
                            Direction = ParameterDirection.Input
                        };
                        if (Rol != null)
                        {
                            par.SqlValue = Rol;
                        }
                        cmd.Parameters.Add(par);

                        par = null;
                        par = new SqlParameter
                        {
                            ParameterName = "@EncuestaId",
                            SqlDbType = SqlDbType.UniqueIdentifier,
                            SqlValue = DBNull.Value,
                            Direction = ParameterDirection.Input
                        };
                        if (EncuestaId != null)
                        {
                            par.SqlValue = EncuestaId;
                        }
                        cmd.Parameters.Add(par);

                        par = null;
                        par = new SqlParameter
                        {
                            ParameterName = "@ClienteId",
                            SqlDbType = SqlDbType.UniqueIdentifier,
                            SqlValue = DBNull.Value,
                            Direction = ParameterDirection.Input
                        };
                        if (ClienteId != null)
                        {
                            par.SqlValue = ClienteId;
                        }
                        cmd.Parameters.Add(par);

                        par = null;
                        par = new SqlParameter
                        {
                            ParameterName = "@AliadoId",
                            SqlDbType = SqlDbType.UniqueIdentifier,
                            SqlValue = DBNull.Value,
                            Direction = ParameterDirection.Input
                        };
                        if (AliadoId != null)
                        {
                            par.SqlValue = AliadoId;
                        }
                        cmd.Parameters.Add(par);

                        par = null;
                        par = new SqlParameter
                        {
                            ParameterName = "@Idioma",
                            SqlDbType = SqlDbType.Int,
                            SqlValue = DBNull.Value,
                            Direction = ParameterDirection.Input
                        };
                        if (Idioma != null)
                        {
                            par.SqlValue = Idioma;
                        }
                        cmd.Parameters.Add(par);

                        using (DbDataReader DataR = cmd.ExecuteReader())
                        {
                            while (DataR.Read())
                            {
                                ViewUser data = new ViewUser
                                {
                                    Id = DataR.GetGuid(0),
                                    Correo = DataR.GetString(1),
                                    Clave = DataR.GetString(2),
                                    RolId = DataR.GetInt32(3),
                                    Cliente = DataR.GetValue(5).ToString(),
                                    Color = DataR.GetValue(8).ToString(),
                                    Idioma = DataR.GetInt32(9),
                                    NombreIdioma = DataR.GetString(10),
                                    Nombres = DataR.GetString(11),
                                    Apellidos = DataR.GetValue(12).ToString(),
                                    Telefono = DataR.GetValue(13).ToString(),
                                    Titulo = DataR.GetValue(14).ToString(),
                                    CorreoTrabajo = DataR.GetValue(15).ToString(),
                                    Empresa = DataR.GetValue(16).ToString(),
                                    Subsidiario = DataR.GetValue(19).ToString(),
                                    Pais = DataR.GetValue(21).ToString(),
                                    DireccionTrabajo = DataR.GetValue(22).ToString(),
                                    Estado = DataR.GetBoolean(23),
                                };
                                if (DataR.GetValue(4).GetType().ToString() != "System.DBNull")
                                {
                                    data.ClienteId = DataR.GetGuid(4);
                                }
                                if (DataR.GetValue(6).GetType().ToString() != "System.DBNull")
                                {
                                    AliadoId = DataR.GetGuid(6);
                                }
                                if (DataR.GetValue(7).GetType().ToString() != "System.DBNull")
                                {
                                    data.ByteIMage = (byte[])DataR.GetValue(7);
                                }
                                if (DataR.GetValue(17).GetType().ToString() != "System.DBNull")
                                {
                                    data.CEmpleadosId = DataR.GetInt32(17);
                                }
                                if (DataR.GetValue(18).GetType().ToString() != "System.DBNull")
                                {
                                    data.IndustriaId = DataR.GetInt32(18);
                                }
                                if (DataR.GetValue(20).GetType().ToString() != "System.DBNull")
                                {
                                    data.PaisId = DataR.GetInt32(20);
                                }
                                if (EncuestaId != null)
                                {
                                    data.IdAsignacion = DataR.GetGuid(24);
                                }
                                Result.Add(data);
                            }
                        }
                    }
                    con.Close();
                }
            }

            return Result;
        }

        public void Dispose()
        {
            if (disposing)
            {
                return;
            }
            disposing = true;
        }
    }
}