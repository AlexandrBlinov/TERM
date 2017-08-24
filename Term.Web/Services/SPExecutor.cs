using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Threading.Tasks;

namespace YstProject.Services
{
    /// <summary>
    /// DTO for passing table type to stored procedure
    /// </summary>
        public struct ProductWithCountOnDep
        {
                public int DepartmentId { get; set; }
                public int ProductId { get; set; }
                public int Count { get; set; }
        }

    /// <summary>
    /// For executing stored procedures
    /// </summary>
    public static class SPExecutor
    {

        private static readonly string _connectionstring = ConfigurationManager.ConnectionStrings["YstTerminal"].ConnectionString;
        public static int Execute(string nameOfProcedure, string filename, out string errorMsg, int timeout = 1800)
        {
            return Execute(nameOfProcedure, new SqlParameter("@FilePath", filename), out errorMsg, timeout);
        }
        public static int Execute(string nameOfProcedure, SqlParameter firstparam, out string errorMsg, int timeout = 1800)
        {

            SqlParameter[] parameters = new[] {firstparam, //{ ParameterName="@FilePath",SqlDbType=SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = fullPathToDirectory + ConfigurationManager.AppSettings[nvc[key]] }
                new SqlParameter { ParameterName="@b",SqlDbType=SqlDbType.Int, Direction=ParameterDirection.ReturnValue },
               new SqlParameter("@Message", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output }};

            errorMsg = String.Empty;
            int result;

            using (SqlConnection SqlConn = new SqlConnection(_connectionstring))
            {
                SqlConn.Open();

                using (SqlCommand sqlcomm = new SqlCommand(nameOfProcedure, SqlConn))
                {
                    sqlcomm.CommandType = CommandType.StoredProcedure;
                    sqlcomm.CommandTimeout = timeout; // 30 minites

                    foreach (var parameter in parameters)
                        sqlcomm.Parameters.Add(parameter);
                    try
                    {
                        sqlcomm.ExecuteNonQuery();
                    }
                    catch (Exception exc)
                    {
                        result = -1;
                        errorMsg = String.Format("{0} {1}", exc.Message, exc.InnerException);
                        return result;
                    }
                    result = (int)sqlcomm.Parameters["@b"].Value;
                    if (result != 0)
                    {
                        SqlConn.Close();
                        errorMsg = sqlcomm.Parameters["@Message"].Value as string;
                    }

                    return result;
                }
            }
        }
        public static int Execute(string nameOfProcedure, SqlParameter[] parameters, out string errorMsg, int timeout=1800)
        {
            errorMsg = "";

            using (SqlConnection SqlConn = new SqlConnection(_connectionstring))
            {
                SqlConn.Open();

                using (SqlCommand sqlcomm = new SqlCommand(nameOfProcedure, SqlConn))
                {
                    sqlcomm.CommandType = CommandType.StoredProcedure;
                    sqlcomm.CommandTimeout = timeout; // 30 minites

                    foreach (var parameter in parameters)
                        sqlcomm.Parameters.Add(parameter);

                    sqlcomm.ExecuteNonQuery();
                    var result = (int)sqlcomm.Parameters["@b"].Value;
                    if (result != 0)
                    {
                        SqlConn.Close();
                        errorMsg = sqlcomm.Parameters["@Message"].Value as string; 
                    }
                   
                    return result;
                }
            }
        }

       

        /// <summary>
        /// Выполнить хранимую процедуру для уменьшения остатков
        /// </summary>
        /// <param name="nameOfProcedure"></param>
        /// <param name="records"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
     /*   public static async Task Execute(string nameOfProcedure, IEnumerable<ProductWithCountOnDep> records, int timeout = 1800)
        {
           

            using (var sqlConn = new SqlConnection(_connectionstring))
            {
                sqlConn.Open();

                using (SqlCommand sqlcomm = new SqlCommand(nameOfProcedure, sqlConn))
                {
                    sqlcomm.CommandType = CommandType.StoredProcedure;
                    sqlcomm.CommandTimeout = timeout; // 30 minites

                    SqlParameter prodParam = sqlcomm.Parameters.AddWithValue("@ProdWithCount", CreateDataTable(records));
                    prodParam.SqlDbType = SqlDbType.Structured;

                  await  sqlcomm.ExecuteNonQueryAsync();
                   
                }
            }
        } */
    }

      


}


