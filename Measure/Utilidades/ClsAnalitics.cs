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

        public SquareTwo AnaliticSquareTwo(Guid EncuestaId, string DataJson)
        {
            SquareTwo Result = new SquareTwo();
            DataSet data = new DataSet();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(db.Database.Connection);
                using (DbConnection con = factory.CreateConnection())
                {
                    con.ConnectionString = db.Database.Connection.ConnectionString;
                    using (con)
                    {
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


                            DbDataAdapter adapter = factory.CreateDataAdapter();
                            adapter.SelectCommand = cmd;
                            adapter.Fill(data);
                        }
                    }
                }
            }

            Result.Base = ConvertDetail(data.Tables[0], new SquareDetail());
            Result.SubTotal = ConvertDetail(data.Tables[1], new SquareDetail());
            Result.Total = ConvertDetail(data.Tables[2], new SquareDetail());

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

            Result.Base = ConvertDetail(data.Tables[0], new SquareDetail());
            Result.SubTotal = ConvertDetail(data.Tables[1], new SquareDetail());
            Result.Total = ConvertDetail(data.Tables[2], new SquareDetail());

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

            Result.Base = ConvertDetail(data.Tables[0], new SquareResult());
            Result.SubTotal = ConvertDetail(data.Tables[1], new SquareResult());
            Result.Total = ConvertDetail(data.Tables[2], new SquareResult());

            return Result;
        }

        private List<T> ConvertDetail<T>(DataTable Data, T AddItem)
        {
            List<T> Result = new List<T>();

            foreach (DataRow Fila in Data.Rows)
            {                
                foreach (DataColumn Columna in Data.Columns)
                {
                    PropertyInfo Property = AddItem.GetType().GetProperty(Columna.ColumnName);
                    Type tipo = Type.GetType(Property.PropertyType.FullName);
                    if (!string.IsNullOrEmpty(Fila[Columna].ToString()))
                    {
                        Property.SetValue(AddItem, Convert.ChangeType(Fila[Columna], tipo));
                    }
                }
                Result.Add(AddItem);
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