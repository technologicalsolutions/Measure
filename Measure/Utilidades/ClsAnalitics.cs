using Measure.Models;
using Measure.ViewModels.Analitic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Measure.Utilidades
{
    public class ClsAnalitics : IDisposable
    {
        private bool disposing = false;

        public List<ViewAnaliticResultQuestion> ResultQuestionForUser(Guid EncuestaId, string IdAsignaciones, Guid? UsuarioId, string Correo)
        {
            DataTable data = new DataTable();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(db.Database.Connection);
                using (DbConnection con = factory.CreateConnection())
                {
                    con.ConnectionString = db.Database.Connection.ConnectionString;
                    using (con)
                    {
                        con.Open();
                        using (DbCommand cmd = factory.CreateCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "[dbo].[AnaliticResultQuestion]";
                            cmd.CommandType = CommandType.StoredProcedure;

                            SqlParameter par = new SqlParameter
                            {
                                ParameterName = "@EncuestaId",
                                SqlDbType = SqlDbType.UniqueIdentifier,
                                SqlValue = EncuestaId,
                                Direction = ParameterDirection.Input
                            };
                            cmd.Parameters.Add(par);

                            par = null;
                            par = new SqlParameter
                            {
                                ParameterName = "@IdAsignaciones",
                                SqlDbType = SqlDbType.NVarChar,
                                SqlValue = IdAsignaciones,
                                Direction = ParameterDirection.Input
                            };
                            cmd.Parameters.Add(par);

                            par = null;
                            par = new SqlParameter
                            {
                                ParameterName = "@UsuarioId",
                                SqlDbType = SqlDbType.NVarChar,                                
                                SqlValue = DBNull.Value,
                                Direction = ParameterDirection.Input
                            };
                            if (UsuarioId != null)
                            {
                                par.SqlValue = UsuarioId;
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
                            if (!string.IsNullOrEmpty(Correo))
                            {
                                par.SqlValue = Correo;
                            }
                            cmd.Parameters.Add(par);

                            using (DbDataReader DataR = cmd.ExecuteReader())
                            {
                                data.Load(DataR);
                            }
                        }
                    }
                }
            }

            List<ViewAnaliticResultQuestion> Result = ConvertDetail<ViewAnaliticResultQuestion>(data);
            return Result;
        }

        public List<SquareOne> AnaliticSquareOne(Guid EncuestaId, string IdAsignaciones)
        {
            List<SquareOne> Result = new List<SquareOne>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(db.Database.Connection);
                using (DbConnection con = factory.CreateConnection())
                {
                    con.ConnectionString = db.Database.Connection.ConnectionString;
                    using (con)
                    {
                        con.Open();
                        using (DbCommand cmd = factory.CreateCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "[dbo].[AnaliticSquareOne]";
                            cmd.CommandType = CommandType.StoredProcedure;

                            SqlParameter par = new SqlParameter
                            {
                                ParameterName = "@EncuestaId",
                                SqlDbType = SqlDbType.UniqueIdentifier,
                                SqlValue = EncuestaId,
                                Direction = ParameterDirection.Input
                            };
                            cmd.Parameters.Add(par);

                            par = null;
                            par = new SqlParameter
                            {
                                ParameterName = "@IdAsignaciones",
                                SqlDbType = SqlDbType.NVarChar,
                                SqlValue = IdAsignaciones,
                                Direction = ParameterDirection.Input
                            };
                            cmd.Parameters.Add(par);

                            using (DbDataReader DataR = cmd.ExecuteReader())
                            {
                                while (DataR.Read())
                                {
                                    SquareOne data = new SquareOne
                                    {
                                        Grupo = DataR.GetString(0),
                                        Promedio = DataR.GetDecimal(1),
                                        Maximo = DataR.GetInt64(2),
                                        Minimo = DataR.GetInt64(3),
                                        Moda = DataR.GetInt64(4),

                                    };
                                    Result.Add(data);
                                }
                            }
                        }
                    }
                }
            }

            return Result;
        }

        public List<SquareDetail> AnaliticSquareTwo(Guid EncuestaId, string DataJson)
        {
            List<SquareDetail> Result = new List<SquareDetail>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(db.Database.Connection);
                using (DbConnection con = factory.CreateConnection())
                {
                    con.ConnectionString = db.Database.Connection.ConnectionString;
                    using (con)
                    {
                        con.Open();
                        using (DbCommand cmd = factory.CreateCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "[dbo].[AnaliticSquareTwo]";
                            cmd.CommandType = CommandType.StoredProcedure;

                            SqlParameter par = new SqlParameter
                            {
                                ParameterName = "@EncuestaId",
                                SqlDbType = SqlDbType.UniqueIdentifier,
                                SqlValue = EncuestaId,
                                Direction = ParameterDirection.Input
                            };
                            cmd.Parameters.Add(par);

                            par = null;
                            par = new SqlParameter
                            {
                                ParameterName = "@DataJson",
                                SqlDbType = SqlDbType.NVarChar,
                                SqlValue = DataJson,
                                Direction = ParameterDirection.Input
                            };
                            cmd.Parameters.Add(par);

                            using (DbDataReader DataR = cmd.ExecuteReader())
                            {
                                while (DataR.Read())
                                {
                                    SquareDetail data = new SquareDetail
                                    {
                                        IdAsignacion = DataR.GetGuid(0),
                                        Pais = DataR.GetString(1),
                                        Sucursal = DataR.GetString(2),
                                        Gerencia = DataR.GetString(3),
                                        Rol = DataR.GetString(4),
                                        Grupo = DataR.GetString(5),
                                        Color = DataR.GetString(6),
                                        Promedio = DataR.GetDecimal(7),
                                    };
                                    Result.Add(data);
                                }
                            }
                        }
                    }
                }
            }

            return Result;
        }

        public SquareThree AnaliticSquareThree(Guid EncuestaId, string DataJson)
        {
            SquareThree Result = new SquareThree();
            DataSet data = new DataSet();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(db.Database.Connection);
                using (DbConnection con = factory.CreateConnection())
                {
                    con.ConnectionString = db.Database.Connection.ConnectionString;
                    using (con)
                    {
                        con.Open();
                        using (DbCommand cmd = factory.CreateCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "[dbo].[AnaliticSquareThree]";
                            cmd.CommandType = CommandType.StoredProcedure;

                            SqlParameter par = new SqlParameter
                            {
                                ParameterName = "@EncuestaId",
                                SqlDbType = SqlDbType.UniqueIdentifier,
                                SqlValue = EncuestaId,
                                Direction = ParameterDirection.Input
                            };
                            cmd.Parameters.Add(par);

                            par = null;
                            par = new SqlParameter
                            {
                                ParameterName = "@DataJson",
                                SqlDbType = SqlDbType.NVarChar,
                                SqlValue = DataJson,
                                Direction = ParameterDirection.Input
                            };
                            cmd.Parameters.Add(par);


                            DbDataAdapter adapter = factory.CreateDataAdapter();
                            adapter.SelectCommand = cmd;
                            adapter.Fill(data);
                        }
                    }
                }
            }

            Result.Base = ConvertDetail<SquareDetail>(data.Tables[0]);
            Result.SubTotal = ConvertDetail<SquareDetail>(data.Tables[1]);
            Result.Total = ConvertDetail<SquareDetail>(data.Tables[2]);

            return Result;
        }

        public List<SquareFourth> AnaliticSquareFourth(Guid EncuestaId, string DataJson)
        {
            List<SquareFourth> Result = new List<SquareFourth>();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(db.Database.Connection);
                using (DbConnection con = factory.CreateConnection())
                {
                    con.ConnectionString = db.Database.Connection.ConnectionString;
                    using (con)
                    {
                        con.Open();
                        using (DbCommand cmd = factory.CreateCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "[dbo].[AnaliticSquareFourth]";
                            cmd.CommandType = CommandType.StoredProcedure;

                            SqlParameter par = new SqlParameter
                            {
                                ParameterName = "@EncuestaId",
                                SqlDbType = SqlDbType.UniqueIdentifier,
                                SqlValue = EncuestaId,
                                Direction = ParameterDirection.Input
                            };
                            cmd.Parameters.Add(par);

                            par = null;
                            par = new SqlParameter
                            {
                                ParameterName = "@DataJson",
                                SqlDbType = SqlDbType.NVarChar,
                                SqlValue = DataJson,
                                Direction = ParameterDirection.Input
                            };
                            cmd.Parameters.Add(par);

                            using (DbDataReader DataR = cmd.ExecuteReader())
                            {
                                while (DataR.Read())
                                {
                                    SquareFourth data = new SquareFourth
                                    {
                                        Grupo = DataR.GetString(0),
                                        Respuesta = DataR.GetInt64(1),
                                        Cantidad = DataR.GetInt64(2)
                                    };
                                    Result.Add(data);
                                }
                            }
                        }
                    }
                }
            }

            return Result;
        }

        public List<SquareFive> AnaliticSquareFive(Guid EncuestaId, string IdAsignaciones)
        {
            List<SquareFive> Result = new List<SquareFive>();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(db.Database.Connection);
                using (DbConnection con = factory.CreateConnection())
                {
                    con.ConnectionString = db.Database.Connection.ConnectionString;
                    using (con)
                    {
                        con.Open();
                        using (DbCommand cmd = factory.CreateCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "[dbo].[AnaliticSquareFive]";
                            cmd.CommandType = CommandType.StoredProcedure;

                            SqlParameter par = new SqlParameter
                            {
                                ParameterName = "@EncuestaId",
                                SqlDbType = SqlDbType.UniqueIdentifier,
                                SqlValue = EncuestaId,
                                Direction = ParameterDirection.Input
                            };
                            cmd.Parameters.Add(par);

                            par = null;
                            par = new SqlParameter
                            {
                                ParameterName = "@IdAsignaciones",
                                SqlDbType = SqlDbType.NVarChar,
                                SqlValue = IdAsignaciones,
                                Direction = ParameterDirection.Input
                            };
                            cmd.Parameters.Add(par);

                            using (DbDataReader DataR = cmd.ExecuteReader())
                            {
                                while (DataR.Read())
                                {
                                    SquareFive data = new SquareFive
                                    {
                                        OrdenGrupo = DataR.GetInt32(0),
                                        Grupo = DataR.GetString(1),
                                        OrdenPregunta = DataR.GetInt32(2),
                                        Pregunta = DataR.GetString(3),
                                        Respuesta = DataR.GetInt32(4),
                                        Conteo = DataR.GetInt32(5),
                                        Percentaje = DataR.GetDecimal(6),
                                    };
                                    Result.Add(data);
                                }
                            }
                        }
                    }
                }
            }

            return Result;
        }

        public SquareSix AnaliticSquareSix(Guid EncuestaId, string DataJson)
        {
            SquareSix Result = new SquareSix();
            DataSet data = new DataSet();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(db.Database.Connection);
                using (DbConnection con = factory.CreateConnection())
                {
                    con.ConnectionString = db.Database.Connection.ConnectionString;
                    using (con)
                    {
                        con.Open();
                        using (DbCommand cmd = factory.CreateCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "[dbo].[AnaliticSquareSix]";
                            cmd.CommandType = CommandType.StoredProcedure;

                            SqlParameter par = new SqlParameter
                            {
                                ParameterName = "@EncuestaId",
                                SqlDbType = SqlDbType.UniqueIdentifier,
                                SqlValue = EncuestaId,
                                Direction = ParameterDirection.Input
                            };
                            cmd.Parameters.Add(par);

                            par = null;
                            par = new SqlParameter
                            {
                                ParameterName = "@DataJson",
                                SqlDbType = SqlDbType.NVarChar,
                                SqlValue = DataJson,
                                Direction = ParameterDirection.Input
                            };
                            cmd.Parameters.Add(par);


                            DbDataAdapter adapter = factory.CreateDataAdapter();
                            adapter.SelectCommand = cmd;
                            adapter.Fill(data);
                        }
                    }
                }
            }

            Result.Base = ConvertDetail<SquareResult>(data.Tables[0]);
            Result.SubTotal = ConvertDetail<SquareResult>(data.Tables[1]);
            Result.Total = ConvertDetail<SquareResult>(data.Tables[2]);

            return Result;
        }

        public List<T> ConvertDetail<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name))
                    {
                        PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                        if (pI.PropertyType.FullName.Equals("System.Guid"))
                        {
                            pro.SetValue(objT, new Guid(row[pro.Name].ToString()));
                        }
                        else
                        {
                            pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], pI.PropertyType));
                        }                        
                    }
                }
                return objT;
            }).ToList();
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